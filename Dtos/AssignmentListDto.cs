namespace mathAi_backend.Dtos;

public class AssignmentListDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime DueDate { get; set; } = DateTime.Today;
    public string ClassId { get; set; } = string.Empty;
    public string ExerciseSetId { get; set; } = string.Empty;
}