using System;
using System.Linq;

public sealed class StateBasedRules
{
    public GameState Apply(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        var current = state;

        foreach (var playerId in state.Players.Keys)
        {
            current = ApplyLifeEmptyLoss(current, playerId);
        }

        return current;
    }

    private static GameState ApplyLifeEmptyLoss(GameState state, int playerId)
    {
        // If already losing (or lost) is pending, don't add duplicates.
        if (state.PendingEvents.OfType<GameLostEvent>().Any(e => e.PlayerId == playerId))
            return state;

        var lifeKey = new ZoneId(playerId, ZoneType.LifeArea);

        if (!state.Zones.TryGetValue(lifeKey, out var lifeZone))
            return state; // zone missing => ignore (or throw, but SBA should be resilient)

        if (lifeZone.Cards == null || !lifeZone.Cards.Any())
        {
            return state.WithEventAdded(new GameLostEvent(
                PlayerId: playerId,
                Reason: "Life is 0 (Life Area is empty)."));
        }

        return state;
    }
}
