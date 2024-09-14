using mathAi_backend.Models;

namespace mathAi_backend.Repositories;

public interface IUserRepository
{
    public bool SaveChanges();
    
    public void AddEntity<T>(T entity);
    public void UpdateEntity<T>(T entity);

    public User? GetUserByEmail(string email);
    public bool UserAlreadyExist(string email);

    public int UserExerciseSetsCount(User user);
    public List<ExerciseSet> GetUsersExerciseSetsByEmail(string email);
}