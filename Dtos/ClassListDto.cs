namespace mathAi_backend.Dtos;

public class ClassListDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UserDto Owner { get; set; } = new ();
}