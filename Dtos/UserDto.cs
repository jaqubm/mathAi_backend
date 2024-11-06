namespace mathAi_backend.Dtos;

public class UserDto
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsTeacher { get; set; }
    public bool FirstTimeSignIn { get; set; }
}