public sealed record DamageEvent(
    int TargetPlayerId,
    int Amount,
    int? SourceCardInstanceId = null
) : IReplaceableGameEvent
{
    public EventCategory Category => EventCategory.Damage;
}
