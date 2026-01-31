using System;

public sealed class ActivateAbilityCommand : ICommand
{
    private readonly int m_playerId;
    private readonly int m_sourceCardInstanceId;
    private readonly int m_abilityIndex;

    public ActivateAbilityCommand(int playerId, int sourceCardInstanceId, int abilityIndex)
    {
        m_playerId = playerId;
        m_sourceCardInstanceId = sourceCardInstanceId;
        m_abilityIndex = abilityIndex;
    }

    public CommandValidationResult Validate(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        if (!state.Players.ContainsKey(m_playerId))
            return CommandValidationResult.Fail($"Unknown player id {m_playerId}.");

        if (!state.TryFindCard(m_playerId, m_sourceCardInstanceId, out var card, out _))
            return CommandValidationResult.Fail($"Source card instance {m_sourceCardInstanceId} was not found for player {m_playerId}.");

        var abilities = card.Definition.Abilities;
        if (m_abilityIndex < 0 || m_abilityIndex >= abilities.Count)
            return CommandValidationResult.Fail($"Ability index {m_abilityIndex} is out of range for card '{card.Name}'.");

        // Later: timing/zone checks (e.g., only if card is on Stage), costs, limits, etc.
        return CommandValidationResult.Ok();
    }

    public GameState Execute(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        return state.WithEventAdded(new AbilityActivatedEvent(
            PlayerId: m_playerId,
            SourceCardInstanceId: m_sourceCardInstanceId,
            AbilityIndex: m_abilityIndex));
    }
}
