using System;
using Unity.Netcode;

public struct NetworkCardInZoneEntry : INetworkSerializable, IEquatable<NetworkCardInZoneEntry>
{
    public int PlayerId;
    public ZoneType ZoneType;
    public int CardInstanceId;

    public bool Equals(NetworkCardInZoneEntry other)
    {
        return PlayerId == other.PlayerId
               && ZoneType == other.ZoneType
               && CardInstanceId == other.CardInstanceId;
    }

    public override bool Equals(object obj)
    {
        return obj is NetworkCardInZoneEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = PlayerId;
            hash = (hash * 397) ^ (int)ZoneType;
            hash = (hash * 397) ^ CardInstanceId;
            return hash;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref ZoneType);
        serializer.SerializeValue(ref CardInstanceId);
    }
}
