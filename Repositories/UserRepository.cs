using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class UserRepository(IConfiguration config) : IUserRepository
{
    private readonly DataContext _entityFramework = new(config);
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _entityFramework.SaveChangesAsync() > 0;
    }

    public async Task AddEntityAsync<T>(T entity)
    {
        if (entity is not null)
            await _entityFramework.AddAsync(entity);
    }

    public void UpdateEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Update(entity);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _entityFramework
            .User
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UserExistAsync(string email)
    {
        var userDb = await _entityFramework
            .User
            .FirstOrDefaultAsync(user => user.Email == email);

        return userDb is not null;
    }

    public async Task<int> UserExerciseSetsCountAsync(User user)
    {
        return await _entityFramework
            .ExerciseSet
            .CountAsync(e => e.UserId == user.Email);
    }
    
    public async Task<List<ExerciseSet>> GetUsersExerciseSetsByEmailAsync(string email)
    {
        var userWithExerciseSets = await _entityFramework
            .User
            .Include(u => u.ExerciseSets)
            .FirstOrDefaultAsync(u => u.Email == email);
        
        return userWithExerciseSets?.ExerciseSets as List<ExerciseSet> ?? [];
    }

    public async Task<List<AssignmentSubmission>> GetAssignmentSubmissionsByEmailAsync(string email)
    {
        var userWithAssignmentSubmissions = await _entityFramework
            .User
            .Include(u => u.AssignmentSubmissions)
            .FirstOrDefaultAsync(u => u.Email == email);
        
        return userWithAssignmentSubmissions?.AssignmentSubmissions as List<AssignmentSubmission> ?? [];
    }
}