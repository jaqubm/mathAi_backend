using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    // One-to-many relationship with ExerciseSets
    public virtual List<ExerciseSet> ExerciseSets { get; set; } = [];

    // One-to-many relationship with Classes (for Teachers)
    public virtual List<Class> Classes { get; set; } = [];

    // Many-to-many relationship with Classes (for Students)
    public virtual List<ClassStudents> StudentClasses { get; set; } = [];

    public User()
    {
        Email ??= "";
        Name ??= "";
        FirstTimeSignIn = true;
    }

    public User(string email, string name, bool isTeacher = false, bool firstTimeSignIn = true)
    {
        Email = email;
        Name = name;
        IsTeacher = isTeacher;
        FirstTimeSignIn = firstTimeSignIn;
    }
}