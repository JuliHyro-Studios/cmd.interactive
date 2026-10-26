using cmd.interactive.menuscore.Configuration;
using cmd.interactive.menuscore.Interface;

namespace cmd.interactive.menuscore.Models;

internal sealed class MenuDefinition : IMenu
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? ParentId { get; init; }
    public bool SupportChildMenu { get; init; }
    public required MenuStyle Style { get; init; }
    public required MenuBuilderOptions Options { get; init; }
    public bool IsParent { get; init; }
    public string? ClientName { get; init; }
}
