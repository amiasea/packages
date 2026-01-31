using System;
using System.Collections.Generic;
using Amiasea.Loom.Projection;
using Microsoft.EntityFrameworkCore;

namespace Amiasea.Loom.EF;

public sealed class EFSchemaContext
{
    public DbContext Db { get; }

    // CLR type → schema type name
    public Dictionary<Type, string> Names { get; } = new();

    // CLR type → mutable field list
    public Dictionary<Type, List<IProjectionFieldDefinition>> Fields { get; } = new();

    public Dictionary<Type, IProjectionInputType> InputTypes { get; } = new();

    public EFSchemaContext(DbContext db)
    {
        Db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public void AddType(Type clr, string name)
    {
        if (clr == null) throw new ArgumentNullException(nameof(clr));
        if (name == null) throw new ArgumentNullException(nameof(name));

        Names[clr] = name;
        if (!Fields.ContainsKey(clr))
            Fields[clr] = new List<IProjectionFieldDefinition>();
    }

    public void AddField(Type clr, IProjectionFieldDefinition field)
    {
        if (clr == null) throw new ArgumentNullException(nameof(clr));
        if (field == null) throw new ArgumentNullException(nameof(field));

        if (!Fields.TryGetValue(clr, out var list))
        {
            list = new List<IProjectionFieldDefinition>();
            Fields[clr] = list;
        }

        list.Add(field);
    }
}