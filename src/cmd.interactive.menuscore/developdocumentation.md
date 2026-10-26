# Menus Core Developer Documentation

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

---

## Overview Documentation

This document is the detailed developer reference for Menus Core. It documents public APIs, their behavior, registration flow, menu runtime, navigation, and configuration options based on the source included with version `0.1.0`.

Documentation is grouped by version. Each version block describes the API and behavior available in that release. The **Shared Version** section is reserved for documentation conventions and concepts that are stable across releases; version-specific API signatures and behavior remain in their release block.

### Documentation rules

- API names and signatures in each version section correspond to the source for that version.
- Do not assume that an API documented for one release exists in another release.
- Future release sections should be added when those releases exist, and should document additions, changes, deprecations, and removals.
- Examples are written for the API shown in the relevant version section.

## Shared Version

This section contains documentation organization and concepts that do not depend on a particular API revision.

### Terminology

- **Bus**: a `FrameworksBus` instance. It owns menu registries and navigation state for one client identity.
- **Menu definition**: a menu created by `ParentMenuBuilder` or `ChildMenuBuilder`.
- **Menu entry / interaction text**: a visible item registered through `MenuInteractions.RegisterText()`.
- **Interaction action**: an `Action` registered through `MenuInteractions.RegisterInteraction()`.
- **Full menu ID**: the ID used by the runtime and registry. Child IDs are formed by appending the local child ID to the parent ID with a dot.
- **Discovery**: the library's reflection-based search for concrete `IMenu` implementations in loaded assemblies and any additional assemblies passed to discovery.

### High-level lifecycle

1. Create a `FrameworksBus` for a client.
2. Declare parent and child menu definitions with the builders.
3. Declare menu text entries and interaction actions.
4. Run `IMenuDiscovery.StartDiscovery(bus)` when the client wants the library to discover `IMenu` implementations.
5. Register the declared parent menus, child menus, interaction texts, and actions into the bus through its registries.
6. Start a display session with `ParentMenu.ShowMenu(bus, menuId).Run()`.

The registration APIs are explicit: declaring a definition with a builder does not, by itself, place that definition in the bus's runtime menu registry. The corresponding registry method must be called.

### Client scopes and bus isolation

`FrameworksBus.RegisterBus(clientName)` creates a new bus with its own menu registry, text registry, action registry, and navigation instance. Separate bus instances do not share their runtime registry state.

Builder declarations can optionally use `ForClient(clientName)`. A declaration with no client scope is global (`*`) in the internal definition catalog and can be considered by any client. A scoped declaration is associated with that client name. If an ID is declared more than once in the same scope and category, the catalog throws an `InvalidOperationException`.

`FrameworksBus.RegisterBus(existingBus, clientName)` returns the same bus only when the supplied name matches its existing identity. It does not rename a bus.

### Error and validation behavior

The builders validate required values during `Register()`. Missing or whitespace-only required values cause `InvalidOperationException`. A blank client name causes `ArgumentException`. Registry methods can throw `KeyNotFoundException` when the requested definition was not declared or is not available for the supplied bus. Child registration also checks that the parent is registered and has `SupportChildMenu` enabled.

---

<details>
<summary><strong>Version 0.1.0</strong> — Initial Release</summary>

## Version 0.1.0: API Reference

Package metadata in the included project file identifies this release as `cmd.Interactive.MenusCore` version `0.1.0`. The project targets `net10.0`, `net8.0`, and `net9.0`, enables implicit usings and nullable reference types, and generates XML documentation.

### Namespaces

| Namespace | Responsibility |
|---|---|
| `cmd.interactive.menuscore.Bus` | Client bus and registries |
| `cmd.interactive.menuscore.Builders` | Parent and child menu builders |
| `cmd.interactive.menuscore.Configuration` | Menu display style |
| `cmd.interactive.menuscore.Discovery` | Reflection-based `IMenu` discovery |
| `cmd.interactive.menuscore.Enums` | Selection styles and navigation results |
| `cmd.interactive.menuscore.Interactions` | Text and action declaration builders |
| `cmd.interactive.menuscore.Interface` | `IMenu` contract |
| `cmd.interactive.menuscore.Registry` | Runtime registries |
| `cmd.interactive.menuscore.Runtime` | Display session and navigation |

## 1. Bus API

### `FrameworksBus`

Namespace: `cmd.interactive.menuscore.Bus`

The bus contains the runtime registries and the navigation controller for a client.

```csharp
FrameworksBus.RegisterBus(string clientName)
FrameworksBus.RegisterBus(FrameworksBus bus, string clientName)
```

Public members:

