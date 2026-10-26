using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Internal;
using cmd.interactive.menuscore.Models;

namespace cmd.interactive.menuscore.Registry;

public sealed class InteractionTextRegistry
{
    private readonly Dictionary<string, MenuTextDefinition> _items = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public void register(FrameworksBus bus, string interactionId) => RegisterCore(bus, interactionId);
    private void RegisterCore(FrameworksBus bus, string interactionId)
    {
        ArgumentNullException.ThrowIfNull(bus);
        var definitions = DefinitionCatalog.GetTexts(bus.ClientName).Where(x => x.Id == interactionId && bus.Menus.Contains(x.MenuId)).OrderBy(x => x.ClientName == bus.ClientName ? 1 : 0).ToArray();
        if (definitions.Length == 0) throw new KeyNotFoundException($"Interaction text '{interactionId}' was not defined for a registered menu on bus '{bus.ClientName}'.");
        lock (_gate) foreach (var definition in definitions) _items[$"{definition.MenuId}:{interactionId}"] = definition;
    }

    internal IReadOnlyList<MenuTextDefinition> GetForMenu(string menuId)
    {
        lock (_gate) return _items.Values.Where(x => x.MenuId == menuId).ToArray();
    }
}
