using System;
using System.Collections.Generic;
using System.Linq;

public sealed class GameState
{
    public int TurnNumber { get; }
    public Phase CurrentPhase { get; }
    public int ActivePlayerId { get; }
    
    public IReadOnlyDictionary<int, PlayerInstance> Players { get; }
    public IReadOnlyDictionary<ZoneType, ZoneInstance> Zones { get; }
    public IReadOnlyList<IGameEvent> PendingEvents { get; }
    
    // Cache for performance
    private readonly Lazy<GameStateSnapshot> _stateSnapshot;

    private GameState(
        int turnNumber,
        Phase currentPhase,
        int activePlayerId,
        IReadOnlyDictionary<int, PlayerInstance> players,
        IReadOnlyDictionary<ZoneType, ZoneInstance> zones,
        IReadOnlyList<IGameEvent> pendingEvents)
    {
        TurnNumber = turnNumber;
        CurrentPhase = currentPhase;
        ActivePlayerId = activePlayerId;
        Players = players ?? throw new ArgumentNullException(nameof(players));
        Zones = zones ?? throw new ArgumentNullException(nameof(zones));
        PendingEvents = pendingEvents ?? throw new ArgumentNullException(nameof(pendingEvents));
        
        _stateSnapshot = new Lazy<GameStateSnapshot>(() =>
            new GameStateSnapshot(
                turnNumber,
                currentPhase,
                activePlayerId,
                players,
                zones,
                pendingEvents));
    }

    public static GameState CreateInitialState(int startingPlayerId, params PlayerInstance[] players)
    {
        if (players == null || players.Length == 0) throw new ArgumentException("At least one player is required", nameof(players));
        
        var playersDict = players.ToDictionary(p => p.Id, p => p);
        
        var zones = new Dictionary<ZoneType, ZoneInstance>()
        {
            {ZoneType.Deck, new ZoneInstance(ZoneType.Deck)},
            {ZoneType.Hand, new ZoneInstance(ZoneType.Hand)},
            {ZoneType.Stage, new ZoneInstance(ZoneType.Stage)},
            {ZoneType.Archive, new ZoneInstance(ZoneType.Archive)},
            {ZoneType.CheerDeck, new ZoneInstance(ZoneType.CheerDeck)},
            {ZoneType.ResolutionZone, new ZoneInstance(ZoneType.ResolutionZone)},
            {ZoneType.HoloPowerArea, new ZoneInstance(ZoneType.HoloPowerArea)},
            {ZoneType.LifeArea, new ZoneInstance(ZoneType.LifeArea)}
        };

        return new GameState(
            turnNumber: 1,
            currentPhase: Phase.StartPhase,
            activePlayerId: startingPlayerId,
            players: playersDict,
            zones: zones,
            pendingEvents: new List<IGameEvent>());
    }

    public GameState WithTurnNumber(int newTurnNumber)
    {
        if (newTurnNumber == TurnNumber) return this;

        return new GameState(
            turnNumber: newTurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: ActivePlayerId,
            players: Players,
            zones: Zones,
            pendingEvents: PendingEvents);
    }

    public GameState WithPlayer(int playerId, PlayerInstance player)
    {
        var newPlayers = new Dictionary<int, PlayerInstance>(Players)
        {
            [playerId] = player
        };

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: playerId,
            players: newPlayers,
            zones: Zones,
            pendingEvents: PendingEvents);
    }

public GameState WithCurrentPhase(Phase newCurrentPhase)
    {
        if (newCurrentPhase == CurrentPhase) return this;

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: newCurrentPhase,
            activePlayerId: ActivePlayerId,
            players: Players,
            zones: Zones,
            pendingEvents: PendingEvents);
    }

    public GameState WithActivePlayer(int newActivePlayerId)
    {
        if (newActivePlayerId == ActivePlayerId) return this;

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: newActivePlayerId,
            players: Players,
            zones: Zones,
            pendingEvents: PendingEvents);
    }

    public GameState WithZone(ZoneType zoneType, ZoneInstance zone)
    {
        if (zone == null) throw new ArgumentNullException(nameof(zone));
        
        var newZones = new Dictionary<ZoneType, ZoneInstance>(Zones)
        {
            [zoneType] = zone
        };

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: ActivePlayerId,
            players: Players,
            zones: newZones,
            pendingEvents: PendingEvents);
    }
    
    public GameState WithCardMoved(ZoneType fromZone, ZoneType toZone, CardInstance card)
    {
        // Create new fromZone without the card
        var fromZoneInstance = Zones[fromZone];
        var newFromZone = fromZoneInstance.WithCardRemoved(card);
    
        // Create new toZone with the card
        var toZoneInstance = Zones[toZone];
        var newToZone = toZoneInstance.WithCardAdded(card);
    
        // Create new zones dictionary
        var newZones = new Dictionary<ZoneType, ZoneInstance>(Zones)
        {
            [fromZone] = newFromZone,
            [toZone] = newToZone
        };
    
        return new GameState(
            TurnNumber, CurrentPhase, ActivePlayerId,
            Players, newZones, PendingEvents);
    }

    public GameState WithEventAdded(IGameEvent gameEvent)
    {
        if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));
        
        var newEvents = new List<IGameEvent>(PendingEvents);
        newEvents.Add(gameEvent);

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: ActivePlayerId,
            players: Players,
            zones: Zones,
            pendingEvents: newEvents);
    }

    public GameState WithEventsCleared()
    {
        if (PendingEvents.Count == 0) return this;

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: ActivePlayerId,
            players: Players,
            zones: Zones,
            pendingEvents: new List<IGameEvent>());
    }

    public GameStateSnapshot GetSnapshot()
    {
        return _stateSnapshot.Value;
    }
}
