using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Light.SharedCore.DataAccessAbstractions;
using Npgsql;

namespace WebApp.DatabaseAccess;

public abstract class NpgsqlSession(
    NpgsqlConnection connection,
    IsolationLevel transactionLevel = IsolationLevel.ReadCommitted
)
    : ReadOnlyNpgsqlSession(connection, transactionLevel), IAsyncSession
{
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is not null)
        {
            await Transaction.CommitAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("The transaction was not initialized beforehand");
        }
    }
}