using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class AssignmentRepository(IConfiguration config) : IAssignmentRepository
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

    public void DeleteEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Remove(entity);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _entityFramework
            .User
            .FindAsync(userId);
    }

    public async Task<Class?> GetClassByIdAsync(string classId)
    {
        return await _entityFramework
            .Class
            .Include(c => c.ClassStudentList)
            .FirstOrDefaultAsync(c => c.Id == classId);
    }

    public async Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId)
    {
        return await _entityFramework
            .ExerciseSet
            .FindAsync(exerciseSetId);
    }

    public async Task<Assignment?> GetAssignmentByIdAsync(string assignmentId)
    {
        return await _entityFramework
            .Assignment
            .Include(a => a.Class)
            .Include(a => a.AssignmentSubmissionList)
            .ThenInclude(s => s.ExerciseAnswerList)
            .FirstOrDefaultAsync(a => a.Id == assignmentId);
    }
}