- `ClientName`: immutable client identity.
- `PARENT_MENU_REGISTER`: parent menu registry.
- `CHILD_MENU_REGISTER`: child menu registry.
- `INTERACTION_REGISTER`: action registry.
- `INTERACTION_TEXT_REGISTER`: visible interaction-text registry.
- `Menus`: runtime `MenuRegistry`.
- `Navigation`: `MenuNavigation` associated with this bus.

`RegisterBus(string)` rejects an empty or whitespace-only client name and creates a new bus. The overload receiving an existing bus rejects a mismatched name and returns the same bus when the name matches.

Each bus owns its own runtime registries and navigation history. The internal definition catalog is shared static state, with optional client scoping on declarations; do not confuse that declaration catalog with the per-bus runtime registries.

## 2. Menu builders

### `MenuBuilder`

Namespace: `cmd.interactive.menuscore.Builders`

Entry points:

```csharp
MenuBuilder.ParentMenuBuilder()
MenuBuilder.ChildMenuBuilder()
```

They return `ParentMenuBuilder` and `ChildMenuBuilder` respectively.

### `ParentMenuBuilder`

Fluent methods:

| Method | Behavior |
|---|---|
| `ID(string id)` | Sets the root menu ID. |
| `ForClient(string clientName)` | Scopes the definition to a client name. |
| `Title(string title)` / `title(string title)` | Sets the displayed title. |
| `SupportChildMenu(bool value = true)` | Sets whether children may be registered beneath this menu. |
| `MenuSelectionStyle(MenuSelectionStyle style)` | Chooses a built-in selection marker. |
| `CustomSelection(string prefix, string suffix = "")` | Uses a custom prefix and suffix around the selected entry. |
| `ExtraText(string? text)` | Adds text below the menu entries. |
| `UpIcon(string icon)` | Sets the displayed up-navigation icon in the help line. |
| `SeperatorUpDownIcon(string icon)` | Sets the separator between the up and down icons. The API name intentionally follows the requested spelling `Seperator`. |
| `DownIcon(string icon)` | Sets the displayed down-navigation icon in the help line. |
| `EnterIcon(string icon)` | Sets the displayed Enter icon/label in the help line. |
| `BackIcon(string icon)` | Sets the displayed back/exit icon/label in the help line. |
| `DefaultExitButton(bool value)` | Enables or disables the automatically appended `Exit` entry. |
| `HowToSelectionUp(Action action)` | Configures the selection-up callback. |
| `HowToSelectionDown(Action action)` | Configures the selection-down callback. |
| `HowToSelectSelection(Action action)` | Configures the selection-changed callback. |
| `HowToConfirSelection(Action action)` | Configures the confirmation callback. |
| `HowToConfirmSelection(Action action)` | Correctly spelled alias for `HowToConfirSelection`. |
| `HowToEnterSelection(Action action)` | Configures the enter callback. |
| `ColorSelection(string color)` | Sets the selected-entry color by `ConsoleColor` name. |
| `ColorConfirm(string color)` | Sets the confirmation color setting. |
| `ColorNormal(string color)` | Sets the normal text color. |
| `Register()` | Validates required fields, adds the definition to the internal catalog, and returns the menu ID. |

The help-line icons are configurable on both parent and child builders with `UpIcon`, `SeperatorUpDownIcon`, `DownIcon`, `EnterIcon`, and `BackIcon`. These values customize the displayed help line only; they do not bind keyboard keys. Defaults are `↑`, `/`, `↓`, `Enter`, and `Esc`. The `Space Confirm` label remains fixed.

`ID` and `Title` must be supplied before `Register()`. `ID` is trimmed but is not required to be a dotted hierarchy path. The builder's options default to `DefaultExitButton = true`, `MenuSelectionStyle.Arrow`, selected color `Yellow`, confirmation color `Green`, and normal color `Gray`.

### `ChildMenuBuilder`

Fluent methods:

