using cmd.interactive.menuscore.Interface;

namespace cmd.interactive.menuscore.Registry;

/// <summary>Per-bus index of menu instances and definitions.</summary>
public sealed class MenuRegistry
{
    private readonly Dictionary<string, IMenu> _menus = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    public int Count { get { lock (_gate) return _menus.Count; } }
    public IReadOnlyCollection<IMenu> Menus { get { lock (_gate) return _menus.Values.ToArray(); } }

    public void Register(IMenu menu)
    {
        ArgumentNullException.ThrowIfNull(menu);
        if (string.IsNullOrWhiteSpace(menu.Id)) throw new ArgumentException("Menu ID cannot be empty.", nameof(menu));
        if (string.IsNullOrWhiteSpace(menu.Title)) throw new ArgumentException("Menu title cannot be empty.", nameof(menu));
        lock (_gate)
            if (!_menus.TryAdd(menu.Id, menu)) throw new InvalidOperationException($"Menu '{menu.Id}' is already registered on this bus.");
    }

    internal void RegisterOrReplaceDiscovered(IMenu menu)
    {
        lock (_gate) _menus[menu.Id] = menu;
    }

    public bool TryGet(string id, out IMenu? menu)
    {
        ArgumentNullException.ThrowIfNull(id);
        lock (_gate) return _menus.TryGetValue(id, out menu);
    }

    public IMenu GetRequired(string id) => TryGet(id, out var menu) ? menu! : throw new KeyNotFoundException($"Menu '{id}' is not registered on this bus.");
    public bool Contains(string id) { ArgumentNullException.ThrowIfNull(id); lock (_gate) return _menus.ContainsKey(id); }
    public bool Unregister(string id) { ArgumentNullException.ThrowIfNull(id); lock (_gate) return _menus.Remove(id); }
    public void Clear() { lock (_gate) _menus.Clear(); }
}
