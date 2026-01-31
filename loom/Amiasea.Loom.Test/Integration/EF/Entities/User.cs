namespace Amiasea.Loom.Test.Integration.EF.Entities;

public sealed class User
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}