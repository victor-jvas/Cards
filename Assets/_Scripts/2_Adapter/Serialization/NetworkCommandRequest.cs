using Unity.Netcode;

public struct NetworkCommandRequest : INetworkSerializable
{
    public NetworkCommandType CommandType;

    // Common
    public int PlayerId;

    // PlayCard
    public int CardInstanceId;
    public NetworkPlayDestination PlayDestination;

    // ActivateAbility
    public int SourceCardInstanceId;
    public int AbilityIndex;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CommandType);
        serializer.SerializeValue(ref PlayerId);

        serializer.SerializeValue(ref CardInstanceId);
        serializer.SerializeValue(ref PlayDestination);

        serializer.SerializeValue(ref SourceCardInstanceId);
        serializer.SerializeValue(ref AbilityIndex);
    }
}
