namespace mathAi_backend.Dtos;

public class AssignmentSubmissionListDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime? SubmissionDate { get; set; }
    public bool Completed { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public UserDto Student { get; set; } = new();
}