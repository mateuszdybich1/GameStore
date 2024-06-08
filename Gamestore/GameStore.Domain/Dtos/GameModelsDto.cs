using GameStore.Domain.Entities;

namespace GameStore.Domain.Dtos;
public class GameModelsDto
{
    public GameModelsDto()
    {
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
        Games = [];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
        TotalPages = 1;
        CurrentPage = 1;
    }

    public GameModelsDto(List<Game> games, int totalPages, int currentPage)
    {
        Games = games;
        TotalPages = totalPages;
        CurrentPage = currentPage;
    }

    public IEnumerable<Game> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}
