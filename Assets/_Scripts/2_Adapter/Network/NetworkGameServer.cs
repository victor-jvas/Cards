using Unity.Netcode;
using UnityEngine;

public sealed class NetworkGameServer : NetworkBehaviour
{
    // Replicated "header"
    public NetworkVariable<int> TurnNumber { get; private set; }
    public NetworkVariable<Phase> CurrentPhase { get; private set; }
    public NetworkVariable<int> ActivePlayerId { get; private set; }

    // Replicated "body"
    public NetworkList<int> PlayerIds { get; private set; }
    public NetworkList<NetworkZoneEntry> Zones { get; private set; }
    public NetworkList<NetworkCardInZoneEntry> CardsInZones { get; private set; }

    // Server-only authoritative state
    private GameState m_state;
    private CommandBus m_commandBus;

    private void Awake()
    {
        TurnNumber = new NetworkVariable<int>();
        CurrentPhase = new NetworkVariable<Phase>();
        ActivePlayerId = new NetworkVariable<int>();

        PlayerIds = new NetworkList<int>();
        Zones = new NetworkList<NetworkZoneEntry>();
        CardsInZones = new NetworkList<NetworkCardInZoneEntry>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        // TODO: initialize m_commandBus + m_state
        // m_state = ...
        // m_commandBus = ...

        PublishState(m_state);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitCommandServerRpc(NetworkCommandRequest request, ServerRpcParams rpcParams = default)
    {
        if (!IsServer)
            return;

        if (m_commandBus == null || m_state == null)
        {
            Debug.LogWarning("Server not initialized yet; command ignored.");
            return;
        }

        var command = NetworkCommandFactory.ToDomainCommand(request);
        var result = m_commandBus.Process(m_state, command);

        if (!result.WasAccepted)
        {
            RejectCommandClientRpc(result.Error ?? "Command rejected.", new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
                }
            });
            return;
        }

        m_state = result.State;
        PublishState(m_state);
    }

    private void PublishState(GameState state)
    {
        if (!IsServer) return;
        if (state == null) return;

        TurnNumber.Value = state.TurnNumber;
        CurrentPhase.Value = state.CurrentPhase;
        ActivePlayerId.Value = state.ActivePlayerId;

        PlayerIds.Clear();
        foreach (var playerId in state.Players.Keys)
        {
            PlayerIds.Add(playerId);
        }

        Zones.Clear();
        CardsInZones.Clear();

        foreach (var kvp in state.Zones)
        {
            var zoneId = kvp.Key;
            var zone = kvp.Value;

            Zones.Add(new NetworkZoneEntry
            {
                PlayerId = zoneId.PlayerId,
                ZoneType = zoneId.ZoneType
            });

            for (int i = 0; i < zone.Cards.Count; i++)
            {
                CardsInZones.Add(new NetworkCardInZoneEntry
                {
                    PlayerId = zoneId.PlayerId,
                    ZoneType = zoneId.ZoneType,
                    CardInstanceId = zone.Cards[i].Id
                });
            }
        }
    }

    [ClientRpc]
    private void RejectCommandClientRpc(string reason, ClientRpcParams clientRpcParams = default)
    {
        Debug.LogWarning($"Command rejected by server: {reason}");
    }
}
