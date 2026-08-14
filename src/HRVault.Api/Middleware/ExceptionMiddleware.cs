using System.Net;
using System.Text.Json;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Models;

namespace HRVault.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false
        };

        switch (exception)
        {
            case NotFoundException ex:
                context.Response.StatusCode =
                    (int)HttpStatusCode.NotFound;

                response.Message = ex.Message;
                break;

            case ConflictException ex:
                context.Response.StatusCode =
                    (int)HttpStatusCode.Conflict;

                response.Message = ex.Message;
                break;

            case BusinessRuleException ex:
                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                response.Message = ex.Message;
                break;

            case ValidationException ex:
                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                response.Message = ex.Message;
                response.Errors = ex.Errors;
                break;

            case UnauthorizedAccessException ex:
                context.Response.StatusCode =
                    (int)HttpStatusCode.Unauthorized;

                response.Message = ex.Message;
                break;
			
			case ForbiddenException ex:
				context.Response.StatusCode =
					(int)HttpStatusCode.Forbidden;

				response.Message = ex.Message;
				break;
			
            default:
                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                response.Message =
                    "An unexpected error occurred.";
                break;
        }

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}