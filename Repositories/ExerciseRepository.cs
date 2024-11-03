using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class ExerciseRepository(IConfiguration config) : IExerciseRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _entityFramework.SaveChangesAsync() > 0;
    }

    public void UpdateEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Update(entity);
    }

    public void DeleteEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Remove(entity);
    }

    public async Task<Exercise?> GetExerciseByIdAsync(string exerciseId)
    {
        return await _entityFramework
            .Exercise
            .FirstOrDefaultAsync(exercise => exercise.Id == exerciseId);
    }
}