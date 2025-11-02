using Authentication.Dto;
using Authentication.Security;
using Authentication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Authentication;

[ApiController]
[EnableRateLimiting("auth-limit")]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly AccessTokenService _accessTokenService;
    private readonly JwtSettings _jwtSettings;


    public AuthController(RefreshTokenService refreshTokenService,AccessTokenService accessTokenService ,UserService userService, IOptions<JwtSettings> jwtSettings)
    
    {
        _refreshTokenService = refreshTokenService;
        _userService = userService;
        _accessTokenService = accessTokenService;
        _jwtSettings = jwtSettings.Value;

    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync([FromBody] UserRequestDto userRequestDto)
    {
        var user = await _userService.RegisterAsync(userRequestDto);
        return Ok(new AuthResponseDto { Message =$"{user.UserName} register successfully"   });
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] UserRequestDto userRequestDto)
    {
        var user = await _userService.LoginAsync(userRequestDto);
        var accessToken = _accessTokenService.CreateAccessToken(user.userId, user.role);
        var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.userId);
        CookieHelper.SetRefreshTokenCookie(Response, refreshToken);


        return Ok(new AuthResponseDto
        {
            Message = "Login successful",
            AccessToken = accessToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
        });    
    }

    [HttpPost("logout")]
    public async Task<ActionResult<AuthResponseDto>> LogoutAsync()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        CookieHelper.DeleteRefreshTokenCookie(Response);
        return Ok(new AuthResponseDto { Message = "Logged out successfully" });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshAsync()
    {
        var oldToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(oldToken))
            return Unauthorized(new AuthResponseDto { Message = "Refresh token is missing" });

        var (newRefreshToken, userId) = await _refreshTokenService.RotateRefreshTokenAsync(oldToken);
        var user = await _userService.GetUserByIdAsync(userId);
        var newAccessToken = _accessTokenService.CreateAccessToken(userId, user.Role);

        CookieHelper.SetRefreshTokenCookie(Response, newRefreshToken);
        return Ok(new AuthResponseDto {
            Message = "Token refreshed successfully",
            AccessToken = newAccessToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
        });
        
    }
    
    
}
