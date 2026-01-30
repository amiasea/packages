using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Amiasea.Loom.Build.Task
{
    public sealed class LoomEfModelTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string AssemblyPath { get; set; }

        [Required]
        public string DbContextTypeName { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public override bool Execute()
        {
            try
            {
                // 1. Load the user's compiled assembly
                var asm = Assembly.LoadFrom(AssemblyPath);

                // 2. Resolve the DbContext type
                var ctxType = asm.GetType(DbContextTypeName);
                if (ctxType == null)
                    throw new Exception("DbContext type '" + DbContextTypeName + "' not found.");

                // 3. Resolve the compiled model (DbContext.Model or nested type)
                var model = ResolveCompiledModel(ctxType);
                if (model == null)
                    throw new Exception("No compiled EF model found on DbContext or nested model type.");

                // 4. Extract EF Core model metadata
                var shapes = new Dictionary<string, Dictionary<string, string>>();

                foreach (var entity in model.GetEntityTypes())
                {
                    var parent = entity.ClrType.Name;
                    shapes[parent] = new Dictionary<string, string>();

                    // Scalar properties
                    foreach (var prop in entity.GetProperties())
                    {
                        shapes[parent][prop.Name] = "Scalar";
                    }

                    // Navigations
                    foreach (var nav in entity.GetNavigations())
                    {
                        bool isCollection = nav.IsCollection();
                        shapes[parent][nav.Name] = isCollection ? "List" : "Object";
                    }
                }

                // 5. Write metadata JSON
                var json = JsonSerializer.Serialize(
                    shapes,
                    new JsonSerializerOptions { WriteIndented = true }
                );

                File.WriteAllText(OutputPath, json);

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
        }

        private static IModel ResolveCompiledModel(Type ctxType)
        {
            // Pattern A: static Model on the DbContext
            var modelProp = ctxType.GetProperty(
                "Model",
                BindingFlags.Public | BindingFlags.Static
            );

            if (modelProp != null)
            {
                var modelA = modelProp.GetValue(null) as IModel;
                if (modelA != null)
                    return modelA;
            }

            // Pattern B: static Model on a nested type ending with "Model"
            var nestedTypes = ctxType.GetNestedTypes(
                BindingFlags.NonPublic | BindingFlags.Public
            );

            foreach (var nt in nestedTypes)
            {
                if (nt.Name.EndsWith("Model"))
                {
                    var nestedModelProp = nt.GetProperty(
                        "Model",
                        BindingFlags.Public | BindingFlags.Static
                    );

                    if (nestedModelProp != null)
                    {
                        var modelB = nestedModelProp.GetValue(null) as IModel;
                        if (modelB != null)
                            return modelB;
                    }
                }
            }

            return null;
        }
    }
}