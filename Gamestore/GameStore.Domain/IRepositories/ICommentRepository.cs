using GameStore.Domain.Entities;

namespace GameStore.Domain.IRepositories;
public interface ICommentRepository
{
    public void AddComment(Comment comment);

    public void UpdateComment(Comment comment);

    public Comment GetComment(Guid commentId, Guid gameId);

    public List<CommentModel> GetGamesComments(Guid gameId);
}
