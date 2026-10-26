# Menus Core

<p align="center">
  <img src="https://raw.githubusercontent.com/JuliHyro-Studios/cmd.interactive/master/Assets/Brand/banner.png" alt="CMD Interactive Library Banner" width="100%">
</p>

<p align="center">
  <strong>Core Component of CMD Interactive Library</strong>
</p>

<p align="center">
  The foundation for building interactive, keyboard-driven menu experiences in C# and .NET console applications.
</p>

<p align="center">
  <a href="https://github.com/JuliHyro-Studios/cmd.interactive">Repository</a>
  ·
  <a href="https://github.com/JuliHyro-Studios/cmd.interactive/issues">Report an Issue</a>
  ·
  <a href="https://github.com/JuliHyro-Studios/cmd.interactive/blob/master/LICENSE">Apache-2.0 License</a>
</p>

---

## Overview

**Menus Core** is the core component of CMD Interactive Library, a C# project focused on interactive menu experiences for .NET console applications.

It provides the foundation for the library's menu functionality, supporting the development of reusable, maintainable, and extensible console menu systems.

## Project Structure

Menus Core is located in the main repository:

[`src/cmd.interactive.menuscore/`](https://github.com/JuliHyro-Studios/cmd.interactive/tree/master/src/cmd.interactive.menuscore)

The project is part of **CMD Interactive Library** and shares the repository's branding and licensing.

## Goals

Menus Core is designed around the following principles:

- **Reusability:** Provide core functionality that can be used across menu-related components.
- **Maintainability:** Keep the core code organized and easier to evolve.
- **Extensibility:** Establish a foundation that can support additional menu capabilities.
- **Developer Experience:** Help .NET developers build interactive console menu experiences through a dedicated library.

## Technology

- **Language:** C#
- **Platform:** .NET

## License

Menus Core is distributed under the [Apache License 2.0](https://github.com/JuliHyro-Studios/cmd.interactive/blob/master/LICENSE).

See the repository's `LICENSE` file for the full license text.

## Related Links

- [CMD Interactive Library Repository](https://github.com/JuliHyro-Studios/cmd.interactive)
- [Menus Core Source Code](https://github.com/JuliHyro-Studios/cmd.interactive/tree/master/src/cmd.interactive.menuscore)
- [Issues and Bug Reports](https://github.com/JuliHyro-Studios/cmd.interactive/issues)
## Note

Menus Core is part of the main CMD Interactive Library repository and is maintained as a component within its `src/` directory. Therefore, it follows the main project's shared policies and documentation, including:

- **License:** [Apache License 2.0](https://github.com/JuliHyro-Studios/cmd.interactive/blob/master/LICENSE)
- **Security Policy:** [SECURITY.md](https://github.com/JuliHyro-Studios/cmd.interactive/blob/master/SECURITY.md)
- **Code of Conduct:** [CODE_OF_CONDUCT.md](https://github.com/JuliHyro-Studios/cmd.interactive/blob/master/CODE_OF_CONDUCT.md)

These policies apply to Menus Core through the main repository. Separate copies are not maintained within this component.

## Quick Start

```csharp
using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Builders;
using cmd.interactive.menuscore.Discovery;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Interactions;
using cmd.interactive.menuscore.Runtime;

var bus = FrameworksBus.RegisterBus("ExampleClient");

// Declarations can be scoped to a client name so separate buses may reuse IDs.
MenuBuilder.ParentMenuBuilder()
    .ForClient("ExampleClient")
    .ID("menucore")
    .Title("Main Menu")
    .SupportChildMenu(true)
    .MenuSelectionStyle(MenuSelectionStyle.SquareBrackets)
    .ExtraText("Use the arrow keys to navigate.")
    .DefaultExitButton(true)
    .Register();

MenuBuilder.ChildMenuBuilder()
    .ForClient("ExampleClient")
    .ChildID("adv")
    .MenuID("menucore")
    .Title("Advanced")
    .MenuSelectionStyle(MenuSelectionStyle.Arrow)
    .Register();

MenuInteractions.RegisterText()
    .ForClient("ExampleClient")
    .ID("menucore")
    .Text("Open Advanced")
    .InteractionID("open-advanced")
    .Register();

MenuInteractions.RegisterInteraction()
    .ForClient("ExampleClient")
    .ID("menucore")
    .InteractionID("open-advanced")
    .Action(() => bus.Navigation.NavigateTo("menucore.adv"))
    .Register();

// Discovery is owned by the library; clients do not scan assemblies themselves.
IMenuDiscovery.StartDiscovery(bus);

bus.PARENT_MENU_REGISTER.register(bus, "menucore");
bus.CHILD_MENU_REGISTER.register(bus, "menucore.adv");
bus.INTERACTION_TEXT_REGISTER.register(bus, "open-advanced");
bus.INTERACTION_REGISTER.register(bus, "open-advanced");

ParentMenu.ShowMenu(bus, "menucore")
    .ClearConsole(false)
    .Run();
```

### Core capabilities

- Isolated `FrameworksBus` instances with separate menu and interaction registries.
- Parent and nested child menus with dotted full IDs (`menucore.adv.settings`).
- Library-owned assembly discovery through `IMenuDiscovery.StartDiscovery(bus)`.
- Separate registration of menu definitions, interaction text, and interaction actions.
- Public navigation API through `bus.Navigation`: `NavigateTo`, `GoBack`, `GoToParent`, and `GoToRoot`.
- Per-menu selection styles, custom markers, colors, extra text, and configurable default Exit entries.
- Optional `ForClient("ClientName")` scoping on builder declarations. Use it when multiple clients in the same process need to declare the same IDs with different definitions.

`ConditionVisible` is evaluated at render time. When it returns `false` (or throws), the entry remains visible in a disabled/dimmed state and cannot be selected.


## Default Behavior When `HowToXxx` Is Not Configured

All `HowToXxx` callbacks are optional. Leaving them unconfigured does not disable the library's built-in keyboard behavior.

- **`↑` / `↓`**: Move the current selection up or down, skipping disabled items.
- **`Enter` / `Space`**: Confirm the currently selected item and execute its registered action.
- **`Enter` on a child menu item**: Automatically navigate to the child menu. The `InteractionID` can be either the child's full menu ID or its local ID relative to the current menu.
- **`Backspace`**: Navigate to the parent menu.
- **`Escape`**: Navigate backward through the navigation history. At the root menu, return to the root and end the session if no further navigation is possible.

Configured callbacks are invoked as extension hooks after their corresponding keyboard operations. They are optional and are not required for the default behaviors to work.