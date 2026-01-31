namespace Amiasea.Loom.Test.Integration.EF.Entities;

public sealed class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public int UserId { get; set; }
    public User User { get; set; } = default!;
}