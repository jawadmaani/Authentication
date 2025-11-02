namespace Authentication.Security;

public interface  ITokenHashStrategy
{
    string Hash(string token, string secretKey);

}