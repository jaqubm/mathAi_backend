using mathAi_backend.Data;

namespace mathAi_backend.Repositories;

public class ClassStudentRepository(IConfiguration config) : IClassStudentRepository
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

    public List<string> GetClassIdsByStudentId(string studentId)
    {
        var classIdsList = _entityFramework
            .ClassStudents
            .Where(cs => cs.StudentId == studentId).Select(cs => cs.ClassId)
            .ToList();

        return classIdsList;
    }
}