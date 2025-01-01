using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IAssignmentSubmissionRepository
{
    public Task<bool> SaveChangesAsync();
    
    public void UpdateEntity<T>(T entity);
    
    public Task<Exercise?> GetExerciseByIdAsync(string exerciseId);
    public Task<ExerciseSet?> GetExerciseSetByIdAsync(string exerciseSetId);
    
    public Task<ICollection<ExerciseAnswer>> GetExerciseAnswerListByAssignmentSubmissionIdAsync(string assignmentSubmissionId);
    
    public Task<AssignmentSubmission?> GetAssignmentSubmissionByIdAsync(string assignmentSubmissionId);
}