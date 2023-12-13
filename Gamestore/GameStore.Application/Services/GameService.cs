using GameStore.Application.Dtos;
using GameStore.Application.Exceptions;
using GameStore.Application.IServices;
using GameStore.Infrastructure.Entities;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.ISearchCriterias;

namespace GameStore.Application.Services;
public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGamesSearchCriteria _gamesSearchCriteria;
    private readonly IPlatformRepository _platformRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IPlatformsSearchCriteria _platformsSearchCriteria;
    private readonly IGenresSearchCriteria _genresSearchCriteria;

    public GameService(IGameRepository gameRepository, IGamesSearchCriteria gamesSearchCriteria, IPlatformRepository platformRepository, IGenreRepository genreRepository, IGenresSearchCriteria genresSearchCriteria, IPlatformsSearchCriteria platformsSearchCriteria)
    {
        _gameRepository = gameRepository;
        _gamesSearchCriteria = gamesSearchCriteria;
        _platformRepository = platformRepository;
        _genreRepository = genreRepository;
        _genresSearchCriteria = genresSearchCriteria;
        _platformsSearchCriteria = platformsSearchCriteria;
    }

    public Guid AddGame(GameDto gameDto)
    {
        Guid gameId = Guid.NewGuid();
        List<Genre> genres = gameDto.GenresIds.Select(_genreRepository.GetGenre).ToList();

        if (!genres.Any())
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        List<Platform> platforms = gameDto.PlatformsIds.Select(_platformRepository.GetPlatform).ToList();

        if (!platforms.Any())
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        string description = string.IsNullOrWhiteSpace(gameDto.Description) ? null : gameDto.Description;

        Game game = string.IsNullOrWhiteSpace(description)
            ? new Game(gameId, gameDto.Name, gameDto.Key, genres, platforms)
            : new Game(gameId, gameDto.Name, gameDto.Key, description, genres, platforms);

        _gameRepository.AddGame(game);
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
        Game game = _gamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

        DateTime currentTime = DateTime.UtcNow;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        game.Name = game.Name.Replace(' ', '_');

        string filePath = $"{game.Name}_{unixTime}.txt";

        object newObj = new
        {
            FilePath = filePath,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            Genres = _genresSearchCriteria.GetByGameKey(gameKey).Select(x => x.Name).ToList(),
            Platforms = _platformsSearchCriteria.GetByGameKey(gameKey).Select(x => x.Type).ToList(),
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
        Game game = _gameRepository.GetGameWithRelations(gameDto.GameId) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameDto.GameId}");

        game.Name = gameDto.Name;
        game.Key = gameDto.Key;
        game.Description = gameDto.Description;

        List<Genre> genres = gameDto.GenresIds.Select(_genreRepository.GetGenre).ToList();

        if (!genres.Any())
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        game.Genres.Clear();
        game.Genres = genres;

        List<Platform> platforms = gameDto.PlatformsIds.Select(_platformRepository.GetPlatform).ToList();

        if (!platforms.Any())
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        game.Platforms.Clear();
        game.Platforms = platforms;

        _gameRepository.UpdateGame(game);

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