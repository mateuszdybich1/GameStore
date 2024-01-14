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

    public Guid AddGenre(GenreDto genreDto)
    {
        Guid genreId = genreDto.Id == Guid.Empty ? Guid.NewGuid() : genreDto.Id;

        Genre genre = new(genreId, genreDto.Name);

        if (genreDto.ParentGerneId != Guid.Empty)
        {
            Genre parentGenre = _genreRepository.GetGenre(genreDto.ParentGerneId) ?? throw new EntityNotFoundException($"Couldn't find parent genre by ID: {genreDto.ParentGerneId}");

            genre.ParentGerneId = parentGenre.Id;
        }
        else
        {
            genre.ParentGerneId = Guid.Empty;
        }

        try
        {
            _genreRepository.AddGenre(genre);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique genre name");
        }

        return genre.Id;
    }

    public Guid DeleteGenre(Guid genreId)
    {
        Genre genre = _genreRepository.GetGenre(genreId) ?? throw new EntityNotFoundException($"Couldn't find genre by ID: {genreId}");

        _genreRepository.RemoveGenre(genre);

        return genre.Id;
    }

    public List<GenreDto> GetAll()
    {
        return _genreRepository.GetAllGenre().Select(x => new GenreDto(x)).ToList();
    }

    public List<GenreDto> GetGamesGenres(string gameKey)
    {
        return _genresSearchCriteria.GetByGameKey(gameKey).Select(x => new GenreDto(x)).ToList();
    }

    public GenreDto GetGenre(Guid genreId)
    {
        Genre genre = _genreRepository.GetGenre(genreId) ?? throw new EntityNotFoundException($"Couldn't find genre by ID: {genreId}");

        return new(genre);
    }

    public List<GenreDto> GetSubGenres(Guid parentGenreId)
    {
        return _genresSearchCriteria.GetByParentId(parentGenreId).Select(x => new GenreDto(x)).ToList();
    }

    public Guid UpdateGenre(GenreDto genreDto)
    {
        Genre genre = _genreRepository.GetGenre(genreDto.Id);

        genre.Name = genreDto.Name;

        if (genreDto.ParentGerneId != Guid.Empty)
        {
            Genre parentGenre = _genreRepository.GetGenre(genreDto.ParentGerneId) ?? throw new EntityNotFoundException($"Couldn't find parent genre by ID: {genreDto.ParentGerneId}");

            genre.ParentGerneId = parentGenre.Id;
        }

        try
        {
            _genreRepository.UpdateGenre(genre);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique genre name");
        }

        return genre.Id;
    }
}
