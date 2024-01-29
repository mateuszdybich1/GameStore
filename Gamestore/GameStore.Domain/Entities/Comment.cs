namespace GameStore.Domain.Entities;

public enum CommentActionType
{
    Normal,
    Reply,
    Quote,
}

public class Comment
{
    public Comment(Guid id, string name, string body, Guid gameId)
    {
        Id = id;
        Name = name;
        Body = body;
        GameId = gameId;
    }

    public Comment(Guid id, string name, string body, Guid gameId, Comment parentComment)
    {
        Id = id;
        Name = name;
        Body = body;
        GameId = gameId;
        ParentComment = parentComment;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Body { get; set; }

    public Guid GameId { get; set; }

    public Comment? ParentComment { get; set; }
}
