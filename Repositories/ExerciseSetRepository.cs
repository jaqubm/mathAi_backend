using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class ExerciseSetRepository(IConfiguration config) : IExerciseSetRepository
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

    public ExerciseSet? GetExerciseSetWithExercisesById(string exerciseSetId)
    {
        var exerciseSetDb = _entityFramework
            .ExerciseSet
            .Include(es => es.Exercises)
            .FirstOrDefault(es => es.Id == exerciseSetId);
        
        return exerciseSetDb;
    }
}