using System.Reflection;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF.Schema;

internal static class EFEntityResolver
{
    public static IReadOnlyDictionary<Type, ProjectionObjectType> ResolveEntities(DbContext db)
    {
        var contextType = db.GetType();

        // Find all DbSet<T> properties
        var dbSetProps = contextType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => IsDbSet(p.PropertyType))
            .ToList();

        var result = new Dictionary<Type, ProjectionObjectType>();

        foreach (var prop in dbSetProps)
        {
            var entityType = prop.PropertyType.GetGenericArguments()[0];

            // Create an empty ProjectionObjectType shell
            var type = new ProjectionObjectType(
                name: entityType.Name,
                fields: Array.Empty<ProjectionFieldNode>()
            );

            result[entityType] = type;
        }

        return result;
    }

    private static bool IsDbSet(Type t)
        => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(DbSet<>);
}