| Method | Behavior |
|---|---|
| `ChildID(string id)` | Sets the local child ID. |
| `MenuID(string parentId)` | Sets the parent menu ID. |
| `ForClient(string clientName)` | Scopes the definition to a client name. |
| `Title(string title)` / `title(string title)` | Sets the displayed title. |
| `DefaultExitButton(bool value)` | Enables or disables the automatically appended `Exit` entry. |
| `MenuSelectionStyle(MenuSelectionStyle style)` | Chooses a built-in selection marker. |
| `CustomSelection(string prefix, string suffix = "")` | Uses a custom selected-entry marker. |
| `ExtraText(string? text)` | Adds text below the entries. |
| `UpIcon(string icon)` | Sets the displayed up-navigation icon in the help line. |
| `SeperatorUpDownIcon(string icon)` | Sets the separator between the up and down icons. The API name intentionally follows the requested spelling `Seperator`. |
| `DownIcon(string icon)` | Sets the displayed down-navigation icon in the help line. |
| `EnterIcon(string icon)` | Sets the displayed Enter icon/label in the help line. |
| `BackIcon(string icon)` | Sets the displayed back/exit icon/label in the help line. |
| `HowToSelectionUp(Action action)` | Configures the selection-up callback. |
| `HowToSelectionDown(Action action)` | Configures the selection-down callback. |
| `HowToSelectSelection(Action action)` | Configures the selection-changed callback. |
| `HowToConfirSelection(Action action)` | Configures the confirmation callback. |
| `HowToConfirmSelection(Action action)` | Correctly spelled alias for `HowToConfirSelection`. |
| `HowToEnterSelection(Action action)` | Configures the enter callback. |
| `ColorSelection(string color)` | Sets the selected-entry color. |
| `ColorConfirm(string color)` | Sets the confirmation color setting. |
| `ColorNormal(string color)` | Sets the normal text color. |
| `Register()` | Creates a full ID as `parentId.localChildId`, adds the definition to the catalog, and returns the full ID. |

`ChildID`, `MenuID`, and `Title` are required. The local child ID cannot be empty or contain a dot. A child definition can be declared before its parent is registered, but it cannot be added to a bus until the parent is registered and has child-menu support enabled.

### Selection styles

Namespace: `cmd.interactive.menuscore.Enums`

`MenuSelectionStyle` contains:

- `Arrow`: `> Entry`
- `SquareBrackets`: `[x] Entry`
- `DoubleArrow`: `>> Entry`
- `Parentheses`: `(x) Entry`
- `Asterisk`: `* Entry`
- `Custom`: uses `CustomSelectionPrefix` and `CustomSelectionSuffix`

Unselected entries are rendered with two leading spaces. The built-in styles only change the selected-entry marker.

### `MenuStyle`

Namespace: `cmd.interactive.menuscore.Configuration`

Public properties:

- `SelectionStyle`
- `CustomSelectionPrefix`
- `CustomSelectionSuffix`
- `SelectionColor`
- `ConfirmColor`
- `NormalColor`

Defaults are `Arrow`, `"[> "`, `"]"`, `"Yellow"`, `"Green"`, and `"Gray"` respectively. Color values are strings parsed as `ConsoleColor` names at render time; an unrecognized value falls back to the runtime's fallback color. The style object is per menu.

## 3. Declaring menu text and actions

### `MenuInteractions`

Namespace: `cmd.interactive.menuscore.Interactions`

Entry points:

```csharp
MenuInteractions.RegisterText()
MenuInteractions.RegisterInteraction()
```

These create independent builders for visible menu entries and their actions.

### `MenuTextBuilder`

| Method | Behavior |
|---|---|
| `ForClient(string clientName)` | Scopes the declaration to a client. |
| `ID(string menuId)` | Sets the menu that owns the entry. |
| `Text(string text)` | Sets the visible label. |
| `InteractionID(string interactionId)` | Sets the ID connecting the text to an action. |
| `Color(string color)` | Sets the entry's optional text color. |
| `ConditionVisible(Func<bool> condition)` | Sets a condition evaluated while building the runtime entry list. |
| `Register()` | Validates required values, adds the text definition to the catalog, and returns the interaction ID. |

`ID`, `Text`, and `InteractionID` are required. `ConditionVisible` is optional. When the condition returns `false`, the entry is displayed as disabled/dimmed and is skipped by selection navigation. An exception thrown by the condition is caught and treated as `false`.

Despite the method name `ConditionVisible`, the implementation does not hide the entry; it disables it.

### `MenuInteractionBuilder`

| Method | Behavior |
|---|---|
| `ForClient(string clientName)` | Scopes the declaration to a client. |
| `ID(string menuId)` | Sets the owning menu ID. |
| `InteractionID(string interactionId)` | Sets the action's ID. |
| `Action(Action action)` | Sets the delegate to execute. |
| `Register()` | Validates the menu ID, interaction ID, and action; adds the action definition and returns the interaction ID. |

The text and action are registered separately. Their IDs and owning menu IDs must correspond for the runtime to find the action associated with a visible entry.

## 4. Registration and discovery

### Definition catalog

`DefinitionCatalog` is internal. It stores menu definitions, visible text definitions, and action definitions in concurrent dictionaries. Builder `Register()` methods add declarations to this catalog; they do not automatically populate a particular bus's runtime registries.

Declarations may be global or scoped with `ForClient`. Duplicate declarations with the same scope, category, and relevant ID throw `InvalidOperationException`.

### `IMenuDiscovery`

Namespace: `cmd.interactive.menuscore.Discovery`

