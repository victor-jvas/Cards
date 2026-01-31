using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public sealed class ReplacementEngine
{
    public delegate ReplacementDecision ChoicePolicy(
        int controllerPlayerId,
        IGameEvent currentEvent,
        IReadOnlyList<ReplacementEffect> applicableEffects);

    public delegate bool OptionalApplicationPolicy(
        int controllerPlayerId,
        IGameEvent currentEvent,
        ReplacementEffect optionalEffect);

    private readonly IReadOnlyList<ReplacementEffect> _effects;
    private readonly ISet<EventCategory> _enabledCategories;

    public ReplacementEngine(IEnumerable<ReplacementEffect> replacementEffects, ISet<EventCategory> enabledCategories)
    {
        _effects = (replacementEffects ?? throw new ArgumentNullException(nameof(replacementEffects)))
            .ToList()
            .AsReadOnly();

        if (enabledCategories == null) throw new ArgumentNullException(nameof(enabledCategories));
        _enabledCategories = new HashSet<EventCategory>(enabledCategories);
    }

    public ReplacementResult Apply(
        GameState state,
        IGameEvent originalEvent,
        ChoicePolicy choose = null,
        OptionalApplicationPolicy applyOptional = null)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (originalEvent == null) throw new ArgumentNullException(nameof(originalEvent));

        if (originalEvent is not IReplaceableGameEvent replaceable)
            return new ReplacementResult(originalEvent, originalEvent, ImmutableHashSet<string>.Empty);

        if (!_enabledCategories.Contains(replaceable.Category))
            return new ReplacementResult(originalEvent, originalEvent, ImmutableHashSet<string>.Empty);

        var current = originalEvent;
        var appliedKeys = ImmutableHashSet<string>.Empty;

        while (current != null)
        {
            if (current is not IReplaceableGameEvent currentReplaceable)
                break;

            if (!_enabledCategories.Contains(currentReplaceable.Category))
                break;

            var applicableAll = _effects
                .Where(e => !appliedKeys.Contains(e.Key))            // 10.11.3: only once per event
                .Where(e => e.AppliesTo(state, current))
                .ToList();

            if (applicableAll.Count == 0)
                break;

            // Resolve controller ordering deterministically:
            // - effect may specify a controller (10.11.2)
            // - otherwise treat as active player for ordering purposes
            var controllerOrder = GetControllerOrder(state);

            ReplacementEffect chosen = null;
            int chosenControllerId = state.ActivePlayerId;

            foreach (var controllerId in controllerOrder)
            {
                var applicableForController = applicableAll
                    .Where(e => (e.GetOrderingControllerPlayerId(state, current) ?? state.ActivePlayerId) == controllerId)
                    .ToList();

                if (applicableForController.Count == 0)
                    continue;

                chosenControllerId = controllerId;

                if (choose != null)
                {
                    var decision = choose(controllerId, current, applicableForController);
                    if (decision == null || decision.Effect == null)
                        return new ReplacementResult(originalEvent, current, appliedKeys);

                    chosen = decision.Effect;
                }
                else
                {
                    chosen = applicableForController
                        .OrderByDescending(e => e.Priority)
                        .ThenBy(e => e.Key, StringComparer.Ordinal)
                        .First();
                }

                break;
            }

            if (chosen == null)
                break;

            if (chosen.IsOptional)
            {
                var shouldApply = applyOptional?.Invoke(chosenControllerId, current, chosen) ?? true;
                if (!shouldApply)
                {
                    appliedKeys = appliedKeys.Add(chosen.Key);
                    continue;
                }
            }

            appliedKeys = appliedKeys.Add(chosen.Key);
            current = chosen.Replace(state, current);
        }

        return new ReplacementResult(originalEvent, current, appliedKeys);
    }

    private static IReadOnlyList<int> GetControllerOrder(GameState state)
    {
        // APNAP-like: active player first, then the rest in stable order.
        // (Deterministic and UI-friendly; avoids "player priority" in check timing.)
        var ids = state.Players.Keys.OrderBy(id => id).ToList();

        if (!ids.Contains(state.ActivePlayerId))
            return ids;

        var ordered = new List<int>(ids.Count) { state.ActivePlayerId };
        ordered.AddRange(ids.Where(id => id != state.ActivePlayerId));
        return ordered;
    }
}

public sealed record ReplacementResult(
    IGameEvent OriginalEvent,
    IGameEvent FinalEvent,
    ImmutableHashSet<string> AppliedReplacementKeys);

public sealed record ReplacementDecision(ReplacementEffect Effect);
