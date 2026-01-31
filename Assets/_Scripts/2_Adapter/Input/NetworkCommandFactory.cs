using System;

public static class NetworkCommandFactory
{
    public static ICommand ToDomainCommand(NetworkCommandRequest request)
    {
        return request.CommandType switch
        {
            NetworkCommandType.PlayCard => new PlayCardCommand(
                playerId: request.PlayerId,
                cardInstanceId: request.CardInstanceId,
                destination: request.PlayDestination == NetworkPlayDestination.CenterStage
                    ? PlayCardCommand.PlayDestination.CenterStage
                    : PlayCardCommand.PlayDestination.BackStage),

            NetworkCommandType.ActivateAbility => new ActivateAbilityCommand(
                playerId: request.PlayerId,
                sourceCardInstanceId: request.SourceCardInstanceId,
                abilityIndex: request.AbilityIndex),

            _ => throw new ArgumentOutOfRangeException(nameof(request.CommandType), request.CommandType, "Unknown command type.")
        };
    }
}
