using System.Net;
using System.Text.Json;
using TaskManagementSystem.Core.Exceptions;

namespace TaskManagementSystem.Web.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = ResolveError(exception);

        _logger.LogError(exception,
            "Unhandled exception. Path={Path}, StatusCode={StatusCode}",
            context.Request.Path, (int)statusCode);

        if (IsAjaxRequest(context))
        {
            await WriteJsonErrorAsync(context, statusCode, message);
            return;
        }

        RedirectToErrorPage(context, statusCode, message);
    }

    private static (HttpStatusCode StatusCode, string Message) ResolveError(Exception exception)
    {
        return exception switch
        {
            TaskNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            UnauthorizedTaskAccessException => (HttpStatusCode.Forbidden, "You do not have permission to access this task."),
            TaskValidationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again.")
        };
    }

    private static bool IsAjaxRequest(HttpContext context)
    {
        return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    private static async Task WriteJsonErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var payload = JsonSerializer.Serialize(new { success = false, message });
        await context.Response.WriteAsync(payload);
    }

    private static void RedirectToErrorPage(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.Redirect($"/Home/Error?statusCode={(int)statusCode}&message={Uri.EscapeDataString(message)}");
    }
}
