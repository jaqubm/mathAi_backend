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
    public virtual ICollection<ExerciseSet> ExerciseSets { get; set; } = [];

    // One-to-many relationship with Classes (for Teachers)
    public virtual ICollection<Class> Classes { get; set; } = [];

    // Many-to-many relationship with Classes (for Students)
    public virtual ICollection<ClassStudents> StudentClasses { get; set; } = [];

    // Many-to-many relationship with Assignments (for Students)
    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = [];


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