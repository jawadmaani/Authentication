namespace Report_System_Backend.middleware.AccessTokenExceptions;

public class MissingAuthorizationHeaderException:Exception
{
    public MissingAuthorizationHeaderException(string message) : base(message)
    {
        
    }
    
}