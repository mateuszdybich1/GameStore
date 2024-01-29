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
        return _appDbContext.Comments.FirstOrDefault(x => x.Id == commentId && x.GameId == gameId);
    }

    public List<Comment> GetGamesComments(Guid gameId)
    {
        var comments = _appDbContext.Comments.Where(x => x.GameId == gameId).ToList();

        for (int i = 0; i < comments.Count; i++)
        {
            comments[i].ParentComment = GetComment(comments[i]);
        }

        return comments;
    }

    public void UpdateComment(Comment comment)
    {
        _appDbContext.Comments.Update(comment);
        _appDbContext.SaveChanges();
    }

    private Comment GetComment(Comment comment)
    {
        var currentComment = comment;
        while (currentComment?.ParentComment != null)
        {
            currentComment = currentComment.ParentComment;
        }

        currentComment.ParentComment = _appDbContext.Comments.Include(x => x.ParentComment).FirstOrDefault(x => x.Id == currentComment.Id).ParentComment;

        return currentComment.ParentComment == null ? comment : GetComment(comment);
    }
}
