using Amiasea.Loom.Projection;

namespace Amiasea.Loom.Test.BringUp;

public static class TestSchemaNullable
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
                false,
                null),

            // name is now nullable: no NonNull wrapper
            new ProjectionFieldDefinition(
                "name",
                scalarString,
                Array.Empty<IProjectionArgumentDefinition>(),
                false,
                null)
        };

        var userType = new ProjectionObjectType("User", userFields);

        var queryFields = new IProjectionFieldDefinition[]
        {
            new ProjectionFieldDefinition(
                "users",
                userType.List().NonNull(),
                Array.Empty<IProjectionArgumentDefinition>(),
                false,
                null)
        };

        var queryType = new ProjectionObjectType("Query", queryFields);

        return new ProjectionSchema(
            new IProjectionType[] { queryType, userType, scalarInt, scalarString },
            new IProjectionType[] { queryType });
    }
}