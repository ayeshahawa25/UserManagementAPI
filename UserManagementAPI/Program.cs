using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseInMemoryDatabase("UserManagementDb"));

// Add controllers
builder.Services.AddControllers();

// Add OpenAPI/Swagger
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Add logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure middleware pipeline in correct order:
// 1. Error handling middleware first
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Authentication middleware next
app.UseMiddleware<AuthenticationMiddleware>();

// 3. Logging middleware last
app.UseMiddleware<LoggingMiddleware>();

// Other middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Health check endpoint (no auth required)
app.MapGet("/api/health", () => new { status = "Healthy", timestamp = DateTime.UtcNow })
    .WithName("HealthCheck")
    .WithOpenApi();

app.Run();
