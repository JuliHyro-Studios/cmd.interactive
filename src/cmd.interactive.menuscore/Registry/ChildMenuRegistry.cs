using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Internal;

namespace cmd.interactive.menuscore.Registry;

public sealed class ChildMenuRegistry
{
    private readonly HashSet<string> _ids = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public void register(FrameworksBus bus, string id) => RegisterCore(bus, id);
    private void RegisterCore(FrameworksBus bus, string id)
    {
        ArgumentNullException.ThrowIfNull(bus);
        if (!DefinitionCatalog.TryGetMenu(false, id, bus.ClientName, out var definition) || definition is null)
            throw new KeyNotFoundException($"Child menu definition '{id}' was not found. Register it with ChildMenuBuilder first.");
        if (definition.ParentId is null || !bus.Menus.TryGet(definition.ParentId, out var parent) || parent is null)
            throw new InvalidOperationException($"Parent menu '{definition.ParentId}' must be registered on bus '{bus.ClientName}' before child '{id}'.");
        if (!parent.SupportChildMenu)
            throw new InvalidOperationException($"Parent menu '{definition.ParentId}' does not support child menus.");
        lock (_gate)
        {
            if (!_ids.Add(id)) return;
            try { bus.Menus.Register(definition); }
            catch { _ids.Remove(id); throw; }
        }
    }
    public IReadOnlyCollection<string> RegisteredIds { get { lock (_gate) return _ids.ToArray(); } }
}
