using System;

public class CardDefinition
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    
    public CardDefinition(string id, string name, string description)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }
}
