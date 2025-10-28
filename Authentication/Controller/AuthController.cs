using Authentication.Dto;
using Authentication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Authentication;

[ApiController]
[EnableRateLimiting("auth-limit")]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthController(RefreshTokenService refreshTokenService, UserService userService)
    {
        _refreshTokenService = refreshTokenService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAsync([FromBody] UserRequestDto userRequestDto)
    {
        var user = await _userService.RegisterAsync(userRequestDto);
        return Ok(new AuthResponseDto { Message =$"{user.UserName} register successfully"   });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] UserRequestDto userRequestDto)
    {
        var userId = await _userService.LoginAsync(userRequestDto);
        var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(userId);
        CookieHelper.SetRefreshTokenCookie(Response, refreshToken);

        return Ok(new AuthResponseDto { Message = "Login Successfully" });
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

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> RefreshAsync()
    {
        var oldToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(oldToken))
            return Unauthorized(new AuthResponseDto { Message = "Refresh token is missing" });

        var newToken = await _refreshTokenService.RotateRefreshTokenAsync(oldToken);

        CookieHelper.SetRefreshTokenCookie(Response, newToken);
        return Ok(new AuthResponseDto { Message = "Token refreshed successfully" });
    }
    
    
}
