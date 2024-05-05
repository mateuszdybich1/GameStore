namespace GameStore.Application.Dtos;

public class GameListDto
{
    public List<GameDto> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}
