﻿using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class CommentService(ICommentRepository commentRepository, IGamesSearchCriteria gamesSearchCriteria) : ICommentService
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IGamesSearchCriteria _gamesSearchCriteria = gamesSearchCriteria;

    public async Task<Guid> AddComment(string gameKey, CommentDtoDto commentDto)
    {
        Game game = await GetGame(gameKey);

        Guid commentId = Guid.NewGuid();

        var actionType = commentDto.Action != null ? commentDto.Action : CommentActionType.Normal;
        Comment comment = new(commentId, commentDto.Comment.Name, commentDto.Comment.Body, (CommentActionType)actionType, game);

        if (commentDto.ParentId != null && commentDto.ParentId != Guid.Empty && commentDto.Action != null)
        {
            Comment parentComment = await GetComment((Guid)commentDto.ParentId, game.Id);
            comment.ParentComment = parentComment;
        }

        await _commentRepository.AddComment(comment);

        return commentId;
    }

    public async Task<Guid> DeleteComment(string gameKey, Guid commentId)
    {
        Game game = await GetGame(gameKey);

        Comment comment = await GetComment(commentId, game.Id);
        comment.Body = "A comment/quote was deleted";
        comment.ModificationDate = DateTime.Now;

        await _commentRepository.UpdateComment(comment);

        return comment.Id;
    }

    public async Task<IEnumerable<CommentModel>> GetComments(string gameKey)
    {
        Game game = await GetGame(gameKey);
        return await _commentRepository.GetGamesComments(game.Id);
    }

    private async Task<Game> GetGame(string gameKey)
    {
        return await _gamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
    }

    private async Task<Comment> GetComment(Guid commentId, Guid gameId)
    {
        return await _commentRepository.GetComment(commentId, gameId) ?? throw new EntityNotFoundException($"Game: {gameId} Comment with ID: {commentId} not found");
    }
}
