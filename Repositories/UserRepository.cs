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

    public void UpdateEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Update(entity);
    }

    public User GetUserByEmail(string email)
    {
        var userDb = _entityFramework.User.FirstOrDefault(u => u.Email == email);
        
        if (userDb is null) throw new Exception("Failed to find user with email: " + email);
        
        return userDb;
    }

    public bool UserAlreadyExist(string email)
    {
        var userDb = _entityFramework.User
            .FirstOrDefault(u => u.Email == email);

        return userDb is not null;
    }
}