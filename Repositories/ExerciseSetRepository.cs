using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class ExerciseSetRepository(IConfiguration config) : IExerciseSetRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _entityFramework.SaveChangesAsync() > 0;
    }

    public async Task AddEntityAsync<T>(T entity)
    {
        if (entity is not null)
            await _entityFramework.AddAsync(entity);
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

    public async Task<int> GetUserExerciseSetsCountAsync(string userId)
    {
        var userExerciseSetsCount = await _entityFramework.
            ExerciseSet
            .Where(es => es.UserId == userId)
            .CountAsync();
        
        return userExerciseSetsCount;
    }

    public async Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId)
    {
        return await _entityFramework
            .ExerciseSet
            .Include(es => es.ExerciseList)
            .FirstOrDefaultAsync(es => es.Id == exerciseSetId);
    }
}