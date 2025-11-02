using Authentication.Dto;
using Authentication.Security;
using Authentication.Service;
using Microsoft.Extensions.Options;

public class AccessTokenService
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly JwtSettings _settings;

    public AccessTokenService(JwtTokenService jwtTokenService, IOptions<JwtSettings> settings)
    {
        _jwtTokenService = jwtTokenService;
        _settings = settings.Value;
    }

    public string CreateAccessToken(int userId, string role)
    {
        var now = DateTime.UtcNow;
        var claims = new AccessTokenClaims
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Subject = userId.ToString(),
            Role = role,
            JwtId = Guid.NewGuid().ToString(),
            IssuedAt = now,
            ExpiresAt = now.AddMinutes(_settings.AccessTokenExpirationMinutes)
        };

        return _jwtTokenService.Generate(claims);
    }
    
    public (int? UserId, string? Role)? ExtractUserData(string token)
    {
        return _jwtTokenService.ExtractUserData(token);
    }
    
}