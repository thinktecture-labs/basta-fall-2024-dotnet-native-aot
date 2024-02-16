using Microsoft.Extensions.DependencyInjection;

namespace WebApp.JsonAccess;

public static class JsonExtensions
{
    public static IServiceCollection AddJsonSerializationContext(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(
            options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializationContext.Default);
            }
        );
}