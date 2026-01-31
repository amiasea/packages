using System.Text.Json;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Test.BringUp;
using Xunit;

namespace Amiasea.Loom.Test;

public sealed class ListTests
{
    private static (ProjectionResult Result, string Json) Run(
        string query,
        IGraphQueryableProvider provider)
    {
        var schema = TestSchemaBuilder.Build();
        var engine = TestEngineBuilder.Build(schema);
        var (document, operation) = TestParser.Parse(query);

        var result = (ProjectionResult)engine
            .ExecuteAsync(document, operation, provider, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        // Wrap under "users" to match test expectations
        var wrapped = new
        {
            Data = new
            {
                users = result.Data
            },
            result.Errors
        };

        var json = JsonSerializer.Serialize(
            wrapped,
            new JsonSerializerOptions { WriteIndented = true });

        return (result, json);
    }

    [Fact]
    public void Users_field_returns_list_of_users()
    {
        var provider = new TestProvider();
        var (_, json) = Run("{ users { id name } }", provider);

        Assert.Contains("\"users\"", json);
        Assert.Contains("\"id\": 1", json);
        Assert.Contains("\"name\": \"Alice\"", json);
    }

    [Fact]
    public void Empty_list_is_allowed()
    {
        var provider = new TestEmptyUsersProvider();
        var (_, json) = Run("{ users { id name } }", provider);

        Assert.Contains("\"users\": []", json);
    }
}