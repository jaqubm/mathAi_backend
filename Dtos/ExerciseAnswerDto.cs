namespace mathAi_backend.Dtos;

public class ExerciseAnswerDto
{
    public string Id { get; set; } = string.Empty;
    public string ExerciseId { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Feedback { get; set; } = string.Empty;
    // public IFormFile? AnswerImageFile { get; set; }
}