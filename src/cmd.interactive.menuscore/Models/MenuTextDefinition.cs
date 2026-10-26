namespace cmd.interactive.menuscore.Models;

internal sealed record MenuTextDefinition(string Id, string MenuId, string Text, string InteractionId, string? Color, Func<bool>? ConditionVisible, string? ClientName);
