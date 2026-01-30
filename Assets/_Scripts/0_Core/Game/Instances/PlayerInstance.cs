using System;

public class PlayerInstance
{
    public int Id { get; }
    public string Name { get; }
    public int LifeTotal { get; }

    public PlayerInstance(int id, string name, int startingLifeTotal)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LifeTotal = startingLifeTotal;
    }

    public PlayerInstance WithLifeTotal(int newLifeTotal)
    {
        if (newLifeTotal == LifeTotal) return this;
        return new PlayerInstance(Id, Name, newLifeTotal);
    }

    public PlayerInstance WithLifeModified(int amount)
    {
        if (amount == 0) return this;
        return new PlayerInstance(Id, Name, LifeTotal + amount);
    }
}
