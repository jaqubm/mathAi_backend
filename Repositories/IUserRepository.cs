using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IUserRepository
{
    public bool SaveChanges();
    
    public void AddEntity<T>(T entity);
    
    public bool UserAlreadyExist(string userId);
}