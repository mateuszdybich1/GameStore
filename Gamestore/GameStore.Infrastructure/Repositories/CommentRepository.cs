using GameStore.Domain.Entities;
using GameStore.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class CommentRepository(AppDbContext appDbContext) : ICommentRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public void AddComment(Comment comment)
    {
        _appDbContext.Comments.Add(comment);
        _appDbContext.SaveChanges();
    }

    public Comment GetComment(Guid commentId, Guid gameId)
    {
        return _appDbContext.Comments.SingleOrDefault(x => x.Id == commentId && x.Game.Id == gameId);
    }

    public List<CommentModel> GetGamesComments(Guid gameId)
    {
        var parentComments = _appDbContext.Comments.Include(x => x.ParentComment).Where(x => x.Game.Id == gameId && x.ParentComment == null).ToList();

        var parentCommentModels = parentComments.Select(x => new CommentModel(x.Id, x.Name, x.Body, x.Type)).ToList();

        foreach (var parentCommentModel in parentCommentModels)
        {
            parentCommentModel.ChildComments = [];
            RecursiveGetComments(parentCommentModel, parentCommentModel.ChildComments);
        }

        return parentCommentModels;
    }

    public void UpdateComment(Comment comment)
    {
        _appDbContext.Comments.Update(comment);
        _appDbContext.SaveChanges();
    }

    private void RecursiveGetComments(CommentModel comment, List<CommentModel> allComments)
    {
        comment.ChildComments =
        [
            .. _appDbContext.Comments.Include(x => x.ParentComment).Where(x => x.ParentComment.Id == comment.Id).Select(x => new CommentModel(x.Id, x.Name, x.Body, x.Type)),
        ];
        foreach (CommentModel childComment in comment.ChildComments)
        {
            allComments.Add(childComment);
            string currentChildBody = childComment.Body;
            if (childComment.Type == CommentActionType.Reply && currentChildBody != "A comment/quote was deleted")
            {
                childComment.Body = $"[{comment.Name}], {currentChildBody}";
            }
            else if (childComment.Type == CommentActionType.Quote && currentChildBody != "A comment/quote was deleted")
            {
                childComment.ParentQuote = comment.OnlyBody;
                childComment.Body = $"[{childComment.ParentQuote}], {currentChildBody}";
            }

            RecursiveGetComments(childComment, allComments);
        }
    }
}
