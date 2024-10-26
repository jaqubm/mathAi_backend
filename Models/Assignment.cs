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
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    public virtual Class? Class { get; set; }

    [Required]
    [MaxLength(50)]
    public string ExerciseSetId { get; set; }
    
    [ForeignKey("ExerciseSetId")]
    public virtual ExerciseSet? ExerciseSet { get; set; }
    
    public virtual ICollection<AssignmentSubmission> Submissions { get; set; } = [];


    public Assignment()
    {
        Id = Guid.NewGuid().ToString();
        Name ??= "";
        StartDate = DateTime.Now;
        DueDate = DateTime.Now;
        ClassId ??= "";
        ExerciseSetId ??= "";
    }

    public Assignment(string name, DateTime startDate, DateTime dueDate, string classId, string exerciseSetId)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        StartDate = startDate;
        DueDate = dueDate;
        ClassId = classId;
        ExerciseSetId = exerciseSetId;
    }
}