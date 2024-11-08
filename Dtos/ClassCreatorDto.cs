namespace mathAi_backend.Dtos;

public class ClassCreatorDto
{
    public string Name { get; set; } = string.Empty;
    public IEnumerable<string> StudentEmailList { get; set; } = [];
}