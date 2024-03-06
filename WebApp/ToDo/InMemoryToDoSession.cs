using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApp.DatabaseAccess.Model;
using WebApp.ToDo.GetToDos;

namespace WebApp.ToDo;

public sealed class InMemoryToDoSession : IGetToDosSession
{
    private readonly List<ToDoItem> _items =
    [
        new ToDoItem
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Title = "Take out garbage",
            Description = "The garbage is starting to smell - I should take it to the dumpster",
            CreatedAtUtc = new DateTime(2024, 3, 6, 7, 0, 0, DateTimeKind.Utc),
            CompletedAtUtc = new DateTime(2024, 3, 6, 7, 45, 0, DateTimeKind.Utc)
        },
        new ToDoItem
        {
            Id = new Guid("00000000-0000-0000-0000-000000000002"),
            Title = "Clean the kitchen sink",
            Description = "Cooking yesterday evening left some stains in the sink - let's clean it up",
            CreatedAtUtc = new DateTime(2024, 3, 6, 7, 0, 1, DateTimeKind.Utc)
        },
        new ToDoItem
        {
            Id = new Guid("00000000-0000-0000-0000-000000000003"),
            Title = "Vacuum the living room",
            Description = "All the crumbles from the chips I ate while watching TV are still on the floor",
            CreatedAtUtc = new DateTime(2024, 3, 6, 7, 0, 2, DateTimeKind.Utc)
        },
        new ToDoItem
        {
            Id = new Guid("00000000-0000-0000-0000-000000000003"),
            Title = "Clean the bathroom",
            Description = "It's been a while...",
            CreatedAtUtc = new DateTime(2024, 3, 6, 7, 0, 3, DateTimeKind.Utc)
        }
    ];

    public void Dispose() { }

    public ValueTask DisposeAsync() => default;

    public Task<List<ToDoItem>> GetToDoListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_items);
}