using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IGenreService
{
    public Task<Guid> AddGenre(GenreDto genreDto);

    public Task<Guid> UpdateGenre(GenreDto genreDto);

    public Task<Guid> DeleteGenre(Guid genreId);

    public Task<GenreDto> GetGenre(Guid genreId);

    public Task<IEnumerable<GenreDto>> GetAll();

    public Task<IEnumerable<GenreDto>> GetGamesGenres(string gameKey);

    public Task<IEnumerable<GenreDto>> GetSubGenres(Guid parentGenreId);
}
