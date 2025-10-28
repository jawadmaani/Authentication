using System.Security.Cryptography;
using System.Text;

namespace Authentication.Security
{
    public class TokenHasher
    {
     
        private readonly string _secretKey;

        public TokenHasher(string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Secret key is missing. Make sure REFRESH_TOKEN_SECRET is set.");

            _secretKey = secretKey;
        }


        public string HashToken(string token)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
        
    }
}