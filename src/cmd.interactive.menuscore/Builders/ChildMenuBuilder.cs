using cmd.interactive.menuscore.Configuration;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Internal;
using cmd.interactive.menuscore.Models;

namespace cmd.interactive.menuscore.Builders;

public sealed class ChildMenuBuilder
{
    private string? _childId;
    private string? _parentId;
    private string? _title;
    private string? _clientName;
    private readonly MenuBuilderOptions _options = new();

    public static ChildMenuBuilder Create() => new();
    public ChildMenuBuilder ChildID(string id) { _childId = id; return this; }
    public ChildMenuBuilder MenuID(string parentId) { _parentId = parentId; return this; }
    public ChildMenuBuilder ForClient(string clientName) { _clientName = string.IsNullOrWhiteSpace(clientName) ? throw new ArgumentException("Client name cannot be empty.", nameof(clientName)) : clientName.Trim(); return this; }
    public ChildMenuBuilder Title(string title) { _title = title; return this; }
    public ChildMenuBuilder title(string title) => Title(title);
    public ChildMenuBuilder DefaultExitButton(bool value) { _options.DefaultExitButton = value; return this; }
    public ChildMenuBuilder MenuSelectionStyle(MenuSelectionStyle style) { _options.Style.SelectionStyle = style; return this; }
    public ChildMenuBuilder CustomSelection(string prefix, string suffix = "") { _options.Style.SelectionStyle = cmd.interactive.menuscore.Enums.MenuSelectionStyle.Custom; _options.Style.CustomSelectionPrefix = prefix; _options.Style.CustomSelectionSuffix = suffix; return this; }
    public ChildMenuBuilder ExtraText(string? text) { _options.ExtraText = text; return this; }
    public ChildMenuBuilder UpIcon(string icon) { _options.UpIcon = icon ?? throw new ArgumentNullException(nameof(icon)); return this; }
    public ChildMenuBuilder SeperatorUpDownIcon(string icon) { _options.SeparatorUpDownIcon = icon ?? throw new ArgumentNullException(nameof(icon)); return this; }
    public ChildMenuBuilder DownIcon(string icon) { _options.DownIcon = icon ?? throw new ArgumentNullException(nameof(icon)); return this; }
    public ChildMenuBuilder EnterIcon(string icon) { _options.EnterIcon = icon ?? throw new ArgumentNullException(nameof(icon)); return this; }
    public ChildMenuBuilder BackIcon(string icon) { _options.BackIcon = icon ?? throw new ArgumentNullException(nameof(icon)); return this; }
    public ChildMenuBuilder HowToSelectionUp(Action action) { _options.SelectionUp = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ChildMenuBuilder HowToSelectionDown(Action action) { _options.SelectionDown = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ChildMenuBuilder HowToSelectSelection(Action action) { _options.SelectionChanged = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ChildMenuBuilder HowToConfirSelection(Action action) { _options.ConfirmSelection = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ChildMenuBuilder HowToConfirmSelection(Action action) => HowToConfirSelection(action);
    public ChildMenuBuilder HowToEnterSelection(Action action) { _options.EnterSelection = action ?? throw new ArgumentNullException(nameof(action)); return this; }
    public ChildMenuBuilder ColorSelection(string color) { _options.Style.SelectionColor = color; return this; }
    public ChildMenuBuilder ColorConfirm(string color) { _options.Style.ConfirmColor = color; return this; }
    public ChildMenuBuilder ColorNormal(string color) { _options.Style.NormalColor = color; return this; }

    public string Register()
    {
        var localId = Validate(_childId, nameof(ChildID)).Trim('.');
        if (localId.Length == 0 || localId.Contains('.')) throw new InvalidOperationException("ChildID must be a non-empty local ID without dots.");
        var parentId = Validate(_parentId, nameof(MenuID));
        var title = Validate(_title, nameof(Title));
        var fullId = $"{parentId}.{localId}";
        DefinitionCatalog.AddMenu(new MenuDefinition { Id = fullId, Title = title, ParentId = parentId, SupportChildMenu = true, Style = _options.Style, Options = _options, IsParent = false, ClientName = _clientName });
        return fullId;
    }

    private static string Validate(string? value, string name) => string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException($"{name} must be set before Register().") : value.Trim().Trim('.');
}
