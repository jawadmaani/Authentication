namespace Authentication.Security
{
   
    public class JwtSettings
    {
        public string Issuer { get; set; } 
        public string Audience { get; set; } 
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public string PrivateKeyPath { get; set; }

        public string PublicKeyPath { get; set; }
        
        public string Algorithm { get; set; } = "ES256";
    }
}