using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;

namespace GameStore.Infrastructure.Repositories;

public class GenreRepository(AppDbContext appDbContext) : IGenreRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public void AddGenre(Genre genre)
    {
        _appDbContext.Genres.Add(genre);
        _appDbContext.SaveChanges();
    }

    public List<Genre> GetAllGenre()
    {
        return [.. _appDbContext.Genres];
    }

    public Genre GetGenre(Guid genreId)
    {
        return _appDbContext.Genres.SingleOrDefault(x => x.Id == genreId);
    }

    public void RemoveGenre(Genre genre)
    {
        _appDbContext.Genres.Remove(genre);
        _appDbContext.SaveChanges();
    }

    public void UpdateGenre(Genre genre)
    {
        _appDbContext.Genres.Update(genre);
        _appDbContext.SaveChanges();
    }
}
