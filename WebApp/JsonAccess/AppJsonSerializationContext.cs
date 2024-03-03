using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebApp.Contacts.Common;
using WebApp.Contacts.GetContacts;
using WebApp.Reflection;
using WebApp.ToDo.GetToDos;

namespace WebApp.JsonAccess;

[JsonSerializable(typeof(ReflectionDto))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(List<ToDoListDto>))]
[JsonSerializable(typeof(List<ContactListDto>))]
[JsonSerializable(typeof(ContactDetailDto))]
[JsonSerializable(typeof(IDictionary<string, string[]>))]
public sealed partial class AppJsonSerializationContext : JsonSerializerContext;