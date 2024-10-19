using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class Assignment
{
    [Key]
    [Required]
    [MaxLength(255)]
    public string Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(255)]
    public string ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    public Class Class { get; set; }

    [Required]
    [MaxLength(255)]
    public string ExerciseSetId { get; set; }
    
    [ForeignKey("ExerciseSetId")]
    public ExerciseSet ExerciseSet { get; set; }

    public Assignment()
    {
        Id = Guid.NewGuid().ToString();
    }
}