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

    public virtual ICollection<ExerciseSet> ExerciseSets { get; set; } = [];
    public virtual ICollection<Class> Classes { get; set; } = [];
    public virtual ICollection<ClassStudents> StudentClasses { get; set; } = [];
    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = [];

    public User()
    {
        Email ??= string.Empty;
        Name ??= string.Empty;
        IsTeacher = false;
        FirstTimeSignIn = true;
    }
}