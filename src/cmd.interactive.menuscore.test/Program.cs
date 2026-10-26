
using System.Diagnostics;
using cmd.interactive.menuscore.Bus;
using cmd.interactive.menuscore.Builders;
using cmd.interactive.menuscore.Discovery;
using cmd.interactive.menuscore.Enums;
using cmd.interactive.menuscore.Interactions;
using cmd.interactive.menuscore.Runtime;

const string Client = "ExtremeStressClient";

var bus = FrameworksBus.RegisterBus(Client);
var stopwatch = Stopwatch.StartNew();

long totalActions = 0;
long totalNavigationCallbacks = 0;
long totalConfirmCallbacks = 0;
long totalEnterCallbacks = 0;
long totalSelectionChanges = 0;
long progressIterations = 0;
long errors = 0;

int counter = 0;
int diagnosticCounter = 0;
bool featureEnabled = false;
bool stressMode = false;

void Log(string message)
{
    Console.WriteLine(
        $"[{DateTime.Now:HH:mm:ss.fff}] {message}");
}

void TrackAction(string name, Action action)
{
    totalActions++;

    try
    {
        action();
    }
    catch (Exception ex)
    {
        errors++;
        Log($"ACTION FAILED: {name}: {ex.Message}");
    }
}

void RegisterText(
    string menuId,
    string text,
    string interactionId,
    string color = "Gray",
    Func<bool>? condition = null)
{
    var builder = MenuInteractions.RegisterText()
        .ForClient(Client)
        .ID(menuId)
        .Text(text)
        .InteractionID(interactionId)
        .Color(color);

    if (condition is not null)
        builder.ConditionVisible(condition);

    builder.Register();
}

void RegisterAction(
    string menuId,
    string id,
    Action action)
{
    MenuInteractions.RegisterInteraction()
        .ForClient(Client)
        .ID(menuId)
        .InteractionID(id)
        .Action(() => TrackAction(id, action))
        .Register();
}

void RegisterHowToCallbacks(
    Action up,
    Action down,
    Action changed,
    Action confirm,
    Action enter)
{
    // Các callback được kích hoạt bởi runtime
    // khi sự kiện tương ứng xảy ra.
    up();
    down();
    changed();
    confirm();
    enter();
}

// ============================================================
// 1. MENU TREE + ICONS + HOWTO CALLBACKS
// ============================================================

MenuBuilder.ParentMenuBuilder()
    .ForClient(Client)
    .ID("stress")
    .Title("EXTREME STRESS TEST")
    .SupportChildMenu()
    .MenuSelectionStyle(MenuSelectionStyle.SquareBrackets)
    .ExtraText("Navigation / callbacks / actions / progress / state")
    .DefaultExitButton(true)
    .UpIcon("W")
    .SeperatorUpDownIcon(" | ")
    .DownIcon("S")
    .EnterIcon("ENTER")
    .BackIcon("ESC")
    .ColorSelection("Yellow")
    .ColorConfirm("Green")
    .ColorNormal("Gray")
    .HowToSelectionUp(() => totalNavigationCallbacks++)
    .HowToSelectionDown(() => totalNavigationCallbacks++)
    .HowToSelectSelection(() => totalSelectionChanges++)
    .HowToConfirmSelection(() => totalConfirmCallbacks++)
    .HowToEnterSelection(() => totalEnterCallbacks++)
    .Register();

string[] childIds =
[
    "navigation",
    "actions",
    "progress",
    "diagnostics",
    "settings",
    "deep",
    "load"
];

