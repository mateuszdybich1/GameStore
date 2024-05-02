using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using GameStore.Domain.IRepositories;
using GameStore.Domain.ISearchCriterias;

namespace GameStore.Application.Services;

public class CommentService(ICommentRepository commentRepository, Func<RepositoryTypes, IGamesSearchCriteria> gameSearchCriteriaRepositoryFactory, Func<RepositoryTypes, IGameRepository> gameRepositoryFactory, IChangeLogService commentChangeLogService) : ICommentService
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IGamesSearchCriteria _mongoGamesSearchCriteria = gameSearchCriteriaRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IGamesSearchCriteria _sqlGamesSearchCriteria = gameSearchCriteriaRepositoryFactory(RepositoryTypes.Sql);
    private readonly IGameRepository _sqlGamesRepository = gameRepositoryFactory(RepositoryTypes.Sql);
    private readonly IGameRepository _mongoGamesRepository = gameRepositoryFactory(RepositoryTypes.Mongo);
    private readonly IChangeLogService _commentChangeLogService = commentChangeLogService;

    public async Task<Guid> AddComment(string gameKey, CommentDtoDto commentDto)
    {
        Game game = await _sqlGamesSearchCriteria.GetByKey(gameKey);

        if (game == null)
        {
            var mongoGame = await _mongoGamesRepository.GetGameWithRelations(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");

            await _sqlGamesRepository.Add(mongoGame);
            game = mongoGame;
        }

        Guid commentId = Guid.NewGuid();

        var actionType = commentDto.Action != null ? commentDto.Action : CommentActionType.Normal;
        Comment comment = new(commentId, commentDto.Comment.Name!, commentDto.Comment.Body, (CommentActionType)actionType, game);

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
        Comment oldComment = new(comment);
        comment.Body = "A comment/quote was deleted";
        comment.ModificationDate = DateTime.Now;

        await _commentRepository.UpdateComment(comment);
        await _commentChangeLogService.LogEntityChanges(LogActionType.Delete, EntityType.Comment, oldComment, comment);
        return comment.Id;
    }

    public async Task<IEnumerable<CommentModel>> GetComments(string gameKey)
    {
        Game game = await _sqlGamesSearchCriteria.GetByKey(gameKey);
        if (game == null)
        {
            var mongoGame = await _mongoGamesSearchCriteria.GetByKey(gameKey);
            if (mongoGame != null)
            {
            }
            else
            {
                throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
            }
        }

        return game != null ? await _commentRepository.GetGamesComments(game.Id) : new List<CommentModel>();
    }

    public async Task<Comment> GetComment(Guid id)
    {
        return await _commentRepository.GetComment(id);
    }

    private async Task<Game> GetGame(string gameKey)
    {
        return await _sqlGamesSearchCriteria.GetByKey(gameKey) ?? throw new EntityNotFoundException($"Couldn't find game by key: {gameKey}");
    }

    private async Task<Comment> GetComment(Guid commentId, Guid gameId)
    {
        return await _commentRepository.GetComment(commentId, gameId) ?? throw new EntityNotFoundException($"Game: {gameId} Comment with ID: {commentId} not found");
    }
}
