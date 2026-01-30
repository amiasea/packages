using System;
using System.Collections.Generic;
using Amiasea.Loom.AST;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionRequestFactory : IProjectionRequestFactory
    {
        public ProjectionRequest Create(
            DocumentNode document,
            OperationNode operation,
            IGraphQueryableProvider provider)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            // ------------------------------------------------------------
            // 1. Determine the root type name
            // ------------------------------------------------------------
            string rootTypeName;

            switch (operation.Kind)
            {
                case OperationKind.Query:
                    rootTypeName = "Query";
                    break;

                case OperationKind.Mutation:
                    rootTypeName = "Mutation";
                    break;

                default:
                    throw new InvalidOperationException(
                        "Unsupported operation kind: " + operation.Kind);
            }

            // ------------------------------------------------------------
            // 2. Convert the selection set into ProjectionFields
            // ------------------------------------------------------------
            List<ProjectionField> fields = new List<ProjectionField>();

            if (operation.SelectionSet != null)
            {
                foreach (var selection in operation.SelectionSet.Selections)
                {
                    FieldNode fieldNode = selection as FieldNode;
                    if (fieldNode == null)
                    {
                        throw new InvalidOperationException(
                            "Unsupported selection node type: " + selection.GetType().Name);
                    }

                    fields.Add(ConvertField(fieldNode));
                }
            }

            // ------------------------------------------------------------
            // 3. Build the ProjectionContext
            // ------------------------------------------------------------
            var contextItems = new Dictionary<string, object>();
            contextItems["provider"] = provider;

            ProjectionContext context = new ProjectionContext(contextItems);

            // ------------------------------------------------------------
            // 4. Return the final ProjectionRequest
            // ------------------------------------------------------------
            return new ProjectionRequest(
                rootTypeName,
                fields,
                context);
        }

        // ====================================================================
        //  FIELD CONVERSION
        // ====================================================================

        private ProjectionField ConvertField(FieldNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            // ------------------------------------------------------------
            // Convert arguments
            // ------------------------------------------------------------
            var args = new Dictionary<string, ProjectionArgumentValue>();

            if (node.Arguments != null)
            {
                foreach (var arg in node.Arguments)
                {
                    args[arg.Name] = ConvertArgument(arg.Value);
                }
            }

            // ------------------------------------------------------------
            // Convert children (nested selection sets)
            // ------------------------------------------------------------
            List<ProjectionField> children = new List<ProjectionField>();

            if (node.SelectionSet != null)
            {
                foreach (var child in node.SelectionSet.Selections)
                {
                    FieldNode childField = child as FieldNode;
                    if (childField == null)
                    {
                        throw new InvalidOperationException(
                            "Unsupported selection node type: " + child.GetType().Name);
                    }

                    children.Add(ConvertField(childField));
                }
            }

            // ------------------------------------------------------------
            // Build the ProjectionField
            // ------------------------------------------------------------
            return new ProjectionField(
                node.Name,
                args,
                children,
                node.Alias,      // may be null
                new List<ProjectionDirective>()); // directives not supported yet
        }

        // ====================================================================
        //  ARGUMENT CONVERSION
        // ====================================================================

        private ProjectionArgumentValue ConvertArgument(ValueNode value)
        {
            if (value is IntValueNode)
                return new ProjectionScalarValue(((IntValueNode)value).Value);

            if (value is FloatValueNode)
                return new ProjectionScalarValue(((FloatValueNode)value).Value);

            if (value is StringValueNode)
                return new ProjectionScalarValue(((StringValueNode)value).Value);

            if (value is BooleanValueNode)
                return new ProjectionScalarValue(((BooleanValueNode)value).Value);

            if (value is EnumValueNode)
                return new ProjectionEnumValue(((EnumValueNode)value).Value);

            if (value is ListValueNode)
            {
                var list = (ListValueNode)value;
                var items = new List<ProjectionArgumentValue>();

                foreach (var element in list.Values)
                    items.Add(ConvertArgument(element));

                return new ProjectionListValue(items);
            }

            if (value is ObjectValueNode)
            {
                var obj = (ObjectValueNode)value;
                var fields = new Dictionary<string, ProjectionArgumentValue>();

                foreach (var field in obj.Fields)
                    fields[field.Name] = ConvertArgument(field.Value);

                return new ProjectionObjectValue(fields);
            }

            throw new InvalidOperationException(
                "Unsupported AST value node type: " + value.GetType().Name);
        }
    }
}