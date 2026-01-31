using UnityEngine;

public sealed class NetworkCommandClient : MonoBehaviour
{
    [SerializeField] private NetworkGameServer m_networkGameServer;

    public void PlayCard(int playerId, int cardInstanceId, NetworkPlayDestination destination)
    {
        var request = new NetworkCommandRequest
        {
            CommandType = NetworkCommandType.PlayCard,
            PlayerId = playerId,
            CardInstanceId = cardInstanceId,
            PlayDestination = destination
        };

        m_networkGameServer.SubmitCommandServerRpc(request);
    }

    public void ActivateAbility(int playerId, int sourceCardInstanceId, int abilityIndex)
    {
        var request = new NetworkCommandRequest
        {
            CommandType = NetworkCommandType.ActivateAbility,
            PlayerId = playerId,
            SourceCardInstanceId = sourceCardInstanceId,
            AbilityIndex = abilityIndex
        };

        m_networkGameServer.SubmitCommandServerRpc(request);
    }
}
