using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Light.SharedCore.DataAccessAbstractions;
using WebApp.Contacts.Common;

namespace WebApp.Contacts.UpsertContact;

public interface IUpsertContactSession : IAsyncSession
{
    Task<Dictionary<Guid, Address>> GetContactAddressesAsync(
        Guid[] addressIds,
        Guid contactId,
        CancellationToken cancellationToken = default
    );

    Task UpsertContactAsync(ContactDetailDto dto, CancellationToken cancellationToken = default);
    
    Task UpsertAddressAsync(Address address, CancellationToken cancellationToken = default);
    
    Task RemoveAddressAsync(Guid addressId, CancellationToken cancellationToken = default);
}