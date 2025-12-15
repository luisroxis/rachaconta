using System.Security.Claims;
using RachaConta.Core.Interfaces.Repositories;

namespace RachaConta.Api.Middleware;

public class AuthenticatedUserMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticatedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        // Check if user is authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // Get userId from JWT claims (sub claim)
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? context.User.FindFirst("sub");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                // Get user from database
                var user = await userRepository.GetByIdAsync(userId);
                
                if (user != null)
                {
                    // Store user in HttpContext.Items for access in controllers
                    context.Items["AuthenticatedUser"] = user;
                    context.Items["AuthenticatedUserId"] = userId;
                }
            }
        }

        await _next(context);
    }
}
