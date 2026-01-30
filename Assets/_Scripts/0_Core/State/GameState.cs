using System;
using System.Collections.Generic;

public sealed class GameState
{
    public int TurnNumber { get; }
    public Phase CurrentPhase { get; }
    public int ActivePlayerId { get; }
    
    public IReadOnlyDictionary<int, PlayerInstance> Players { get; }
    public IReadOnlyDictionary<ZoneType, ZoneInstance> Zones { get; }
    public IReadOnlyList<IGameEvent> PendingEvents { get; }
    
    private readonly Lazy<GameStateSnapshot> _stateSnapshot;
}
