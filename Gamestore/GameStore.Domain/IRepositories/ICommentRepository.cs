using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface ICommentRepository
{
    Task AddComment(Comment comment);

    Task UpdateComment(Comment comment);

    Task<Comment> GetComment(Guid commentId, Guid gameId);

    Task<Comment> GetComment(Guid commentId);

    Task<IEnumerable<CommentModel>> GetGamesComments(Guid gameId);
}
