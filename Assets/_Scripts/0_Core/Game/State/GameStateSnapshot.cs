using System;
using System.Collections.Generic;
using System.Collections.Immutable;

public class GameStateSnapshot
{
    public int TurnNumber { get; }
    public Phase CurrentPhase { get; }
    public int ActivePlayerId { get; }
    public IReadOnlyDictionary<int, PlayerInstance> Players { get; }
    public ImmutableDictionary<ZoneId, ZoneInstance> Zones { get; }
    public IReadOnlyList<IGameEvent> PendingEvents { get; }
    
    public GameStateSnapshot(
        int turnNumber,
        Phase currentPhase,
        int activePlayerId,
        IReadOnlyDictionary<int, PlayerInstance> players,
        ImmutableDictionary<ZoneId, ZoneInstance> zones,
        IReadOnlyList<IGameEvent> pendingEvents)
    {
        TurnNumber = turnNumber;
        CurrentPhase = currentPhase;
        ActivePlayerId = activePlayerId;
        Players = players ?? throw new ArgumentNullException(nameof(players));
        Zones = zones ?? throw new ArgumentNullException(nameof(zones));
        PendingEvents = pendingEvents ?? throw new ArgumentNullException(nameof(pendingEvents));
    }
}
