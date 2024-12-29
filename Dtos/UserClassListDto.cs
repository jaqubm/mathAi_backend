namespace mathAi_backend.Dtos;

public class UserClassListDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public UserDto Owner { get; set; } = new ();
    public bool IsOwner { get; set; }
}