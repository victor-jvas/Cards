using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameState
{
    // create the files for the missing types AI!
    
    public int TurnNumber { get; }
    public Phase CurrentPhase { get; }
    public int ActivePlayerId { get; }
    
    public IReadOnlyDictionary<int, PlayerInstance> Players { get; }
    public IReadOnlyDictionary<ZoneType, ZoneInstance> Zones { get; }
    public IReadOnlyList<IGameEvent> PendingEvents { get; }
    
    private readonly Lazy<GameStateSnapshot> _stateSnapshot;
}
