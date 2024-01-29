using GameStore.Application.Dtos;

namespace GameStore.Application.IServices;

public interface ICommentService
{
    public Guid AddComment(string gameKey, CommentDtoDto commentDto);

    public Guid DeleteComment(string gameKey, Guid commentId);

    public List<object> GetComments(string gameKey);
}
