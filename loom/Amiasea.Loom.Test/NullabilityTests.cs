using System.Threading;
using System.Text.Json;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Test.BringUp;
using Xunit;

namespace Amiasea.Loom.Test;

public class NullabilityTests
{
    private static (object result, string json) Run(
        string query,
        IGraphQueryableProvider provider)
    {
        var schema = TestSchemaBuilder.Build();
        var engine = TestEngineBuilder.Build(schema);
        var (document, operation) = TestParser.Parse(query);

        var result = engine.ExecuteAsync(document, operation, provider, CancellationToken.None)
                           .GetAwaiter()
                           .GetResult();

        var json = JsonSerializer.Serialize(
            result,
            new JsonSerializerOptions { WriteIndented = true });

        return (result, json);
    }

    [Fact]
    public void NonNull_field_throws_when_null_encountered()
    {
        var provider = new TestNullUserProvider();

        Assert.Throws<ProjectionNullabilityException>(() =>
        {
            Run("{ users { id name } }", provider);
        });
    }

    [Fact]
    public void Nullable_field_allows_null_when_schema_is_nullable()
    {
        var schema = TestSchemaNullable.Build(); // see next section
        var provider = new TestNullUserProvider();
        var engine = TestEngineBuilder.Build(schema);
        var (document, operation) = TestParser.Parse("{ users { id name } }");

        var result = engine.ExecuteAsync(document, operation, provider, CancellationToken.None)
                           .GetAwaiter()
                           .GetResult();

        var json = JsonSerializer.Serialize(
            result,
            new JsonSerializerOptions { WriteIndented = true });

        Assert.Contains("\"name\": null", json);
    }
}