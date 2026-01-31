using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Amiasea.Loom.Projection;

namespace Amiasea.Loom.EF.Schema
{
    public static class EFWhereBuilder
    {
        public static Expression<Func<T, bool>> Build<T>(FilterNode filter)
        {
            if (filter == null)
                return e => true;

            var param = Expression.Parameter(typeof(T), "e");
            var body = BuildNode(param, filter);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression BuildNode(ParameterExpression param, FilterNode node)
        {
            // Logical: AND
            if (node.And != null && node.And.Count > 0)
            {
                Expression combined = null;

                foreach (var child in node.And)
                {
                    var expr = BuildNode(param, child);
                    combined = combined == null ? expr : Expression.AndAlso(combined, expr);
                }

                return combined ?? Expression.Constant(true);
            }

            // Logical: OR
            if (node.Or != null && node.Or.Count > 0)
            {
                Expression combined = null;

                foreach (var child in node.Or)
                {
                    var expr = BuildNode(param, child);
                    combined = combined == null ? expr : Expression.OrElse(combined, expr);
                }

                return combined ?? Expression.Constant(false);
            }

            // Logical: NOT
            if (node.Not != null)
            {
                return Expression.Not(BuildNode(param, node.Not));
            }

            // Comparison
            if (node.Field == null || node.Operator == null)
                throw new InvalidOperationException("Invalid comparison FilterNode.");

            var member = BuildMemberAccess(param, node.Field);
            return BuildComparison(member, node.Operator, node.Value);
        }

        private static Expression BuildMemberAccess(Expression param, string fieldPath)
        {
            var parts = fieldPath.Split('.');
            Expression current = param;

            for (int i = 0; i < parts.Length; i++)
            {
                current = Expression.PropertyOrField(current, parts[i]);
            }

            return current;
        }

        private static Expression BuildComparison(Expression member, string op, object value)
        {
            // Handle null
            if (value == null)
            {
                if (op == "eq") return Expression.Equal(member, Expression.Constant(null));
                if (op == "neq") return Expression.NotEqual(member, Expression.Constant(null));

                throw new InvalidOperationException("Null only supports eq/neq.");
            }

            var constant = Expression.Constant(value, member.Type);

            // eq / neq
            if (op == "eq") return Expression.Equal(member, constant);
            if (op == "neq") return Expression.NotEqual(member, constant);

            // numeric
            if (op == "lt") return Expression.LessThan(member, constant);
            if (op == "lte") return Expression.LessThanOrEqual(member, constant);
            if (op == "gt") return Expression.GreaterThan(member, constant);
            if (op == "gte") return Expression.GreaterThanOrEqual(member, constant);

            // string
            if (member.Type == typeof(string))
            {
                if (op == "contains")
                    return Expression.Call(member, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);

                if (op == "startsWith")
                    return Expression.Call(member, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);

                if (op == "endsWith")
                    return Expression.Call(member, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant);
            }

            // in / nin
            if (op == "in")
                return BuildIn(member, value);

            if (op == "nin")
                return Expression.Not(BuildIn(member, value));

            throw new InvalidOperationException("Unsupported operator '" + op + "'.");
        }

        private static Expression BuildIn(Expression member, object value)
        {
            var enumerable = value as IEnumerable;
            if (enumerable == null)
                throw new InvalidOperationException("'in' operator requires a list.");

            var list = enumerable.Cast<object>().ToList();
            var listConst = Expression.Constant(list);

            return Expression.Call(
                typeof(Enumerable),
                "Contains",
                new[] { member.Type },
                listConst,
                member
            );
        }
    }
}