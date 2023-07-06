using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PRO.Domain.Exceptions;
using System.Net;

namespace PRO.Api.Infrastructure.Filters;

public class ErrorHandlingFilter : IExceptionFilter
{
    private readonly ILogger<ErrorHandlingFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment=environment;
    }
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            context.Exception.Message);

        var problemDetails = new ValidationProblemDetails()
        {
            Title = "An error ocurred while processing your request.",
            Instance = context.HttpContext.Request.Path,
            Status = StatusCodes.Status400BadRequest,
            Detail = "Please refer to the errors property for additional details."
        };

        if (context.Exception.GetType() == typeof(DomainException))
        {
            problemDetails.Errors.Add("DomainValidations", new string[] { context.Exception.Message.ToString() });

            context.Result = new BadRequestObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            if (_environment.IsDevelopment())
            {
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Detail = context.Exception.Message;
            }
            context.Result = new ObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        context.ExceptionHandled = true;
    }
}