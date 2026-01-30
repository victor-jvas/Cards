public sealed record DrawEvent(
    int PlayerId,
    int CardInstanceId) : IGameEvent;
