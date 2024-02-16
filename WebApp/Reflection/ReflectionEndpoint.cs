using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebApp.JsonAccess;

namespace WebApp.Reflection;

public static class ReflectionEndpoint
{
    public static WebApplication MapReflectionEndpoint(this WebApplication app)
    {
        app.MapGet("/api/reflection", GetSimpleReflectionData);
        return app;
    }

    private static async Task GetSimpleReflectionData(HttpContext httpContext)
    {
        var properties = typeof(HttpContext)
           .GetProperties()
           .Select(p => new PropertyInfoDto(p.Name, p.PropertyType.Name))
           .ToList();
        var dto = new ReflectionDto(nameof(HttpContext), properties);

        var statusCodePropertyInfo = typeof(HttpResponse).GetProperty(nameof(HttpResponse.StatusCode))!;
        statusCodePropertyInfo.SetValue(httpContext.Response, StatusCodes.Status200OK);
        await httpContext.Response.WriteAsJsonAsync(dto, AppJsonSerializationContext.Default.ReflectionDto);
    }
}