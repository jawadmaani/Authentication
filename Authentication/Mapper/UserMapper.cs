using Authentication.Dto;
using Authentication.Model;

namespace Authentication.Mapper;

public class UserMapper
{
    public static UserResponseDto ToUserResponseDto( User user)
    {
        return new UserResponseDto
        {
            UserName = user.Name,
            Email = user.Email
        };
    }
    
    public static List<UserResponseDto> ToUserResponseDtoList(IEnumerable<User> users)
    {
        return users.Select(ToUserResponseDto).ToList();
    }
}
