using System;

public sealed class PlayCardCommand : ICommand
{
    public enum PlayDestination
    {
        CenterStage,
        BackStage
    }

    private const int k_BackStageCapacity = 5;

    private readonly int m_playerId;
    private readonly int m_cardInstanceId;
    private readonly PlayDestination m_destination;

    public PlayCardCommand(int playerId, int cardInstanceId, PlayDestination destination)
    {
        m_playerId = playerId;
        m_cardInstanceId = cardInstanceId;
        m_destination = destination;
    }

    public CommandValidationResult Validate(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        if (!state.Players.ContainsKey(m_playerId))
            return CommandValidationResult.Fail($"Unknown player id {m_playerId}.");

        if (!state.TryFindCard(m_playerId, m_cardInstanceId, out _, out var zoneId))
            return CommandValidationResult.Fail($"Card instance {m_cardInstanceId} was not found for player {m_playerId}.");

        if (zoneId.ZoneType != ZoneType.Hand)
            return CommandValidationResult.Fail($"Card instance {m_cardInstanceId} is not in Hand (currently in {zoneId.ZoneType}).");

        var toZoneType = GetDestinationZoneType(m_destination);

        // Optional rule hook: enforce destination capacity (common for CenterStage).
        // If you don't want this yet, delete this block.
        var toKey = new ZoneId(m_playerId, toZoneType);
        if (!state.Zones.TryGetValue(toKey, out var toZone))
            return CommandValidationResult.Fail($"Missing zone for destination {toZoneType}.");

        if (toZoneType == ZoneType.CenterStage && toZone.Cards.Count > 0)
            return CommandValidationResult.Fail("CenterStage is already occupied.");

        if (toZoneType == ZoneType.BackStage && toZone.Cards.Count >= k_BackStageCapacity)
            return CommandValidationResult.Fail($"BackStage is full (max {k_BackStageCapacity}).");

        return CommandValidationResult.Ok();
    }

    public GameState Execute(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        if (!state.TryFindCard(m_playerId, m_cardInstanceId, out var card, out var fromZone))
            throw new InvalidOperationException("Execute called on an invalid PlayCardCommand (card not found).");

        var toZoneType = GetDestinationZoneType(m_destination);
        var toZoneId = new ZoneId(m_playerId, toZoneType);

        var s = state.WithCardMoved(
            playerId: m_playerId,
            fromZone: fromZone.ZoneType,
            toZone: toZoneType,
            card: card,
            placement: CardMovePlacement.Top);

        s = s.WithEventAdded(new ZoneChangeEvent(
            PlayerId: m_playerId,
            CardInstanceId: m_cardInstanceId,
            From: fromZone,
            To: toZoneId,
            Placement: CardMovePlacement.Top));

        return s;
    }

    private static ZoneType GetDestinationZoneType(PlayDestination destination)
    {
        return destination switch
        {
            PlayDestination.CenterStage => ZoneType.CenterStage,
            PlayDestination.BackStage => ZoneType.BackStage,
            _ => throw new ArgumentOutOfRangeException(nameof(destination), destination, null)
        };
    }
}
