using System.Net;

namespace REST_API_GNB.Utilities
{
    // Clase para loggear errores asincronamente. Codigo extraido de ChatGPT
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await LogExceptionToFile(ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware."
            };

            return context.Response.WriteAsJsonAsync(result);
        }

        private async Task LogExceptionToFile(Exception exception)
        {
            var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "error.log");
            var logMessage = $"[{DateTime.Now}]: {exception.Message}{Environment.NewLine}";

            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            await File.AppendAllTextAsync(logFilePath, logMessage);
        }
    }
}
