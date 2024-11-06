namespace mathAi_backend.Dtos;

public class ClassCreatorDto
{
    public string Name { get; set; } = string.Empty;
    public List<string> ClassStudentsIdList { get; set; } = [];
}