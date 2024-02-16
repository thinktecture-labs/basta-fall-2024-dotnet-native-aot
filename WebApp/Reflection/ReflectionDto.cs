using System.Collections.Generic;

namespace WebApp.Reflection;

public readonly record struct ReflectionDto(string TypeName, List<PropertyInfoDto> Properties);