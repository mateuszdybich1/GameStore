using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IGameService
{
    public Task<Guid> AddGame(GameDtoDto gameDto);

    public Task<Guid> UpdateGame(GameDtoDto gameDto);

    public Task<Guid> UpdateGameDescr(Guid gameId, string updatedDesc);

    public Task<Guid> DeleteGame(string gameKey);

    public Task<GameDto> GetGameByKey(string gameKey);

    public Task<object> GetGameByKeyWithRelations(string gameKey);

    public Task<GameDto> GetGameById(Guid gameId);

    public Task<GameListDto> GetGames();

    public Task<GameListDto> GetGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, string? datePublishing, string? sort, uint page, string pageCount, int minPrice, int maxPrice);

    public Task<IEnumerable<GameDto>> GetGamesByPlatformId(Guid platformId);

    public Task<IEnumerable<GameDto>> GetGamesByGenreId(Guid genreId);

    public Task<IEnumerable<GameDto>> GetGamesByPublisherName(string companyName);
}