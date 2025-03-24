using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using CMS.Shared;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionMiddleware> _logger;

    public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        if (ex is SqlException)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var response = new
            {
                Status = StatusCodes.Status500InternalServerError,
                Message = GlobalSettings.ResponseMessages.ServerErrorMsg
            };
            var responseJson = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(responseJson);
        }

        // Handle other exceptions or rethrow
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var defaultResponse = new
        {
            Status = StatusCodes.Status500InternalServerError,
            Message = GlobalSettings.ResponseMessages.ServerErrorMsg
        };
        var defaultResponseJson = JsonSerializer.Serialize(defaultResponse);
        return context.Response.WriteAsync(defaultResponseJson);
    }
}
