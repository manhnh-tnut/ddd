using Grpc.Core;
using MediatR;
using PRO.Api.Features.Grpc.Protos;
namespace PRO.Api.Features.Grpc.Services;

public class GrpcPlcService : GrpcPlc.GrpcPlcBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GrpcPlcService> _logger;

    public GrpcPlcService(IMediator mediator, ILogger<GrpcPlcService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
    // public override async Task<OrderDraftDTO> CreateOrderDraftFromBasketData(CreateOrderDraftCommand createOrderDraftCommand, ServerCallContext context)
    // {
    //     _logger.LogInformation("Begin grpc call from method {Method} for ordering get order draft {CreateOrderDraftCommand}", context.Method, createOrderDraftCommand);
    //     _logger.LogTrace(
    //         "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
    //         createOrderDraftCommand.GetGenericTypeName(),
    //         nameof(createOrderDraftCommand.BuyerId),
    //         createOrderDraftCommand.BuyerId,
    //         createOrderDraftCommand);

    //     var command = new AppCommand.CreateOrderDraftCommand(
    //                     createOrderDraftCommand.BuyerId,
    //                     this.MapBasketItems(createOrderDraftCommand.Items));


    //     var data = await _mediator.Send(command);

    //     if (data != null)
    //     {
    //         context.Status = new Status(StatusCode.OK, $" ordering get order draft {createOrderDraftCommand} do exist");

    //         return this.MapResponse(data);
    //     }
    //     else
    //     {
    //         context.Status = new Status(StatusCode.NotFound, $" ordering get order draft {createOrderDraftCommand} do not exist");
    //     }

    //     return new OrderDraftDTO();
    // }
}
