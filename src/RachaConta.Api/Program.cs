using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RachaConta.Api.Middleware;
using RachaConta.Application.Interfaces;
using RachaConta.Application.UseCases;
using RachaConta.Core.Interfaces.Services;
using RachaConta.Infrastructure.Data;
using RachaConta.Infrastructure.Repositories;
using RachaConta.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RachaContaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("RachaConta.Infrastructure")));

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

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

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IInviteRepository, InviteRepository>();

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// Register use cases
builder.Services.AddScoped<RegisterUserUseCase>();
builder.Services.AddScoped<RequestPasswordRecoveryUseCase>();
builder.Services.AddScoped<ResetPasswordUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<SendInviteUseCase>();
builder.Services.AddScoped<ResendInviteUseCase>();
builder.Services.AddScoped<DeleteInviteUseCase>();
builder.Services.AddScoped<ListInvitesUseCase>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Add controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
