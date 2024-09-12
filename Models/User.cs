using System.ComponentModel.DataAnnotations;

namespace mathAi_backend.Models;

public class User
{
    [Key]
    [Required]
    [MaxLength(255)]
    public string Email { get; init; }

    [Required]
    [MaxLength(255)]
    public string Name { get; init; }
    
    [Required]
    public bool IsTeacher { get; set; }
    
    public bool FirstTimeSignIn { get; set; }

    public User()
    {
        Email ??= "";
        Name ??= "";
    }

    public User(string email, string name, bool isTeacher = false, bool firstTimeSignIn = true)
    {
        Email = email;
        Name = name;
        IsTeacher = isTeacher;
        FirstTimeSignIn = firstTimeSignIn;
    }
}