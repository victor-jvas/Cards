

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

public sealed partial class GameState
{
    public GameState WithCardMoved(int playerId, ZoneType fromZone, ZoneType toZone, CardInstance card, CardMovePlacement placement)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));

        var fromKey = new ZoneId(playerId, fromZone);
        var toKey = new ZoneId(playerId, toZone);

        var fromZoneInstance = Zones[fromKey];
        var newFromZone = fromZoneInstance.WithCardRemoved(card);

        var toZoneInstance = Zones[toKey];

        ZoneInstance newToZone = placement switch
        {
            CardMovePlacement.Top => toZoneInstance.WithCardAddedToTop(card),
            CardMovePlacement.Bottom => toZoneInstance.WithCardAddedToBottom(card),
            CardMovePlacement.ShuffleInto => ShuffleInto(toZoneInstance, card),
            _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, null)
        };

        var newZones = Zones.SetItem(fromKey, newFromZone).SetItem(toKey, newToZone);

        return new GameState(
            TurnNumber, CurrentPhase, ActivePlayerId,
            Players, newZones, PendingEvents);
    }

    public GameState DrawCard(int playerId)
    {
        var deckKey = new ZoneId(playerId, ZoneType.Deck);
        var handKey = new ZoneId(playerId, ZoneType.Hand);

        if (!Zones.TryGetValue(deckKey, out var deck))
            throw new InvalidOperationException($"Missing zone: {deckKey}");

        if (!Zones.TryGetValue(handKey, out var hand))
            throw new InvalidOperationException($"Missing zone: {handKey}");

        var newDeck = deck.WithTopCardRemoved(out var drawn);
        if (drawn == null)
        {
            // Immediate loss by rule: drawing from empty deck.
            return WithEventAdded(new GameLostEvent(
                PlayerId: playerId,
                Reason: "Tried to draw from an empty deck."));
        }

        var newHand = hand.WithCardAddedToTop(drawn);
        var newZones = Zones.SetItem(deckKey, newDeck).SetItem(handKey, newHand);

        var s = new GameState(
            TurnNumber, CurrentPhase, ActivePlayerId,
            Players, newZones, PendingEvents);

        s = s.WithEventAdded(new DrawEvent(playerId, drawn.Id));
        s = s.WithEventAdded(new ZoneChangeEvent(
            PlayerId: playerId,
            CardInstanceId: drawn.Id,
            From: deckKey,
            To: handKey,
            Placement: CardMovePlacement.Top));

        return s;
    }

    public GameState SendToArchive(CardInstance card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));

        var playerId = card.OwnerPlayerId;
        var archiveKey = new ZoneId(playerId, ZoneType.Archive);

        if (!Zones.TryGetValue(archiveKey, out var archive))
            throw new InvalidOperationException($"Missing zone: {archiveKey}");

        if (!TryFindCardZone(playerId, card.Id, out var fromZoneKey))
            throw new InvalidOperationException($"Card instance {card.Id} not found in any zone for player {playerId}.");

        if (fromZoneKey.Equals(archiveKey))
            return this; // already archived

        var fromZone = Zones[fromZoneKey];
        var newFrom = fromZone.WithCardRemoved(card);
        var newArchive = archive.WithCardAddedToTop(card);

        var newZones = Zones
            .SetItem(fromZoneKey, newFrom)
            .SetItem(archiveKey, newArchive);

        var s = new GameState(
            TurnNumber, CurrentPhase, ActivePlayerId,
            Players, newZones, PendingEvents);

        s = s.WithEventAdded(new ZoneChangeEvent(
            PlayerId: playerId,
            CardInstanceId: card.Id,
            From: fromZoneKey,
            To: archiveKey,
            Placement: CardMovePlacement.Top));

        return s;
    }

    private bool TryFindCardZone(int playerId, int cardInstanceId, out ZoneId zoneId)
    {
        // Search only this player’s zones; order doesn’t matter.
        foreach (var kvp in Zones)
        {
            if (kvp.Key.PlayerId != playerId) continue;

            var zone = kvp.Value;
            if (zone.Cards.Any(c => c.Id == cardInstanceId))
            {
                zoneId = kvp.Key;
                return true;
            }
        }

        zoneId = default;
        return false;
    }

    private static ZoneInstance ShuffleInto(ZoneInstance zone, CardInstance card)
    {
        // Determinism note: for real gameplay you’ll want RNG seeded from match state,
        // but for now we can do a simple randomized insert with System.Random passed later.
        var cards = zone.Cards.ToList();
        cards.Add(card);

        // Placeholder shuffle strategy: insert at random position.
        // Replace with injected RNG when you build the rules engine.
        var rng = new Random();
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }

        return new ZoneInstance(zone.OwnerPlayerId, zone.Type, cards);
    }
}