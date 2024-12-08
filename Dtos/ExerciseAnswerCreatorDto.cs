namespace mathAi_backend.Dtos;

public class ExerciseAnswerCreatorDto
{
    public string AssignmentSubmissionId { get; set; } = string.Empty;
    public string ExerciseId { get; set; } = string.Empty;
    public IFormFile? AnswerImageFile { get; set; }
}