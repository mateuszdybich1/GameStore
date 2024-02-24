using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class GenreService(IGenreRepository genreRepository, IGenresSearchCriteria genresSearchCriteria) : IGenreService
{
    private readonly IGenreRepository _genreRepository = genreRepository;
    private readonly IGenresSearchCriteria _genresSearchCriteria = genresSearchCriteria;

    public async Task<Guid> AddGenre(GenreDto genreDto)
    {
        Guid genreId = (genreDto.Id == null || genreDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)genreDto.Id;

        Genre genre = new(genreId, genreDto.Name);

        if (genreDto.ParentGenreId != null && genreDto.ParentGenreId != Guid.Empty)
        {
            Genre parentGenre = await _genreRepository.Get((Guid)genreDto.ParentGenreId) ?? throw new EntityNotFoundException($"Couldn't find parent genre by ID: {genreDto.ParentGenreId}");

            genre.ParentGenre = parentGenre;
        }
        else
        {
            genre.ParentGenre = null;
        }

        try
        {
            await _genreRepository.Add(genre);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique genre name");
        }

        return genre.Id;
    }

    public async Task<Guid> DeleteGenre(Guid genreId)
    {
        Genre genre = await _genreRepository.Get(genreId) ?? throw new EntityNotFoundException($"Couldn't find genre by ID: {genreId}");

        await _genreRepository.Delete(genre);

        return genre.Id;
    }

    public async Task<IEnumerable<GenreDto>> GetAll()
    {
        var genres = await _genreRepository.GetAllGenre();
        return genres.Select(x => new GenreDto(x));
    }

    public async Task<IEnumerable<GenreDto>> GetGamesGenres(string gameKey)
    {
        var genres = await _genresSearchCriteria.GetByGameKey(gameKey);
        return genres.Select(x => new GenreDto(x));
    }

    public async Task<GenreDto> GetGenre(Guid genreId)
    {
        Genre genre = await _genreRepository.Get(genreId) ?? throw new EntityNotFoundException($"Couldn't find genre by ID: {genreId}");

        return new(genre);
    }

    public async Task<IEnumerable<GenreDto>> GetSubGenres(Guid parentGenreId)
    {
        var genres = await _genresSearchCriteria.GetByParentId(parentGenreId);
        return genres.Select(x => new GenreDto(x));
    }

    public async Task<Guid> UpdateGenre(GenreDto genreDto)
    {
        if (genreDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update genre. Id is null");
        }

        Genre genre = await _genreRepository.Get((Guid)genreDto.Id) ?? throw new EntityNotFoundException($"Couldn't find genre by ID: {genreDto.Id}");

        genre.Name = genreDto.Name;
        genre.ModificationDate = DateTime.Now;

        if (genreDto.ParentGenreId != null && genreDto.ParentGenreId != Guid.Empty)
        {
            Genre parentGenre = await _genreRepository.Get((Guid)genreDto.ParentGenreId) ?? throw new EntityNotFoundException($"Couldn't find parent genre by ID: {genreDto.ParentGenreId}");

            genre.ParentGenre = parentGenre;
        }

        try
        {
            await _genreRepository.Update(genre);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique genre name");
        }

        return genre.Id;
    }
}
