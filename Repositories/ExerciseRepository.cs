using mathAi_backend.Data;
using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public class ExerciseRepository(IConfiguration config) : IExerciseRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Add(entity);
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

    public Exercise? GetExerciseById(string exerciseId)
    {
        var exerciseDb = _entityFramework
            .Find<Exercise>(exerciseId);
        
        return exerciseDb;
    }
}