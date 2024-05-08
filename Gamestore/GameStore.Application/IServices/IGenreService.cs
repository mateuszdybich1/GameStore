using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IGenreService
{
    Task<Guid> AddGenre(GenreDto genreDto);

    Task<Guid> UpdateGenre(GenreDto genreDto);

    Task<Guid> DeleteGenre(Guid genreId);

    Task<GenreDto> GetGenre(Guid genreId);

    Task<IEnumerable<GenreDto>> GetAll();

    Task<IEnumerable<GenreDto>> GetGamesGenres(string gameKey);

    Task<IEnumerable<GenreDto>> GetSubGenres(Guid parentGenreId);
}
