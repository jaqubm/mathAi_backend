using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class Class
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
    public string OwnerId { get; set; }
    
    [ForeignKey("OwnerId")]
    public User Owner { get; set; }
    
    public List<ClassStudents> ClassStudents { get; set; }

    public List<Assignment> Assignments { get; set; }

    public Class()
    {
        Id = Guid.NewGuid().ToString();
        ClassStudents = [];
        Assignments = [];
    }
}