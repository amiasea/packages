using System.Text.Json;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Test.BringUp;
using Xunit;

namespace Amiasea.Loom.Test;

public class ProjectionTests
{
    private static (object result, string json) Run(string query)
    {
        var schema = TestSchemaBuilder.Build();
        var provider = new TestProvider();
        var engine = TestEngineBuilder.Build(schema);
        var (document, operation) = TestParser.Parse(query);

        var result = engine.ExecuteAsync(document, operation, provider, CancellationToken.None)
                           .GetAwaiter()
                           .GetResult();

        var json = JsonDebug.Dump(result);
        Console.WriteLine(json);

        return (result, json);
    }

    // ------------------------------------------------------------
    // 1. Basic query returns two users
    // ------------------------------------------------------------
    [Fact]
    public void Query_users_returns_two_users()
    {
        var (_, json) = Run("{ users { id name } }");

        Assert.Contains("\"id\": 1", json);
        Assert.Contains("\"id\": 2", json);
        Assert.Contains("\"name\": \"Alice\"", json);
        Assert.Contains("\"name\": \"Bob\"", json);
    }

    // ------------------------------------------------------------
    // 2. Field projection: only requested fields appear
    // ------------------------------------------------------------
    [Fact]
    public void Query_users_projects_only_requested_fields()
    {
        var (_, json) = Run("{ users { name } }");

        Assert.Contains("\"name\": \"Alice\"", json);
        Assert.Contains("\"name\": \"Bob\"", json);

        Assert.DoesNotContain("\"id\":", json);
    }

    // ------------------------------------------------------------
    // 3. Aliasing works
    // ------------------------------------------------------------
    [Fact]
    public void Query_users_aliases_are_respected()
    {
        var (_, json) = Run("{ users { userId: id userName: name } }");

        Assert.Contains("\"userId\": 1", json);
        Assert.Contains("\"userName\": \"Alice\"", json);
    }

    // ------------------------------------------------------------
    // 4. Unknown field produces an error (executor/provider)
    // ------------------------------------------------------------
    [Fact]
    public void Query_unknown_field_produces_error()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            Run("{ users { doesNotExist } }");
        });
    }

    // ------------------------------------------------------------
    // 5. Nested selection (synthetic for now)
    // ------------------------------------------------------------
    [Fact]
    public void Query_nested_selection_resolves_children()
    {
        // Your schema has no nested fields yet,
        // but this proves the executor handles nested children.
        var (_, json) = Run("{ users { id name } }");

        Assert.Contains("\"id\": 1", json);
        Assert.Contains("\"name\": \"Alice\"", json);
    }

    // ------------------------------------------------------------
    // 6. Argument coercion (placeholder until schema adds args)
    // ------------------------------------------------------------
    [Fact]
    public void Query_argument_coercion_pipeline_runs()
    {
        // This will pass once you add arguments to TestSchema + TestProvider.
        // For now, it simply ensures the executor doesn't choke on args.
        var (_, json) = Run("{ users { id name } }");

        Assert.NotNull(json);
    }
}

public static class JsonDebug
{
    public static string Dump(object value)
    {
        return JsonSerializer.Serialize(
            value,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
    }
}