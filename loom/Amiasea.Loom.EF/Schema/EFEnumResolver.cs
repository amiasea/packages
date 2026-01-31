using System;
using System.Linq;
using Amiasea.Loom.EF.Schema;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF;

public sealed class EFEnumResolver : IEFSchemaResolver
{
    private readonly DbContext _db;

    public EFEnumResolver(DbContext db)
    {
        _db = db;
    }

    public void Resolve(EFSchemaContext context)
    {
        var model = _db.Model;

        foreach (var clr in context.Names.Keys)
        {
            var entity = model.FindEntityType(clr);
            if (entity == null)
                continue;

            var fields = context.Fields[clr];

            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                // Only scalar properties can be enums
                var prop = entity.FindProperty(field.Name);
                if (prop == null)
                    continue;

                var type = prop.ClrType;

                // Nullable<T> → unwrap
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);

                if (!type.IsEnum)
                    continue;

                // Build enum output type
                var enumType = new ProjectionOutputEnumType(
                    name: type.Name,
                    values: Enum.GetNames(type)
                );

                // Preserve nullability wrapper if present
                var updatedReturnType =
                    field.ReturnType is ProjectionOutputNonNullType nn
                        ? new ProjectionOutputNonNullType(enumType)
                        : (IProjectionOutputType)enumType;

                // Replace field definition
                fields[i] = new ProjectionFieldDefinition(
                    name: field.Name,
                    returnType: updatedReturnType,
                    arguments: field.Arguments,
                    allowExtraArguments: false,
                    extraArgumentTypeResolver: null
                );
            }
        }
    }
}