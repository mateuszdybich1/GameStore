namespace GameStore.Infrastructure.Entities;

public class GameGenre
{
    public GameGenre()
    {
    }

    public GameGenre(Guid gameId, Guid genreId)
    {
        GameId = gameId;
        GenreId = genreId;
    }

    public Guid GameId { get; private set; }

    public Guid GenreId { get; private set; }

    public Game Game { get; set; }

    public Genre Genre { get; set; }
}