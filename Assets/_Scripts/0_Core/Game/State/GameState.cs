using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

public sealed partial class GameState
{
    public int TurnNumber { get; }
    public Phase CurrentPhase { get; }
    public int ActivePlayerId { get; }

    public ImmutableDictionary<int, PlayerInstance> Players { get; }
    public ImmutableDictionary<ZoneId, ZoneInstance> Zones { get; }
    public ImmutableList<IGameEvent> PendingEvents { get; }

    // Cache for performance
    private readonly Lazy<GameStateSnapshot> _stateSnapshot;

    private GameState(
        int turnNumber,
        Phase currentPhase,
        int activePlayerId,
        ImmutableDictionary<int, PlayerInstance> players,
        ImmutableDictionary<ZoneId, ZoneInstance> zones,
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

        var zonesBuilder = ImmutableDictionary.CreateBuilder<ZoneId, ZoneInstance>();

        foreach (var player in players)
        {
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.Deck), new ZoneInstance(player.Id, ZoneType.Deck));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.Hand), new ZoneInstance(player.Id, ZoneType.Hand));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.Stage), new ZoneInstance(player.Id, ZoneType.Stage));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.Archive), new ZoneInstance(player.Id, ZoneType.Archive));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.CheerDeck), new ZoneInstance(player.Id, ZoneType.CheerDeck));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.ResolutionZone), new ZoneInstance(player.Id, ZoneType.ResolutionZone));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.HoloPowerArea), new ZoneInstance(player.Id, ZoneType.HoloPowerArea));
            zonesBuilder.Add(new ZoneId(player.Id, ZoneType.LifeArea), new ZoneInstance(player.Id, ZoneType.LifeArea));
        }

        return new GameState(
            turnNumber: 1,
            currentPhase: Phase.StartPhase,
            activePlayerId: startingPlayerId,
            players: playersDict,
            zones: zonesBuilder.ToImmutable(),
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
            activePlayerId: ActivePlayerId,
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

    public GameState WithZone(int playerId, ZoneType zoneType, ZoneInstance zone)
    {
        if (zone == null) throw new ArgumentNullException(nameof(zone));

        var key = new ZoneId(playerId, zoneType);
        var newZones = Zones.SetItem(key, zone);
        if (newZones == Zones) return this;

        return new GameState(
            turnNumber: TurnNumber,
            currentPhase: CurrentPhase,
            activePlayerId: ActivePlayerId,
            players: Players,
            zones: newZones,
            pendingEvents: PendingEvents);
    }

    /*public GameState WithCardMoved(int playerId, ZoneType fromZone, ZoneType toZone, CardInstance card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));

        var fromKey = new ZoneId(playerId, fromZone);
        var toKey = new ZoneId(playerId, toZone);

        var fromZoneInstance = Zones[fromKey];
        var newFromZone = fromZoneInstance.WithCardRemoved(card);

        var toZoneInstance = Zones[toKey];
        var newToZone = toZoneInstance.WithCardAddedToTop(card);

        var newZones = Zones.SetItem(fromKey, newFromZone).SetItem(toKey, newToZone);

        return new GameState(
            TurnNumber, CurrentPhase, ActivePlayerId,
            Players, newZones, PendingEvents);
    }*/

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