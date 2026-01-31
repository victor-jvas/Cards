using System;

public sealed class CheckTimingEngine
{
    public delegate GameState ApplyEventPolicy(GameState state, IGameEvent gameEvent);

    private readonly StateBasedRules _stateBasedRules;
    private readonly ApplyEventPolicy _applyEvent;

    public CheckTimingEngine(StateBasedRules stateBasedRules, ApplyEventPolicy applyEvent)
    {
        _stateBasedRules = stateBasedRules ?? throw new ArgumentNullException(nameof(stateBasedRules));
        _applyEvent = applyEvent ?? throw new ArgumentNullException(nameof(applyEvent));
    }

    /// <summary>
    /// Stabilization loop: detect → generate → apply → repeat.
    /// No player priority window is granted here (pure engine maintenance step).
    /// </summary>
    public GameState Run(GameState state, int maxIterations = 256)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (maxIterations <= 0) throw new ArgumentOutOfRangeException(nameof(maxIterations));

        var current = state;

        for (int i = 0; i < maxIterations; i++)
        {
            // 1) Detect + generate (state-based rules may enqueue events)
            var afterSba = _stateBasedRules.Apply(current);

            // 2) Apply (consume exactly one pending event per tick, then re-stabilize)
            if (afterSba.TryDequeuePendingEvent(out var nextEvent, out var stateWithoutEvent))
            {
                var afterEvent = _applyEvent(stateWithoutEvent, nextEvent);
                current = afterEvent;
                continue;
            }

            // 3) If SBA produced nothing AND there are no events, we're stable
            if (ReferenceEquals(afterSba, current))
                return current;

            current = afterSba;
        }

        throw new InvalidOperationException(
            $"CheckTimingEngine failed to stabilize after {maxIterations} iterations. Possible infinite loop.");
    }
}
