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

    public Guid AddComment(string gameKey, CommentDtoDto commentDto)
    {
        Game game = GetGame(gameKey);

        Guid commentId = Guid.NewGuid();

        var actionType = commentDto.Action != null ? commentDto.Action : CommentActionType.Normal;
        Comment comment = new(commentId, commentDto.Comment.Name, commentDto.Comment.Body, (CommentActionType)actionType, game);

        if (commentDto.ParentId != null && commentDto.ParentId != Guid.Empty && commentDto.Action != null)
        {
            Comment parentComment = GetComment((Guid)commentDto.ParentId, game.Id);
            comment.ParentComment = parentComment;
        }

        _commentRepository.AddComment(comment);

        return commentId;
    }

    public Guid DeleteComment(string gameKey, Guid commentId)
    {
        Game game = GetGame(gameKey);

        Comment comment = GetComment(commentId, game.Id);
        comment.Body = "A comment/quote was deleted";

        _commentRepository.UpdateComment(comment);

        return comment.Id;
    }

    public List<CommentModel> GetComments(string gameKey)
    {
        Game game = GetGame(gameKey);
        return _commentRepository.GetGamesComments(game.Id);
    }

    private Game GetGame(string gameKey)
    {
        return _gamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
    }

    private Comment GetComment(Guid commentId, Guid gameId)
    {
        return _commentRepository.GetComment(commentId, gameId) ?? throw new EntityNotFoundException($"Game: {gameId} Comment with ID: {commentId} not found");
    }
}