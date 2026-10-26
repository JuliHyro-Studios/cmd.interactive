using System.Collections.Concurrent;
using cmd.interactive.menuscore.Models;

namespace cmd.interactive.menuscore.Internal;

internal static class DefinitionCatalog
{
    private static readonly ConcurrentDictionary<string, MenuDefinition> Menus = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, MenuTextDefinition> Texts = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, MenuInteractionDefinition> Interactions = new(StringComparer.Ordinal);
    private static string Scope(string? client) => string.IsNullOrWhiteSpace(client) ? "*" : client.Trim();
    private static string Key(string scope, string kind, string id) => $"{scope}|{kind}|{id}";
    internal static void AddMenu(MenuDefinition d)
    {
        var key = Key(Scope(d.ClientName), d.IsParent ? "P" : "C", d.Id);
        if (!Menus.TryAdd(key, d)) throw new InvalidOperationException($"A menu definition '{d.Id}' already exists for scope '{Scope(d.ClientName)}'.");
    }
    internal static bool TryGetMenu(bool parent, string id, string clientName, out MenuDefinition? definition)
    {
        if (Menus.TryGetValue(Key(Scope(clientName), parent ? "P" : "C", id), out definition)) return true;
        return Menus.TryGetValue(Key("*", parent ? "P" : "C", id), out definition);
    }
    internal static IEnumerable<MenuDefinition> GetMenus() => Menus.Values;
    internal static void AddText(MenuTextDefinition d)
    {
        if (!Texts.TryAdd(Key(Scope(d.ClientName), "T", $"{d.MenuId}:{d.Id}"), d)) throw new InvalidOperationException($"Menu text '{d.Id}' already exists for menu '{d.MenuId}' in scope '{Scope(d.ClientName)}'.");
    }
    internal static IEnumerable<MenuTextDefinition> GetTexts(string clientName) => Texts.Values.Where(x => x.ClientName is null || x.ClientName == clientName);
    internal static void AddInteraction(MenuInteractionDefinition d)
    {
        if (!Interactions.TryAdd(Key(Scope(d.ClientName), "I", $"{d.MenuId}:{d.InteractionId}"), d)) throw new InvalidOperationException($"Interaction '{d.InteractionId}' already exists for menu '{d.MenuId}' in scope '{Scope(d.ClientName)}'.");
    }
    internal static IEnumerable<MenuInteractionDefinition> GetInteractions(string clientName) => Interactions.Values.Where(x => x.ClientName is null || x.ClientName == clientName);
}
