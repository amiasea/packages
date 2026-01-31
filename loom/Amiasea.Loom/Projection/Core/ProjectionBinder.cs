using System;
using System.Collections.Generic;
using Amiasea.Loom.AST;
using Amiasea.Loom.Execution;

namespace Amiasea.Loom.Projection
{
    public sealed class ProjectionBinder
    {
        private readonly IFilterNormalizer _filterNormalizer;
        private readonly IProjectionTypeRegistry _types;

        public ProjectionBinder(
            IFilterNormalizer filterNormalizer,
            IProjectionTypeRegistry types)
        {
            _filterNormalizer = filterNormalizer ?? throw new ArgumentNullException(nameof(filterNormalizer));
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public ProjectionPlan Bind(
            Type rootClr,
            ProjectionObjectType rootType,
            SelectionSetNode ast,
            ObjectValueNode rawFilter)
        {
            if (rootClr == null) throw new ArgumentNullException(nameof(rootClr));
            if (rootType == null) throw new ArgumentNullException(nameof(rootType));
            if (ast == null) throw new ArgumentNullException(nameof(ast));

            var includes = new List<string>();
            var selection = BindSelection(rootType, ast, string.Empty, includes);

            FilterNode filter = null;
            if (rawFilter != null)
            {
                // Assuming Normalize(ObjectValueNode, string typeName)
                filter = _filterNormalizer.Normalize(rawFilter, rootType.Name);
            }

            return new ProjectionPlan(
                rootClr,
                selection,
                includes,
                filter
            );
        }

        private ProjectionSelection BindSelection(
            ProjectionObjectType type,
            SelectionSetNode ast,
            string prefix,
            List<string> includes)
        {
            var fields = new List<ProjectionSelectionField>();

            foreach (var node in ast.Selections)
            {
                var fieldNode = node as FieldNode;
                if (fieldNode == null)
                    throw new InvalidOperationException("SelectionSet contains non-field node.");

                var fieldDef = type.GetField(fieldNode.Name);
                if (fieldDef == null)
                {
                    throw new InvalidOperationException(
                        $"Field '{fieldNode.Name}' does not exist on type '{type.Name}'.");
                }

                // fieldDef.Type is System.Type (CLR)
                var clr = fieldDef.Type;
                ProjectionObjectType returnType = null;

                if (fieldNode.SelectionSet != null)
                {
                    // Resolve projection object type from CLR type via registry
                    returnType = _types.GetObjectType(clr);
                    if (returnType == null)
                    {
                        throw new InvalidOperationException(
                            $"Field '{fieldNode.Name}' is not an object type and cannot have a selection.");
                    }

                    var newPrefix = string.IsNullOrEmpty(prefix)
                        ? fieldNode.Name
                        : prefix + "." + fieldNode.Name;

                    includes.Add(newPrefix);

                    var nested = BindSelection(returnType, fieldNode.SelectionSet, newPrefix, includes);

                    fields.Add(new ProjectionSelectionField(
                        fieldNode.Name,
                        nested
                    ));

                    continue;
                }

                // No nested selection
                fields.Add(new ProjectionSelectionField(
                    fieldNode.Name,
                    nested: null
                ));
            }

            return new ProjectionSelection(fields);
        }
    }
}