using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebApp.CommonValidation;
using WebApp.Contacts.Common;

namespace WebApp.Contacts.GetContact;

public static class GetContactEndpoint
{
    public static WebApplication MapGetContact(this WebApplication app)
    {
        app.MapGet("/api/contacts/{id:required:guid}", GetContact);
        return app;
    }

    public static async Task<IResult> GetContact(
        IGetContactSession session,
        GuidValidator validator,
        Guid id,
        CancellationToken cancellationToken
    )
    {
        if (validator.CheckForErrors(new GuidDto(id), out var errors))
        {
            return Results.BadRequest(errors);
        }

        var contactDto = await session.GetContactDetailDtoAsync(id, cancellationToken);
        return contactDto is null ? Results.NotFound() : Results.Ok(contactDto);
    }
}