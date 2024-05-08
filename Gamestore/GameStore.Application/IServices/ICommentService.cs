using GameStore.Application.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Application.IServices;

public interface ICommentService
{
    Task<Guid> AddComment(string gameKey, CommentDtoDto commentDto);

    Task<Guid> DeleteComment(string gameKey, Guid commentId);

    Task<Comment> GetComment(Guid id);

    Task<IEnumerable<CommentModel>> GetComments(string gameKey);
}
