namespace mathAi_backend.Dtos;

public class ClassDto
{
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public List<string> ClassStudents { get; set; } = [];
}