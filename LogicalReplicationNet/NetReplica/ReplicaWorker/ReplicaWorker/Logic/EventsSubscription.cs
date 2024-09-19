using System.Runtime.CompilerServices;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using ReplicaWorker.Abstraction;
using ReplicaWorker.Models;

namespace ReplicaWorker.Logic;

public class EventsSubscription : IEventsSubscription
{
    public async IAsyncEnumerable<Blog> Subscribe(EventsSubscriptionOptions options, [EnumeratorCancellation] CancellationToken ct)
    {
        var (connectionString, slotName, publicationName) = options;
        await using var conn = new LogicalReplicationConnection(connectionString);
        await conn.Open(ct);

        var slot = new PgOutputReplicationSlot(slotName);

        await foreach (var message in conn.StartReplication(slot, new PgOutputReplicationOptions(publicationName, 1), ct))
        {
            if (message is InsertMessage insertMessage)
            {
                yield return await InsertMessageHandler.Handle(insertMessage, ct);
            }

            conn.SetReplicationStatus(message.WalEnd);
            await conn.SendStatusUpdate(ct);
        }
    }
}