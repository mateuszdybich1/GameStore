using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IGameService
{
    Task<Guid> AddGame(GameDtoDto gameDto);

    Task<Guid> UpdateGame(GameDtoDto gameDto);

    Task<Guid> UpdateGameDescr(Guid gameId, string updatedDesc);

    Task<Guid> DeleteGame(string gameKey);

    Task<GameDto> GetGameByKey(string gameKey);

    Task<object> GetGameByKeyWithRelations(string gameKey);

    Task<GameDto> GetGameById(Guid gameId);

    Task<GameListDto> GetGames();

    Task<GameListDto> GetGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, string? datePublishing, string? sort, uint page, string pageCount, int minPrice, int maxPrice);

    Task<IEnumerable<GameDto>> GetGamesByPlatformId(Guid platformId);

    Task<IEnumerable<GameDto>> GetGamesByGenreId(Guid genreId);

    Task<IEnumerable<GameDto>> GetGamesByPublisherName(string companyName);
}