```csharp
int IMenuDiscovery.StartDiscovery(
    FrameworksBus bus,
    params Assembly[] additionalAssemblies)
```

The method scans currently loaded, non-dynamic assemblies plus any additional assemblies supplied by the caller. It finds non-abstract, closed classes implementing `IMenu` that expose a public parameterless constructor, creates instances, and registers them in `bus.Menus`. It returns the number of instances discovered and registered during that call.

`ReflectionTypeLoadException` is handled by using the types that loaded successfully; `NotSupportedException` causes the assembly's type list to be treated as empty. Discovery is not the same as registering builder declarations: those definitions still need their corresponding registry calls.

### Registry methods

The bus exposes four registration registries:

```csharp
bus.PARENT_MENU_REGISTER.register(bus, menuId);
bus.CHILD_MENU_REGISTER.register(bus, fullChildMenuId);
bus.INTERACTION_TEXT_REGISTER.register(bus, interactionId);
bus.INTERACTION_REGISTER.register(bus, interactionId);
```

The method spelling `register` is lowercase in this release.

- `ParentMenuRegistry.register` looks up a parent definition, then adds it to `bus.Menus`.
- `ChildMenuRegistry.register` looks up a child definition, verifies that its parent is already registered, and verifies `SupportChildMenu` on the parent before adding the child.
- `InteractionTextRegistry.register` registers text definitions for the supplied interaction ID when their owning menus are already registered on the bus.
- `InteractionRegistry.register` registers action definitions for the supplied interaction ID when their owning menus are already registered on the bus.

The text and action registries key runtime entries by menu ID plus interaction ID. This allows the same interaction ID to be used in different menus. Repeating a registry registration for an already-recorded parent or child ID returns without adding it again.

## 5. Implementing `IMenu`

Namespace: `cmd.interactive.menuscore.Interface`

```csharp
public interface IMenu
{
    string Id { get; }
    string Title { get; }
    string? ParentId { get; }
    bool SupportChildMenu { get; }
    MenuStyle Style { get; }
}
```

A discovered menu implementation must be a concrete, non-open-generic class with a public parameterless constructor. `Id` and `Title` must be non-empty when a menu is registered through `MenuRegistry.Register`.

### `MenuRegistry`

Public members:

- `Count`: number of currently registered menu instances.
- `Menus`: snapshot of registered menu instances.
- `Register(IMenu menu)`: registers a menu and rejects duplicate IDs.
- `TryGet(string id, out IMenu? menu)`: attempts to find a menu.
- `GetRequired(string id)`: returns a menu or throws `KeyNotFoundException`.
- `Contains(string id)`: checks whether an ID exists.
- `Unregister(string id)`: removes a menu and returns whether it existed.
- `Clear()`: removes all menus from this registry.

Discovered menus are added through an internal replace operation, so discovery can replace an existing entry with the same ID. This differs from public `Register`, which throws on duplicates.

## 6. Runtime display

### `ParentMenu.ShowMenu`

Namespace: `cmd.interactive.menuscore.Runtime`

```csharp
ParentMenu.ShowMenu(FrameworksBus bus, string menuId)
```

Returns a `MenuDisplaySession`.

### `MenuDisplaySession`

Public members:

- `Navigation`: the bus's `MenuNavigation`.
- `ClearConsole(bool clearConsole)`: configures whether the console is cleared before each render and returns the same session.
- `Run()`: starts the interactive loop. A session can be run only once; a second call throws `InvalidOperationException`.

At startup, `Run()` navigates to the requested menu ID. If the menu is not registered on the bus, it throws `KeyNotFoundException`. The loop renders the title, entries, optional extra text, and the keyboard hint.

The default `Exit` entry is appended when the menu's `DefaultExitButton` option is `true`. If the menu has no entries, the runtime prints an empty-menu message and waits for keyboard input.

### Keyboard behavior in the implementation

- **Up arrow / Down arrow**: move between enabled entries, skipping disabled entries and wrapping around the list.
- **Enter / Space**: process the selected entry. The default `Exit` entry ends the display loop. For other entries, the runtime first checks whether the interaction ID resolves to a direct child menu; if so, it invokes `EnterSelection` when configured and navigates to that child. Otherwise, it runs the registered action when found, then invokes the configured `EnterSelection` callback.
- **Backspace**: calls `GoToParent()`.
- **Escape**: attempts `GoBack()`. If no history exists, it attempts `GoToRoot()`. If the result is not `Navigated`, the display loop ends.
- **`HowToSelectionUp` / `HowToSelectionDown`**: the supplied `Action` is executed when the runtime processes the corresponding up/down navigation event.
- **`HowToSelectSelection`**: the supplied `Action` is executed after the selected entry changes.
- **`HowToConfirSelection` / `HowToConfirmSelection`**: the supplied `Action` is executed when Space confirms an enabled selected entry. It is a callback hook, not a key-binding API.
- **`HowToEnterSelection`**: the supplied `Action` is executed when Enter processes the selected entry.

