using System;
using Unity.Netcode;

public struct NetworkZoneEntry : INetworkSerializable, IEquatable<NetworkZoneEntry>
{
    public int PlayerId;
    public ZoneType ZoneType;

    public bool Equals(NetworkZoneEntry other)
    {
        return PlayerId == other.PlayerId && ZoneType == other.ZoneType;
    }

    public override bool Equals(object obj)
    {
        return obj is NetworkZoneEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (PlayerId * 397) ^ (int)ZoneType;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref ZoneType);
    }
}