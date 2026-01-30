using System;
using System.Collections.Generic;

public sealed class RulesEventEmitter
{
    private readonly ReplacementEngine _replacementEngine;

    public RulesEventEmitter(ReplacementEngine replacementEngine)
    {
        _replacementEngine = replacementEngine ?? throw new ArgumentNullException(nameof(replacementEngine));
    }

    /// <summary>
    /// Emits an event into the game by running the Replacement pipeline (if applicable),
    /// then adding the final event to the state's PendingEvents (if not prevented).
    /// </summary>
    public GameState EmitEvent(
        GameState state,
        IGameEvent gameEvent,
        ReplacementEngine.ChoicePolicy choose = null,
        ReplacementEngine.OptionalApplicationPolicy applyOptional = null)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));

        var result = _replacementEngine.Apply(state, gameEvent, choose, applyOptional);

        // Prevented event => no-op
        if (result.FinalEvent == null)
            return state;

        return state.WithEventAdded(result.FinalEvent);
    }
}
