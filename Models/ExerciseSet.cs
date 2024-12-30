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
    [MaxLength(30)]
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
    public string Personalized { get; set; }
    
    [MaxLength(21)]
    [ForeignKey("UserId")]
    public string? UserId { get; set; }
    public virtual User? User { get; set; }

    public virtual ICollection<Exercise> ExerciseList { get; set; } = [];

    public ExerciseSet()
    {
        Id = Guid.NewGuid().ToString();
        Name ??= string.Empty;
        SchoolType ??= string.Empty;
        Subject ??= string.Empty;
        Personalized ??= string.Empty;
    }
}