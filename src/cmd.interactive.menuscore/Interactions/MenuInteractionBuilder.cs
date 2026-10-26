using cmd.interactive.menuscore.Internal;
using cmd.interactive.menuscore.Models;

namespace cmd.interactive.menuscore.Interactions;

public sealed class MenuInteractionBuilder
{
    private string? _menuId;
    private string? _interactionId;
    private Action? _action;
    private string? _clientName;

    public MenuInteractionBuilder ForClient(string clientName) { _clientName = string.IsNullOrWhiteSpace(clientName) ? throw new ArgumentException("Client name cannot be empty.", nameof(clientName)) : clientName.Trim(); return this; }
    public MenuInteractionBuilder ID(string menuId) { _menuId = menuId; return this; }
    public MenuInteractionBuilder InteractionID(string interactionId) { _interactionId = interactionId; return this; }
    public MenuInteractionBuilder Action(Action action) { _action = action ?? throw new ArgumentNullException(nameof(action)); return this; }

    public string Register()
    {
        var menuId = Required(_menuId, nameof(ID));
        var interactionId = Required(_interactionId, nameof(InteractionID));
        if (_action is null) throw new InvalidOperationException("Action must be set before Register().");
        DefinitionCatalog.AddInteraction(new MenuInteractionDefinition(interactionId, menuId, interactionId, _action, _clientName));
        return interactionId;
    }

    private static string Required(string? value, string member) => string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException($"{member} must be set before Register().") : value.Trim();
}
