using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;
public interface IGameService
{
    public Guid AddGame(GameDto gameDto);

    public Guid UpdateGame(GameDto gameDto);

    public Guid UpdateGameDescr(Guid gameId, string updatedDesc);

    public Guid DeleteGame(string gameKey);

    public GameDto GetGameByKey(string gameKey);

    public GameDto GetGameById(Guid gameId);

    public List<GameDto> GetGames();

    public List<GameDto> GetGamesByPlatformId(Guid platformId);

    public List<GameDto> GetGamesByGenreId(Guid genreId);
}