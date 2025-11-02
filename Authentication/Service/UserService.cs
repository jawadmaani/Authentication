using Authentication.Dto;
using Authentication.Exceptions;
using Authentication.Mapper;
using Authentication.Model;
using Authentication.Repository;
using Authentication.Security;


namespace Authentication.Service;

public class UserService
{
    private readonly IUserRepository _repository;
    private readonly PasswordHasher _passwordHasher;
    
    public UserService(IUserRepository repository, PasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserResponseDto>>GetAllUsersAsync()
    {
        var users= await _repository.GetAllUsersAsync();
        if (users == null || users.Count() == 0)
        {
            throw new EmptyDataBaseFromUsers("No users found");
        }
     
        return UserMapper.ToUserResponseDtoList(users);

    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new EmptyDataBaseFromUsers("No user found");
        }
        
      return UserMapper.ToUserResponseDto(user);

    }

    public async Task<UserResponseDto> RegisterAsync(UserRequestDto userRequestDto)
    { 
        var existingUserByName = await _repository.GetByUserNameAsync(userRequestDto.Username);
        var existingUserByEmail = await _repository.GetByEmailAsync(userRequestDto.Email);
        if (existingUserByName != null || existingUserByEmail!= null)
        {
            throw new UserAlreadyExistsException("Username or email already exists");
        }
        User user = new User
        {
            Name = userRequestDto.Username,
            Email = userRequestDto.Email,
            PasswordHash = _passwordHasher.HashPassword(userRequestDto.Password),
            Role = "User"
        };
       await _repository.CreateAsync(user);
       await _repository.SaveAsync();

        return UserMapper.ToUserResponseDto(user);
    }

    public async Task<(int userId, string role)> LoginAsync(UserRequestDto userRequestDto)
    {
        var user = await _repository.GetByUserNameAsync(userRequestDto.Username);
        if (user == null)
            throw new InvalidCredentialsException("Invalid username or password");
    
        bool passwordMatch = _passwordHasher.VerifyPassword(
            user.PasswordHash, 
            userRequestDto.Password
        );
    
        if (!passwordMatch)
            throw new InvalidCredentialsException("Invalid username or password");

        return (user.Id, user.Role);
        
    }
    
    
    
}