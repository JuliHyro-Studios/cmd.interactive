using cmd.interactive.menuscore.Configuration;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Internal;
using cmd.interactive.menuscore.Models;

namespace cmd.interactive.menuscore.Builders;

public sealed class ParentMenuBuilder
{
    private string? _id;
    private string? _title;
    private string? _clientName;
    private bool _supportChildren;
    private readonly MenuBuilderOptions _options = new();

    public static ParentMenuBuilder Create() => new();
    public ParentMenuBuilder ID(string id) { _id = id; return this; }
    public ParentMenuBuilder ForClient(string clientName) { _clientName = string.IsNullOrWhiteSpace(clientName) ? throw new ArgumentException("Client name cannot be empty.", nameof(clientName)) : clientName.Trim(); return this; }
    public ParentMenuBuilder Title(string title) { _title = title; return this; }
    public ParentMenuBuilder title(string title) => Title(title);
    public ParentMenuBuilder SupportChildMenu(bool value = true) { _supportChildren = value; return this; }
    public ParentMenuBuilder MenuSelectionStyle(MenuSelectionStyle style) { _options.Style.SelectionStyle = style; return this; }
    public ParentMenuBuilder CustomSelection(string prefix, string suffix = "") { _options.Style.SelectionStyle = cmd.interactive.menuscore.Enums.MenuSelectionStyle.Custom; _options.Style.CustomSelectionPrefix = prefix; _options.Style.CustomSelectionSuffix = suffix; return this; }
    public ParentMenuBuilder ExtraText(string? text) { _options.ExtraText = text; return this; }
    public ParentMenuBuilder DefaultExitButton(bool value) { _options.DefaultExitButton = value; return this; }
    public ParentMenuBuilder HowToSelectionUp(Action action) { _options.SelectionUp = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ParentMenuBuilder HowToSelectionDown(Action action) { _options.SelectionDown = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ParentMenuBuilder HowToSelectSelection(Action action) { _options.SelectionChanged = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ParentMenuBuilder HowToConfirSelection(Action action) { _options.ConfirmSelection = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ParentMenuBuilder HowToConfirmSelection(Action action) => HowToConfirSelection(action);
    public ParentMenuBuilder HowToEnterSelection(Action action) { _options.EnterSelection = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ParentMenuBuilder ColorSelection(string color) { _options.Style.SelectionColor = color; return this; }
    public ParentMenuBuilder ColorConfirm(string color) { _options.Style.ConfirmColor = color; return this; }
    public ParentMenuBuilder ColorNormal(string color) { _options.Style.NormalColor = color; return this; }

    public string Register()
    {
        var id = Validate(_id, nameof(ID));
        var title = Validate(_title, nameof(Title));
        DefinitionCatalog.AddMenu(new MenuDefinition { Id = id, Title = title, ParentId = null, SupportChildMenu = _supportChildren, Style = _options.Style, Options = _options, IsParent = true, ClientName = _clientName });
        return id;
    }

    private static string Validate(string? value, string name) => string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException($"{name} must be set before Register().") : value.Trim();
}
