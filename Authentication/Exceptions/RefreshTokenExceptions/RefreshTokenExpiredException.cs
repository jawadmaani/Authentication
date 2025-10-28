namespace Report_System_Backend.middleware.RefreshTokenExceptions;

public class RefreshTokenExpiredException:Exception
{
    public RefreshTokenExpiredException(String message) : base(message)
    {
        
    }
    
}