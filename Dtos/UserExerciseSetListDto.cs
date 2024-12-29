namespace mathAi_backend.Dtos;

public class UserExerciseSetListDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string SchoolType { get; set; } = "";
    public string Grade { get; set; } = "";
    public string Subject { get; set; } = "";
    public string? Personalized  { get; set; } = string.Empty;
}