using GameStore.Domain.IRepositories;

namespace GameStore.Infrastructure.Repositories;

public class Repository<T>(AppDbContext appDbContext) : IRepository<T>
    where T : class
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task Add(T entity)
    {
        _appDbContext.Set<T>().Add(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        _appDbContext.Set<T>().Remove(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<T> Get(Guid id)
    {
        return await _appDbContext.Set<T>().FindAsync(id);
    }

    public async Task Update(T entity)
    {
        _appDbContext.Set<T>().Update(entity);
        await _appDbContext.SaveChangesAsync();
    }
}
