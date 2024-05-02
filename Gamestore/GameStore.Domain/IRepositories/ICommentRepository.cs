using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface ICommentRepository
{
    public Task AddComment(Comment comment);

    public Task UpdateComment(Comment comment);

    public Task<Comment> GetComment(Guid commentId, Guid gameId);

    public Task<Comment> GetComment(Guid commentId);

    public Task<IEnumerable<CommentModel>> GetGamesComments(Guid gameId);
}