foreach (var id in childIds)
{
    MenuBuilder.ChildMenuBuilder()
        .ForClient(Client)
        .ChildID(id)
        .MenuID("stress")
        .Title($"Stress / {id.ToUpperInvariant()}")
        .ExtraText($"Test section: {id}")
        .DefaultExitButton(true)
        .MenuSelectionStyle(MenuSelectionStyle.Arrow)
        .UpIcon("W")
        .SeperatorUpDownIcon(" / ")
        .DownIcon("S")
        .EnterIcon("GO")
        .BackIcon("ESC")
        .ColorSelection("Cyan")
        .ColorConfirm("Green")
        .ColorNormal("Gray")
        .HowToSelectionUp(() => totalNavigationCallbacks++)
        .HowToSelectionDown(() => totalNavigationCallbacks++)
        .HowToSelectSelection(() => totalSelectionChanges++)
        .HowToConfirmSelection(() => totalConfirmCallbacks++)
        .HowToEnterSelection(() => totalEnterCallbacks++)
        .Register();
}

// Nested menus
MenuBuilder.ChildMenuBuilder()
    .ForClient(Client)
    .ChildID("deepest")
    .MenuID("stress.deep")
    .Title("DEEPEST NESTED MENU")
    .ExtraText("Deep navigation stress")
    .DefaultExitButton(true)
    .UpIcon("UP")
    .SeperatorUpDownIcon(" <> ")
    .DownIcon("DOWN")
    .EnterIcon("OPEN")
    .BackIcon("RETURN")
    .HowToSelectionUp(() => totalNavigationCallbacks++)
    .HowToSelectionDown(() => totalNavigationCallbacks++)
    .HowToSelectSelection(() => totalSelectionChanges++)
    .HowToConfirmSelection(() => totalConfirmCallbacks++)
    .HowToEnterSelection(() => totalEnterCallbacks++)
    .Register();

// ============================================================
// 2. MENU ENTRIES
// ============================================================

RegisterText("stress", "Navigation Tests", "navigation", "Cyan");
RegisterText("stress", "Action Flood", "actions", "Yellow");
RegisterText("stress", "Progress Bar Lab", "progress", "Green");
RegisterText("stress", "Runtime Diagnostics", "diagnostics", "Magenta");
RegisterText("stress", "Settings / Conditions", "settings", "Cyan");
RegisterText("stress", "Deep Nesting", "deep", "Yellow");
RegisterText("stress", "Load Generator", "load", "Red");

RegisterText("stress", "Print Runtime Report", "report", "White");

RegisterAction("stress", "report", PrintReport);

// ============================================================
// 3. NAVIGATION TESTS
// ============================================================

RegisterText("stress.navigation", "Open Deepest Menu", "deepest", "Cyan");
RegisterText("stress.navigation", "Return Navigation Probe", "nav-probe");
RegisterText("stress.navigation", "Run Callback Burst x1000", "callback-burst", "Yellow");

RegisterAction("stress.navigation", "nav-probe", () =>
{
    Log("Navigation probe executed.");
});

RegisterAction("stress.navigation", "callback-burst", () =>
{
    for (int i = 0; i < 1000; i++)
    {
        totalNavigationCallbacks++;
        totalSelectionChanges++;
    }

    Log("Callback burst: 1000 iterations completed.");
});

RegisterText("stress.deep", "Open Deepest Menu", "deepest", "Cyan");
RegisterText("stress.deep.deepest", "Return to Root", "return-root", "Yellow");

RegisterAction("stress.deep.deepest", "return-root", () =>
{
    Log("Deepest menu action executed.");
});

// ============================================================
// 4. ACTION FLOOD
// ============================================================

RegisterText("stress.actions", "Increment Counter", "increment", "Green");
RegisterText("stress.actions", "Increment Counter x100", "increment100", "Yellow");
RegisterText("stress.actions", "Increment Counter x10000", "increment10000", "Red");
RegisterText("stress.actions", "Toggle Stress Mode", "toggle-stress", "Cyan");
RegisterText("stress.actions", "Throw Test Exception", "throw-test", "Magenta");

RegisterAction("stress.actions", "increment", () =>
{
    counter++;
    Log($"Counter = {counter}");
});

RegisterAction("stress.actions", "increment100", () =>
{
    counter += 100;
    Log($"Counter = {counter}");
});

RegisterAction("stress.actions", "increment10000", () =>
{
    for (int i = 0; i < 10_000; i++)
        counter++;

    Log($"Counter = {counter}");
});

