using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IAssignmentRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
    
    public Task<User?> GetUserByIdAsync(string userId);
    public Task<Class?> GetClassByIdAsync(string classId);
    public Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId);
    
    public Task<Assignment?> GetAssignmentByIdAsync(string assignmentId);
}