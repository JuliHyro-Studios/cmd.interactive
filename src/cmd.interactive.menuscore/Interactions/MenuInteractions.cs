namespace cmd.interactive.menuscore.Interactions;

/// <summary>Public entry point for declaring menu text entries and their actions.</summary>
public static class MenuInteractions
{
    public static MenuTextBuilder RegisterText() => new();
    public static MenuInteractionBuilder RegisterInteraction() => new();
}
