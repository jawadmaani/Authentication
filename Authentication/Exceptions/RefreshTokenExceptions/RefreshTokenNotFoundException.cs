namespace Report_System_Backend.middleware.RefreshTokenExceptions;

public class RefreshTokenNotFoundException:Exception
{
    public RefreshTokenNotFoundException(String message) : base(message)
    {
        
    }
    
}