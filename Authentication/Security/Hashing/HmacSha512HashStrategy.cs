using System.Security.Cryptography;
using System.Text;

namespace Authentication.Security;

public class HmacSha512HashStrategy:ITokenHashStrategy
{
    public string Hash(string token, string secretKey)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }
}