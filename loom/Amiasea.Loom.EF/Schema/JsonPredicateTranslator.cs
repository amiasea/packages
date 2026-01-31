using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Amiasea.Loom.EF.Schema;

public static class JsonPredicateTranslator
{
    public static Expression Rewrite(Expression expr)
    {
        return new JsonVisitor().Visit(expr);
    }

    private sealed class JsonVisitor : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            // If EF sees a member access on a JSON-owned type,
            // it automatically rewrites it to JSON_VALUE / JSON_QUERY.
            //
            // So we do NOT rewrite anything here.
            // We only ensure the expression tree stays "pure".

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // EF supports:
            // - string.Contains
            // - string.StartsWith
            // - string.EndsWith
            // - Enumerable.Contains
            // - Any/All on JSON collections
            //
            // So again: do NOT rewrite.
            // Just ensure arguments are visited.

            return base.VisitMethodCall(node);
        }
    }
}