using System;
using System.Linq;
using Amiasea.Loom.EF.Schema;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF;

public sealed class EFNavigationResolver : IEFSchemaResolver
{
    private readonly DbContext _db;

    public EFNavigationResolver(DbContext db)
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

            foreach (var nav in entity.GetNavigations())
            {
                var fieldName = nav.Name;
                var targetClr = nav.TargetEntityType.ClrType;

                if (!context.Names.ContainsKey(targetClr))
                    continue;

                IProjectionOutputType returnType =
                    nav.IsCollection
                        ? new ProjectionOutputListType(
                            new ProjectionObjectType(
                                context.Names[targetClr],
                                context.Fields[targetClr]
                            )
                          )
                        : new ProjectionObjectType(
                            context.Names[targetClr],
                            context.Fields[targetClr]
                          );

                var field = new ProjectionFieldDefinition(
                    name: fieldName,
                    returnType: returnType,
                    arguments: Array.Empty<IProjectionArgumentDefinition>(),
                    allowExtraArguments: false,
                    extraArgumentTypeResolver: null
                );

                context.AddField(clr, field);
            }
        }
    }
}