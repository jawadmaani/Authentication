using System.ComponentModel.DataAnnotations;

namespace Authentication.Dto;

public class UserRequestDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    
    
}