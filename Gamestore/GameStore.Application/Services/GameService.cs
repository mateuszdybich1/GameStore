using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.Extensions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class GameService(Func<RepositoryTypes, IGameRepository> gameRepositoryFactory, Func<RepositoryTypes, IGamesSearchCriteria> gameSearchCriteria, IPlatformRepository platformRepository, Func<RepositoryTypes, IGenreRepository> genreRepository, Func<RepositoryTypes, IPublisherRepository> publisherRepository, IChangeLogService gameChangeLogService) : IGameService
{
    private readonly IGameRepository _sqlGameRepository = gameRepositoryFactory(RepositoryTypes.Sql);
    private readonly IGameRepository _mongoGameRepository = gameRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IGamesSearchCriteria _sqlGamesSearchCriteria = gameSearchCriteria(RepositoryTypes.Sql);
    private readonly IGamesSearchCriteria _mongoGamesSearchCriteria = gameSearchCriteria(RepositoryTypes.Mongo);
    private readonly IPlatformRepository _platformRepository = platformRepository;
    private readonly IGenreRepository _sqlGenreRepository = genreRepository(RepositoryTypes.Sql);
    private readonly IGenreRepository _mongoGenreRepository = genreRepository(RepositoryTypes.Mongo);
    private readonly IPublisherRepository _sqlPublisherRepository = publisherRepository(RepositoryTypes.Sql);
    private readonly IPublisherRepository _mongoPublisherRepository = publisherRepository(RepositoryTypes.Mongo);
    private readonly IChangeLogService _gameChangeLogService = gameChangeLogService;

    public async Task<Guid> AddGame(GameDtoDto gameDto)
    {
        Guid gameId = (gameDto.Game.Id == null || gameDto.Game.Id == Guid.Empty) ? Guid.NewGuid() : (Guid)gameDto.Game.Id;

        if (gameDto.Game.Discount is < 0 or > 1)
        {
            throw new ArgumentException("Discount must be in range 0 to 1");
        }

        if (gameDto.Publisher == null)
        {
            throw new ArgumentException("Empty publisher ID");
        }

        Publisher publisher = await _sqlPublisherRepository.Get((Guid)gameDto.Publisher) ?? await _mongoPublisherRepository.Get((Guid)gameDto.Publisher) ?? throw new EntityNotFoundException($"Publisher with ID: {gameDto.Publisher} does not exists");

        var genres = new List<Genre>();
        if (gameDto.Genres != null)
        {
            foreach (var item in gameDto.Genres)
            {
                var tempGenre = await _sqlGenreRepository.Get(item) ?? await _mongoGenreRepository.Get(item) ?? throw new EntityNotFoundException($"Genre ID: {item} is incorrect");
                genres.Add(tempGenre);
            }
        }

        if (genres.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        var platforms = new List<Platform>();
        if (gameDto.Platforms != null)
        {
            foreach (var item in gameDto.Platforms)
            {
                var tempPlatform = await _platformRepository.Get(item) ?? throw new EntityNotFoundException($"Genre ID: {item} is incorrect");
                platforms.Add(tempPlatform);
            }
        }

        if (platforms.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        string description = string.IsNullOrWhiteSpace(gameDto.Game.Description) ? null : gameDto.Game.Description;

        Game game = string.IsNullOrWhiteSpace(description)
            ? new Game(gameId, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discount, publisher.Id, genres, platforms, publisher)
            : new Game(gameId, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discount, description, publisher.Id, genres, platforms, publisher);

        try
        {
            await _sqlGameRepository.Add(game);
        }
        catch (Exception)
        {
            throw new ExistingFieldException("Please provide unique game key");
        }

        return game.Id;
    }

    public async Task<Guid> DeleteGame(string gameKey)
    {
        Game game = await _sqlGamesSearchCriteria.GetByKey(gameKey);
        Game mongoGame = await _mongoGamesSearchCriteria.GetByKey(gameKey);

        if (game == null && mongoGame == null)
        {
            throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
        }

        if (game != null)
        {
            await _sqlGameRepository.Delete(game);
        }

        if (mongoGame != null)
        {
            await _mongoGameRepository.Delete(mongoGame);
        }

        return game == null ? mongoGame.Id : game.Id;
    }

    public async Task<GameDto> GetGameById(Guid gameId)
    {
        Game game = await _sqlGameRepository.Get(gameId);
        Game mongoGame = await _mongoGameRepository.Get(gameId);

        if (game == null && mongoGame == null)
        {
            throw new EntityNotFoundException($"Couldn't find game by ID: {gameId}");
        }
        else
        {
            if (game != null)
            {
                game.NumberOfViews += 1;
                game.ModificationDate = DateTime.Now;
                await _sqlGameRepository.Update(game);
            }

            if (mongoGame != null)
            {
                mongoGame.NumberOfViews += 1;
                await _mongoGameRepository.Update(mongoGame);
            }
        }

        game ??= mongoGame;

        return new(game!);
    }

    public async Task<GameDto> GetGameByKey(string gameKey)
    {
        Game game = await _sqlGamesSearchCriteria.GetByKey(gameKey);
        Game mongoGame = await _mongoGamesSearchCriteria.GetByKey(gameKey);

        if (game == null && mongoGame == null)
        {
            throw new EntityNotFoundException($"Couldn't find game by Key: {gameKey}");
        }
        else
        {
            if (game != null)
            {
                game.NumberOfViews += 1;
                game.ModificationDate = DateTime.Now;
                await _sqlGameRepository.Update(game);
            }

            if (mongoGame != null)
            {
                mongoGame.NumberOfViews += 1;
                await _mongoGameRepository.Update(mongoGame);
            }
        }

        game ??= mongoGame;

        return new(game!);
    }

    public async Task<object> GetGameByKeyWithRelations(string gameKey)
    {
        return await _sqlGamesSearchCriteria.GetByKeyWithRelations(gameKey) ?? await _mongoGamesSearchCriteria.GetByKeyWithRelations(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
    }

    public async Task<GameListDto> GetGames()
    {
        var games = await _sqlGameRepository.GetAllGames();
        var mongoGames = await _mongoGameRepository.GetAllGames();
        mongoGames = mongoGames.Where(x => !games.Any(y => y.Key == x.Key));

        GameListDto returnObj = new()
        {
            Games = games.Concat(mongoGames).Select(x => new GameDto(x)).ToList(),
            TotalPages = 1,
            CurrentPage = 1,
        };

        return returnObj;
    }

    public async Task<GameListDto> GetGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, string? datePublishing, string? sort, uint page, string pageCount, int minPrice, int maxPrice)
    {
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

        var games = await _sqlGameRepository.GetAllGames(genreIds, platformIds, publisherIds, name, publishDate, sortMode, page, (NumberOfGamesOnPageFilteringMode)numberOfGamesOnPage!, minPrice, maxPrice);
        var mongoGames = await _mongoGameRepository.GetAllGames(genreIds, platformIds, publisherIds, name, publishDate, sortMode, page, (NumberOfGamesOnPageFilteringMode)numberOfGamesOnPage!, minPrice, maxPrice);

        if (mongoGames != null)
        {
            games = games.Concat(mongoGames);
        }

        games = games.ToList();

        if (publishDate != null)
        {
            DateTime from = DateTime.Now;

            switch (publishDate)
            {
                case PublishDateFilteringMode.LastWeek:
                    from = from.AddDays(-7);
                    break;
                case PublishDateFilteringMode.LastMonth:
                    from = from.AddMonths(-1);
                    break;
                case PublishDateFilteringMode.LastYear:
                    from = from.AddYears(-1);
                    break;
                case PublishDateFilteringMode.TwoYears:
                    from = from.AddYears(-2);
                    break;
                case PublishDateFilteringMode.ThreeYears:
                    from = from.AddYears(-3);
                    break;
                default:
                    break;
            }

            games = games.Where(x => x.CreationDate >= from);
        }

        if (sortMode != null)
        {
            switch (sortMode)
            {
                case GameSortingMode.MostPopular:
                    games = games.OrderByDescending(x => x.NumberOfViews);
                    break;
                case GameSortingMode.MostCommented:
                    foreach (var game in games)
                    {
                        game.Comments ??= [];
                    }

                    games = games.OrderByDescending(x => x.Comments.Count);
                    break;
                case GameSortingMode.PriceASC:
                    games = games.OrderBy(x => x.Price);
                    break;
                case GameSortingMode.PriceDESC:
                    games = games.OrderByDescending(x => x.Price);
                    break;
                case GameSortingMode.New:
                    games = games.OrderByDescending(x => x.CreationDate);
                    break;
                default:
                    break;
            }
        }

        if (numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All)
        {
            games = games.Skip(((int)page - 1) * (int)numberOfGamesOnPage).Take((int)numberOfGamesOnPage);
        }

        var gameDtos = games.Select(x => new GameDto(x));

        int numberOfPages = 1;
        if (numberOfGamesOnPage != NumberOfGamesOnPageFilteringMode.All)
        {
            var sqlGamesCount = await _sqlGameRepository.GetAllGames();
            var mongoGamesCount = await _mongoGameRepository.GetAllGames();
            if (mongoGamesCount != null)
            {
                mongoGamesCount = mongoGamesCount.Where(x => !sqlGamesCount.Any(y => y.Key == x.Key));
                sqlGamesCount = sqlGamesCount.Concat(mongoGamesCount).ToList();
            }

            numberOfPages = sqlGamesCount.Count() / (int)numberOfGamesOnPage;
            if (numberOfPages % (int)numberOfGamesOnPage > 0)
            {
                numberOfPages += 1;
            }
        }

        GameListDto returnObj = new()
        {
            Games = gameDtos.ToList(),
            TotalPages = numberOfPages,
            CurrentPage = (int)page,
        };

        return returnObj;
    }

    public async Task<IEnumerable<GameDto>> GetGamesByGenreId(Guid genreId)
    {
        var games = await _sqlGamesSearchCriteria.GetByGenreId(genreId);
        var mongoGames = await _mongoGamesSearchCriteria.GetByGenreId(genreId);
        if (mongoGames != null)
        {
            mongoGames = mongoGames.Where(x => !games.Any(y => y.Key == x.Key));
            return games.Concat(mongoGames).Select(x => new GameDto(x));
        }

        return games.Select(x => new GameDto(x));
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPlatformId(Guid platformId)
    {
        var games = await _sqlGamesSearchCriteria.GetByPlatformId(platformId);
        return games.Select(x => new GameDto(x));
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisherName(string companyName)
    {
        var games = await _sqlGamesSearchCriteria.GetByPublisherName(companyName);
        var mongoGames = await _mongoGamesSearchCriteria.GetByPublisherName(companyName);
        if (mongoGames != null)
        {
            mongoGames = mongoGames.Where(x => !games.Any(y => y.Key == x.Key));

            return games.Concat(mongoGames).Select(x => new GameDto(x));
        }

        return games.Select(x => new GameDto(x));
    }

    public async Task<Guid> UpdateGame(GameDtoDto gameDto)
    {
        if (gameDto.Game.Discount is < 0 or > 1)
        {
            throw new ArgumentException("Discount must be in range 0 to 1");
        }

        if (gameDto.Game.Id == null)
        {
            gameDto.Game.Id = Guid.NewGuid();
        }

        if (gameDto.Publisher == null)
        {
            throw new ArgumentException("Empty publisher ID");
        }

        Publisher publisher = await _sqlPublisherRepository.Get((Guid)gameDto.Publisher!);
        if (publisher == null)
        {
            publisher = await _mongoPublisherRepository.Get((Guid)gameDto.Publisher) ?? throw new EntityNotFoundException($"Publisher with ID: {gameDto.Publisher} does not exists");
            await _sqlPublisherRepository.Add(publisher);
        }

        var genres = new List<Genre>();
        if (gameDto.Genres != null)
        {
            foreach (var item in gameDto.Genres)
            {
                var tempGenre = await _sqlGenreRepository.Get(item);
                if (tempGenre == null)
                {
                    tempGenre = await _mongoGenreRepository.Get(item) ?? throw new EntityNotFoundException($"Genre ID: {item} is incorrect");
                    await _sqlGenreRepository.Add(tempGenre);
                }

                genres.Add(tempGenre);
            }
        }

        if (genres.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one genre");
        }

        var platforms = new List<Platform>();
        if (gameDto.Platforms != null)
        {
            foreach (var item in gameDto.Platforms)
            {
                var tempPlatform = await _platformRepository.Get(item) ?? throw new EntityNotFoundException($"Genre ID: {item} is incorrect");
                platforms.Add(tempPlatform);
            }
        }

        if (platforms.Count == 0)
        {
            throw new EntityNotFoundException("You must provide at least one platform");
        }

        Game tempGame = await _sqlGameRepository.GetGameWithRelations((Guid)gameDto.Game.Id);
        Game mongoGame = await _mongoGameRepository.Get((Guid)gameDto.Game.Id);
        bool isSameKey = false;
        if (tempGame == null && mongoGame == null)
        {
            throw new EntityNotFoundException($"Couldn't find game by ID: {gameDto.Game.Id}");
        }
        else
        {
            Game oldGame = new();
            Game newGame = new();

            if (tempGame != null)
            {
                oldGame = tempGame;
            }

            if (mongoGame != null)
            {
                if (tempGame == null)
                {
                    oldGame = await _mongoGameRepository.GetGameWithRelations(mongoGame.Id);
                }

                if (gameDto.Game.Key.Equals(mongoGame.Key))
                {
                    isSameKey = true;
                }

                mongoGame.Name = gameDto.Game.Name;
                mongoGame.Key = gameDto.Game.Key;
                mongoGame.Description = gameDto.Game.Description;

                if (mongoGame.Genres != null)
                {
                    mongoGame.Genres.AddRange(genres.Except(mongoGame.Genres));
                }
                else
                {
                    mongoGame.Genres = genres;
                }

                if (mongoGame.Platforms != null)
                {
                    mongoGame.Platforms.AddRange(platforms.Except(mongoGame.Platforms));
                }
                else
                {
                    mongoGame.Platforms = platforms;
                }

                mongoGame.PublisherId = publisher.Id;
                mongoGame.Publisher = publisher;
                mongoGame.Price = gameDto.Game.Price;
                mongoGame.UnitInStock = gameDto.Game.UnitInStock;
                mongoGame.Discount = gameDto.Game.Discount;

                mongoGame.ModificationDate = DateTime.Now;

                if (!isSameKey)
                {
                    var tempMongoGame = _mongoGamesSearchCriteria.GetByKey(mongoGame.Key);
                    if (tempMongoGame != null)
                    {
                        throw new ExistingFieldException($"Game with key: {mongoGame.Key} already exists");
                    }

                    await _mongoGameRepository.Update(mongoGame);
                }

                await _mongoGameRepository.Update(mongoGame);
                if (tempGame == null)
                {
                    await _sqlGameRepository.Add(mongoGame);
                }

                newGame = mongoGame;
            }

            if (tempGame != null)
            {
                Game game = new((Guid)gameDto.Game.Id, gameDto.Game.Name, gameDto.Game.Key, gameDto.Game.Price, gameDto.Game.UnitInStock, gameDto.Game.Discount, publisher.Id, genres, platforms, publisher);

                if (mongoGame == null)
                {
                    newGame = new(game);
                }

                await _sqlGameRepository.Delete(tempGame);
                await _sqlGameRepository.Add(game);
            }

            await _gameChangeLogService.LogEntityChanges(LogActionType.Update, EntityType.Game, oldGame, newGame);
        }

        return tempGame != null ? tempGame.Id : mongoGame.Id;
    }

    public async Task<Guid> UpdateGameDescr(Guid gameId, string updatedDesc)
    {
        Game game = await _sqlGameRepository.Get(gameId);
        Game mongoGame = await _mongoGameRepository.Get(gameId);
        if (game == null && mongoGame == null)
        {
            throw new EntityNotFoundException($"Couldn't find game by ID: {gameId}");
        }
        else
        {
            Game oldGame = new();
            Game newGame = new();
            if (game != null)
            {
                oldGame = new(game);
                game.Description = updatedDesc;
                game.ModificationDate = DateTime.Now;

                await _sqlGameRepository.Update(game);
                newGame = new(game);
            }

            if (mongoGame != null)
            {
                if (game == null)
                {
                    oldGame = new(mongoGame);
                }

                mongoGame.Description = updatedDesc;
                mongoGame.ModificationDate = DateTime.Now;

                await _mongoGameRepository.Update(mongoGame);
                if (game == null)
                {
                    newGame = new(mongoGame);
                }
            }

            await _gameChangeLogService.LogEntityChanges(LogActionType.Update, EntityType.Game, oldGame, newGame);
        }

        return game != null ? game.Id : mongoGame.Id;
    }
}