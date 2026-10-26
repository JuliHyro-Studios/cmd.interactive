using cmd.interactive.menuscore.Registry;
using cmd.interactive.menuscore.Runtime;

namespace cmd.interactive.menuscore.Bus;

/// <summary>An isolated client namespace and its menu registries. Different bus instances never share registered runtime state.</summary>
public sealed class FrameworksBus
{
    private FrameworksBus(string clientName)
    {
        ClientName = ValidateName(clientName);
        PARENT_MENU_REGISTER = new ParentMenuRegistry();
        CHILD_MENU_REGISTER = new ChildMenuRegistry();
        INTERACTION_REGISTER = new InteractionRegistry();
        INTERACTION_TEXT_REGISTER = new InteractionTextRegistry();
        Navigation = new MenuNavigation(this);
    }

    public string ClientName { get; }
    public ParentMenuRegistry PARENT_MENU_REGISTER { get; }
    public ChildMenuRegistry CHILD_MENU_REGISTER { get; }
    public InteractionRegistry INTERACTION_REGISTER { get; }
    public InteractionTextRegistry INTERACTION_TEXT_REGISTER { get; }
    public MenuRegistry Menus { get; } = new();
    public MenuNavigation Navigation { get; }

    public static FrameworksBus RegisterBus(string clientName) => new(clientName);

    /// <summary>Registers the client identity on an existing bus variable and returns that same bus.</summary>
    public static FrameworksBus RegisterBus(FrameworksBus bus, string clientName)
    {
        ArgumentNullException.ThrowIfNull(bus);
        if (!string.Equals(bus.ClientName, ValidateName(clientName), StringComparison.Ordinal))
            throw new InvalidOperationException("A bus identity cannot be changed after creation.");
        return bus;
    }

    private static string ValidateName(string name) => string.IsNullOrWhiteSpace(name)
        ? throw new ArgumentException("Client name cannot be empty.", nameof(name))
        : name.Trim();
}
