using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class ExerciseSet
{
    [Key]
    [Required]
    [MaxLength(50)]
    public string Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string SchoolType { get; set; }
    
    [Required]
    public int Grade { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Subject { get; set; }
    
    [MaxLength(255)]
    [ForeignKey("UserId")]
    public string? UserId { get; set; }
    public virtual User? User { get; set; }

    public virtual ICollection<Exercise> Exercises { get; set; } = [];

    public ExerciseSet()
    {
        Id = Guid.NewGuid().ToString();
        Name ??= "";
        SchoolType ??= "";
        Subject ??= "";
    }
}