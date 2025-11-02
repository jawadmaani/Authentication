using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Authentication.Dto;
using Authentication.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.Service;

public class JwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly ECDsa _privateKey;
    private readonly ECDsa _publicKey;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
        _privateKey = KeyLoader.LoadKey(_settings.PrivateKeyPath);
        _publicKey = KeyLoader.LoadKey(_settings.PublicKeyPath);
    }

    public string Generate(AccessTokenClaims claims)
    {
        var credentials = new SigningCredentials(new ECDsaSecurityKey(_privateKey), SecurityAlgorithms.EcdsaSha256);

        var jwt = new JwtSecurityToken(
            issuer: claims.Issuer,
            audience: claims.Audience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, claims.Subject), 
                new Claim(JwtRegisteredClaimNames.Jti, claims.JwtId),
                new Claim(ClaimTypes.Role, claims.Role)
            },
            notBefore: claims.IssuedAt,
            expires: claims.ExpiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public ClaimsPrincipal? Validate(string token)
    {
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,

            ValidateAudience = true,
            ValidAudience = _settings.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new ECDsaSecurityKey(_publicKey),

            RequireExpirationTime = true,
            RequireSignedTokens = true
        };
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParams, out _);
            return principal;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }
    
    public (int? UserId, string? Role)? ExtractUserData(string token)
    {
        var principal = Validate(token);

        if (principal == null)
            return null; 

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return (userId, roleClaim);

        return null;
    }

}