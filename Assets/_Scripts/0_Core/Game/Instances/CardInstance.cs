using System;

public class CardInstance
{
    public int Id { get; }
    public string Name { get; }
    public CardDefinition Definition { get; }
    
    public CardInstance(int id, string name, CardDefinition definition)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
    }
}
