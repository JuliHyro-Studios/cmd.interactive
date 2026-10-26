using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Interface;

namespace cmd.interactive.menuscore.Runtime;

/// <summary>Public navigation API scoped to one client bus.</summary>
public sealed class MenuNavigation
{
    private readonly FrameworksBus _bus;
    private readonly List<string> _history = new();

    public MenuNavigation(FrameworksBus bus) => _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    public IMenu? CurrentMenu { get; private set; }
    public string? CurrentMenuId => CurrentMenu?.Id;
    public bool CanGoBack => _history.Count > 0;
    public IReadOnlyList<string> NavigationHistory => _history.ToArray();

    public MenuNavigationResult NavigateTo(string menuId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        if (!_bus.Menus.TryGet(menuId, out var target) || target is null) return MenuNavigationResult.TargetNotFound;
        if (CurrentMenu is not null && !string.Equals(CurrentMenu.Id, target.Id, StringComparison.Ordinal)) _history.Add(CurrentMenu.Id);
        CurrentMenu = target;
        return MenuNavigationResult.Navigated;
    }

    public MenuNavigationResult GoBack()
    {
        while (_history.Count > 0)
        {
            var index = _history.Count - 1;
            var id = _history[index];
            _history.RemoveAt(index);
            if (_bus.Menus.TryGet(id, out var target) && target is not null) { CurrentMenu = target; return MenuNavigationResult.Navigated; }
        }
        return MenuNavigationResult.NoHistory;
    }

    public MenuNavigationResult GoToParent()
    {
        if (CurrentMenu?.ParentId is not { Length: > 0 } parentId) return MenuNavigationResult.AlreadyAtRoot;
        return NavigateTo(parentId);
    }

    public MenuNavigationResult GoToRoot()
    {
        if (CurrentMenu is null) return MenuNavigationResult.TargetNotFound;
        var root = CurrentMenu;
        while (root.ParentId is { Length: > 0 } parentId && _bus.Menus.TryGet(parentId, out var parent) && parent is not null) root = parent;
        return NavigateTo(root.Id);
    }
}
