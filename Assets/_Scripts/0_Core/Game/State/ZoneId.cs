using System;

public readonly struct ZoneId : IEquatable<ZoneId>
{
    public int PlayerId { get; }
    public ZoneType ZoneType { get; }

    public ZoneId(int playerId, ZoneType zoneType)
    {
        PlayerId = playerId;
        ZoneType = zoneType;
    }

    public bool Equals(ZoneId other) => PlayerId == other.PlayerId && ZoneType == other.ZoneType;
    public override bool Equals(object obj) => obj is ZoneId other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(PlayerId, (int)ZoneType);
    public override string ToString() => $"P{PlayerId}:{ZoneType}";
}