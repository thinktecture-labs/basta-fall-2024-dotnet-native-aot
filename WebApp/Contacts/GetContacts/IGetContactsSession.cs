using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Light.SharedCore.DataAccessAbstractions;

namespace WebApp.Contacts.GetContacts;

public interface IGetContactsSession : IAsyncReadOnlySession
{
    Task<List<ContactListDto>> GetContactsAsync(int skip, int take, CancellationToken cancellationToken = default);
}