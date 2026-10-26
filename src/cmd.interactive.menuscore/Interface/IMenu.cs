using cmd.interactive.menuscore.Configuration;

namespace cmd.interactive.menuscore.Interface;

/// <summary>Describes a menu that can participate in the Menus Core runtime.</summary>
public interface IMenu
{
    string Id { get; }
    string Title { get; }
    string? ParentId { get; }
    bool SupportChildMenu { get; }
    MenuStyle Style { get; }
}
