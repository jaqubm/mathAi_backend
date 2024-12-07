using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class ExerciseAnswer
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string AssistantId { get; set; }
    
    public int? Grade { get; set; }
    public string? Feedback { get; set; }
    
    [Required]
    [MaxLength(50)]
    [ForeignKey("AssignmentSubmissionId")]
    public string AssignmentSubmissionId { get; set; }
    public virtual AssignmentSubmission AssignmentSubmission { get; set; }
    
    [Required]
    [MaxLength(50)]
    [ForeignKey("ExerciseId")]
    public string ExerciseId { get; set; }
    public virtual Exercise Exercise { get; set; }

    public ExerciseAnswer()
    {
        Id = Guid.NewGuid().ToString();
        AssistantId = string.Empty;
        Feedback = string.Empty;
        AssignmentSubmissionId = string.Empty;
        ExerciseId = string.Empty;
    }
}