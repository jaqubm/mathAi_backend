using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class ClassRepository(IConfiguration config) : IClassRepository
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

    public Class? GetClassById(string id)
    {
        return _entityFramework
            .Class
            .Include(c => c.ClassStudents)
            .Include(c => c.Assignments)
            .FirstOrDefault(c => c.Id == id);
    }

    public List<Class> GetClassesByOwnerId(string id)
    {
        return _entityFramework.Class
            .Include(c => c.ClassStudents)
            .Where(c => c.OwnerId == id)
            .ToList();
    }

    public List<Class> GetClassesByStudentId(string id)
    {
        var classIdsList = _entityFramework
            .ClassStudents
            .Where(cs => cs.StudentId == id).Select(cs => cs.ClassId)
            .ToList();
        
        var studentClasses = new List<Class>();
        
        classIdsList.ForEach(x =>
        {
            var classDb = GetClassById(x);
            if (classDb is null) return;
            studentClasses.Add(classDb);
        });

        return studentClasses;
    }
}