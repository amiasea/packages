using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Test.BringUp;

public static class TestEngineBuilder
{
    public static IProjectionEngine Build(IProjectionSchema schema)
    {
        var planner = new TestProjectionPlanner();
        var executor = new TestProjectionExecutor();
        var execEngine = new ExecutionEngine(schema, planner, executor);
        var requestFactory = new ProjectionRequestFactory();

        return new ProjectionEngine(execEngine, requestFactory);
    }
}