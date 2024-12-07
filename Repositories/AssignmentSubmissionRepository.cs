using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class AssignmentSubmissionRepository(IConfiguration config) : IAssignmentSubmissionRepository
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

    public async Task<Exercise?> GetExerciseByIdAsync(string exerciseId)
    {
        return await _entityFramework
            .Exercise
            .FindAsync(exerciseId);
    }

    public async Task<AssignmentSubmission?> GetAssignmentSubmissionByIdAsync(string assignmentSubmissionId)
    {
        return await _entityFramework
            .AssignmentSubmission
            .Include(assignmentSubmission => assignmentSubmission.ExerciseAnswers)
            .FirstOrDefaultAsync(assignmentSubmission => assignmentSubmission.Id == assignmentSubmissionId);
    }
}