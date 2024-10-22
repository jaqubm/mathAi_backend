using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IClassRepository
{
    public bool SaveChanges();

    public void AddEntity<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
    
    public Class? GetClassById(string id);
    public List<Class> GetClassesByOwnerId(string id);
}