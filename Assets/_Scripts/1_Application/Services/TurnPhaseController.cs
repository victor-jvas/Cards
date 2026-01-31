using System;
using System.Collections.Generic;
using System.Linq;

public sealed class TurnPhaseController
{
    private readonly CheckTimingEngine m_checkTimingEngine;

    public TurnPhaseController(CheckTimingEngine checkTimingEngine)
    {
        m_checkTimingEngine = checkTimingEngine ?? throw new ArgumentNullException(nameof(checkTimingEngine));
    }

    /// <summary>
    /// Enters the current phase: performs any mandatory phase start actions,
    /// then runs check timing to stabilize. No player priority here.
    /// </summary>
    public GameState EnterCurrentPhase(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        var s = state;

        switch (s.CurrentPhase)
        {
            case Phase.StartPhase: // Reset
                // TODO: reset-step actions (ready/exhaust refresh, once-per-turn flags, etc.)
                break;

            case Phase.DrawPhase:
                s = s.DrawCard(s.ActivePlayerId);
                break;

            case Phase.CheerPhase:
                // TODO: cheer-step actions (if any are automatic)
                break;

            case Phase.MainPhase:
                // Player action window (commands) happens outside; nothing automatic here.
                break;

            case Phase.PerformancePhase:
                // Player action window (commands) happens outside; nothing automatic here.
                break;

            case Phase.EndPhase:
                // TODO: cleanup/discard-to-hand-size/etc. if your rules require it
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return m_checkTimingEngine.Run(s);
    }

    /// <summary>
    /// Advances to the next phase (or next turn) and enters it (including mandatory actions + stabilization).
    /// </summary>
    public GameState Advance(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        var next = GetNextPhase(state.CurrentPhase);

        var s = state.WithCurrentPhase(next);

        if (next == Phase.StartPhase)
        {
            // New turn begins when we wrap from End → Start.
            s = s.WithTurnNumber(s.TurnNumber + 1);

            // Simple active player rotation (deterministic):
            // active becomes the next player id in ascending order, wrapping.
            s = s.WithActivePlayer(GetNextPlayerId(s));
        }

        return EnterCurrentPhase(s);
    }

    public bool IsPlayerActionWindow(Phase phase)
    {
        // Adjust as your timing rules mature.
        return phase == Phase.MainPhase || phase == Phase.PerformancePhase || phase == Phase.CheerPhase;
    }

    private static Phase GetNextPhase(Phase phase)
    {
        return phase switch
        {
            Phase.StartPhase => Phase.DrawPhase,
            Phase.DrawPhase => Phase.CheerPhase,
            Phase.CheerPhase => Phase.MainPhase,
            Phase.MainPhase => Phase.PerformancePhase,
            Phase.PerformancePhase => Phase.EndPhase,
            Phase.EndPhase => Phase.StartPhase,
            _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
        };
    }

    private static int GetNextPlayerId(GameState state)
    {
        var ids = state.Players.Keys;
        var enumerable = ids.ToList();
        if (enumerable.Count == 0) throw new InvalidOperationException("No players in state.");

        var ordered = new List<int>(enumerable);
        ordered.Sort();

        var currentIndex = ordered.IndexOf(state.ActivePlayerId);
        if (currentIndex < 0)
            return ordered[0];

        var nextIndex = (currentIndex + 1) % ordered.Count;
        return ordered[nextIndex];
    }
}
