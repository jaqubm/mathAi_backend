namespace mathAi_backend.Dtos;

public class UserAssignmentSubmissionListDto
{
    public string Id { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string AssignmentId { get; set; } = string.Empty;
    public string AssignmentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
}