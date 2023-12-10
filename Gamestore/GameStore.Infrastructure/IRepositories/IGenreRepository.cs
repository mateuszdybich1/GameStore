using GameStore.Infrastructure.Entities;

namespace GameStore.Infrastructure.IRepositories;
public interface IGenreRepository
{
    public void AddGenre(Genre genre);

    public Genre GetGenre(Guid genreId);

    public void UpdateGenre(Genre genre);

    public void RemoveGenre(Genre genre);
}
