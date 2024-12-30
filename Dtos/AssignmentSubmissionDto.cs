namespace mathAi_backend.Dtos;

public class AssignmentSubmissionDto
{
    public string Id { get; set; } = string.Empty;
    public string AssignmentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public ICollection<ExerciseDto> ExerciseList { get; set; } = [];
}