The runtime detects keyboard input and invokes the matching configured callback. The callback body is the behavior to execute when that event occurs; it does not tell the runtime which key should trigger the event. The `HowToXxx` callbacks are optional and do not replace built-in navigation, confirmation, or action execution.

## 7. Navigation API

Namespace: `cmd.interactive.menuscore.Runtime`

`MenuNavigation` is scoped to one bus.

| Member | Description |
|---|---|
| `CurrentMenu` | Currently selected `IMenu`, or `null`. |
| `CurrentMenuId` | Current menu ID, or `null`. |
| `CanGoBack` | Whether the history list currently contains entries. |
| `NavigationHistory` | Snapshot of the navigation history. |
| `NavigateTo(string menuId)` | Navigates to a registered menu; adds the previous menu to history when changing menus. |
| `GoBack()` | Pops history until a still-registered menu is found; returns `NoHistory` if none remains. |
| `GoToParent()` | Navigates to `ParentId`; returns `AlreadyAtRoot` when there is no parent ID. |
| `GoToRoot()` | Walks parent links to the root and navigates there. Returns `TargetNotFound` when there is no current menu. |

### `MenuNavigationResult`

Values:

- `Navigated`: navigation succeeded.
- `AlreadyAtRoot`: there is no parent to navigate to.
- `NoHistory`: no usable history entry exists.
- `TargetNotFound`: the target menu was not registered or no current menu is available for the operation.

`GoToParent()` and `GoToRoot()` use `NavigateTo()` for the final navigation, so successful changes can add the previous menu to history.

## 8. End-to-end example

The following example follows the public API and registration order present in this release:

```csharp
using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Builders;
using cmd.interactive.menuscore.Discovery;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Interactions;
using cmd.interactive.menuscore.Runtime;

var bus = FrameworksBus.RegisterBus("ExampleClient");

MenuBuilder.ParentMenuBuilder()
    .ForClient("ExampleClient")
    .ID("menucore")
    .Title("Main Menu")
    .SupportChildMenu(true)
    .MenuSelectionStyle(MenuSelectionStyle.SquareBrackets)
    .ExtraText("Use the arrow keys to navigate.")
    .UpIcon("↑")
    .SeperatorUpDownIcon("/")
    .DownIcon("↓")
    .EnterIcon("↵")
    .BackIcon("Esc")
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

// Optional discovery of IMenu implementations from loaded assemblies.
IMenuDiscovery.StartDiscovery(bus);

bus.PARENT_MENU_REGISTER.register(bus, "menucore");
bus.CHILD_MENU_REGISTER.register(bus, "menucore.adv");
bus.INTERACTION_TEXT_REGISTER.register(bus, "open-advanced");
bus.INTERACTION_REGISTER.register(bus, "open-advanced");

ParentMenu.ShowMenu(bus, "menucore")
    .ClearConsole(false)
    .Run();
```

This example declares an explicit navigation action for the `Open Advanced` entry. The runtime also contains child-menu resolution for entries whose `InteractionID` matches a direct child's full ID or local ID; that path is separate from the action-based example above.

## 9. Version 0.1.0 implementation notes

- The project file declares package version `0.1.0`.
- The project targets `net10.0`, `net8.0`, and `net9.0`.
- `ConditionVisible(false)` disables/dims an entry instead of removing it from the display.
- The `HowToConfirSelection` spelling is present in the source, and `HowToConfirmSelection` is an alias.
- Registry registration is explicit after builder declaration.
- Menu definitions may be client-scoped or global through `ForClient`.
- The public `MenuRegistry.Register` rejects duplicate menu IDs; discovery uses an internal replacement method.
- Selection callbacks are optional. The built-in input loop handles keyboard movement and confirmation; callback invocation is an additional behavior defined by the runtime implementation.


## 10. Complete API inventory

This inventory lists the public members exposed by the version `0.1.0` source. It complements the earlier conceptual explanations.

### `FrameworksBus`

| Member | Signature | Result |
|---|---|---|
| `RegisterBus` | `RegisterBus(string clientName)` | Creates a bus for the specified client. |
| `RegisterBus` | `RegisterBus(FrameworksBus bus, string clientName)` | Validates and returns the same bus instance. |
| `ClientName` | `string ClientName { get; }` | The immutable client name. |
| `PARENT_MENU_REGISTER` | `ParentMenuRegistry` | Registry for parent menu definitions. |
| `CHILD_MENU_REGISTER` | `ChildMenuRegistry` | Registry for child menu definitions. |
| `INTERACTION_REGISTER` | `InteractionRegistry` | Registry for actions. |
| `INTERACTION_TEXT_REGISTER` | `InteractionTextRegistry` | Registry for visible text entries. |
| `Menus` | `MenuRegistry` | Per-bus menu index. |
| `Navigation` | `MenuNavigation` | Per-bus navigation controller. |

