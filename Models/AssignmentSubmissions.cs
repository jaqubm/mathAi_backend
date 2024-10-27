using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class AssignmentSubmission
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string AssignmentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string StudentId { get; set; }

    public DateTime? SubmissionDate { get; set; }

    [Required]
    public bool Completed { get; set; }

    [ForeignKey("AssignmentId")]
    public virtual Assignment Assignment { get; set; }

    [ForeignKey("StudentId")]
    public virtual User Student { get; set; }

    public virtual ICollection<ExerciseAnswers> ExerciseAnswers { get; set; } = [];

    public AssignmentSubmission(string assignmentId, string studentId)
    {
        Id = Guid.NewGuid().ToString();
        AssignmentId = assignmentId;
        StudentId = studentId;
    }
}
