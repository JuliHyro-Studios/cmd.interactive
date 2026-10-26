using cmd.interactive.menuscore.Bus;

namespace cmd.interactive.menuscore.Runtime;

/// <summary>Entry point for displaying a registered menu.</summary>
public static class ParentMenu
{
    public static MenuDisplaySession ShowMenu(FrameworksBus bus, string menuId) => new(bus, menuId);
}
