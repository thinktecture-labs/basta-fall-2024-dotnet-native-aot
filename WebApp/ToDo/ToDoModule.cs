using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebApp.ToDo.GetToDos;

namespace WebApp.ToDo;

public static class ToDoModule
{
    public static IServiceCollection AddToDoModule(this IServiceCollection services) =>
        services
           .AddSingleton<InMemoryToDoSession>()
           .AddScoped<IGetToDosSession>(serviceProvider => serviceProvider.GetRequiredService<InMemoryToDoSession>());

    public static WebApplication MapToDoEndpoints(this WebApplication app) => app.MapGetToDos();
}