using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class GenreRepository(AppDbContext appDbContext) : Repository<Genre>(appDbContext), IGenreRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Genre>> GetAllGenre()
    {
        return await _appDbContext.Genres.ToListAsync();
    }
}
