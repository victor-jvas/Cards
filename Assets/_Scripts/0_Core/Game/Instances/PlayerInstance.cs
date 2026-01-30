using System;

public class PlayerInstance
{
    public int Id { get; }
    public string Name { get; }
    public int LifeTotal { get; private set; }
    
    public PlayerInstance(int id, string name, int startingLifeTotal)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LifeTotal = startingLifeTotal;
    }
    
    public void ModifyLife(int amount)
    {
        LifeTotal += amount;
    }
}
