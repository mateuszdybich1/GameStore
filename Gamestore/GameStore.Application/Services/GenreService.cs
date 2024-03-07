using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class GenreService(Func<RepositoryTypes, IGenreRepository> genreRepository, Func<RepositoryTypes, IGenresSearchCriteria> genreSearchCriteria) : IGenreService
{
    private readonly IGenreRepository _sqlGenreRepository = genreRepository(RepositoryTypes.Sql);
    private readonly IGenreRepository _mongoGenreRepository = genreRepository(RepositoryTypes.Mongo);
    private readonly IGenresSearchCriteria _sqlGenresSearchCriteria = genreSearchCriteria(RepositoryTypes.Sql);
    private readonly IGenresSearchCriteria _mongoGenresSearchCriteria = genreSearchCriteria(RepositoryTypes.Mongo);

    public async Task<Guid> AddGenre(GenreDto genreDto)
    {
        Guid genreId = (genreDto.Id == null || genreDto.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)genreDto.Id;

        Genre genre = new(genreId, genreDto.Name, genreDto.Description, genreDto.Picture);

        if (genreDto.ParentGenreId != null && genreDto.ParentGenreId != Guid.Empty)
        {
            Genre parentGenre = await _sqlGenreRepository.Get((Guid)genreDto.ParentGenreId) ?? throw new EntityNotFoundException($"Couldn't find parent genre by ID: {genreDto.ParentGenreId}");

            genre.ParentGenre = parentGenre;
        }
        else
        {
            genre.ParentGenre = null;
        }

        try
        {
            await _sqlGenreRepository.Add(genre);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique genre name");
        }

        return genre.Id;
    }

    public async Task<Guid> DeleteGenre(Guid genreId)
    {
        Genre genre = await _sqlGenreRepository.Get(genreId);
        Genre mongoGenre = await _mongoGenreRepository.Get(genreId);

        if (genre == null && mongoGenre == null)
        {
            throw new EntityNotFoundException($"Couldn't find genre by ID: {genreId}");
        }
        else
        {
            if (mongoGenre != null)
            {
                await _mongoGenreRepository.Delete(mongoGenre);
            }

            if (genre != null)
            {
                await _sqlGenreRepository.Delete(genre);
            }
        }

        return genre == null ? mongoGenre.Id : genre.Id;
    }

    public async Task<IEnumerable<GenreDto>> GetAll()
    {
        var genres = await _sqlGenreRepository.GetAllGenre();
        var mongoGenres = await _mongoGenreRepository.GetAllGenre();
        mongoGenres = mongoGenres.Where(x => !genres.Any(y => y.Name == x.Name));
        return genres.Concat(mongoGenres).Select(x => new GenreDto(x));
    }

    public async Task<IEnumerable<GenreDto>> GetGamesGenres(string gameKey)
    {
        var genres = await _sqlGenresSearchCriteria.GetByGameKey(gameKey);
        var mongoGenres = await _mongoGenresSearchCriteria.GetByGameKey(gameKey);
        mongoGenres = mongoGenres.Where(x => !genres.Any(y => y.Name == x.Name));
        return genres.Concat(mongoGenres).Select(x => new GenreDto(x));
    }

    public async Task<GenreDto> GetGenre(Guid genreId)
    {
        Genre genre = await _sqlGenreRepository.Get(genreId) ?? await _mongoGenreRepository.Get(genreId) ?? throw new EntityNotFoundException($"Couldn't find genre by ID: {genreId}");

        return new(genre);
    }

    public async Task<IEnumerable<GenreDto>> GetSubGenres(Guid parentGenreId)
    {
        var genres = await _sqlGenresSearchCriteria.GetByParentId(parentGenreId);
        return genres.Select(x => new GenreDto(x));
    }

    public async Task<Guid> UpdateGenre(GenreDto genreDto)
    {
        if (genreDto.Id == null)
        {
            throw new ArgumentNullException("Cannot update genre. Id is null");
        }

        Genre genre = await _sqlGenreRepository.Get((Guid)genreDto.Id);
        Genre mongoGenre = await _mongoGenreRepository.Get((Guid)genreDto.Id);

        if (genre == null && mongoGenre == null)
        {
            throw new EntityNotFoundException($"Couldn't find genre by ID: {genreDto.Id}");
        }

        if (genreDto.ParentGenreId != null && genreDto.ParentGenreId != Guid.Empty)
        {
            Genre parentGenre = await _sqlGenreRepository.Get((Guid)genreDto.ParentGenreId) ?? await _mongoGenreRepository.Get((Guid)genreDto.ParentGenreId) ?? throw new EntityNotFoundException($"Couldn't find parent genre by ID: {genreDto.ParentGenreId}");

            if (genre != null)
            {
                genre.ParentGenre = parentGenre;
            }

            if (mongoGenre != null)
            {
                mongoGenre.ParentGenre = parentGenre;
            }
        }

        if (genre != null)
        {
            genre.Name = genreDto.Name;
            genre.Description = genreDto.Description;
            genre.Picture = genreDto.Picture;
            genre.ModificationDate = DateTime.Now;

            try
            {
                await _sqlGenreRepository.Update(genre);
            }
            catch (Exception)
            {
                throw new ExistingFieldException("Please provide unique genre name");
            }
        }

        if (mongoGenre != null)
        {
            var isSameGenreName = mongoGenre.Name == genreDto.Name;
            mongoGenre.Name = genreDto.Name;
            mongoGenre.Description = genreDto.Description;
            mongoGenre.Picture = genreDto.Picture;
            mongoGenre.ModificationDate = DateTime.Now;

            if (!isSameGenreName)
            {
                var existingMongoGenre = _mongoGenresSearchCriteria.GetByGenreName(mongoGenre.Name);

                if (existingMongoGenre != null)
                {
                    throw new ExistingFieldException("Please provide unique genre name");
                }
            }

            await _sqlGenreRepository.Add(mongoGenre);
            await _mongoGenreRepository.Update(mongoGenre);
        }

        return genre != null ? genre.Id : mongoGenre.Id;
    }
}
