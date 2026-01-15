using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Api.Common.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        ResponseBody response = ToResponseBody(exception);

        context.Response.StatusCode = response.StatusCode;
        context.Response.ContentType = "application/json";

        string jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private ResponseBody ToResponseBody(Exception exception) => exception switch
    {
        NotFoundException => new ResponseBody
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            Message = exception.Message,
        },

        UnauthorizedAccessException => new ResponseBody
        {
            StatusCode = (int)HttpStatusCode.Forbidden,
            Message = "You are not authorized to perform this action",
        },

        ValidationException e => new ResponseBody
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Message = e.Message,

            Errors = e.Errors.Where(error => string.IsNullOrEmpty(error.PropertyName))
                             .Select(error => error.ErrorMessage)
                             .ToList(),

            ValidationErrors = e.Errors.Where(error => !string.IsNullOrEmpty(error.PropertyName))
                                       .GroupBy(x => x.PropertyName)
                                       .ToDictionary(group => group.Key, group => group.Select(x => x.ErrorMessage)),
        },

        ApplicationValidationException e => ToResponseBody(e, (int)HttpStatusCode.BadRequest),

        _ => new ResponseBody
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "An unexpected error occurred. Please try again later.",
            Errors = [exception.Message],
        },
    };

    private ResponseBody ToResponseBody(ApplicationValidationException applicationException, int statusCode)
    {
        List<string> errors = applicationException.Errors.ToList();
        applicationException.ValidationErrors.TryGetValue(string.Empty, out IEnumerable<string>? validationErrors);
        errors.AddRange(validationErrors ?? []);

        return new()
        {
            StatusCode = statusCode,
            Message = applicationException.Message,
            Errors = errors,
            ValidationErrors = applicationException.ValidationErrors
                                                   .Where(validationError => !string.IsNullOrEmpty(validationError.Key))
                                                   .ToDictionary(errors => errors.Key, errors => errors.Value),
        };
    }
}
