using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Test.BringUp;

public sealed class TestProjectionExecutor : IProjectionExecutor
{
    public async Task<ProjectionResult> ExecuteAsync(
        ProjectionPlan plan,
        CancellationToken cancellationToken)
    {
        var request = (NormalizedProjectionRequest)plan.RootExecutionNode;

        if (!request.Context.Items.TryGetValue("provider", out var providerObj) ||
            providerObj is not IGraphQueryableProvider provider)
        {
            throw new InvalidOperationException("ProjectionContext missing provider.");
        }

        var rootQueryable = await provider
            .GetRootAsync(request.RootType.Name, cancellationToken)
            .ConfigureAwait(false);

        var rootResult = await provider
            .ExecuteQueryAsync(rootQueryable, cancellationToken)
            .ConfigureAwait(false);

        var data = await ResolveRootAsync(rootResult, request.Fields, provider, cancellationToken)
            .ConfigureAwait(false);

        return new ProjectionResult(data, new List<ProjectionError>());
    }

    // ------------------------------------------------------------
    // ROOT
    // ------------------------------------------------------------
    private static async Task<object?> ResolveRootAsync(
        object rootResult,
        IReadOnlyList<NormalizedProjectionField> fields,
        IGraphQueryableProvider provider,
        CancellationToken token)
    {
        if (fields.Count == 1)
            return await ResolveFieldOnRootAsync(rootResult, fields[0], provider, token);

        var obj = new Dictionary<string, object?>();

        foreach (var field in fields)
        {
            var name = field.Alias ?? field.Definition.Name;
            obj[name] = await ResolveFieldOnRootAsync(rootResult, field, provider, token);
        }

        return obj;
    }

    private static async Task<object?> ResolveFieldOnRootAsync(
        object rootResult,
        NormalizedProjectionField field,
        IGraphQueryableProvider provider,
        CancellationToken token)
    {
        object? instance = ExtractFirst(rootResult);
        if (instance == null)
            return null;

        var raw = await provider
            .GetValueAsync(instance, field.Definition.Name, token)
            .ConfigureAwait(false);

        return await ResolveValueAsync(raw, field.Definition.ReturnType, field, provider, token);
    }

    private static object? ExtractFirst(object value)
    {
        if (value is IEnumerable e)
        {
            foreach (var item in e)
                return item;
        }
        return value;
    }

    // ------------------------------------------------------------
    // VALUE DISPATCH
    // ------------------------------------------------------------
    private static async Task<object?> ResolveValueAsync(
        object? value,
        IProjectionOutputType type,
        NormalizedProjectionField field,
        IGraphQueryableProvider provider,
        CancellationToken token)
    {
        switch (type)
        {
            case ProjectionOutputNonNullType nn:
                return await ResolveNonNullAsync(value, nn, field, provider, token);

            case ProjectionOutputListType list:
                return await ResolveListAsync(value, list, field, provider, token);

            case ProjectionOutputScalarType scalar:
                return value; // scalars are leaf values

            case ProjectionObjectType obj:
                return await ResolveObjectAsync(value, obj, field.Children, provider, token);

            default:
                throw new InvalidOperationException($"Unknown output type: {type.GetType().Name}");
        }
    }

    // ------------------------------------------------------------
    // NON-NULL
    // ------------------------------------------------------------
    private static async Task<object?> ResolveNonNullAsync(
        object? value,
        ProjectionOutputNonNullType nn,
        NormalizedProjectionField field,
        IGraphQueryableProvider provider,
        CancellationToken token)
    {
        if (value == null)
            throw new ProjectionNullabilityException(
                $"Non-null field '{field.Definition.Name}' resolved to null.");

        return await ResolveValueAsync(value, nn.InnerType, field, provider, token);
    }

    // ------------------------------------------------------------
    // LIST
    // ------------------------------------------------------------
    private static async Task<object?> ResolveListAsync(
        object? value,
        ProjectionOutputListType listType,
        NormalizedProjectionField field,
        IGraphQueryableProvider provider,
        CancellationToken token)
    {
        if (value == null)
            return null;

        if (value is not IEnumerable e)
            throw new InvalidOperationException(
                $"Field '{field.Definition.Name}' expected a list but got {value.GetType().Name}");

        var result = new List<object?>();

        foreach (var item in e)
        {
            result.Add(await ResolveValueAsync(item, listType.ElementType, field, provider, token));
        }

        return result;
    }

    // ------------------------------------------------------------
    // OBJECT
    // ------------------------------------------------------------
    private static async Task<object?> ResolveObjectAsync(
        object? value,
        ProjectionObjectType objType,
        IReadOnlyList<NormalizedProjectionField> fields,
        IGraphQueryableProvider provider,
        CancellationToken token)
    {
        if (value == null)
            return null;

        var result = new Dictionary<string, object?>();

        foreach (var field in fields)
        {
            var name = field.Alias ?? field.Definition.Name;

            var raw = await provider
                .GetValueAsync(value, field.Definition.Name, token)
                .ConfigureAwait(false);

            result[name] = await ResolveValueAsync(
                raw,
                field.Definition.ReturnType,
                field,
                provider,
                token);
        }

        return result;
    }
}