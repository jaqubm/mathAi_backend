namespace mathAi_backend.Dtos;

public class ExerciseSetGeneratorDto
{
    public string? UserId { get; set; } = null;
    
    public string SchoolType { get; set; } = "";
    public int Grade { get; set; }
    public string Subject { get; set; } = "";
    
    public int NumberOfExercises { get; set; } = 0;
}