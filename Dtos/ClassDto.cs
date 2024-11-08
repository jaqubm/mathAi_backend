namespace mathAi_backend.Dtos;

public class ClassDto
{
    public string Name { get; set; } = string.Empty;
    public UserDto Owner { get; set; } = new ();
    public bool IsOwner { get; set; }
    public List<UserDto> Students { get; set; } = [];
    // TODO: Implement Assignments!!!
}