using Microsoft.Extensions.DependencyInjection;

namespace Amiasea.EF
{
    public static class LoomEFServiceCollectionExtensions
    {
        public static IServiceCollection AddLoomEF(this IServiceCollection services)
        {
            services.AddScoped<IGraphQueryableProvider, EFGraphQueryableProvider>();
            return services;
        }
    }
}