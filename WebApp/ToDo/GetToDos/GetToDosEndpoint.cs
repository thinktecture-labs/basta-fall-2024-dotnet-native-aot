using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
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

    public static async Task<IResult> GetToDos(ISender sender, CancellationToken cancellationToken = default)
    {
        var dtoList = await sender.Send(ToDoRequest.Instance, cancellationToken);
        return Results.Ok(dtoList);
    }
}

public sealed class ToDoRequest : IRequest<List<ToDoListDto>>
{
    public static ToDoRequest Instance { get; } = new ();
}

public sealed class ToDoRequestHandler : IRequestHandler<ToDoRequest, List<ToDoListDto>>
{
    private readonly IGetToDosSession _session;

    public ToDoRequestHandler(IGetToDosSession session) => _session = session;

    public async ValueTask<List<ToDoListDto>> Handle(ToDoRequest request, CancellationToken cancellationToken)
    {
        var toDoList = await _session.GetToDoListAsync(cancellationToken);
        var dtoList = toDoList.MapToDtoList();
        return dtoList;
    }
}