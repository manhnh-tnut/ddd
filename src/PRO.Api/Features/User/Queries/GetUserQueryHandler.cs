using System.Data;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Dapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Nest;
using PRO.Api.Extensions;
using PRO.Api.Features.User.Responses;
using PRO.Domain.Interfaces;

namespace PRO.Api.Features.User.Queries;

// Regular QueryHandler
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, IEnumerable<UserInfoResponse>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IElasticClient _client;
    private readonly IDistributedCache _cache;
    private readonly IDapperRepository _dapperRepository;

    // Using DI to inject infrastructure persistence Repositories
    public GetUserQueryHandler(IMediator mediator
    , IElasticClient client
    , IDistributedCache cache
    , IDapperRepository dapperRepository
    , IMapper mapper)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _dapperRepository = dapperRepository ?? throw new ArgumentNullException(nameof(dapperRepository));
    }
    /// <summary>
    /// Handler which processes the query when
    /// customer executes cancel user from app
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<IEnumerable<UserInfoResponse>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query?.Keyword))
        {
            var result = await _client.SearchAsync<PRO.Domain.Entities.Users.User>(s => s
                .Query(q => q
                    .QueryString(d => d
                        .Query('*' + query.Keyword + '*')
                    )).Size(5000)
                );
            if (result != null && result.Documents != null && result.Documents.Any())
            {
                return result.Documents.Select(i => _mapper.Map<UserInfoResponse>(i));
            }
        }

        return await _cache.GetOrSetCacheAsync<IEnumerable<UserInfoResponse>>(
            $"get-users: {JsonSerializer.Serialize(query, Infrastructure.Contract.jsonOptions)}",
            async () =>
            {
                StringBuilder sbQuery = new StringBuilder(1000);
                sbQuery.Append(@"
                    SELECT [Id]
                        ,[UserName]
                        ,[Password]
                        ,[FirstName]
                        ,[LastName]
                        ,[Address]
                        ,[BirthDate]
                        ,[DepartmentId]
                        ,[CoefficientsSalary]
                        ,[Created]
                        ,[CreatedBy]
                        ,[Updated]
                        ,[UpdatedBy]
                        ,[Deleted]
                        ,[Used]
                    FROM [dbo].[User]
                    WHERE   [UserName] LIKE CONCAT('%',@Keyword,'%') 
                            OR [FirstName] LIKE CONCAT('%',@Keyword,'%') 
                            OR [LastName] LIKE CONCAT('%',@Keyword,'%') 
                ");

                using (IDbConnection conn = _dapperRepository.connection)
                {
                    conn.Open();
                    var data = await conn.QueryAsync<PRO.Domain.Entities.Users.User>(sbQuery.ToString(), new { Keyword = query.Keyword });
                    return data.Select(i => _mapper.Map<UserInfoResponse>(i));
                }
            }, Infrastructure.Contract.Cache5M);
    }
}