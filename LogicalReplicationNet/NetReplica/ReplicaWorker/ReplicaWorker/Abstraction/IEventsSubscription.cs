using ReplicaWorker.Models;

namespace ReplicaWorker.Abstraction;

public interface IEventsSubscription
{
    IAsyncEnumerable<Blog> Subscribe(EventsSubscriptionOptions options, CancellationToken ct);
}