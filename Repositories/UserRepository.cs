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

    public void UpdateEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Update(entity);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _entityFramework
            .User
            .FindAsync(userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var userDb = await _entityFramework
            .User
            .FirstOrDefaultAsync(u => u.Email == email);

        return userDb;
    }

    public async Task<List<ExerciseSet>> GetExerciseSetListByUserIdAsync(string userId)
    {
        var listOfExerciseSets = await _entityFramework
            .ExerciseSet
            .Where(s => s.UserId == userId)
            .ToListAsync();
        
        return listOfExerciseSets;
    }

    public async Task<List<Class>> GetClassListByOwnerIdAsync(string ownerId)
    {
        var classList = await _entityFramework
            .Class
            .Include(c => c.Owner)
            .Include(c => c.ClassStudentList)
            .Where(c => c.OwnerId == ownerId)
            .ToListAsync();
        
        return classList;
    }

    public async Task<List<Class>> GetClassListByStudentIdAsync(string userId)
    {
        var classList = await _entityFramework
            .Class
            .Include(c => c.Owner)
            .Include(c => c.ClassStudentList)
            .Where(c => c.ClassStudentList.Any(s => s.StudentId == userId))
            .ToListAsync();
        
        return classList;
    }

    public async Task<List<AssignmentSubmission>> GetAssignmentSubmissionListByUserIdAsync(string userId)
    {
        var assignmentSubmissionList = await _entityFramework
            .AssignmentSubmission
            .Include(s => s.Assignment)
            .ThenInclude(a => a.Class)
            .Where(s => s.StudentId == userId)
            .ToListAsync();
        
        return assignmentSubmissionList;
    }
}