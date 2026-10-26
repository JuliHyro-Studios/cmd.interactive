using cmd.interactive.menuscore.Enums;

namespace cmd.interactive.menuscore.Configuration;

/// <summary>Per-menu display configuration. Each menu owns its own instance.</summary>
public sealed class MenuStyle
{
    public MenuSelectionStyle SelectionStyle { get; set; } = MenuSelectionStyle.Arrow;
    public string CustomSelectionPrefix { get; set; } = "[> ";
    public string CustomSelectionSuffix { get; set; } = "]";
    public string SelectionColor { get; set; } = nameof(ConsoleColor.Yellow);
    public string ConfirmColor { get; set; } = nameof(ConsoleColor.Green);
    public string NormalColor { get; set; } = nameof(ConsoleColor.Gray);

    internal string Format(string text, bool selected)
    {
        if (!selected) return $"  {text}";
        return SelectionStyle switch
        {
            MenuSelectionStyle.Arrow => $"> {text}",
            MenuSelectionStyle.SquareBrackets => $"[x] {text}",
            MenuSelectionStyle.DoubleArrow => $">> {text}",
            MenuSelectionStyle.Parentheses => $"(x) {text}",
            MenuSelectionStyle.Asterisk => $"* {text}",
            MenuSelectionStyle.Custom => $"{CustomSelectionPrefix}{text}{CustomSelectionSuffix}",
            _ => $"> {text}"
        };
    }
}
