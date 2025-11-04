
namespace Authentication.Security;

public static class CookieHelper
{
    public static void SetRefreshTokenCookie(HttpResponse response, string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        response.Cookies.Append("refreshToken", token, cookieOptions);
    }

    public static void DeleteRefreshTokenCookie(HttpResponse response)
    {
        response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    }
}