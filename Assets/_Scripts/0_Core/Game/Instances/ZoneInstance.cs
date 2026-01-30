using System;
using System.Collections.Generic;

public class ZoneInstance
{
    public ZoneType Type { get; }
    public IReadOnlyList<CardInstance> Cards => _cards.AsReadOnly();
    
    private readonly List<CardInstance> _cards;
    
    public ZoneInstance(ZoneType type)
    {
        Type = type;
        _cards = new List<CardInstance>();
    }
    
    public void AddCard(CardInstance card)
    {
        if (card == null) throw new ArgumentNullException(nameof(card));
        _cards.Add(card);
    }
    
    public bool RemoveCard(CardInstance card)
    {
        return _cards.Remove(card);
    }
}
