using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApp.Reflection;

public static class UnboundReflectionEndpoint
{
    public static WebApplication MapUnboundReflectionEndpoint(this WebApplication app)
    {
        app.MapGet("/api/reflection/unbound", GetUnboundReflectionData);
        return app;
    }

    private static IResult GetUnboundReflectionData()
    {
        var type = Type.GetType("WebApp.Reflection.Calculator");
        if (type is null)
        {
            return Results.NotFound();
        }

        var addMethod = type.GetMethod("Add", BindingFlags.Public | BindingFlags.Static);
        if (addMethod is null)
        {
            return Results.NotFound();
        }

        var result = addMethod.Invoke(null, [40, 2]);
        return Results.Ok(result);
    }
}