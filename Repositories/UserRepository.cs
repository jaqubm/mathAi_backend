using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

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

    public User? GetUserByEmail(string email)
    {
        return _entityFramework
            .User
            .FirstOrDefault(u => u.Email == email);
    }

    public bool UserExist(string email)
    {
        var userDb = _entityFramework
            .Find<User>(email);

        return userDb is not null;
    }

    public int UserExerciseSetsCount(User user)
    {
        return _entityFramework
            .ExerciseSet
            .Count(e => e.UserId == user.Email);
    }
    
    public List<ExerciseSet> GetUsersExerciseSetsByEmail(string email)
    {
        var userWithExerciseSets = _entityFramework
            .User
            .Include(u => u.ExerciseSets)
            .FirstOrDefault(u => u.Email == email);
        
        return userWithExerciseSets?.ExerciseSets ?? [];
    }
}