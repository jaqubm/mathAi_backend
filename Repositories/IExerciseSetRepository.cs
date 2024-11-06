using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IExerciseSetRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
    
    public Task<int> GetUserExerciseSetsCountAsync(string userId);
    public Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId);
}