RegisterAction("stress.actions", "toggle-stress", () =>
{
    stressMode = !stressMode;
    Log($"Stress mode = {stressMode}");
});

RegisterAction("stress.actions", "throw-test", () =>
{
    throw new InvalidOperationException(
        "Intentional exception for stress testing.");
});

// ============================================================
// 5. PROGRESS BAR
// ============================================================

RegisterText("stress.progress", "Progress 100 Steps", "progress100", "Green");
RegisterText("stress.progress", "Progress 1000 Steps", "progress1000", "Yellow");
RegisterText("stress.progress", "Progress 10000 Steps", "progress10000", "Red");

RegisterAction("stress.progress", "progress100", () => RunProgress(100));
RegisterAction("stress.progress", "progress1000", () => RunProgress(1000));
RegisterAction("stress.progress", "progress10000", () => RunProgress(10000));

void RunProgress(int total)
{
    var timer = Stopwatch.StartNew();

    for (int i = 1; i <= total; i++)
    {
        progressIterations++;

        int percent = (int)((long)i * 100 / total);
        int filled = percent / 2;

        Console.Write(
            $"\r[{new string('#', filled)}{new string('-', 50 - filled)}] " +
            $"{percent,3}% ({i:N0}/{total:N0})");

        if (stressMode && i % 100 == 0)
            Thread.Yield();
    }

    timer.Stop();
    Console.WriteLine();
    Log($"Progress completed: {total:N0} iterations in {timer.ElapsedMilliseconds:N0} ms.");
}

// ============================================================
// 6. RUNTIME DIAGNOSTICS
// ============================================================

RegisterText("stress.diagnostics", "Print Full Report", "diag-report", "Magenta");
RegisterText("stress.diagnostics", "Run Diagnostic Burst x5000", "diag-burst", "Yellow");
RegisterText("stress.diagnostics", "Toggle Feature", "diag-toggle", "Cyan");
RegisterText("stress.diagnostics", "Unlocked Diagnostic", "diag-unlocked", "Green",
    () => featureEnabled);

RegisterAction("stress.diagnostics", "diag-report", PrintReport);

RegisterAction("stress.diagnostics", "diag-burst", () =>
{
    for (int i = 0; i < 5000; i++)
        diagnosticCounter++;

    Log($"Diagnostic counter = {diagnosticCounter:N0}");
});

RegisterAction("stress.diagnostics", "diag-toggle", () =>
{
    featureEnabled = !featureEnabled;
    Log($"Feature enabled = {featureEnabled}");
});

RegisterAction("stress.diagnostics", "diag-unlocked", () =>
{
    Log("Conditional diagnostic executed.");
});

// ============================================================
// 7. SETTINGS + DYNAMIC VISIBILITY
// ============================================================

RegisterText("stress.settings", "Enable Feature", "enable-feature", "Green");
RegisterText("stress.settings", "Disable Feature", "disable-feature", "Red");
RegisterText("stress.settings", "Conditional Entry", "conditional-setting", "Yellow",
    () => featureEnabled);

RegisterAction("stress.settings", "enable-feature", () =>
{
    featureEnabled = true;
    Log("Feature enabled.");
});

RegisterAction("stress.settings", "disable-feature", () =>
{
    featureEnabled = false;
    Log("Feature disabled.");
});

RegisterAction("stress.settings", "conditional-setting", () =>
{
    Log($"Conditional entry executed. Feature = {featureEnabled}");
});

// ============================================================
// 8. LOAD GENERATOR
// ============================================================

RegisterText("stress.load", "Run 100000 Counter Operations", "load100k", "Yellow");
RegisterText("stress.load", "Run 1000000 Counter Operations", "load1m", "Red");
RegisterText("stress.load", "Run Mixed Workload", "mixed-load", "Magenta");

RegisterAction("stress.load", "load100k", () => RunLoad(100_000));
RegisterAction("stress.load", "load1m", () => RunLoad(1_000_000));

