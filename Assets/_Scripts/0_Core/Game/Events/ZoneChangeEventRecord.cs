public sealed record ZoneChangeEvent(
    int PlayerId,
    int CardInstanceId,
    ZoneId From,
    ZoneId To,
    CardMovePlacement Placement
) : IReplaceableGameEvent
{
    public EventCategory Category => EventCategory.ZoneChange;
}
