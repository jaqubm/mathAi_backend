using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IExerciseRepository
{
    public Task<bool> SaveChangesAsync();
    
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
    
    public Task<Exercise?> GetExerciseByIdAsync(string exerciseId);
    public Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId);
}