RegisterAction("stress.load", "mixed-load", () =>
{
    var timer = Stopwatch.StartNew();

    for (int i = 0; i < 100_000; i++)
    {
        counter++;
        diagnosticCounter++;

        if (i % 10 == 0)
            progressIterations++;
    }

    timer.Stop();
    Log($"Mixed workload completed in {timer.ElapsedMilliseconds:N0} ms.");
});

void RunLoad(int iterations)
{
    var timer = Stopwatch.StartNew();

    for (int i = 0; i < iterations; i++)
        counter++;

    timer.Stop();

    Log($"Load complete: {iterations:N0} iterations, " +
        $"{timer.ElapsedMilliseconds:N0} ms.");
}

// ============================================================
// 9. REGISTER MENUS IN PARENT-FIRST ORDER
// ============================================================

bus.PARENT_MENU_REGISTER.register(bus, "stress");

foreach (var id in childIds)
    bus.CHILD_MENU_REGISTER.register(bus, $"stress.{id}");

bus.CHILD_MENU_REGISTER.register(bus, "stress.deep.deepest");

IMenuDiscovery.StartDiscovery(bus);

// Register all text definitions.
string[] textIds =
[
    "navigation", "actions", "progress", "diagnostics",
    "settings", "deep", "load", "report",
    "deepest", "nav-probe", "callback-burst",
    "return-root",
    "increment", "increment100", "increment10000",
    "toggle-stress", "throw-test",
    "progress100", "progress1000", "progress10000",
    "diag-report", "diag-burst", "diag-toggle", "diag-unlocked",
    "enable-feature", "disable-feature", "conditional-setting",
    "load100k", "load1m", "mixed-load"
];

foreach (var id in textIds)
    bus.INTERACTION_TEXT_REGISTER.register(bus, id);

// Register action definitions.
string[] actionIds =
[
    "report", "nav-probe", "callback-burst", "return-root",
    "increment", "increment100", "increment10000",
    "toggle-stress", "throw-test",
    "progress100", "progress1000", "progress10000",
    "diag-report", "diag-burst", "diag-toggle", "diag-unlocked",
    "enable-feature", "disable-feature", "conditional-setting",
    "load100k", "load1m", "mixed-load"
];

foreach (var id in actionIds)
    bus.INTERACTION_REGISTER.register(bus, id);

// ============================================================
// 10. START
// ============================================================

void PrintReport()
{
    Console.WriteLine();
    Console.WriteLine("========== EXTREME STRESS REPORT ==========");
    Console.WriteLine($"Elapsed:                {stopwatch.Elapsed}");
    Console.WriteLine($"Registered menus:       {bus.Menus.Count}");
    Console.WriteLine($"Actions executed:       {totalActions:N0}");
    Console.WriteLine($"Navigation callbacks:   {totalNavigationCallbacks:N0}");
    Console.WriteLine($"Selection callbacks:    {totalSelectionChanges:N0}");
    Console.WriteLine($"Confirm callbacks:      {totalConfirmCallbacks:N0}");
    Console.WriteLine($"Enter callbacks:        {totalEnterCallbacks:N0}");
    Console.WriteLine($"Progress iterations:    {progressIterations:N0}");
    Console.WriteLine($"Counter:                {counter:N0}");
    Console.WriteLine($"Diagnostic counter:     {diagnosticCounter:N0}");
    Console.WriteLine($"Feature enabled:        {featureEnabled}");
    Console.WriteLine($"Stress mode:            {stressMode}");
    Console.WriteLine($"Caught action errors:   {errors:N0}");
    Console.WriteLine("===========================================");
}

Console.WriteLine("CMD INTERACTIVE MENUSCORE");
Console.WriteLine("EXTREME STRESS TEST");
Console.WriteLine($"Client: {bus.ClientName}");
Console.WriteLine($"Menus registered: {bus.Menus.Count}");
Console.WriteLine("Press any key to start...");
Console.ReadKey(true);

ParentMenu.ShowMenu(bus, "stress")
    .ClearConsole(true)
    .Run();

Console.WriteLine();
PrintReport();
