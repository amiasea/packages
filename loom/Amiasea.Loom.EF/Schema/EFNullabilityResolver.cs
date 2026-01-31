using System;
using System.Linq;
using Amiasea.Loom.EF.Schema;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF;

public sealed class EFNullabilityResolver : IEFSchemaResolver
{
    private readonly DbContext _db;

    public EFNullabilityResolver(DbContext db)
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

                // Only navigation fields have nullability metadata
                var nav = entity.FindNavigation(field.Name);
                if (nav == null)
                    continue;

                bool isRequired = nav.ForeignKey.IsRequired;

                var original = field.ReturnType;

                IProjectionOutputType updatedReturnType;

                if (isRequired)
                {
                    // Required → wrap in NonNull if not already
                    updatedReturnType = original is ProjectionOutputNonNullType
                        ? original
                        : new ProjectionOutputNonNullType(original);
                }
                else
                {
                    // Optional → unwrap NonNull if present
                    updatedReturnType = original is ProjectionOutputNonNullType nn
                        ? nn.InnerType
                        : original;
                }

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