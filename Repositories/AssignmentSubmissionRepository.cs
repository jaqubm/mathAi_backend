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

    public void UpdateEntity<T>(T entity)
    {
        if (entity is not null)
            _entityFramework.Update(entity);
    }

    public async Task<Exercise?> GetExerciseByIdAsync(string exerciseId)
    {
        return await _entityFramework
            .Exercise
            .FindAsync(exerciseId);
    }

    public async Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId)
    {
        return await _entityFramework
            .ExerciseSet
            .Include(exerciseSet => exerciseSet.ExerciseList)
            .FirstOrDefaultAsync(exerciseSet => exerciseSet.Id == exerciseSetId);
    }

    public async Task<ICollection<ExerciseAnswer>> GetExerciseAnswerListByAssignmentSubmissionIdAsync(string assignmentSubmissionId)
    {
        return await _entityFramework
            .ExerciseAnswer
            .Where(exerciseAnswer => exerciseAnswer.AssignmentSubmissionId == assignmentSubmissionId)
            .ToListAsync();
    }

    public async Task<AssignmentSubmission?> GetAssignmentSubmissionByIdAsync(string assignmentSubmissionId)
    {
        return await _entityFramework
            .AssignmentSubmission
            .Include(assignmentSubmission => assignmentSubmission.Assignment)
            .Include(assignmentSubmission => assignmentSubmission.ExerciseAnswerList)
            .FirstOrDefaultAsync(assignmentSubmission => assignmentSubmission.Id == assignmentSubmissionId);
    }
}