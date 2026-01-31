// EngineHeartbeatTests.cs
using System.Collections.Generic;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Test.BringUp;
using Xunit;

namespace Amiasea.Loom.Test;

public sealed class EngineHeartbeatTests
{
    [Fact]
    public void Engine_executes_minimal_query_end_to_end()
    {
        // Arrange
        var schema = TestSchemaBuilder.Build();
        var provider = new TestProvider();
        var engine = TestEngineBuilder.Build(schema);
        var (document, operation) = TestParser.Parse("{ users { id name } }");

        // Act
        var result = engine.ExecuteAsync(document, operation, provider, default)
                           .GetAwaiter()
                           .GetResult();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ProjectionResult>(result);

        var data = ((ProjectionResult)result).Data as IList<object?>;
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        var user = data[0] as IDictionary<string, object?>;
        Assert.NotNull(user);

        Assert.Equal(1, user["id"]);
        Assert.Equal("Alice", user["name"]);
    }
}