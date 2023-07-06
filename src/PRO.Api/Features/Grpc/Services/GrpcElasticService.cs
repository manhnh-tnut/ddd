using Grpc.Core;
using MediatR;
using PRO.Api.Features.Grpc.Protos;
namespace PRO.Api.Features.Grpc.Services;

public class GrpcElasticService : GrpcElastic.GrpcElasticBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GrpcElasticService> _logger;

    public GrpcElasticService(IMediator mediator, ILogger<GrpcElasticService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override async Task SyncIndex(SyncRequest request, IServerStreamWriter<SyncReply> responseStream, ServerCallContext context)
    {
        Console.WriteLine($"Initial Message from Client: {request.Name}");
        try
        {
            int count = 0;
            while (++count < 10 && !context.CancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(10000);
                await responseStream.WriteAsync(new SyncReply
                {
                    Message = $"Ping Response from the Server at {DateTime.UtcNow}"
                });
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            Console.WriteLine("Operation Cancelled.");
        }

        Console.WriteLine("Processing Complete.");
    }
}
