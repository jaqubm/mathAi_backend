using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class ClassStudents
{
    [Required]
    [MaxLength(50)]
    [ForeignKey("ClassId")]
    public string ClassId { get; set; }
    public virtual Class? Class { get; set; }

    [Required]
    [MaxLength(255)]
    [ForeignKey("StudentId")]
    public string StudentId { get; set; }
    public virtual User? Student { get; set; }

    public ClassStudents()
    {
        ClassId ??= string.Empty;
        StudentId ??= string.Empty;
    }
}