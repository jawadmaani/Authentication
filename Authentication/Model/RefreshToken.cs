using System.ComponentModel.DataAnnotations;

namespace Authentication.Model;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string RefreshTokenHash { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    [Required] 
    public int UserId { get; set; }
    public User  User { get; set; }
}