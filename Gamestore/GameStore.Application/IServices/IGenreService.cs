using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;
public interface IGenreService
{
    public Guid AddGenre(GenreDto genreDto);

    public Guid UpdateGenre(GenreDto genreDto);

    public Guid DeleteGenre(Guid genreId);

    public GenreDto GetGenre(Guid genreId);

    public List<GenreDto> GetAll();

    List<GenreDto> GetGamesGenres(string gameKey);

    public List<GenreDto> GetSubGenres(Guid parentGenreId);
}
