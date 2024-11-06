namespace mathAi_backend.Dtos;

public class ExerciseSetSettingsDto
{
    public string SchoolType { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int NumberOfExercises { get; set; }
}