using mathAi_backend.Data;
using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Repositories;

public class ClassRepository(IConfiguration config) : IClassRepository
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

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _entityFramework
            .User
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Class?> GetClassByIdAsync(string classId)
    {
        return await _entityFramework
            .Class
            .Include(c => c.Owner)
            .Include(c => c.ClassStudents)
            .Include(c => c.Assignments)
            .FirstOrDefaultAsync(c => c.Id == classId);
    }
}