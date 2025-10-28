namespace Report_System_Backend.middleware.RefreshTokenExceptions;

public class RefreshTokenRevokedException:Exception
{
    public RefreshTokenRevokedException(string message) : base(message)
    {
        
    }
    
}