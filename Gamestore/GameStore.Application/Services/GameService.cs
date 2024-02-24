using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Extensions;
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

    public async Task<Guid> AddGame(GameDtoDto gameDto)
    {
        Guid gameId = (gameDto.Game.Id == null || gameDto.Game.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)gameDto.Game.Id;

        if (gameDto.Game.Discount > gameDto.Game.Price)
        {
            throw new ArgumentException("Discount cannot be greater than price");
        }

        if (gameDto.Publisher == null)
        {
            throw new ArgumentException("Empty publisher ID");
        }

        Publisher publisher = await _publisherRepository.Get((Guid)gameDto.Publisher) ?? throw new EntityNotFoundException($"Publisher with ID: {gameDto.Publisher} does not exists");

        var genres = gameDto.Genres == null
                                    ? Enumerable.Empty<Genre>()
                                    : await Task.WhenAll(gameDto.Genres.Select(async x => await _genreRepository.Get(x) ?? throw new EntityNotFoundException($"Genre ID: {x} is incorrect")));

        if (!genres.Any())
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        var platforms = gameDto.Platforms == null
                                          ? Enumerable.Empty<Platform>()
                                          : await Task.WhenAll(gameDto.Platforms.Select(async x => await _platformRepository.Get(x) ?? throw new EntityNotFoundException($"Platform ID: {x} is incorrect")));

        if (!platforms.Any())
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        string description = string.IsNullOrWhiteSpace(gameDto.Game.Description) ? null : gameDto.Game.Description;

        Game game = string.IsNullOrWhiteSpace(description)
            ? new Game(gameId, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discount, publisher.Id, genres.ToList(), platforms.ToList())
            : new Game(gameId, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discount, description, publisher.Id, genres.ToList(), platforms.ToList());

        try
        {
            await _gameRepository.Add(game);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique game key");
        }

        return game.Id;
    }

    public async Task<Guid> DeleteGame(string gameKey)
    {
        Game game = await GameByKey(gameKey);

        await _gameRepository.Delete(game);

        return game.Id;
    }

    public async Task<GameDto> GetGameById(Guid gameId)
    {
        Game game = await GameById(gameId);
        game.NumberOfViews += 1;
        game.ModificationDate = DateTime.Now;
        await _gameRepository.Update(game);

        return new(game);
    }

    public async Task<GameDto> GetGameByKey(string gameKey)
    {
        Game game = await GameByKey(gameKey);
        game.NumberOfViews += 1;
        game.ModificationDate = DateTime.Now;
        await _gameRepository.Update(game);

        return new(game);
    }

    public async Task<object> GetGameByKeyWithRelations(string gameKey)
    {
        return await _gamesSearchCriteria.GetByKeyWithRelations(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
    }

    public async Task<IEnumerable<GameDto>> GetGames()
    {
        var games = await _gameRepository.GetAllGames();
        return games.Select(x => new GameDto(x));
    }

    public async Task<object> GetGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, string? datePublishing, string? sort, uint page, string pageCount, int minPrice, int maxPrice)
    {
        List<Guid>? updatedGenreIds = genreIds != null ? new List<Guid>(genreIds) : null;
        if (genreIds != null)
        {
            foreach (var genreId in genreIds)
            {
                Genre genre = await _genreRepository.Get(genreId);
                if (genre == null)
                {
                    updatedGenreIds.Remove(genreId);
                }
            }
        }

        List<Guid>? updatedPlatformIds = platformIds != null ? new List<Guid>(platformIds) : null;
        if (platformIds != null)
        {
            foreach (var platformId in platformIds)
            {
                Platform platform = await _platformRepository.Get(platformId);
                if (platform == null)
                {
                    updatedPlatformIds.Remove(platformId);
                }
            }
        }

        List<Guid>? updatedPublisherIds = publisherIds != null ? new List<Guid>(publisherIds) : null;
        if (publisherIds != null)
        {
            foreach (var publisherId in publisherIds)
            {
                Publisher publisher = await _publisherRepository.Get(publisherId);
                if (publisher == null)
                {
                    updatedPublisherIds.Remove(publisherId);
                }
            }
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || name.Length < 3)
        {
            name = null;
        }

        if (page < 1)
        {
            page = 1;
        }

        if (minPrice < 0)
        {
            minPrice = 0;
        }

        if (maxPrice < minPrice)
        {
            maxPrice = int.MaxValue;
        }

        PublishDateFilteringMode? publishDate = EnumExtensions.GetEnumValueFromDescription<PublishDateFilteringMode>(datePublishing);

        GameSortingMode? sortMode = EnumExtensions.GetEnumValueFromDescription<GameSortingMode>(sort);

        NumberOfGamesOnPageFilteringMode? numberOfGamesOnPage = EnumExtensions.GetEnumValueFromDescription<NumberOfGamesOnPageFilteringMode>(pageCount);
        numberOfGamesOnPage ??= NumberOfGamesOnPageFilteringMode.All;

        var games = await _gameRepository.GetAllGames(updatedGenreIds, updatedPlatformIds, updatedPublisherIds, name, publishDate, sortMode, page, (NumberOfGamesOnPageFilteringMode)numberOfGamesOnPage!, minPrice, maxPrice);

        var gameDtos = games.Select(x => new GameDto(x));
        int numberOfPages = numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All ? await _gameRepository.GetNumberOfPages((NumberOfGamesOnPageFilteringMode)numberOfGamesOnPage) : 1;

        object returnObj = new
        {
            Games = gameDtos,
            TotalPages = numberOfPages,
            CurrentPage = page,
        };

        return returnObj;
    }

    public async Task<IEnumerable<GameDto>> GetGamesByGenreId(Guid genreId)
    {
        var games = await _gamesSearchCriteria.GetByGenreId(genreId);

        return games.Select(x => new GameDto(x));
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPlatformId(Guid platformId)
    {
        var games = await _gamesSearchCriteria.GetByPlatformId(platformId);

        return games.Select(x => new GameDto(x));
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisherName(string companyName)
    {
        var games = await _gamesSearchCriteria.GetByPublisherName(companyName);

        return games.Select(x => new GameDto(x));
    }

    public async Task<Guid> UpdateGame(GameDtoDto gameDto)
    {
        if (gameDto.Game.Id == null)
        {
            gameDto.Game.Id = Guid.NewGuid();
        }

        if (gameDto.Publisher == null)
        {
            throw new ArgumentException("Empty publisher ID");
        }

        Publisher publisher = await _publisherRepository.Get((Guid)gameDto.Publisher) ?? throw new EntityNotFoundException($"Publisher with ID: {gameDto.Publisher} does not exists");

        var genres = gameDto.Genres == null
                                   ? Enumerable.Empty<Genre>()
                                   : await Task.WhenAll(gameDto.Genres.Select(async x => await _genreRepository.Get(x) ?? throw new EntityNotFoundException($"Genre ID: {x} is incorrect")));

        if (!genres.Any())
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        var platforms = gameDto.Platforms == null
                                          ? Enumerable.Empty<Platform>()
                                          : await Task.WhenAll(gameDto.Platforms.Select(async x => await _platformRepository.Get(x) ?? throw new EntityNotFoundException($"Platform ID: {x} is incorrect")));

        if (!platforms.Any())
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        Game game = await _gameRepository.GetGameWithRelations((Guid)gameDto.Game.Id) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameDto.Game.Id}");

        game.Name = gameDto.Game.Name;
        game.Key = gameDto.Game.Key;
        game.Description = gameDto.Game.Description;

        game.Genres.Clear();
        game.Genres = genres.ToList();

        game.Platforms.Clear();
        game.Platforms = platforms.ToList();

        game.PublisherId = publisher.Id;

        game.Price = gameDto.Game.Price;
        game.UnitInStock = gameDto.Game.UnitInStock;
        game.Discount = gameDto.Game.Discount;

        game.ModificationDate = DateTime.Now;

        try
        {
            await _gameRepository.Update(game);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique game key");
        }

        return game.Id;
    }

    public async Task<Guid> UpdateGameDescr(Guid gameId, string updatedDesc)
    {
        Game game = await GameById(gameId);

        game.Description = updatedDesc;
        game.ModificationDate = DateTime.Now;

        await _gameRepository.Update(game);

        return game.Id;
    }

    private async Task<Game> GameById(Guid gameId)
    {
        return await _gameRepository.Get(gameId) ?? throw new EntityNotFoundException($"Couldn't find game by ID: {gameId}");
    }

    private async Task<Game> GameByKey(string gameKey)
    {
        return await _gamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
    }
}