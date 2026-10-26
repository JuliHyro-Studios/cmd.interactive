
using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Configuration;
using cmd.interactive.menuscore.Internal;
using cmd.interactive.menuscore.Interface;
using cmd.interactive.menuscore.Models;
using cmd.interactive.menuscore.Registry;

namespace cmd.interactive.menuscore.Runtime;

/// <summary>Configures and runs the interactive console loop for a menu on a specific bus.</summary>
public sealed class MenuDisplaySession
{
    private readonly FrameworksBus _bus;
    private readonly string _initialMenuId;
    private bool _clearConsole = true;
    private bool _hasRun;

    internal MenuDisplaySession(FrameworksBus bus, string menuId)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        ArgumentException.ThrowIfNullOrWhiteSpace(menuId);
        _initialMenuId = menuId;
        Navigation = _bus.Navigation;
    }

    public MenuNavigation Navigation { get; }

    public MenuDisplaySession ClearConsole(bool clearConsole)
    {
        _clearConsole = clearConsole;
        return this;
    }

    public void Run()
    {
        if (_hasRun)
            throw new InvalidOperationException("A menu display session can only be run once.");

        _hasRun = true;

        var navigation = Navigation;
        var result = navigation.NavigateTo(_initialMenuId);

        if (result != cmd.interactive.menuscore.Enums.MenuNavigationResult.Navigated)
            throw new KeyNotFoundException(
                $"Menu '{_initialMenuId}' is not registered on bus '{_bus.ClientName}'.");

        var selectedIndices = new Dictionary<string, int>(StringComparer.Ordinal);
        var confirmedIndices = new Dictionary<string, int>(StringComparer.Ordinal);
        string? renderedMenuId = null;

        while (navigation.CurrentMenu is { } menu)
        {
            var options = menu is MenuDefinition definition
                ? definition.Options
                : new MenuBuilderOptions();

            var entries = GetEntries(menu, options);

            if (!string.Equals(renderedMenuId, menu.Id, StringComparison.Ordinal))
            {
                if (_clearConsole)
                    Console.Clear();

                renderedMenuId = menu.Id;
            }

            var index = selectedIndices.TryGetValue(menu.Id, out var previous)
                ? Math.Clamp(previous, 0, Math.Max(entries.Count - 1, 0))
                : 0;

            index = FindEnabled(entries, index, 1);

            // Reset the previous frame before rendering the current selection.
            Console.SetCursorPosition(0, 0);
Console.ResetColor();

int renderHeight = entries.Count + 3
    + (string.IsNullOrWhiteSpace(options.ExtraText) ? 2 : 3);

for (int i = 0; i < renderHeight; i++)
{
    Console.Write(new string(' ', Math.Max(Console.BufferWidth - 1, 1)));
    Console.WriteLine();
}

Console.SetCursorPosition(0, 0);
int renderConfirmedIndex = confirmedIndices.TryGetValue(
    menu.Id, out var confirmed)
    ? confirmed
    : -1;

Render(menu, options, entries, index, renderConfirmedIndex);

            var key = Console.ReadKey(true).Key;
            
            if (key == ConsoleKey.Escape)
            {
                var currentMenuId = menu.Id;

                if (navigation.CanGoBack)
                    navigation.GoBack();
                else
                    navigation.GoToRoot();

                if (navigation.CurrentMenu?.Id == currentMenuId)
                    break;

                continue;
            }

            if (entries.Count == 0)
                continue;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    index = FindEnabled(entries, index - 1, -1);
                    selectedIndices[menu.Id] = index;
                    confirmedIndices.Remove(menu.Id);

                    options.SelectionUp?.Invoke();
                    options.SelectionChanged?.Invoke();
                    break;

                case ConsoleKey.DownArrow:
                    index = FindEnabled(entries, index + 1, 1);
                    selectedIndices[menu.Id] = index;
                    confirmedIndices.Remove(menu.Id);

                    options.SelectionDown?.Invoke();
                    options.SelectionChanged?.Invoke();
                    break;

                case ConsoleKey.Spacebar:
                    if (index < 0 ||
                        index >= entries.Count ||
                        !entries[index].Enabled)
                    {
                        break;
                    }

                    // Phase 1: confirm the currently selected entry.
                    confirmedIndices[menu.Id] = index;
                    selectedIndices[menu.Id] = index;

                    Console.ForegroundColor = ParseColor(
                        menu.Style.ConfirmColor,
                        ConsoleColor.Green);

                    try
                    {
                        options.ConfirmSelection?.Invoke();
                    }
                    finally
                    {
                        Console.ResetColor();
                    }

                    break;

                case ConsoleKey.Enter:
                    if (index < 0 ||
                        index >= entries.Count ||
                        !entries[index].Enabled)
                    {
                        break;
                    }

                    // Phase 2: only execute the entry confirmed by Backspace.
                    if (!confirmedIndices.TryGetValue(menu.Id, out var confirmedIndex))
    break;

if (confirmedIndex != index)
{
    confirmedIndices.Remove(menu.Id);
    break;
}

                    confirmedIndices.Remove(menu.Id);
                    selectedIndices[menu.Id] = index;

                    var entry = entries[index];

                    if (entry.IsExit)
                    {
                        if (navigation.GoToParent() ==
                            cmd.interactive.menuscore.Enums.MenuNavigationResult.AlreadyAtRoot)
                        {
                            return;
                        }

                        break;
                    }

                    var childMenuId = ResolveChildMenuId(
                        menu.Id,
                        entry.InteractionId);

                    if (childMenuId is not null)
                    {
                        options.EnterSelection?.Invoke();
                        navigation.NavigateTo(childMenuId);
                        break;
                    }

                    if (_bus.INTERACTION_REGISTER.TryGet(
                            menu.Id,
                            entry.InteractionId,
                            out var interaction) &&
                        interaction is not null)
                    {
                        interaction.Action();
                    }

                    options.EnterSelection?.Invoke();
                    break;
            }
        }
    }

    private string? ResolveChildMenuId(string parentMenuId, string interactionId)
    {
        if (_bus.Menus.TryGet(interactionId, out var exact) &&
            exact is not null &&
            string.Equals(exact.ParentId, parentMenuId, StringComparison.Ordinal))
        {
            return exact.Id;
        }

        var localChildId = $"{parentMenuId}.{interactionId}";

        if (_bus.Menus.TryGet(localChildId, out var child) &&
            child is not null &&
            string.Equals(child.ParentId, parentMenuId, StringComparison.Ordinal))
        {
            return child.Id;
        }

        return null;
    }

    private List<RuntimeEntry> GetEntries(IMenu menu, MenuBuilderOptions options)
    {
        var entries = _bus.INTERACTION_TEXT_REGISTER.GetForMenu(menu.Id)
            .Select(text => new RuntimeEntry(
                text.Text,
                text.Id,
                IsEnabled(text),
                false,
                text.Color))
            .ToList();

        if (options.DefaultExitButton)
            entries.Add(new RuntimeEntry("Exit", "__exit", true, true, null));

        return entries;
    }

    private static bool IsEnabled(MenuTextDefinition text)
    {
        if (text.ConditionVisible is null)
            return true;

        try
        {
            return text.ConditionVisible();
        }
        catch
        {
            return false;
        }
    }

    private static int FindEnabled(
        IReadOnlyList<RuntimeEntry> entries,
        int start,
        int direction)
    {
        if (entries.Count == 0)
            return -1;

        var index = (start % entries.Count + entries.Count) % entries.Count;

        for (var i = 0; i < entries.Count; i++)
        {
            if (entries[index].Enabled)
                return index;

            index = (index + direction + entries.Count) % entries.Count;
        }

        return -1;
    }

    
