namespace mathAi_backend.Dtos;

public class UserDto
{
    public string Email { get; set; }
    public string Name { get; set; }
    public bool IsTeacher { get; set; }

    public UserDto()
    {
        Email ??= "";
        Name ??= "";
    }
}