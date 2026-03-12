using System.Diagnostics;

namespace UserManagementAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log incoming request
            _logger.LogInformation("Incoming Request: Method={Method}, Path={Path}, Time={Time}", 
                context.Request.Method, 
                context.Request.Path, 
                DateTime.UtcNow);

            var stopwatch = Stopwatch.StartNew();

            // Call the next middleware
            await _next(context);

            stopwatch.Stop();

            // Log outgoing response
            _logger.LogInformation("Outgoing Response: StatusCode={StatusCode}, Duration={Duration}ms, Time={Time}", 
                context.Response.StatusCode, 
                stopwatch.ElapsedMilliseconds, 
                DateTime.UtcNow);
        }
    }
}
