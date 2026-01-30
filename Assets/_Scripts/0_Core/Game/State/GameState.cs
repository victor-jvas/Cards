using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

public sealed class GameState
{
    public int TurnNumber { get; }
    public Phase CurrentPhase { get; }
    public int ActivePlayerId { get; }
    
    public ImmutableDictionary<int, PlayerInstance> Players { get; }
    public ImmutableDictionary<ZoneType, ZoneInstance> Zones { get; }
    public ImmutableList<IGameEvent> PendingEvents { get; }
    
    // Cache for performance
    private readonly Lazy<GameStateSnapshot> _stateSnapshot;

    private GameState(
        int turnNumber,
        Phase currentPhase,
        int activePlayerId,
        ImmutableDictionary<int, PlayerInstance> players,
        ImmutableDictionary<ZoneType, ZoneInstance> zones,
        ImmutableList<IGameEvent> pendingEvents)
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
        if (players == null || players.Length == 0) 
            throw new ArgumentException("At least one player is required", nameof(players));
        
        var playersDict = players.ToImmutableDictionary(p => p.Id, p => p);
        
        var zones = ImmutableDictionary<ZoneType, ZoneInstance>.Empty
            .Add(ZoneType.Deck, new ZoneInstance(ZoneType.Deck))
            .Add(ZoneType.Hand, new ZoneInstance(ZoneType.Hand))
            .Add(ZoneType.Stage, new ZoneInstance(ZoneType.Stage))
            .Add(ZoneType.Archive, new ZoneInstance(ZoneType.Archive))
            .Add(ZoneType.CheerDeck, new ZoneInstance(ZoneType.CheerDeck))
            .Add(ZoneType.ResolutionZone, new ZoneInstance(ZoneType.ResolutionZone))
            .Add(ZoneType.HoloPowerArea, new ZoneInstance(ZoneType.HoloPowerArea))
            .Add(ZoneType.LifeArea, new ZoneInstance(ZoneType.LifeArea));

        return new GameState(
            turnNumber: 1,
            currentPhase: Phase.StartPhase,
            activePlayerId: startingPlayerId,
            players: playersDict,
            zones: zones,
            pendingEvents: ImmutableList<IGameEvent>.Empty);
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
        var newPlayers = Players.SetItem(playerId, player);

        if (newPlayers == Players) return this;

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
        
        var newZones = Zones.SetItem(zoneType, zone);
        
        if (newZones == Zones) return this; // Reference equality check

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
        
        var newZones = Zones.SetItem(fromZone, newFromZone).SetItem(toZone, newToZone);
    
        return new GameState(
            TurnNumber, CurrentPhase, ActivePlayerId,
            Players, newZones, PendingEvents);
    }

    public GameState WithEventAdded(IGameEvent gameEvent)
    {
        if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));
        
        var newEvents = PendingEvents.Add(gameEvent);
        
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
            pendingEvents: new List<IGameEvent>().ToImmutableList());
    }

    public GameStateSnapshot GetSnapshot()
    {
        return _stateSnapshot.Value;
    }
}
