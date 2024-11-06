using System.ComponentModel.DataAnnotations;

namespace mathAi_backend.Models;

public class User
{
    [Key]
    [Required]
    [MaxLength(21)]
    public string Id { get; set; }
    
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
    public virtual ICollection<ClassStudent> StudentClasses { get; set; } = [];
    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = [];

    public User()
    {
        Id ??= string.Empty;
        Email ??= string.Empty;
        Name ??= string.Empty;
        IsTeacher = true;
        FirstTimeSignIn = true;
    }
}