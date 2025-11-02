namespace Report_System_Backend.middleware.AccessTokenExceptions;

public class InvalidTokenException:Exception
{
    public InvalidTokenException(string message) : base(message)
    {
        
    }
    
}