using System;
using System.Threading;
using System.Threading.Tasks;
using Light.SharedCore.DataAccessAbstractions;
using WebApp.Contacts.Common;

namespace WebApp.Contacts.DeleteContact;

public interface IDeleteContactSession : IGetContactSession, IAsyncSession
{
    Task DeleteContactAsync(Guid id, CancellationToken cancellationToken = default);
}