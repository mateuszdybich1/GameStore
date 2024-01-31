namespace GameStore.Domain.Entities;

public class CommentModel(Guid id, string name, string body, CommentActionType type)
{
    public Guid Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string Body { get; set; } = body;

    public CommentActionType Type { get; set; } = type;

    public List<CommentModel> ChildComments { get; set; }
}
