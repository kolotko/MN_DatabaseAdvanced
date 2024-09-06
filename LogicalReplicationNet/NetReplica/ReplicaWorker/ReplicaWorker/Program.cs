using Npgsql.Replication;
using Npgsql.Replication.PgOutput;

await using var conn = new LogicalReplicationConnection("Server=localhost;Port=9097;Database=master_db;User Id=replica_user;Password=replica_user;");
await conn.Open();

var slot = new PgOutputReplicationSlot("blog_slot");

// The following will loop until the cancellation token is triggered, and will print message types coming from PostgreSQL:
var cancellationTokenSource = new CancellationTokenSource();
await foreach (var message in conn.StartReplication(
                   slot, new PgOutputReplicationOptions("blog_pub", 1), cancellationTokenSource.Token))
{
    Console.WriteLine($"Received message type: {message.GetType().Name}");

    // Always call SetReplicationStatus() or assign LastAppliedLsn and LastFlushedLsn individually
    // so that Npgsql can inform the server which WAL files can be removed/recycled.
    conn.SetReplicationStatus(message.WalEnd);
}