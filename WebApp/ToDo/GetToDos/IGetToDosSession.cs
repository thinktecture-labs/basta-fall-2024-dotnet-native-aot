using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Light.SharedCore.DataAccessAbstractions;
using WebApp.DatabaseAccess.Model;

namespace WebApp.ToDo.GetToDos;

public interface IGetToDosSession : IAsyncReadOnlySession
{
    Task<List<ToDoItem>> GetToDoListAsync(CancellationToken cancellationToken = default);
}