using System.Reflection;
using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Interface;

namespace cmd.interactive.menuscore.Discovery;

/// <summary>Library-owned discovery entry point. Scans loaded assemblies for concrete IMenu implementations and registers them on the supplied bus.</summary>
public static class IMenuDiscovery
{
    public static int StartDiscovery(FrameworksBus bus, params Assembly[] additionalAssemblies)
    {
        ArgumentNullException.ThrowIfNull(bus);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic)
            .Concat(additionalAssemblies ?? Array.Empty<Assembly>())
            .Distinct()
            .ToArray();
        var discovered = 0;
        foreach (var assembly in assemblies)
        {
            foreach (var type in GetLoadableTypes(assembly))
            {
                if (!type.IsClass || type.IsAbstract || type.ContainsGenericParameters) continue;
                if (!typeof(IMenu).IsAssignableFrom(type)) continue;
                if (type.GetConstructor(Type.EmptyTypes) is null) continue;
                if (Activator.CreateInstance(type) is not IMenu menu)
    continue;

if (string.IsNullOrWhiteSpace(menu.Id) ||
    string.IsNullOrWhiteSpace(menu.Title))
{
    continue;
}

bus.Menus.RegisterOrReplaceDiscovered(menu);
discovered++;
            }
        }
        return discovered;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException exception) { return exception.Types.Where(x => x is not null).Cast<Type>(); }
        catch (NotSupportedException) { return Array.Empty<Type>(); }
    }
}
