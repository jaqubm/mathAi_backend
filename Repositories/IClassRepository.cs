using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IClassRepository
{
    public Task<bool> SaveChangesAsync();

    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
    
    public Task<Class?> GetClassByIdAsync(string id);
    public Task<List<Class>> GetClassesByOwnerIdAsync(string id);
    public Task<List<Class>> GetClassesByStudentIdAsync(string id);
}