using System;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Test.BringUp;

public static class TestSchemaBuilder
{
    public static IProjectionSchema Build()
    {
        var scalarInt = new ProjectionOutputScalarType("Int", typeof(int));
        var scalarString = new ProjectionOutputScalarType("String", typeof(string));

        var userFields = new IProjectionFieldDefinition[]
        {
            new ProjectionFieldDefinition(
                "id",
                scalarInt,
                Array.Empty<IProjectionArgumentDefinition>(),
                allowExtraArguments: false,
                extraArgumentTypeResolver: null),

            // NON-NULL name: this is what the failing test expects
            new ProjectionFieldDefinition(
                "name",
                scalarString.NonNull(),
                Array.Empty<IProjectionArgumentDefinition>(),
                allowExtraArguments: false,
                extraArgumentTypeResolver: null)
        };

        var userType = new ProjectionObjectType("User", userFields);

        var queryFields = new IProjectionFieldDefinition[]
        {
            new ProjectionFieldDefinition(
                "users",
                userType.List().NonNull(),
                Array.Empty<IProjectionArgumentDefinition>(),
                allowExtraArguments: false,
                extraArgumentTypeResolver: null),

            new ProjectionFieldDefinition(
                "user",
                userType.NonNull(),
                Array.Empty<IProjectionArgumentDefinition>(),
                allowExtraArguments: false,
                extraArgumentTypeResolver: null)
        };

        var queryType = new ProjectionObjectType("Query", queryFields);

        return new ProjectionSchema(
            new IProjectionType[] { queryType, userType, scalarInt, scalarString },
            new IProjectionType[] { queryType });
    }
}