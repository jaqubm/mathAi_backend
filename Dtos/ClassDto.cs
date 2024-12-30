using System.Collections;

namespace mathAi_backend.Dtos;

public class ClassDto
{
    public string Name { get; set; } = string.Empty;
    public UserDto Owner { get; set; } = new ();
    public bool IsOwner { get; set; }
    public ICollection<UserDto> StudentList { get; set; } = [];
    public IEnumerable<AssignmentListDto> AssignmentList { get; set; } = [];   // TODO: In future update it to pass less info and more specific
}