public record GameLostEvent(
    int PlayerId,
    string Reason) : IGameEvent;
