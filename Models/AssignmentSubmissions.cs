using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class AssignmentSubmission
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; }
    
    public DateTime? SubmissionDate { get; set; }

    [Required]
    public bool Completed { get; set; }
    
    [Required]
    [MaxLength(50)]
    [ForeignKey("AssignmentId")]
    public string AssignmentId { get; set; }
    public virtual Assignment? Assignment { get; set; }
    
    [Required]
    [MaxLength(21)]
    [ForeignKey("StudentId")]
    public string StudentId { get; set; }
    public virtual User? Student { get; set; }

    public virtual ExerciseAnswer ExerciseAnswer { get; set; } = new();

    public AssignmentSubmission()
    {
        Id = Guid.NewGuid().ToString();
        AssignmentId ??= string.Empty;
        StudentId ??= string.Empty;
    }
}
