using System.Net;
using System.Text.Json;

namespace TransportManagement.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware ( RequestDelegate next )
        {
            _next = next;
        }

        public async Task Invoke ( HttpContext httpContext )
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync ( HttpContext context, Exception exception )
        {
            var code = HttpStatusCode.InternalServerError;

            if (exception is UnauthorizedAccessException)
                code = HttpStatusCode.Unauthorized;

            var result = JsonSerializer.Serialize(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
