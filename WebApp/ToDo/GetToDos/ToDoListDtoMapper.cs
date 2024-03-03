using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using WebApp.DatabaseAccess.Model;

namespace WebApp.ToDo.GetToDos;

[Mapper]
public static partial class ToDoListDtoMapper
{
    public static partial List<ToDoListDto> MapToDtoList(this List<ToDoItem> toDoItem);
}