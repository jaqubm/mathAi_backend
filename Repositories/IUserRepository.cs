using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IUserRepository
{
    public Task<bool> SaveChangesAsync();
    
    public Task AddEntityAsync<T>(T entity);
    public void UpdateEntity<T>(T entity);

    public Task<User?> GetUserByEmailAsync(string email);
    public Task<bool> UserExistAsync(string email);

    public Task<int> UserExerciseSetsCountAsync(User user);
    public Task<List<ExerciseSet>> GetUsersExerciseSetsByEmailAsync(string email);
    public Task<List<AssignmentSubmission>> GetAssignmentSubmissionsByEmailAsync(string email);
}