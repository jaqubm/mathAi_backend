using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class ExerciseAnswers
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string AssignmentSubmissionId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ExerciseId { get; set; }

    public string StudentAnswer { get; set; }
    public int? Grade { get; set; }
    public string Feedback { get; set; }
    public DateTime? AnsweredDate { get; set; }

    [ForeignKey("AssignmentSubmissionId")]
    public virtual AssignmentSubmission AssignmentSubmission { get; set; }

    [ForeignKey("ExerciseId")]
    public virtual Exercise Exercise { get; set; }
}