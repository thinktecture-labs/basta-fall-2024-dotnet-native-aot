using System.Text.Json.Serialization;
using WebApp.Reflection;

namespace WebApp.JsonAccess;

[JsonSerializable(typeof(ReflectionDto))]
[JsonSerializable(typeof(int))]
public sealed partial class AppJsonSerializationContext : JsonSerializerContext;