using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebApp.Reflection;
using WebApp.ToDo.GetToDos;

namespace WebApp.JsonAccess;

[JsonSerializable(typeof(ReflectionDto))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(List<ToDoListDto>))]
public sealed partial class AppJsonSerializationContext : JsonSerializerContext;