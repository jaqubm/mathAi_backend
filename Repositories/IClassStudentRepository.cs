namespace mathAi_backend.Repositories;

public interface IClassStudentRepository
{
    public bool SaveChanges();

    public void AddEntity<T>(T entity);
    public void UpdateEntity<T>(T entity);
    public void DeleteEntity<T>(T entity);
}