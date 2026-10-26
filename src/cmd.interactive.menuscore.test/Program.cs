
using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Builders;
using cmd.interactive.menuscore.Discovery;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Interactions;
using cmd.interactive.menuscore.Runtime;

const string Client = "ExampleClient";

var bus = FrameworksBus.RegisterBus(Client);

int actionCounter = 0;
int diagnosticCounter = 0;
int deepCounter = 0;
bool featureEnabled = false;
bool diagnosticsUnlocked = false;

// ============================================================
// 1. MENU TREE
// ============================================================

MenuBuilder.ParentMenuBuilder()
    .ForClient(Client)
    .ID("menucore")
    .Title("Main Menu - Stress Test")
    .SupportChildMenu(true)
    .MenuSelectionStyle(MenuSelectionStyle.SquareBrackets)
    .ExtraText("Stress test: navigation, nested menus, actions and state.")
    .DefaultExitButton(true)
    .ColorSelection("Yellow")
    .ColorConfirm("Green")
    .ColorNormal("Gray")
    .Register();

MenuBuilder.ChildMenuBuilder()
    .ForClient(Client)
    .ChildID("adv")
    .MenuID("menucore")
    .Title("Advanced")
    .MenuSelectionStyle(MenuSelectionStyle.Arrow)
    .ExtraText("Nested menu level 1")
    .DefaultExitButton(true)
    .ColorSelection("Cyan")
    .ColorConfirm("Green")
    .ColorNormal("Gray")
    .Register();

MenuBuilder.ChildMenuBuilder()
    .ForClient(Client)
    .ChildID("actions")
    .MenuID("menucore")
    .Title("Action Center")
    .MenuSelectionStyle(MenuSelectionStyle.SquareBrackets)
    .ExtraText("Run actions repeatedly to test state changes.")
    .DefaultExitButton(true)
    .Register();

MenuBuilder.ChildMenuBuilder()
    .ForClient(Client)
    .ChildID("diagnostics")
    .MenuID("menucore.adv")
    .Title("Diagnostics")
    .ExtraText("Nested menu level 2")
    .DefaultExitButton(true)
    .Register();

MenuBuilder.ChildMenuBuilder()
    .ForClient(Client)
    .ChildID("settings")
    .MenuID("menucore.adv")
    .Title("Settings")
    .ExtraText("Toggle the feature to unlock diagnostics.")
    .DefaultExitButton(true)
    .Register();

MenuBuilder.ChildMenuBuilder()
    .ForClient(Client)
    .ChildID("deep")
    .MenuID("menucore.adv.diagnostics")
    .Title("Deep Diagnostics")
    .ExtraText("Nested menu level 3")
    .DefaultExitButton(true)
    .Register();

// ============================================================
// 2. MAIN MENU ENTRIES
// ============================================================

RegisterText("menucore", "Open Advanced", "adv", "Cyan");
RegisterText("menucore", "Open Action Center", "actions", "Yellow");

RegisterText(
    "menucore",
    "Show Stress Test Status",
    "main-status",
    "Magenta");

RegisterAction("menucore", "main-status", () =>
{
    Console.WriteLine();
    Console.WriteLine("=== STRESS TEST STATUS ===");
    Console.WriteLine($"Action counter: {actionCounter}");
    Console.WriteLine($"Diagnostic counter: {diagnosticCounter}");
    Console.WriteLine($"Deep counter: {deepCounter}");
    Console.WriteLine($"Feature enabled: {featureEnabled}");
    Console.WriteLine($"Diagnostics unlocked: {diagnosticsUnlocked}");
});

// ============================================================
// 3. ACTION CENTER
// ============================================================

RegisterText("menucore.actions", "Increment Counter", "increment", "Green");

RegisterAction("menucore.actions", "increment", () =>
{
    actionCounter++;
    Console.WriteLine($"Action counter incremented: {actionCounter}");
});

RegisterText("menucore.actions", "Increment Counter x10", "increment-10", "Yellow");

RegisterAction("menucore.actions", "increment-10", () =>
{
    actionCounter += 10;
    Console.WriteLine($"Action counter increased by 10: {actionCounter}");
});

RegisterText("menucore.actions", "Toggle Feature", "toggle-feature", "Cyan");

RegisterAction("menucore.actions", "toggle-feature", () =>
{
    featureEnabled = !featureEnabled;
    diagnosticsUnlocked = featureEnabled;

    Console.WriteLine($"Feature enabled: {featureEnabled}");
});

RegisterText(
    "menucore.actions",
    "Conditional Entry: Visible When Enabled",
    "conditional-action",
    "Green",
    () => featureEnabled);

RegisterAction("menucore.actions", "conditional-action", () =>
{
    Console.WriteLine("Conditional action executed.");
});

// ============================================================
// 4. ADVANCED MENU
// ============================================================

RegisterText("menucore.adv", "Open Diagnostics", "diagnostics", "Cyan");
RegisterText("menucore.adv", "Open Settings", "settings", "Yellow");

RegisterText(
    "menucore.adv",
    "Run Diagnostic Action",
    "diagnostic-action",
    "Green");

RegisterAction("menucore.adv", "diagnostic-action", () =>
{
    diagnosticCounter++;
    Console.WriteLine($"Diagnostic action executed: {diagnosticCounter}");
});

// ============================================================
// 5. DIAGNOSTICS
// ============================================================

RegisterText("menucore.adv.diagnostics", "Open Deep Diagnostics", "deep", "Cyan");

RegisterText(
    "menucore.adv.diagnostics",
    "Increment Diagnostic Counter",
    "diagnostic-increment",
    "Green");

