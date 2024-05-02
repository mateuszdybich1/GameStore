using GameStore.Domain.IRepositories;

namespace GameStore.Users.Repositories;
public class Repository<T>(IdentityDbContext identityDbContext) : IRepository<T>
    where T : class
{
    private readonly IdentityDbContext _identityDbContext = identityDbContext;

    public async Task Add(T entity)
    {
        _identityDbContext.Set<T>().Add(entity);
        await _identityDbContext.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        _identityDbContext.Set<T>().Remove(entity);
        await _identityDbContext.SaveChangesAsync();
    }

    public async Task<T> Get(Guid id)
    {
        return await _identityDbContext.Set<T>().FindAsync(id);
    }

    public async Task Update(T entity)
    {
        _identityDbContext.Set<T>().Update(entity);
        await _identityDbContext.SaveChangesAsync();
    }
}
