using ReplicaWorker.Logic;
using ReplicaWorker.Models;

var connectrionString = "Server=localhost;Port=9097;Database=master_db;User Id=replica_user;Password=replica_user;";
var subscriptionOptions = new EventsSubscriptionOptions(connectrionString, "blog_slot", "blog_pub");
var subscription = new EventsSubscription();
var cancellationTokenSource = new CancellationTokenSource();
await foreach (var readEvent in subscription.Subscribe(subscriptionOptions, cancellationTokenSource.Token))
{
    var test = readEvent;
}