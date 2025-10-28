using Authentication.Model;

namespace Authentication.Repository;

public interface IUserRepository:IRepository<User>
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    
    
}