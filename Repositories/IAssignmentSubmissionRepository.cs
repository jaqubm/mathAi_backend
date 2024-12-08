using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IAssignmentSubmissionRepository
{
    public Task<bool> SaveChangesAsync();
    
    public void UpdateEntity<T>(T entity);
    
    public Task<Exercise?> GetExerciseByIdAsync(string exerciseId);
    public Task<AssignmentSubmission?> GetAssignmentSubmissionByIdAsync(string assignmentSubmissionId);
}