namespace cmd.interactive.menuscore.Models;

internal sealed record MenuInteractionDefinition(string Id, string MenuId, string InteractionId, Action Action, string? ClientName);
