namespace Authentication.Repository
{
    public interface IRepository<T> where T : class
    { 
        Task CreateAsync(T entity);
        Task SaveAsync();
    }
}