private static void Render(
    IMenu menu,
    MenuBuilderOptions options,
    IReadOnlyList<RuntimeEntry> entries,
    int selectedIndex,
    int confirmedIndex)
{
    Console.ResetColor();

    Console.ForegroundColor = ParseColor(
        menu.Style.NormalColor,
        ConsoleColor.Gray);

    Console.WriteLine(menu.Title);
    Console.WriteLine(new string('=', Math.Max(menu.Title.Length, 8)));

    for (var i = 0; i < entries.Count; i++)
    {
        var entry = entries[i];

        bool selected = i == selectedIndex;
        bool confirmed = i == confirmedIndex;

        if (!entry.Enabled)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        else if (confirmed)
        {
            // Confirmed selection has its own configured color.
            Console.ForegroundColor = ParseColor(
                menu.Style.ConfirmColor,
                ConsoleColor.Green);
        }
        else if (selected)
        {
            // Navigation selection has its own configured color.
            Console.ForegroundColor = ParseColor(
                menu.Style.SelectionColor,
                ConsoleColor.Yellow);
        }
        else
        {
            // Preserve the color explicitly configured by the client.
            Console.ForegroundColor = ParseColor(
                entry.Color,
                ParseColor(menu.Style.NormalColor, ConsoleColor.Gray));
        }

        Console.WriteLine(
            menu.Style.Format(entry.Text, selected && entry.Enabled));

        Console.ResetColor();
    }

    if (!string.IsNullOrWhiteSpace(options.ExtraText))
        Console.WriteLine($"\n{options.ExtraText}");

    Console.WriteLine(
        "\n↑/↓ Navigate · Space Confirm · Enter Enter Selection · Esc Back/Exit");

    Console.ResetColor();
}


    private static ConsoleColor ParseColor(string? value, ConsoleColor fallback)
        => Enum.TryParse<ConsoleColor>(value, true, out var color)
            ? color
            : fallback;

    private sealed record RuntimeEntry(
        string Text,
        string InteractionId,
        bool Enabled,
        bool IsExit,
        string? Color);
}
