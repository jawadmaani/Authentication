namespace Authentication.Security
{
    public class TokenHasher
    {
        private readonly string _secretKey;
        private readonly ITokenHashStrategy _hashStrategy;

        public TokenHasher(string secretKey, ITokenHashStrategy hashStrategy)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("Secret key is missing. Make sure REFRESH_TOKEN_SECRET is set.");

            _secretKey = secretKey;
            _hashStrategy = hashStrategy;
        }

        public string HashToken(string token)
        {
            return _hashStrategy.Hash(token, _secretKey);
        }
    }
}