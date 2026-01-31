public sealed record AbilityActivatedEvent(
    int PlayerId,
    int SourceCardInstanceId,
    int AbilityIndex) : IGameEvent;
