using System;

public sealed class CommandBus
{
    private readonly CheckTimingEngine m_checkTimingEngine;

    public CommandBus(CheckTimingEngine checkTimingEngine)
    {
        m_checkTimingEngine = checkTimingEngine ?? throw new ArgumentNullException(nameof(checkTimingEngine));
    }

    /// <summary>
    /// Server-side pipeline: Validate → Execute → CheckTiming(Stabilize).
    /// No client authority; no side effects beyond returning a new immutable GameState.
    /// </summary>
    public CommandResult Process(GameState state, ICommand command)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (command == null) throw new ArgumentNullException(nameof(command));

        var validation = command.Validate(state);
        if (!validation.IsValid)
            return CommandResult.Rejected(validation.Error);

        var afterExecute = command.Execute(state);
        var stabilized = m_checkTimingEngine.Run(afterExecute);

        return CommandResult.Accepted(stabilized);
    }
}

public sealed record CommandResult(bool WasAccepted, string Error, GameState State)
{
    public static CommandResult Accepted(GameState state) =>
        new(true, null, state ?? throw new ArgumentNullException(nameof(state)));

    public static CommandResult Rejected(string error) =>
        new(false, error, null);
}
