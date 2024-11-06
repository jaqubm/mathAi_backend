namespace mathAi_backend.Dtos;

public class ExerciseDto
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FirstHint { get; set; } = string.Empty;
    public string SecondHint { get; set; } = string.Empty;
    public string ThirdHint { get; set; } = string.Empty;
    public string Solution { get; set; } = string.Empty;
}