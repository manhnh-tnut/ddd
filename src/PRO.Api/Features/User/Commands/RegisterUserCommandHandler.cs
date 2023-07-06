using PRO.Domain.Entities.Users;
using MediatR;
using Nest;

namespace PRO.Api.Features.User.Commands;

// Regular CommandHandler
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly IElasticClient _client;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    // Using DI to inject infrastructure persistence Repositories
    public RegisterUserCommandHandler(IMediator mediator
    , IElasticClient client
    , IUserRepository userRepository
    , ILogger<RegisterUserCommandHandler> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel user from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.AddAsync(new PRO.Domain.Entities.Users.User()
        {
            Address = command.address,
            BirthDate = command.birthDate,
            FirstName = command.firstName,
            LastName = command.lastName,
            CreatedBy = "admin",
            UserName = command.userName,
            Password = command.password,
            DepartmentId = command.departmentId
        });
         _logger.LogInformation("----- Creating user: {@user}", user.UserName);

        var result = await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (result)
        {
            await _client.IndexDocumentAsync(user);
        }
        return result;
    }
}