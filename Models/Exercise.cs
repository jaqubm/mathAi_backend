using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mathAi_backend.Models;

public class Exercise
{
    [Key]
    [Required]
    [MaxLength(255)]
    public string ExerciseId { get; init; }
    
    [Required]
    public string ExerciseContent { get; set; }
    
    [Required]
    public string FirstExerciseHint { get; set; }
    
    [Required]
    public string SecondExerciseHint { get; set; }
    
    [Required]
    public string ThirdExerciseHint { get; set; }
    
    [Required]
    public string ExerciseSolution { get; set; }
    
    [Required]
    public string ExerciseSetId { get; set; }
    
    [ForeignKey("ExerciseSetId")]
    public ExerciseSet ExerciseSet { get; set; }

    public Exercise()
    {
        ExerciseId = Guid.NewGuid().ToString();
        ExerciseContent = "";
        FirstExerciseHint = "";
        SecondExerciseHint = "";
        ThirdExerciseHint = "";
        ExerciseSolution = "";
    }

    public Exercise(string exerciseContent, string firstExerciseHint, string secondExerciseHint, string thirdExerciseHint, string exerciseSolution, string exerciseSetId)
    {
        ExerciseId = Guid.NewGuid().ToString();
        ExerciseContent = exerciseContent;
        FirstExerciseHint = firstExerciseHint;
        SecondExerciseHint = secondExerciseHint;
        ThirdExerciseHint = thirdExerciseHint;
        ExerciseSolution = exerciseSolution;
        ExerciseSetId = exerciseSetId;
    }
}