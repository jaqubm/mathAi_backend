namespace mathAi_backend.Dtos;

public class AssignmentDto
{
    public string Name { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime DueDate { get; set; } = DateTime.Today;
    public string ClassId { get; set; } = "";
    public string ExerciseSetId { get; set; } = "";
}