using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApp.ToDo.GetToDos;

public static class GetToDosEndpoint
{
    public static WebApplication MapGetToDos(this WebApplication app)
    {
        app.MapGet("/todos", GetToDos);
        return app;
    }

    public static async Task<IResult> GetToDos(
        IGetToDosSession session,
        CancellationToken cancellationToken = default
    )
    {
        var toDoList = await session.GetToDoListAsync(cancellationToken);
        var dtoList = toDoList.MapToDtoList();
        return Results.Ok(dtoList);
    }
}