RegisterAction("menucore.adv.diagnostics", "diagnostic-increment", () =>
{
    diagnosticCounter++;
    Console.WriteLine($"Diagnostic counter: {diagnosticCounter}");
});

RegisterText(
    "menucore.adv.diagnostics",
    "Run Repeated Diagnostic x100",
    "diagnostic-100",
    "Yellow");

RegisterAction("menucore.adv.diagnostics", "diagnostic-100", () =>
{
    for (int i = 0; i < 100; i++)
        diagnosticCounter++;

    Console.WriteLine($"100 diagnostic actions completed. Total: {diagnosticCounter}");
});

RegisterText(
    "menucore.adv.diagnostics",
    "Unlocked Diagnostic",
    "unlocked-diagnostic",
    "Green",
    () => diagnosticsUnlocked);


RegisterAction("menucore.adv.diagnostics", "unlocked-diagnostic", () =>
{
    Console.WriteLine("Unlocked diagnostic executed.");
});

// ============================================================
// 6. DEEP DIAGNOSTICS
// ============================================================

RegisterText(
    "menucore.adv.diagnostics.deep",
    "Increment Deep Counter",
    "deep-increment",
    "Green");

RegisterAction("menucore.adv.diagnostics.deep", "deep-increment", () =>
{
    deepCounter++;
    Console.WriteLine($"Deep counter: {deepCounter}");
});

RegisterText(
    "menucore.adv.diagnostics.deep",
    "Stress Loop x1000",
    "deep-stress",
    "Yellow");

RegisterAction("menucore.adv.diagnostics.deep", "deep-stress", () =>
{
    for (int i = 0; i < 1000; i++)
        deepCounter++;

    Console.WriteLine($"1000 iterations completed. Deep counter: {deepCounter}");
});

// ============================================================
// 7. SETTINGS
// ============================================================

RegisterText("menucore.adv.settings", "Enable Feature", "enable-feature", "Green");

RegisterAction("menucore.adv.settings", "enable-feature", () =>
{
    featureEnabled = true;
    diagnosticsUnlocked = true;
    Console.WriteLine("Feature enabled.");
});

RegisterText("menucore.adv.settings", "Disable Feature", "disable-feature", "Red");

RegisterAction("menucore.adv.settings", "disable-feature", () =>
{
    featureEnabled = false;
    diagnosticsUnlocked = false;
    Console.WriteLine("Feature disabled.");
});

RegisterText(
    "menucore.adv.settings",
    "Current State",
    "settings-status",
    "Magenta");

RegisterAction("menucore.adv.settings", "settings-status", () =>
{
    Console.WriteLine($"Feature enabled: {featureEnabled}");
    Console.WriteLine($"Diagnostics unlocked: {diagnosticsUnlocked}");
});

// ============================================================
// 8. REGISTER MENUS ON THE BUS
// Parent must be registered before its children.
// ============================================================

bus.PARENT_MENU_REGISTER.register(bus, "menucore");

bus.CHILD_MENU_REGISTER.register(bus, "menucore.adv");
bus.CHILD_MENU_REGISTER.register(bus, "menucore.actions");

bus.CHILD_MENU_REGISTER.register(bus, "menucore.adv.diagnostics");
bus.CHILD_MENU_REGISTER.register(bus, "menucore.adv.settings");

bus.CHILD_MENU_REGISTER.register(bus, "menucore.adv.diagnostics.deep");

// Discovery is part of the library lifecycle.
IMenuDiscovery.StartDiscovery(bus);

// Register all interaction text IDs.
string[] textIds =
[
    "adv",
    "actions",
    "main-status",
    "increment",
    "increment-10",
    "toggle-feature",
    "conditional-action",
    "diagnostics",
    "settings",
    "diagnostic-action",
    "deep",
    "diagnostic-increment",
    "diagnostic-100",
    "unlocked-diagnostic",
    "deep-increment",
    "deep-stress",
    "enable-feature",
    "disable-feature",
    "settings-status"
];

foreach (var id in textIds)
    bus.INTERACTION_TEXT_REGISTER.register(bus, id);

// Register all interaction action IDs.
string[] interactionIds =
[
    "main-status",
    "increment",
    "increment-10",
    "toggle-feature",
    "conditional-action",
    "diagnostic-action",
    "diagnostic-increment",
    "diagnostic-100",
    "unlocked-diagnostic",
    "deep-increment",
    "deep-stress",
    "enable-feature",
    "disable-feature",
    "settings-status"
];

foreach (var id in interactionIds)
    bus.INTERACTION_REGISTER.register(bus, id);

Console.WriteLine("CMD Interactive Library Stress Test");
Console.WriteLine($"Client bus: {bus.ClientName}");
Console.WriteLine($"Registered menus: {bus.Menus.Count}");
Console.WriteLine("Use Space to confirm and Enter to execute.");
Console.WriteLine("Press any key to open the main menu...");
Console.ReadKey(true);

ParentMenu.ShowMenu(bus, "menucore")
    .ClearConsole(true)
    .Run();

// ============================================================
// LOCAL REGISTRATION HELPERS
// ============================================================

static void RegisterText(
    string menuId,
    string text,
    string interactionId,
    string color,
    Func<bool>? condition = null)
{
    var builder = MenuInteractions.RegisterText()
        .ForClient(Client)
        .ID(menuId)
        .Text(text)
        .InteractionID(interactionId)
        .Color(color);

    if (condition is not null)
    {
        builder.ConditionVisible(condition);
    }

    builder.Register();
}

void RegisterAction(
    string menuId,
    string interactionId,
    Action action)
{
    MenuInteractions.RegisterInteraction()
        .ForClient(Client)
        .ID(menuId)
        .InteractionID(interactionId)
        .Action(action)
        .Register();
}
