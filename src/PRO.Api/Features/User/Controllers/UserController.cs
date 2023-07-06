using System.Net;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRO.Api.Features.User.Commands;
using PRO.Api.Features.User.Queries;
using PRO.Api.Features.User.Requests;
using PRO.Api.Features.User.Responses;

namespace PRO.Api.Features.User.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger
        , IMediator mediator
        , IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<UserInfoResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] Requests.GetUserRequest request)
        {
            GetUserQuery query = _mapper.Map<GetUserQuery>(request);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Add([FromBody] AddUserRequest request)
        {
            RegisterUserCommand command = _mapper.Map<RegisterUserCommand>(request);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("payslips")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddPayslip([FromBody] AddPayslipRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}