### `MenuRegistry`

| Member | Signature | Result |
|---|---|---|
| `Count` | `int Count { get; }` | Number of registered menus. |
| `Menus` | `IReadOnlyCollection<IMenu> Menus { get; }` | Snapshot of registered menus. |
| `Register` | `void Register(IMenu menu)` | Adds a menu; duplicate IDs throw. |
| `TryGet` | `bool TryGet(string id, out IMenu? menu)` | Looks up a menu without throwing for a missing ID. |
| `GetRequired` | `IMenu GetRequired(string id)` | Returns the menu or throws `KeyNotFoundException`. |
| `Contains` | `bool Contains(string id)` | Checks whether an ID is registered. |
| `Unregister` | `bool Unregister(string id)` | Removes a menu and reports whether it existed. |
| `Clear` | `void Clear()` | Removes all menus from this bus registry. |

### `ParentMenuRegistry` and `ChildMenuRegistry`

Both expose `register(FrameworksBus bus, string id)` and `RegisteredIds`.

- `ParentMenuRegistry.register` resolves a parent definition using the bus client name and registers it in `bus.Menus`.
- `ChildMenuRegistry.register` resolves a child definition, requires its parent to already exist in `bus.Menus`, and requires that parent's `SupportChildMenu` to be `true`.
- Re-registering an ID already held by the same registry returns without adding it again.
- These registry instances are bus-owned. A child registered on one bus does not automatically appear on another bus.

### `InteractionTextRegistry` and `InteractionRegistry`

Both expose `register(FrameworksBus bus, string interactionId)`.

- `InteractionTextRegistry` exposes `GetForMenu(string menuId)` internally to the runtime, not as a public API.
- `InteractionRegistry` exposes `TryGet(string menuId, string interactionId, out MenuInteractionDefinition? definition)` internally, not as a public API.
- Text and action registrations require the relevant menu to be registered on that bus before the registry call.
- Registry lookup combines menu ID and interaction ID. The same interaction ID may therefore be reused in different menus.

### `MenuNavigation`

| Member | Signature | Notes |
|---|---|---|
| Constructor | `MenuNavigation(FrameworksBus bus)` | Public constructor; throws `ArgumentNullException` for null. |
| `CurrentMenu` | `IMenu? CurrentMenu { get; }` | Current menu instance. |
| `CurrentMenuId` | `string? CurrentMenuId { get; }` | Convenience access to current ID. |
| `CanGoBack` | `bool CanGoBack { get; }` | True when history contains at least one entry. |
| `NavigationHistory` | `IReadOnlyList<string> NavigationHistory { get; }` | Snapshot of history IDs. |
| `NavigateTo` | `MenuNavigationResult NavigateTo(string menuId)` | Throws for null/blank IDs; returns `TargetNotFound` when not registered. |
| `GoBack` | `MenuNavigationResult GoBack()` | Pops history until it finds a menu still registered. |
| `GoToParent` | `MenuNavigationResult GoToParent()` | Returns `AlreadyAtRoot` when current menu has no parent. |
| `GoToRoot` | `MenuNavigationResult GoToRoot()` | Returns `TargetNotFound` if there is no current menu. |

### `ParentMenu` and `MenuDisplaySession`

```csharp
ParentMenu.ShowMenu(FrameworksBus bus, string menuId)
```

Returns a `MenuDisplaySession`.

`MenuDisplaySession` public API:

- `Navigation`: the `MenuNavigation` used by the session.
- `ClearConsole(bool clearConsole)`: sets the console-clearing option and returns the session for chaining.
- `Run()`: executes the interactive loop and returns `void`.

`Run()` can be called only once on a session. It starts by navigating to the initial menu ID. A missing initial menu results in `KeyNotFoundException`.

### Builder entry points

| API | Signature |
|---|---|
| Parent builder factory | `MenuBuilder.ParentMenuBuilder()` |
| Child builder factory | `MenuBuilder.ChildMenuBuilder()` |
| Parent builder direct factory | `ParentMenuBuilder.Create()` |
| Child builder direct factory | `ChildMenuBuilder.Create()` |
| Text declaration factory | `MenuInteractions.RegisterText()` |
| Action declaration factory | `MenuInteractions.RegisterInteraction()` |

