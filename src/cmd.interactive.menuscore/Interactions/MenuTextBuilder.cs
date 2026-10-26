using cmd.interactive.menuscore.Internal;
using cmd.interactive.menuscore.Models;

namespace cmd.interactive.menuscore.Interactions;

public sealed class MenuTextBuilder
{
    private string? _menuId;
    private string? _text;
    private string? _interactionId;
    private string? _color;
    private Func<bool>? _condition;
    private string? _clientName;

    public MenuTextBuilder ForClient(string clientName) { _clientName = string.IsNullOrWhiteSpace(clientName) ? throw new ArgumentException("Client name cannot be empty.", nameof(clientName)) : clientName.Trim(); return this; }
    public MenuTextBuilder ID(string menuId) { _menuId = menuId; return this; }
    public MenuTextBuilder Text(string text) { _text = text; return this; }
    public MenuTextBuilder InteractionID(string interactionId) { _interactionId = interactionId; return this; }
    public MenuTextBuilder Color(string color) { _color = color; return this; }
    public MenuTextBuilder ConditionVisible(Func<bool> condition) { _condition = condition ?? throw new ArgumentNullException(nameof(condition)); return this; }

    public string Register()
    {
        var menuId = Required(_menuId, nameof(ID));
        var text = Required(_text, nameof(Text));
        var interactionId = Required(_interactionId, nameof(InteractionID));
        DefinitionCatalog.AddText(new MenuTextDefinition(interactionId, menuId, text, interactionId, _color, _condition, _clientName));
        return interactionId;
    }

    private static string Required(string? value, string member) => string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException($"{member} must be set before Register().") : value.Trim();
}
