using System.ComponentModel.DataAnnotations;

namespace mathAi_backend.Models;

public class User(string userId, string name, string email, string picture)
{
    [Key]
    [MaxLength(255)]
    public string UserId { get; init; } = userId;

    [Required]
    [MaxLength(255)]
    public string Name { get; init; } = name;

    [Required]
    [MaxLength(255)]
    public string Email { get; init; } = email;
    
    public string Picture { get; init; } = picture;
}