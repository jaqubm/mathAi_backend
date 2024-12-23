namespace mathAi_backend.Dtos;

public class AssignmentDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now;
    public string ClassId { get; set; } = string.Empty;
    public ClassDto Class { get; set; } = new();
    public string ExerciseSetId { get; set; } = string.Empty;
    public ICollection<AssignmentSubmissionListDto> Submissions { get; set; } = [];
}