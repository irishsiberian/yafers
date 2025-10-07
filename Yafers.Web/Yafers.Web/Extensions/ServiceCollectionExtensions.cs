using Yafers.Web.Domain;

namespace Yafers.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonConfiguration(
            this IServiceCollection context,
            IConfiguration configuration
        )
        {
            var instance = configuration
                .GetSection("CommonConfiguration")
                .Get<CommonConfiguration>();

            return context.AddScoped(sp =>
            {
                return instance;
            });
        }
    }
}
