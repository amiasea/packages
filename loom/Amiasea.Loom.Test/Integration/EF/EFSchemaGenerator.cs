using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.EF;

public static class EfSchemaGenerator
{
    public static IProjectionSchema FromDbContext(DbContext db)
    {
        var dbType = db.GetType();
        var dbSets = dbType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .ToArray();

        var scalarInt = new ProjectionOutputScalarType("Int", typeof(int));
        var scalarString = new ProjectionOutputScalarType("String", typeof(string));

        var objectTypes = dbSets
            .Select(p => BuildObjectType(p.PropertyType.GetGenericArguments()[0], scalarInt, scalarString))
            .ToList();

        var queryFields = dbSets
            .Select(p =>
                new ProjectionFieldDefinition(
                    p.Name,
                    objectTypes.First(t => t.Name == p.PropertyType.GetGenericArguments()[0].Name)
                        .List()
                        .NonNull(),
                    Array.Empty<IProjectionArgumentDefinition>(),
                    allowExtraArguments: false,
                    extraArgumentTypeResolver: null))
            .ToArray();

        var queryType = new ProjectionObjectType("Query", queryFields);

        var allTypes = objectTypes.Cast<IProjectionType>()
            .Concat(new IProjectionType[] { queryType, scalarInt, scalarString })
            .ToArray();

        return new ProjectionSchema(allTypes, new[] { queryType });
    }

    private static ProjectionObjectType BuildObjectType(
        Type clrType,
        ProjectionOutputScalarType scalarInt,
        ProjectionOutputScalarType scalarString)
    {
        var fields = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => BuildField(p, scalarInt, scalarString))
            .Where(f => f != null)
            .Cast<IProjectionFieldDefinition>()
            .ToArray();

        return new ProjectionObjectType(clrType.Name, fields);
    }

    private static IProjectionFieldDefinition? BuildField(
        PropertyInfo prop,
        ProjectionOutputScalarType scalarInt,
        ProjectionOutputScalarType scalarString)
    {
        var type = prop.PropertyType;

        if (type == typeof(int))
            return new ProjectionFieldDefinition(prop.Name, scalarInt, Array.Empty<IProjectionArgumentDefinition>(), false, null);

        if (type == typeof(string))
            return new ProjectionFieldDefinition(prop.Name, scalarString, Array.Empty<IProjectionArgumentDefinition>(), false, null);

        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            if (type.IsGenericType)
            {
                var elementType = type.GetGenericArguments()[0];
                return new ProjectionFieldDefinition(
                    prop.Name,
                    new ProjectionObjectType(elementType.Name, Array.Empty<IProjectionFieldDefinition>()).List(),
                    Array.Empty<IProjectionArgumentDefinition>(),
                    false,
                    null);
            }
        }

        if (!type.IsPrimitive && !type.IsEnum && type != typeof(string))
        {
            return new ProjectionFieldDefinition(
                prop.Name,
                new ProjectionObjectType(type.Name, Array.Empty<IProjectionFieldDefinition>()),
                Array.Empty<IProjectionArgumentDefinition>(),
                false,
                null);
        }

        return null;
    }
}