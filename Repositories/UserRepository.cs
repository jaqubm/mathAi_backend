using mathAi_backend.Data;
using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public class UserRepository(IConfiguration config) : IUserRepository
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

    public bool UserAlreadyExist(string userId)
    {
        var userDb = _entityFramework.User
            .FirstOrDefault(u => u.UserId == userId);

        return userDb is not null;
    }
}