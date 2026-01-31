using System.Collections.Generic;

namespace Amiasea.Loom.Projection
{
    public static class ProjectionIncludeBuilder
    {
        public static IReadOnlyList<string> Build(ProjectionSelection selection)
        {
            var includes = new List<string>();
            BuildInternal(selection, "", includes);
            return includes;
        }

        private static void BuildInternal(
            ProjectionSelection selection,
            string prefix,
            List<string> includes)
        {
            foreach (var field in selection.Fields)
            {
                if (field.Nested != null)
                {
                    var path = prefix == "" ? field.Name : prefix + "." + field.Name;
                    includes.Add(path);
                    BuildInternal(field.Nested, path, includes);
                }
            }
        }
    }
}