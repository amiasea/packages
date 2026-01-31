using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amiasea.Loom.Projection;
using Amiasea.Loom.Schema;
using Amiasea.Loom.Schema.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Amiasea.Loom.EF
{
    public sealed class EFSchemaGenerator
    {
        private readonly IModel _model;

        public EFSchemaGenerator(IModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            _model = model;
        }

        public IProjectionSchema Generate(IEnumerable<Type> rootEntityTypes)
        {
            var builder = new SchemaBuilder();

            foreach (var entityType in _model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                var typeBuilder = builder.Type(clrType.Name);

                foreach (var prop in entityType.GetProperties())
                {
                    BuildField(typeBuilder, clrType, prop);
                }

                typeBuilder.EndType();
            }

            foreach (var root in rootEntityTypes)
                builder.Root(root.Name);

            return builder.Build();
        }

        private void BuildField(TypeBuilder typeBuilder, Type clrType, IProperty prop)
        {
            var propInfo = clrType.GetProperty(prop.Name);
            var fb = typeBuilder.Field(prop.Name);

            var fieldType = prop.ClrType;
            var kind = InferKind(fieldType, prop);

            fb.Type(fieldType, kind)
              .Nullable(prop.IsNullable);

            if (IsEnum(fieldType))
            {
                fb.Enum(Enum.GetNames(fieldType));
            }

            if (IsList(fieldType))
            {
                var elementType = fieldType.IsGenericType
                    ? fieldType.GetGenericArguments()[0]
                    : typeof(object);

                fb.List(elementType);
            }

            fb.Operators(InferOperators(kind));

            ApplyAttributes(fb, propInfo);
            ApplyConventions(fb, prop.Name, kind);
        }

        private FieldKind InferKind(Type fieldType, IProperty prop)
        {
            if (IsEnum(fieldType)) return FieldKind.Enum;
            if (IsList(fieldType)) return FieldKind.List;

            if (fieldType == typeof(string)) return FieldKind.String;
            if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?)) return FieldKind.Date;

            if (fieldType == typeof(int) || fieldType == typeof(int?) ||
                fieldType == typeof(long) || fieldType == typeof(long?) ||
                fieldType == typeof(float) || fieldType == typeof(float?) ||
                fieldType == typeof(double) || fieldType == typeof(double?) ||
                fieldType == typeof(decimal) || fieldType == typeof(decimal?))
                return FieldKind.Numeric;

            return FieldKind.Scalar;
        }

        private bool IsEnum(Type t)
        {
            return t.IsEnum || (Nullable.GetUnderlyingType(t) != null && Nullable.GetUnderlyingType(t).IsEnum);
        }

        private bool IsList(Type t)
        {
            if (!t.IsGenericType) return false;
            var gen = t.GetGenericTypeDefinition();
            return gen == typeof(List<>);
        }

        private string[] InferOperators(FieldKind kind)
        {
            if (kind == FieldKind.Numeric || kind == FieldKind.Date)
                return new[] { "eq", "neq", "gt", "gte", "lt", "lte", "isNull", "notNull" };

            if (kind == FieldKind.String)
                return new[] { "eq", "neq", "contains", "startsWith", "endsWith", "isNull", "notNull" };

            if (kind == FieldKind.Enum)
                return new[] { "eq", "neq", "in", "isNull", "notNull" };

            if (kind == FieldKind.List)
                return new[] { "contains", "isNull", "notNull" };

            return new[] { "eq", "neq", "isNull", "notNull" };
        }

        private void ApplyAttributes(FieldBuilder fb, PropertyInfo prop)
        {
            if (prop == null) return;

            var temporal = (TemporalAttribute)Attribute.GetCustomAttribute(prop, typeof(TemporalAttribute));
            if (temporal != null)
                fb.Temporal(temporal.Group, temporal.Role);

            var range = (RangeAttribute)Attribute.GetCustomAttribute(prop, typeof(RangeAttribute));
            if (range != null)
                fb.Range(range.Group, range.Role);

            var geo = (GeoAttribute)Attribute.GetCustomAttribute(prop, typeof(GeoAttribute));
            if (geo != null)
                fb.Geo(geo.Group, geo.Role);

            var depends = (DependsOnAttribute[])prop.GetCustomAttributes(typeof(DependsOnAttribute), true);
            if (depends != null)
            {
                foreach (var d in depends)
                    fb.DependsOn(d.WhenField, d.WhenEquals, prop.Name);
            }
        }

        private void ApplyConventions(FieldBuilder fb, string name, FieldKind kind)
        {
            var lower = name.ToLowerInvariant();

            if (lower == "createdat")
                fb.Temporal("lifecycle", "start");

            if (lower == "updatedat")
                fb.Temporal("lifecycle", "end");

            if (lower.EndsWith("min"))
                fb.Range(name.Substring(0, name.Length - 3), "min");

            if (lower.EndsWith("max"))
                fb.Range(name.Substring(0, name.Length - 3), "max");

            if (lower == "lat" || lower == "latitude")
                fb.Geo("location", "lat");

            if (lower == "lng" || lower == "lon" || lower == "longitude")
                fb.Geo("location", "lng");
        }
    }
}