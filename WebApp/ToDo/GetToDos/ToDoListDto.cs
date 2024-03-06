using System;
using System.Collections.Generic;
using WebApp.DatabaseAccess.Model;

namespace WebApp.ToDo.GetToDos;

public sealed record ToDoListDto(Guid Id, string Title, DateTime CreatedAtUtc, DateTime? CompletedAtUtc)
{
    public static List<ToDoListDto> FromToDoItems(List<ToDoItem> toDoItems)
    {
        var dtoList = new List<ToDoListDto>(toDoItems.Count);
        foreach (var toDoItem in toDoItems)
        {
            var dto = new ToDoListDto(toDoItem.Id, toDoItem.Title, toDoItem.CreatedAtUtc, toDoItem.CompletedAtUtc);
            dtoList.Add(dto);
        }

        return dtoList;
    }
}