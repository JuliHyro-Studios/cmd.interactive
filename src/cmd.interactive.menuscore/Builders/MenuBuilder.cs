namespace cmd.interactive.menuscore.Builders;

/// <summary>Entry point for constructing parent and child menu definitions.</summary>
public static class MenuBuilder
{
    public static ParentMenuBuilder ParentMenuBuilder() => new();
    public static ChildMenuBuilder ChildMenuBuilder() => new();
}
