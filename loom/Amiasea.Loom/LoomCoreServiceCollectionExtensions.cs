using Amiasea.Loom.Metadata;
using Amiasea.Loom.Projection;
using Microsoft.Extensions.DependencyInjection;

namespace Amiasea.Loom
{
    public static class LoomCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddLoomCore(this IServiceCollection services)
        {
            var shapes = LoomGeneratedMetadata.FieldShapes;

            services.AddSingleton<IFieldShapeResolver>(new FieldShapeResolver(shapes));
            services.AddSingleton<IProjectionEngine, ProjectionEngine>();

            return services;
        }
    }
}