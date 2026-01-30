using System;

public class CardInstance
{
    public int Id { get; }
    public int OwnerPlayerId { get; }
    public CardDefinition Definition { get; }

    public string Name => Definition != null ? Definition.Name : "<Missing CardDefinition>";

    public CardInstance(int id, int ownerPlayerId, CardDefinition definition)
    {
        Id = id;
        OwnerPlayerId = ownerPlayerId;
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
    }
}
