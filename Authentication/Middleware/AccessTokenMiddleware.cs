using System.Security.Claims;
using Authentication.Service;
using Microsoft.AspNetCore.Authorization;
using Report_System_Backend.middleware.AccessTokenExceptions;

namespace Authentication.Middleware;

public class AccessTokenMiddleware
{
    private readonly RequestDelegate _next;

    public AccessTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

        if (allowAnonymous)
        {
            await _next(context);
            return;
        }
        
        var accessTokenService = context.RequestServices.GetRequiredService<AccessTokenService>();
        
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            throw new MissingAuthorizationHeaderException("Authorization header missing.");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var userData = accessTokenService.ExtractUserData(token);

        if (userData == null)
        {
            throw new InvalidTokenException("Invalid or expired token.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userData.Value.UserId.ToString()),
            new Claim(ClaimTypes.Role, userData.Value.Role ?? "User")
        };
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

        await _next(context);
    }
}