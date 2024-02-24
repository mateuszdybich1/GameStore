using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IGameRepository : IRepository<Game>
{
    public Task<Game> GetGameWithRelations(Guid gameId);

    public Task<IEnumerable<Game>> GetAllGames();

    public Task<int> GetAllGamesCount();

    public Task<int> GetNumberOfPages(NumberOfGamesOnPageFilteringMode numberOfGamesOnPage);

    public Task<IEnumerable<Game>> GetAllGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, PublishDateFilteringMode? publishDate, GameSortingMode? sortMode, uint page, NumberOfGamesOnPageFilteringMode numberOfGamesOnPage, int minPrice, int maxPrice);
}
