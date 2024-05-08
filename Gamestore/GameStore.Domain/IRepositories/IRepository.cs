namespace GameStore.Domain.IRepositories;
public interface IRepository<T>
{
    Task Add(T entity);

    Task Update(T entity);

    Task Delete(T entity);

    Task<T> Get(Guid id);
}
