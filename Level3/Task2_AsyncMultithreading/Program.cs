// ============================================================
//  Program.cs — Entry Point
//  Codveda Internship · Level 3 · Task 2
//  Asynchronous Programming & Multithreading
// ============================================================

using CodvedaAsync.AsyncAwait;
using CodvedaAsync.Helpers;
using CodvedaAsync.RaceConditions;
using CodvedaAsync.Services;
using CodvedaAsync.ThreadManagement;
using CodvedaAsync.TPL;

Console.OutputEncoding = System.Text.Encoding.UTF8;

ConsoleHelper.Header(
    "Codveda Internship · Level 3 · Task 2",
    "Asynchronous Programming & Multithreading");

Console.WriteLine(@"
  Objectives covered:
   ✔ Objective 1 — async/await for non-blocking execution
   ✔ Objective 2 — Task Parallel Library (TPL)
   ✔ Objective 3 — ThreadPool & BackgroundWorker
   ✔ Objective 4 — Race conditions & deadlocks
");

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("  Press any key to start...");
Console.ResetColor();
Console.ReadKey(intercept: true);

// ── Run all objectives in sequence ────────────────────────
await AsyncAwaitDemo.RunAsync();

Console.WriteLine("\n  Press any key for Objective 2 (TPL)...");
Console.ReadKey(intercept: true);

await TPLDemo.RunAsync();

Console.WriteLine("\n  Press any key for Objective 3 (ThreadPool)...");
Console.ReadKey(intercept: true);

await ThreadManagementDemo.RunAsync();

Console.WriteLine("\n  Press any key for Objective 4 (Race Conditions)...");
Console.ReadKey(intercept: true);

await RaceConditionDemo.RunAsync();

Console.WriteLine("\n  Press any key for Real-World Demo (all combined)...");
Console.ReadKey(intercept: true);

await RealWorldDemo.RunAsync();

// ── Done ───────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✅  All objectives complete!                        ║");
Console.WriteLine("║                                                      ║");
Console.WriteLine("║  Concepts demonstrated:                              ║");
Console.WriteLine("║   • async/await, Task<T>, ValueTask                 ║");
Console.WriteLine("║   • IAsyncEnumerable, CancellationToken             ║");
Console.WriteLine("║   • Parallel.For/ForEach/Invoke, PLINQ              ║");
Console.WriteLine("║   • Task.Run, ContinueWith, WhenAll, WhenAny       ║");
Console.WriteLine("║   • Thread, ThreadPool, BackgroundWorker            ║");
Console.WriteLine("║   • ThreadLocal<T>, PeriodicTimer                   ║");
Console.WriteLine("║   • lock, Monitor, Interlocked, Mutex               ║");
Console.WriteLine("║   • SemaphoreSlim, ReaderWriterLockSlim             ║");
Console.WriteLine("║   • ConcurrentDictionary/Queue/Bag/BlockingCollection║");
Console.WriteLine("║   • System.Threading.Channels                       ║");
Console.WriteLine("╚══════════════════════════════════════════════════════╝");
Console.ResetColor();

Console.WriteLine("\n  Press any key to exit...");
Console.ReadKey(intercept: true);