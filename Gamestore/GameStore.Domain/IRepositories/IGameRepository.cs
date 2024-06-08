using GameStore.Domain.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IGameRepository : IRepository<Game>
{
    Task<Game> GetGameWithRelations(Guid gameId);

    Task<Game> GetGameWithRelations(string gameKey);

    Task<IEnumerable<Game>> GetAllGames();

    Task<int> GetAllGamesCount();

    Task<int> GetNumberOfPages(NumberOfGamesOnPageFilteringMode numberOfGamesOnPage);

    Task<GameModelsDto> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice);
}
