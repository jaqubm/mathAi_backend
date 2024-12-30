using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IUserRepository
{
    public Task<bool> SaveChangesAsync();
    
    public void UpdateEntity<T>(T entity);

    public Task<User?> GetUserByIdAsync(string userId);
    public Task<User?> GetUserByEmailAsync(string email);
    
    public Task<List<ExerciseSet>> GetExerciseSetListByUserIdAsync(string userId);
    
    public Task<List<Class>> GetClassListByOwnerIdAsync(string ownerId);
    public Task<List<Class>> GetClassListByStudentIdAsync(string userId);
    
    public Task<List<AssignmentSubmission>> GetAssignmentSubmissionListByUserIdAsync(string userId);
}