using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace mathAi_backend.Models;

public class Exercise
{
    [Key]
    [Required]
    [MaxLength(255)]
    public string Id { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public string FirstHint { get; set; }
    
    [Required]
    public string SecondHint { get; set; }
    
    [Required]
    public string ThirdHint { get; set; }
    
    [Required]
    public string Solution { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string ExerciseSetId { get; set; }
    
    [ForeignKey("ExerciseSetId")]
    public ExerciseSet? ExerciseSet { get; set; }

    public Exercise()
    {
        Id = Guid.NewGuid().ToString();
        Content = "";
        FirstHint = "";
        SecondHint = "";
        ThirdHint = "";
        Solution = "";
        ExerciseSetId = "";
    }

    public Exercise(string jsonString, string exerciseSetId)
    {
        if (jsonString.StartsWith("```json") && jsonString.EndsWith("```"))
        {
            var lines = jsonString.Split('\n');
        
            if (lines.Length > 2)
                jsonString = string.Join("\n", lines.Skip(1).Take(lines.Length - 2));
        }
        
        Console.WriteLine(jsonString);
        
        var exerciseData = JsonSerializer.Deserialize<Exercise>(jsonString);

        if (exerciseData != null)
        {
            Id = Guid.NewGuid().ToString();
            Content = exerciseData.Content;
            FirstHint = exerciseData.FirstHint;
            SecondHint = exerciseData.SecondHint;
            ThirdHint = exerciseData.ThirdHint;
            Solution = exerciseData.Solution;
            ExerciseSetId = exerciseSetId;
        }
        else
        {
            throw new ArgumentException("Invalid JSON format");
        }
    }
}