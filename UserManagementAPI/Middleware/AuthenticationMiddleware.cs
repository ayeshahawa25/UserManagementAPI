namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Extract token from Authorization header
            string token = context.Request.Headers["Authorization"].ToString();

            // Allow unauthenticated access to Swagger documentation
            if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/api/health"))
            {
                await _next(context);
                return;
            }

            // Validate token
            if (string.IsNullOrEmpty(token) || !ValidateToken(token))
            {
                _logger.LogWarning("Invalid or missing authentication token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized. Invalid or missing token." });
                return;
            }

            _logger.LogInformation("Valid token provided for request to {Path}", context.Request.Path);
            await _next(context);
        }

        private static bool ValidateToken(string token)
        {
            // Simple token validation - in production, use JWT tokens
            // For this demo, we accept any token that starts with "Bearer "
            const string validTokenPrefix = "Bearer ";
            
            if (!token.StartsWith(validTokenPrefix))
            {
                return false;
            }

            // Extract the actual token (remove "Bearer " prefix)
            var actualToken = token.Substring(validTokenPrefix.Length);
            
            // For demo purposes, accept any non-empty token after "Bearer "
            // In production, validate JWT signature and claims
            return !string.IsNullOrWhiteSpace(actualToken);
        }
    }
}
