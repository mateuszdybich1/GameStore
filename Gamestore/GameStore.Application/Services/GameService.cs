using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class GameService(IGameRepository gameRepository, IGamesSearchCriteria gamesSearchCriteria, IPlatformRepository platformRepository, IGenreRepository genreRepository) : IGameService
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IGamesSearchCriteria _gamesSearchCriteria = gamesSearchCriteria;
    private readonly IPlatformRepository _platformRepository = platformRepository;
    private readonly IGenreRepository _genreRepository = genreRepository;

    public Guid AddGame(GameDto gameDto)
    {
        Guid gameId = Guid.NewGuid();
        List<Genre> genres = gameDto.GenresIds == null ? [] : gameDto.GenresIds.Select(x => _genreRepository.GetGenre(x) ?? throw new EntityNotFoundException($"Genre ID: {x} is incorrect")).ToList();

        if (genres.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        List<Platform> platforms = gameDto.PlatformsIds == null ? [] : gameDto.PlatformsIds.Select(x => _platformRepository.GetPlatform(x) ?? throw new EntityNotFoundException($"Platform ID: {x} is incorrect")).ToList();

        if (platforms.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        string description = string.IsNullOrWhiteSpace(gameDto.Description) ? null : gameDto.Description;

        Game game = string.IsNullOrWhiteSpace(description)
            ? new Game(gameId, gameDto.Name, gameDto.Key, genres, platforms)
            : new Game(gameId, gameDto.Name, gameDto.Key, description, genres, platforms);

        try
        {
            _gameRepository.AddGame(game);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique game key");
        }

        return game.Id;
    }

    public Guid DeleteGame(string gameKey)
    {
        Game game = _gamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        _gameRepository.RemoveGame(game);

        return game.Id;
    }

    public GameDto GetGameById(Guid gameId)
    {
        Game game = _gameRepository.GetGame(gameId) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameId}");

        return new(game);
    }

    public GameDto GetGameByKey(string gameKey)
    {
        Game game = _gamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        return new(game);
    }

    public object GetGameByKeyWithRelations(string gameKey)
    {
        Game game = _gamesSearchCriteria.GetByKeyWithRelations(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        game.Name = game.Name.Replace(' ', '_');

        string filePath = $"{game.Name}_{unixTime}000.txt";

        object newObj = new
        {
            FilePath = filePath,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Genres = game.Genres.Select(x => x.Name).ToList(),
            Platforms = game.Platforms.Select(x => x.Type).ToList(),
        };

        return newObj;
    }

    public List<GameDto> GetGames()
    {
        return _gameRepository.GetAllGames().Select(x => new GameDto(x)).ToList();
    }

    public List<GameDto> GetGamesByGenreId(Guid genreId)
    {
        return _gamesSearchCriteria.GetByGenreId(genreId).Select(x => new GameDto(x)).ToList();
    }

    public List<GameDto> GetGamesByPlatformId(Guid platformId)
    {
        return _gamesSearchCriteria.GetByPlatformId(platformId).Select(x => new GameDto(x)).ToList();
    }

    public Guid UpdateGame(GameDto gameDto)
    {
        List<Genre> genres = gameDto.GenresIds == null ? [] : gameDto.GenresIds.Select(x => _genreRepository.GetGenre(x) ?? throw new EntityNotFoundException($"Genre ID: {x} is incorrect")).ToList();

        if (genres.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        List<Platform> platforms = gameDto.PlatformsIds == null ? [] : gameDto.PlatformsIds.Select(x => _platformRepository.GetPlatform(x) ?? throw new EntityNotFoundException($"Platform ID: {x} is incorrect")).ToList();

        if (platforms.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        Game game = _gameRepository.GetGameWithRelations(gameDto.GameId) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameDto.GameId}");

        game.Name = gameDto.Name;
        game.Key = gameDto.Key;
        game.Description = gameDto.Description;

        game.Genres.Clear();
        game.Genres = genres;

        game.Platforms.Clear();
        game.Platforms = platforms;

        try
        {
            _gameRepository.UpdateGame(game);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique game key");
        }

        return game.Id;
    }

    public Guid UpdateGameDescr(Guid gameId, string updatedDesc)
    {
        Game game = _gameRepository.GetGame(gameId) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameId}");

        game.Description = updatedDesc;

        _gameRepository.UpdateGame(game);

        return game.Id;
    }
}