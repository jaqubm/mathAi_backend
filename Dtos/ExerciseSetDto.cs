namespace mathAi_backend.Dtos;

public class ExerciseSetDto
{
    public string Name { get; set; } = string.Empty;
    public string SchoolType { get; set; } = string.Empty;
    public int Grade { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Personalized  { get; set; } = string.Empty;
    public bool IsOwner { get; set; }
    public IEnumerable<ExerciseDetailedDto> ExerciseList { get; set; } = [];
}
