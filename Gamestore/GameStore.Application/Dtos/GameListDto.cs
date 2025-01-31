﻿namespace GameStore.Application.Dtos;

public class GameListDto
{
    public GameListDto()
    {
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
        Games = [];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
        TotalPages = 1;
        CurrentPage = 1;
    }

    public GameListDto(List<GameDto> games, int totalPages, int currentPage)
    {
        Games = games;
        TotalPages = totalPages;
        CurrentPage = currentPage;
    }

    public List<GameDto> Games { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }
}
