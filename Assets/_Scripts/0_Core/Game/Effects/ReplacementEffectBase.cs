using System;

public abstract class ReplacementEffect
{
    /// <summary>
    /// Stable identity so the engine can enforce: "apply only once per event".
    /// Example: a card instance id + ability id.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Higher executes earlier by default (only used if no chooser is provided).
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// If true, the controller may choose to apply it or skip it.
    /// (Rule 10.11.4)
    /// </summary>
    public bool IsOptional { get; }

    protected ReplacementEffect(string key, int priority = 0, bool isOptional = false)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Priority = priority;
        IsOptional = isOptional;
    }

    /// <summary>
    /// Return true if this replacement effect applies to the given event in the current state.
    /// </summary>
    public abstract bool AppliesTo(GameState state, IGameEvent gameEvent);

    /// <summary>
    /// Transform the event:
    /// - return a new event => replaced
    /// - return the same event => effectively no change
    /// - return null => prevented (event does not happen)
    /// </summary>
    public abstract IGameEvent Replace(GameState state, IGameEvent gameEvent);

    /// <summary>
    /// Used for ordering choices (Rule 10.11.2): “affected object controller chooses”.
    /// Return a player id if applicable; otherwise null.
    /// </summary>
    public virtual int? GetOrderingControllerPlayerId(GameState state, IGameEvent gameEvent) => null;
}
