using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Contacts.DeleteContact;

public static class DeleteContactModule
{
    public static IServiceCollection AddDeleteContactModule(this IServiceCollection services) =>
        services.AddScoped<IDeleteContactSession, NpgsqlDeleteContactSession>();
}