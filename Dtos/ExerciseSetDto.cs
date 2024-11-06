using mathAi_backend.Models;

namespace mathAi_backend.Dtos;

public class ExerciseSetDto
{
    public string Name { get; set; } = string.Empty;
    public string SchoolType { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<ExerciseDto> Exercises { get; set; } = [];
}
