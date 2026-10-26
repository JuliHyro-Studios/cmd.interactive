using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Internal;

namespace cmd.interactive.menuscore.Registry;

public sealed class ParentMenuRegistry
{
    private readonly HashSet<string> _ids = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public void register(FrameworksBus bus, string id) => RegisterCore(bus, id);
    private void RegisterCore(FrameworksBus bus, string id)
    {
        ArgumentNullException.ThrowIfNull(bus);
        if (!DefinitionCatalog.TryGetMenu(true, id, bus.ClientName, out var definition) || definition is null)
            throw new KeyNotFoundException($"Parent menu definition '{id}' was not found. Register it with ParentMenuBuilder first.");
        lock (_gate)
        {
            if (!_ids.Add(id)) return;
            try { bus.Menus.Register(definition); }
            catch { _ids.Remove(id); throw; }
        }
    }
    public IReadOnlyCollection<string> RegisteredIds { get { lock (_gate) return _ids.ToArray(); } }
}
