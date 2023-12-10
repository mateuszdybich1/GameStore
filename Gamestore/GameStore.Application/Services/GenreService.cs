﻿using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;

namespace GameStore.Application.Services;
public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IGenresSearchCriteria _genresSearchCriteria;

    public GenreService(IGenreRepository genreRepository, IGenresSearchCriteria genresSearchCriteria)
    {
        _genreRepository = genreRepository;
        _genresSearchCriteria = genresSearchCriteria;
    }

    public Guid AddGenre(GenreDto genreDto)
    {
        Guid genreId = Guid.NewGuid();

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

        _genreRepository.AddGenre(genre);

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

        _genreRepository.UpdateGenre(genre);

        return genre.Id;
    }
}
