using System.Linq;
using System.Threading.Tasks;
using Amiasea.Loom.EF;
using Amiasea.Loom.Test.BringUp;
using Amiasea.Loom.Test.Integration.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Amiasea.Loom.Test.Integration.EF;

public sealed class EFProviderTests
{
    [Fact]
    public async Task Users_Query_Returns_Data_From_InMemory()
    {
        // Arrange EF
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("loom-ef-test")
            .Options;

        using var db = new TestDbContext(options);

        db.Users.Add(new User
        {
            Id = 1,
            Name = "Alice",
            Posts = { new Post { Id = 10, Title = "Hello" } }
        });

        db.SaveChanges();

        // Arrange Loom
        var schema = TestSchemaBuilder.Build();
        var engine = TestEngineBuilder.Build(schema);
        var provider = new EFProvider(db);

        var (doc, op) = TestParser.Parse(@"
        {
            users {
                id
                name
                posts {
                    id
                    title
                }
            }
        }");

        // Act
        var result = await engine.ExecuteAsync(doc, op, provider, default);

        // Unwrap ProjectionResult
        var root = (IDictionary<string, object?>)result.Data;

        // Extract users
        var users = (IEnumerable<IDictionary<string, object?>>)root["users"];
        var user = users.Single();

        Assert.Equal(1, user["id"]);
        Assert.Equal("Alice", user["name"]);

        // Extract posts
        var posts = (IEnumerable<IDictionary<string, object?>>)user["posts"];
        var post = posts.Single();

        Assert.Equal(10, post["id"]);
        Assert.Equal("Hello", post["title"]);
    }
}