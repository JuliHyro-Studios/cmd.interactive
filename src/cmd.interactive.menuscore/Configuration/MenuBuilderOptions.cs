namespace cmd.interactive.menuscore.Configuration;

/// <summary>Configuration shared by the fluent menu builders.</summary>
internal sealed class MenuBuilderOptions
{
    public bool DefaultExitButton { get; set; } = true;
    public string? ExtraText { get; set; }
    public string UpIcon { get; set; } = "↑";
    public string SeparatorUpDownIcon { get; set; } = "/";
    public string DownIcon { get; set; } = "↓";
    public string EnterIcon { get; set; } = "Enter";
    public string BackIcon { get; set; } = "Esc";
    public Action? SelectionUp { get; set; }
    public Action? SelectionDown { get; set; }
    public Action? SelectionChanged { get; set; }
    public Action? ConfirmSelection { get; set; }
    public Action? EnterSelection { get; set; }
    public MenuStyle Style { get; set; } = new();
}
