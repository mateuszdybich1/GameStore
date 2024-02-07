﻿using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;

public interface IGameRepository
{
    public void AddGame(Game game);

    public Game GetGame(Guid gameId);

    public Game GetGameWithRelations(Guid gameId);

    public void UpdateGame(Game game);

    public void RemoveGame(Game game);

    public List<Game> GetAllGames();
}