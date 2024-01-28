using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class GameService(IGameRepository gameRepository, IGamesSearchCriteria gamesSearchCriteria, IPlatformRepository platformRepository, IGenreRepository genreRepository, IPublisherRepository publisherRepository) : IGameService
{
    private readonly IGameRepository _gameRepository = gameRepository;
    private readonly IGamesSearchCriteria _gamesSearchCriteria = gamesSearchCriteria;
    private readonly IPlatformRepository _platformRepository = platformRepository;
    private readonly IGenreRepository _genreRepository = genreRepository;
    private readonly IPublisherRepository _publisherRepository = publisherRepository;

    public Guid AddGame(GameDtoDto gameDto)
    {
        Guid gameId = (gameDto.Game.Id == null || gameDto.Game.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)gameDto.Game.Id;

        if (gameDto.Publisher == null)
        {
            throw new ArgumentException("Empty publisher ID");
        }

        Publisher publisher = _publisherRepository.GetPublisher((Guid)gameDto.Publisher) ?? throw new EntityNotFoundException($"Publisher with ID: {gameDto.Publisher} does not exists");

        List<Genre> genres = gameDto.Genres == null ? [] : gameDto.Genres.Select(x => _genreRepository.GetGenre(x) ?? throw new EntityNotFoundException($"Genre ID: {x} is incorrect")).ToList();

        if (genres.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        List<Platform> platforms = gameDto.Platforms == null ? [] : gameDto.Platforms.Select(x => _platformRepository.GetPlatform(x) ?? throw new EntityNotFoundException($"Platform ID: {x} is incorrect")).ToList();

        if (platforms.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        string description = string.IsNullOrWhiteSpace(gameDto.Game.Description) ? null : gameDto.Game.Description;

        Game game = string.IsNullOrWhiteSpace(description)
            ? new Game(gameId, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discontinued, publisher.Id, genres, platforms)
            : new Game(gameId, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discontinued, description, publisher.Id, genres, platforms);

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
        return _gamesSearchCriteria.GetByKeyWithRelations(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
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

    public List<GameDto> GetGamesByPublisherName(string companyName)
    {
        return _gamesSearchCriteria.GetByPublisherName(companyName).Select(x => new GameDto(x)).ToList();
    }

    public Guid UpdateGame(GameDtoDto gameDto)
    {
        if (gameDto.Game.Id == null)
        {
            gameDto.Game.Id = Guid.NewGuid();
        }

        if (gameDto.Publisher == null)
        {
            throw new ArgumentException("Empty publisher ID");
        }

        Publisher publisher = _publisherRepository.GetPublisher((Guid)gameDto.Publisher) ?? throw new EntityNotFoundException($"Publisher with ID: {gameDto.Publisher} does not exists");

        List<Genre> genres = gameDto.Genres == null ? [] : gameDto.Genres.Select(x => _genreRepository.GetGenre(x) ?? throw new EntityNotFoundException($"Genre ID: {x} is incorrect")).ToList();

        if (genres.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        List<Platform> platforms = gameDto.Platforms == null ? [] : gameDto.Platforms.Select(x => _platformRepository.GetPlatform(x) ?? throw new EntityNotFoundException($"Platform ID: {x} is incorrect")).ToList();

        if (platforms.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        Game game = _gameRepository.GetGameWithRelations((Guid)gameDto.Game.Id) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameDto.Game.Id}");

        game.Name = gameDto.Game.Name;
        game.Key = gameDto.Game.Key;
        game.Description = gameDto.Game.Description;

        game.Genres.Clear();
        game.Genres = genres;

        game.Platforms.Clear();
        game.Platforms = platforms;

        game.PublisherId = publisher.Id;

        game.Price = gameDto.Game.Price;
        game.UnitInStock = gameDto.Game.UnitInStock;
        game.Discount = gameDto.Game.Discontinued;

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