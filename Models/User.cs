using System.ComponentModel.DataAnnotations;

namespace mathAi_backend.Models;

public class User
{
    [Key]
    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    public bool IsTeacher { get; set; }

    public User()
    {
        Email ??= "";
        Name ??= "";
    }

    public User(string email, string name, bool isTeacher = false)
    {
        Email = email;
        Name = name;
        IsTeacher = isTeacher;
    }
}