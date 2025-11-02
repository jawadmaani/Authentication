using System.ComponentModel.DataAnnotations;

namespace Authentication.Model;

public class User
{
    [Key]
    public int Id {get; set; }
    [Required]
    [MaxLength(50)]
    public string Name {get;set;}
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email {get;set;}
        
    public string Role { get; set; } = "User";

    [Required]
    public string PasswordHash {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}