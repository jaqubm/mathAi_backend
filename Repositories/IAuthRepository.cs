namespace mathAi_backend.Repositories;

public interface IAuthRepository
{
    public Task<bool> SaveChangesAsync();
    
    public Task AddEntityAsync<T>(T entity);
    
    public Task<bool> CheckIfUserExistsByIdAsync(string userId);
}