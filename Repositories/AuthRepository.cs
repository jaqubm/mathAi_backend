using mathAi_backend.Data;

namespace mathAi_backend.Repositories;

public class AuthRepository(IConfiguration config) : IAuthRepository
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

    public async Task<bool> CheckIfUserExistsByIdAsync(string userId)
    {
        var userDb = await _entityFramework
            .User
            .FindAsync(userId);

        return userDb is not null;
    }
}