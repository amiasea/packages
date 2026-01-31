// BringUpTests.cs
using System;
using System.Text.Json;
using System.Threading;
using Amiasea.Loom.Execution;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Test.BringUp;
using Xunit;

namespace Amiasea.Loom.Test
{
    public sealed class BringUpTests
    {
        private static (ProjectionResult Result, string Json) Run(
            string query,
            IGraphQueryableProvider? providerOverride = null,
            IProjectionSchema? schemaOverride = null)
        {
            var schema = schemaOverride ?? TestSchemaBuilder.Build();
            var provider = providerOverride ?? new TestProvider();
            var engine = TestEngineBuilder.Build(schema);
            var (document, operation) = TestParser.Parse(query);

            var result = (ProjectionResult)engine
                .ExecuteAsync(document, operation, provider, CancellationToken.None)
                .GetAwaiter()
                .GetResult();

            var json = JsonSerializer.Serialize(
                result,
                new JsonSerializerOptions { WriteIndented = true });

            return (result, json);
        }

        // BASIC EXECUTION
        [Fact]
        public void Basic_query_executes()
        {
            var (_, json) = Run("{ users { id name } }");

            Assert.Contains("\"id\": 1", json);
            Assert.Contains("\"name\": \"Alice\"", json);
        }

        // LIST SEMANTICS
        [Fact]
        public void Empty_list_is_allowed()
        {
            var provider = new TestEmptyUsersProvider();
            var (_, json) = Run("{ users { id name } }", provider);

            Assert.Contains("[]", json);
        }

        [Fact]
        public void Single_user_list_is_allowed()
        {
            var provider = new TestSingleUserProvider();
            var (_, json) = Run("{ users { id name } }", provider);

            Assert.Contains("\"id\": 42", json);
            Assert.Contains("\"name\": \"Solo\"", json);
        }

        [Fact]
        public void Null_list_throws_when_list_is_non_null()
        {
            var provider = new TestNullUsersProvider();

            Assert.Throws<ProjectionNullabilityException>(() =>
            {
                Run("{ users { id name } }", provider);
            });
        }

        // NULLABILITY
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
        public void Nullable_field_allows_null()
        {
            var provider = new TestNullUserProvider();
            var schema = TestSchemaNullable.Build();

            var (_, json) = Run("{ users { id name } }", provider, schema);

            Assert.Contains("\"name\": null", json);
        }

        // ALIASING
        [Fact]
        public void Aliasing_works()
        {
            var (_, json) = Run("{ users { x: id y: name } }");

            Assert.Contains("\"x\": 1", json);
            Assert.Contains("\"y\": \"Alice\"", json);
        }

        // SCALAR LEAF BEHAVIOR
        [Fact]
        public void Scalars_are_returned_as_raw_values()
        {
            var (_, json) = Run("{ users { id } }");

            Assert.Contains("\"id\": 1", json);
        }

        // OBJECT RESOLUTION
        [Fact]
        public void Object_fields_resolve_correctly()
        {
            var (_, json) = Run("{ users { id name } }");

            Assert.Contains("\"id\": 1", json);
            Assert.Contains("\"name\": \"Alice\"", json);
        }
    }
}