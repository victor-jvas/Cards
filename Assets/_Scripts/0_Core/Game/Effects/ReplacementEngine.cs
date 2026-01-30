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

            var applicable = _effects
                .Where(e => !appliedKeys.Contains(e.Key))
                .Where(e => e.AppliesTo(state, current))
                .ToList();

            if (applicable.Count == 0)
                break;

            var controllers = applicable
                .Select(e => e.GetOrderingControllerPlayerId(state, current))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var controllerPlayerId = controllers.Count == 1 ? controllers[0] : state.ActivePlayerId;

            ReplacementEffect chosen;
            if (choose != null)
            {
                var decision = choose(controllerPlayerId, current, applicable);
                if (decision == null || decision.Effect == null)
                    break;

                chosen = decision.Effect;
            }
            else
            {
                chosen = applicable
                    .OrderByDescending(e => e.Priority)
                    .ThenBy(e => e.Key, StringComparer.Ordinal)
                    .First();
            }

            if (chosen.IsOptional)
            {
                var shouldApply = applyOptional?.Invoke(controllerPlayerId, current, chosen) ?? true;
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
}

public sealed record ReplacementResult(
    IGameEvent OriginalEvent,
    IGameEvent FinalEvent,
    ImmutableHashSet<string> AppliedReplacementKeys);

public sealed record ReplacementDecision(ReplacementEffect Effect);
