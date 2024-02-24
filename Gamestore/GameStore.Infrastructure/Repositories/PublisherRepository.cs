using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class PublisherRepository(AppDbContext appDbContext) : Repository<Publisher>(appDbContext), IPublisherRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Publisher>> GetAllPublishers()
    {
        return await _appDbContext.Publishers.ToListAsync();
    }
}
