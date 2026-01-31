using Amiasea.Loom;
using Microsoft.Extensions.DependencyInjection;

namespace Amiasea.Loom.EF
{
    public static class LoomEFServiceCollectionExtensions
    {
        public static IServiceCollection AddLoomEF(this IServiceCollection services)
        {
            services.AddScoped<IGraphQueryableProvider, EFProvider>();
            return services;
        }
    }
}