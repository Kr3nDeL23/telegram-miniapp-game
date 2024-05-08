using Presentation.Common.Domain.Models.Response;

namespace Presentation.Middlewares;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            var response = new Response(false)
            {
                Result = "Interval Server Error"
            };

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
