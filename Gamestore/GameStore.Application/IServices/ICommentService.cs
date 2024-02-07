using GameStore.Application.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.IServices;

public interface ICommentService
{
    public Guid AddComment(string gameKey, CommentDtoDto commentDto);

    public Guid DeleteComment(string gameKey, Guid commentId);

    public List<CommentModel> GetComments(string gameKey);
}
