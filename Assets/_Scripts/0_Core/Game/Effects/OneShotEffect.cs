using System;

public sealed class OneShotEffect : IEffect
{
    private readonly Func<GameState, GameState> _resolve;

    public string DebugName { get; }

    public OneShotEffect(string debugName, Func<GameState, GameState> resolve)
    {
        DebugName = debugName ?? throw new ArgumentNullException(nameof(debugName));
        _resolve = resolve ?? throw new ArgumentNullException(nameof(resolve));
    }

    public GameState Resolve(GameState state) => _resolve(state);
}
