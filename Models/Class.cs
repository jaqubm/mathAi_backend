using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class Class
{
    [Key]
    [Required]
    [MaxLength(50)]
    public string Id { get; set; }
    
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(21)]
    [ForeignKey("OwnerId")]
    public string OwnerId { get; set; }
    public virtual User? Owner { get; set; }

    public virtual List<ClassStudent> ClassStudentList { get; set; } = [];
    public virtual List<Assignment> AssignmentList { get; set; } = [];

    public Class()
    {
        Id = Guid.NewGuid().ToString();
        Name ??= string.Empty;
        OwnerId ??= string.Empty;
    }
}