using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class Assignment
{
    [Key]
    [Required]
    [MaxLength(50)]
    public string Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("ClassId")]
    public string ClassId { get; set; }
    public virtual Class? Class { get; set; }

    [Required]
    [MaxLength(50)]
    [ForeignKey("ExerciseSetId")]
    public string ExerciseSetId { get; set; }
    public virtual ExerciseSet? ExerciseSet { get; set; }
    
    public virtual ICollection<AssignmentSubmission> AssignmentSubmissionList { get; set; } = [];


    public Assignment()
    {
        Id = Guid.NewGuid().ToString();
        Name ??= string.Empty;
        StartDate = DateTime.Now;
        DueDate = DateTime.Now;
        ClassId ??= string.Empty;
        ExerciseSetId ??= string.Empty;
    }
}