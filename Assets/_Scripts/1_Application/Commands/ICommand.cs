using System;

public interface ICommand
{
    CommandValidationResult Validate(GameState state);
    GameState Execute(GameState state);
}

public sealed record CommandValidationResult(bool IsValid, string Error)
{
    public static CommandValidationResult Ok() => new(true, null);

    public static CommandValidationResult Fail(string error)
    {
        if (string.IsNullOrWhiteSpace(error)) throw new ArgumentException("Error must be provided.", nameof(error));
        return new CommandValidationResult(false, error);
    }
}
