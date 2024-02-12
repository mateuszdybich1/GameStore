using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface IGameService
{
    public Guid AddGame(GameDtoDto gameDto);

    public Guid UpdateGame(GameDtoDto gameDto);

    public Guid UpdateGameDescr(Guid gameId, string updatedDesc);

    public Guid DeleteGame(string gameKey);

    public GameDto GetGameByKey(string gameKey);

    public object GetGameByKeyWithRelations(string gameKey);

    public GameDto GetGameById(Guid gameId);

    public List<GameDto> GetGames();

    public object GetGames(List<Guid>? genreIds, List<Guid>? platformIds, List<Guid>? publisherIds, string? name, string? datePublishing, string? sort, uint page, string pageCount, int minPrice, int maxPrice);

    public List<GameDto> GetGamesByPlatformId(Guid platformId);

    public List<GameDto> GetGamesByGenreId(Guid genreId);

    public List<GameDto> GetGamesByPublisherName(string companyName);
}