Both menu builders also expose `title(string title)` as a lowercase alias for `Title(string title)`. Both expose `HowToConfirSelection(Action action)` and the correctly spelled alias `HowToConfirmSelection(Action action)`.

## 11. Recommended usage workflow

The following order reflects the actual dependencies in the implementation.

### Step 1: Create a client bus

```csharp
var bus = FrameworksBus.RegisterBus("MyApplication");
```

Use a stable client name. It is the scope used when definitions are resolved for registration.

### Step 2: Declare the root menu

```csharp
var rootId = MenuBuilder.ParentMenuBuilder()
    .ForClient("MyApplication")
    .ID("main")
    .Title("Main Menu")
    .SupportChildMenu(true)
    .DefaultExitButton(true)
    .MenuSelectionStyle(MenuSelectionStyle.SquareBrackets)
    .Register();
```

`Register()` returns the menu ID. The definition is now in the internal definition catalog, but it is not yet in `bus.Menus`.

### Step 3: Declare child menus before registering them

```csharp
var childId = MenuBuilder.ChildMenuBuilder()
    .ForClient("MyApplication")
    .ChildID("settings")
    .MenuID(rootId)
    .Title("Settings")
    .Register();
```

This returns `main.settings`. The parent must be registered on the bus before the child can be registered on the bus.

### Step 4: Declare a visible entry and its action

```csharp
MenuInteractions.RegisterText()
    .ForClient("MyApplication")
    .ID(rootId)
    .Text("Open settings")
    .InteractionID("open-settings")
    .Register();

MenuInteractions.RegisterInteraction()
    .ForClient("MyApplication")
    .ID(rootId)
    .InteractionID("open-settings")
    .Action(() => bus.Navigation.NavigateTo(childId))
    .Register();
```

The text declaration and action declaration are separate. Their menu ID and interaction ID must match.

### Step 5: Register the definitions on the bus

```csharp
bus.PARENT_MENU_REGISTER.register(bus, rootId);
bus.CHILD_MENU_REGISTER.register(bus, childId);
bus.INTERACTION_TEXT_REGISTER.register(bus, "open-settings");
bus.INTERACTION_REGISTER.register(bus, "open-settings");
```

Register the parent first, then its child. Register the menu before registering its text and actions.

### Step 6: Start the menu session

```csharp
ParentMenu.ShowMenu(bus, rootId)
    .ClearConsole(false)
    .Run();
```

`ClearConsole(false)` keeps previous console output instead of calling `Console.Clear()` before each render.

### Conditional entries

```csharp
MenuInteractions.RegisterText()
    .ForClient("MyApplication")
    .ID(rootId)
    .Text("Restricted option")
    .InteractionID("restricted")
    .ConditionVisible(() => userCanAccessRestrictedOption)
    .Register();
```

The condition is evaluated while the runtime constructs the entry list. Returning `false` disables and dims the entry; it does not remove the entry from the menu. If the condition throws, the runtime treats it as disabled.

### Menu-specific appearance

```csharp
MenuBuilder.ParentMenuBuilder()
    .ForClient("MyApplication")
    .ID("colored")
    .Title("Colored Menu")
    .MenuSelectionStyle(MenuSelectionStyle.Custom)
    .CustomSelection(">>> ", " <<<")
    .ColorSelection("Cyan")
    .ColorConfirm("Green")
    .ColorNormal("Gray")
    .ExtraText("Additional information")
    .Register();
```

Color strings are interpreted as `ConsoleColor` names. Invalid names fall back to the relevant runtime fallback color rather than raising a parsing exception.

## 12. Practical error handling

The library exposes ordinary .NET exceptions for invalid arguments, invalid declaration order, missing definitions, and duplicate IDs. Handle these at the boundary where the application configures its menus, rather than hiding them inside the menu action code.

