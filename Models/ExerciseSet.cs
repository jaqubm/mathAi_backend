using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace mathAi_backend.Models;

public class ExerciseSet
{
    [Key]
    [Required]
    [MinLength(255)]
    public string ExerciseSetId { get; init; }
    
    [Required]
    public List<Exercise> Exercises { get; set; }

    public ExerciseSet()
    {
        ExerciseSetId = Guid.NewGuid().ToString();
        Exercises = new List<Exercise>();
    }

    public ExerciseSet(List<Exercise> exercises)
    {
        ExerciseSetId = Guid.NewGuid().ToString();
        Exercises = exercises;
    }
}