using System;
using System.Collections.Generic;
using System.Linq;

public class ZoneInstance
{
    public ZoneType Type { get; }
    public IReadOnlyList<CardInstance> Cards { get; }
    
    
    public ZoneInstance(ZoneType type, IEnumerable<CardInstance> cards = null)
    {
        Type = type;
        Cards = cards?.ToList().AsReadOnly() ?? new List<CardInstance>().AsReadOnly();
    }
    
    public ZoneInstance WithCardAdded(CardInstance card)
    {
        var newCards = new List<CardInstance>(Cards) { card };
        return new ZoneInstance(Type, newCards);
    }
    
    public ZoneInstance WithCardRemoved(CardInstance card)
    {
        var newCards = Cards.Where(c => c.Id != card.Id).ToList();
        return new ZoneInstance(Type, newCards);
    }
}
