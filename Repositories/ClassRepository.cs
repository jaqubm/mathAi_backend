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

    public async Task<Class?> GetClassByIdAsync(string id)
    {
        return await _entityFramework
            .Class
            .Include(c => c.ClassStudents)
            .Include(c => c.Assignments)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Class>> GetClassesByOwnerIdAsync(string id)
    {
        return await _entityFramework
            .Class
            .Include(c => c.ClassStudents)
            .Where(c => c.OwnerId == id)
            .ToListAsync();
    }

    public async Task<List<Class>> GetClassesByStudentIdAsync(string id)
    {
        var classIdsList = await _entityFramework
            .ClassStudents
            .Where(cs => cs.StudentId == id).Select(cs => cs.ClassId)
            .ToListAsync();
        
        var studentClasses = new List<Class>();

        foreach (var classId in classIdsList)
        {
            var classDb = await GetClassByIdAsync(classId);
            if (classDb is null) continue;
            studentClasses.Add(classDb);
        }

        return studentClasses;
    }
}