using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class ExerciseSet
{
    [Key]
    [Required]
    [MaxLength(255)]
    public string Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string SchoolType { get; set; }
    
    [Required]
    public int Grade { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Subject { get; set; }

    public virtual List<Exercise> Exercises { get; set; } = [];
    
    [MaxLength(255)]
    public string? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    public ExerciseSet()
    {
        Id = Guid.NewGuid().ToString();
        Name ??= "";
        SchoolType ??= "";
        Subject ??= "";
    }
}