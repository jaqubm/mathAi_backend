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
    
    [Required]
    public bool FirstTimeSignIn { get; set; }
    
    public List<ExerciseSet> ExerciseSets { get; set; }

    public User()
    {
        Email ??= "";
        Name ??= "";
        ExerciseSets = [];
    }

    public User(string email, string name, bool isTeacher = false, bool firstTimeSignIn = true, List<ExerciseSet>? exerciseSets = null)
    {
        Email = email;
        Name = name;
        IsTeacher = isTeacher;
        FirstTimeSignIn = firstTimeSignIn;
        ExerciseSets = exerciseSets ?? [];
    }
}