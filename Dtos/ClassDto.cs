namespace mathAi_backend.Dtos;

public class ClassDto
{
    public string Name { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public List<string> ClassStudents { get; set; } = [];
}