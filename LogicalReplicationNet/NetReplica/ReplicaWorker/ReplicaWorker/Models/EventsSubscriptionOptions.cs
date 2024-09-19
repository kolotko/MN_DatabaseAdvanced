namespace ReplicaWorker.Models;

public record  EventsSubscriptionOptions(string ConnectionString,
    string SlotName,
    string PublicationName);