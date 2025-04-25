using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middleware;

public class ExceptionHandlerMiddleware(ILogger logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError("From middleware: {Message}", e.Message);
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            var details = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Detail = "An unexpected internal server error occurred."
            };
            
            await context.Response.WriteAsJsonAsync(details);
        }
    }
}