| Symptom / exception | Actual trigger in version 0.1.0 | Resolution |
|---|---|---|
| `ArgumentException` from `RegisterBus` | Client name is null, empty, or whitespace. | Supply a non-empty stable client name. |
| `InvalidOperationException` from `RegisterBus(bus, name)` | The supplied name differs from the bus's immutable `ClientName`. | Create a new bus for a different client identity. |
| `InvalidOperationException` from builder `Register()` | Required IDs or titles are absent/blank; a child ID is invalid; or a duplicate declaration exists in the same scope. | Set required values and ensure each declaration key is unique. |
| `ArgumentNullException` from callback setters | A callback argument is null. | Supply a non-null delegate. |
| `KeyNotFoundException` from parent registration | No parent definition exists for the requested ID and bus scope. | Call `ParentMenuBuilder...Register()` with that ID and the correct `ForClient` scope first. |
| `KeyNotFoundException` from child registration | No child definition exists for the requested full child ID and scope. | Call `ChildMenuBuilder...Register()` first and use its returned full ID. |
| `InvalidOperationException` from child registration | The parent is not registered on this bus, or does not support child menus. | Register the parent first and set `SupportChildMenu(true)`. |
| `KeyNotFoundException` from text/action registration | No matching text/action definition is available for the requested interaction ID on a registered menu. | Register the owning menu first, then declare the text/action with matching menu and interaction IDs and the correct client scope. |
| `ArgumentException` from `MenuRegistry.Register` | The menu ID or title is blank. | Supply non-empty `Id` and `Title`. |
| `InvalidOperationException` from `MenuRegistry.Register` | A menu with the same ID is already registered on that bus. | Avoid duplicate IDs or explicitly remove the existing entry before registering another. |
| `ArgumentNullException` from registry lookups | A null ID is passed to `TryGet`, `Contains`, or similar guarded API. | Pass a non-null ID. |
| `KeyNotFoundException` from `GetRequired` or `MenuDisplaySession.Run` | The requested menu ID is not registered on the bus. | Check the ID and confirm that the correct registration calls completed on the same bus. |
| `InvalidOperationException` from `MenuDisplaySession.Run` | `Run()` is called more than once on the same session. | Create a new session for each run. |
| A menu item appears dimmed and cannot be selected | Its `ConditionVisible` callback returned `false` or threw an exception. | Check the condition and any exception it raises. The runtime intentionally treats a throwing condition as disabled. |
| The menu appears but has no options | No text entries were registered for that menu, and/or `ConditionVisible` disabled all entries while the default exit entry is off. | Verify text registration and conditions; enable `DefaultExitButton(true)` if an Exit entry is desired. |
| An action does not run | The entry has no matching action in `INTERACTION_REGISTER`, or the entry instead resolves to a direct child menu. | Check the menu ID and `InteractionID` pair, registration order, and whether the ID matches a child menu. |

### Catch configuration failures close to startup

```csharp
try
{
    bus.PARENT_MENU_REGISTER.register(bus, "main");
    bus.INTERACTION_TEXT_REGISTER.register(bus, "open-settings");
    bus.INTERACTION_REGISTER.register(bus, "open-settings");
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Invalid menu configuration: {ex.Message}");
    throw;
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine($"Menu registration order or configuration is invalid: {ex.Message}");
    throw;
}
catch (KeyNotFoundException ex)
{
    Console.Error.WriteLine($"A required menu or interaction definition is missing: {ex.Message}");
    throw;
}
```

The example rethrows the exception after logging so the application does not silently continue with a partially registered menu system. Applications may choose a different recovery policy, but should not silently swallow registration failures.

### Runtime-specific behavior and limitations

- Exceptions thrown by `ConditionVisible` are caught by the runtime and converted to a disabled entry.
- Exceptions thrown by registered action delegates and configured `HowToXxx` callbacks are not caught by the display loop; they can propagate to the caller of `Run()`.
- `Console.ReadKey(true)`, `Console.Clear()`, and console color operations rely on a usable console environment. The library does not wrap every console I/O failure in a library-specific exception.
- `GoBack()` discards history entries whose menu IDs are no longer registered and continues looking for a valid target.
- The runtime maintains selection indexes by menu ID during one display session. This is separate from the navigation history.
- `ConditionVisible` is evaluated while entries are built for a render iteration, not continuously while the user is idle.
- `HowToSelectionUp` and `HowToSelectionDown` are callbacks in addition to built-in selection movement. `HowToSelectSelection` is invoked after movement. The runtime invokes `ConfirmSelection` when an enabled entry is confirmed, then handles Exit, child-menu resolution, or action execution. `EnterSelection` is invoked on the child-navigation path and on the ordinary action path.

## 13. Diagnostic checklist

When a menu does not behave as expected, check these items in order:

1. Is the same `FrameworksBus` instance used for registration and display?
2. Does the bus's `ClientName` match the `ForClient(...)` scope used by the declarations?
3. Did each builder's `Register()` return the ID actually used by the registry call?
4. Was the parent registered before its child?
5. Was the menu registered before its interaction text and action?
6. Do the visible entry and action use the same menu ID and interaction ID?
7. Does the `ConditionVisible` callback return `true`?
8. Does the `InteractionID` accidentally match a direct child menu ID, causing navigation instead of action execution?
9. Is `DefaultExitButton` enabled, and is the selected row the automatically appended `Exit` entry?
10. Is the same `MenuDisplaySession` being run more than once?

</details>

## Future Versions

Add a new collapsible version section for each subsequent release. Document only APIs and behaviors available in that release, and record changes relative to earlier versions there. Move material into **Shared Version** only when it is genuinely stable across releases.
