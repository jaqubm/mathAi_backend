using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class ClassStudents
{
    [Required]
    [MaxLength(255)]
    public string ClassId { get; set; }
    
    [ForeignKey("ClassId")]
    public Class Class { get; set; }

    [Required]
    [MaxLength(255)]
    public string StudentId { get; set; }
    
    [ForeignKey("StudentId")]
    public User Student { get; set; }

    public ClassStudents()
    {
    }

    public ClassStudents(string classId, string studentId)
    {
        ClassId = classId;
        StudentId = studentId;
    }
}