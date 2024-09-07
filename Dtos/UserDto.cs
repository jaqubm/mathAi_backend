namespace mathAi_backend.Dtos;

public class UserDto
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public UserDto()
    {
        UserId ??= "";
        Name ??= "";
        Email ??= "";
    }
}