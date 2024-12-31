namespace mathAi_backend.Dtos;

public class AssignmentDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now;
    public string ClassId { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ExerciseSetId { get; set; } = string.Empty;
    public virtual ICollection<AssignmentSubmissionListDto> AssignmentSubmissionList { get; set; } = [];
}