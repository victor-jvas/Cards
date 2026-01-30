using System;
using System.Collections.Generic;
using System.Linq;

public class ZoneInstance
{
    public int OwnerPlayerId { get; }
    public ZoneType Type { get; }
    public IReadOnlyList<CardInstance> Cards { get; }

    /// <summary>
    /// Stack convention:
    /// - Last index = TOP of the zone (draw/pop from here)
    /// - Index 0 = BOTTOM of the zone
    /// </summary>
    public ZoneInstance(int ownerPlayerId, ZoneType type, IEnumerable<CardInstance> cards = null)
    {
        OwnerPlayerId = ownerPlayerId;
        Type = type;
        Cards = cards?.ToList().AsReadOnly() ?? new List<CardInstance>().AsReadOnly();
    }

    public ZoneInstance WithCardAddedToTop(CardInstance card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));
        var newCards = new List<CardInstance>(Cards) { card };
        return new ZoneInstance(OwnerPlayerId, Type, newCards);
    }

    public ZoneInstance WithCardAddedToBottom(CardInstance card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));
        var newCards = new List<CardInstance>(Cards.Count + 1) { card };
        newCards.AddRange(Cards);
        return new ZoneInstance(OwnerPlayerId, Type, newCards);
    }

    public ZoneInstance WithCardRemoved(CardInstance card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));
        var newCards = Cards.Where(c => c.Id != card.Id).ToList();
        return new ZoneInstance(OwnerPlayerId, Type, newCards);
    }

    public CardInstance PeekTop() => Cards.Count > 0 ? Cards[^1] : null;

    public ZoneInstance WithTopCardRemoved(out CardInstance removed)
    {
        removed = PeekTop();
        if (removed == null) return this;

        var newCards = Cards.Take(Cards.Count - 1).ToList();
        return new ZoneInstance(OwnerPlayerId, Type, newCards);
    }
}
