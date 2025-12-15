using RachaConta.Api.Middleware;
using RachaConta.Infrastructure.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using Infrastructure Builders
builder.Services.AddProjectServices(builder.Configuration);
builder.Services.AddAuthenticationConfig(builder.Configuration);

// Add HealthChecks
var healthChecksBuilder = builder.Services.AddHealthChecks();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    healthChecksBuilder.AddNpgSql(connectionString);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerConfig();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Add authenticated user middleware
app.UseMiddleware<AuthenticatedUserMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

// Make the implicit Program class public for integration tests
public partial class Program { }
