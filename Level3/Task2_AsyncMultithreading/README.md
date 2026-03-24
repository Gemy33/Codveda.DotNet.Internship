# ⚡ Codveda .NET Internship — Level 3 | Task 2
## Asynchronous Programming & Multithreading

> **Intern:** [Your Name]
> **Company:** [Codveda Technology](https://www.codveda.com)
> **Domain:** .NET Development | **Level:** 3 (Advanced)

---

## 📋 Task Objectives

| # | Objective | Status |
|---|-----------|--------|
| 1 | Implement async/await for non-blocking code execution | ✅ |
| 2 | Use Task Parallel Library (TPL) for concurrent programming | ✅ |
| 3 | Manage threads using ThreadPool and BackgroundWorker | ✅ |
| 4 | Handle race conditions and deadlocks efficiently | ✅ |

---

## 📁 Project Structure

```
Task2_AsyncMultithreading/
│
├── AsyncAwait/
│   └── AsyncAwaitDemo.cs        ← async/await, Task<T>, ValueTask,
│                                   IAsyncEnumerable, CancellationToken
├── TPL/
│   └── TPLDemo.cs               ← Parallel.For/ForEach/Invoke, PLINQ,
│                                   Task.Run, ContinueWith, WhenAll/WhenAny
├── ThreadManagement/
│   └── ThreadManagementDemo.cs  ← Thread class, ThreadPool, BackgroundWorker,
│                                   ThreadLocal<T>, PeriodicTimer
├── RaceConditions/
│   └── RaceConditionDemo.cs     ← Race conditions, lock, Monitor, Interlocked,
│                                   SemaphoreSlim, ReaderWriterLockSlim,
│                                   Deadlock prevention, Thread-safe collections,
│                                   System.Threading.Channels
├── Services/
│   └── RealWorldDemo.cs         ← Full async order pipeline (all concepts combined)
├── Models/
│   └── SharedModels.cs          ← Order, DownloadResult, WorkItem + ConsoleHelper
├── Program.cs                   ← Entry point — runs all objectives in sequence
└── CodvedaAsync.csproj
```

---

## 🗺️ Concepts Map

### ✅ Objective 1 — async/await

| Demo | Concept |
|---|---|
| 1a | `async`/`await` basics — non-blocking I/O |
| 1b | Sequential vs Parallel (`Task.WhenAll`) — performance comparison |
| 1c | Exception handling — `try/catch`, `AggregateException`, `Task.WhenAny` |
| 1d | `CancellationToken` — graceful cancellation + linked tokens |
| 1e | `ValueTask` — zero-allocation async for hot paths |
| 1f | `IAsyncEnumerable` — async streaming with `await foreach` |

```csharp
// Sequential — 1200ms total
var r1 = await CallService("A", 400);   // wait 400ms
var r2 = await CallService("B", 300);   // then wait 300ms
var r3 = await CallService("C", 500);   // then wait 500ms

// Parallel — ~500ms total (limited by slowest)
var t1 = CallService("A", 400);
var t2 = CallService("B", 300);
var t3 = CallService("C", 500);
await Task.WhenAll(t1, t2, t3);   // all running concurrently
```

### ✅ Objective 2 — TPL

| Demo | Concept |
|---|---|
| 2a | `Parallel.For` with local state — safe accumulation |
| 2b | `Parallel.ForEach` + `Partitioner` — batch processing |
| 2c | `Parallel.Invoke` — independent actions in parallel |
| 2d | PLINQ — add `.AsParallel()` to any LINQ query |
| 2e | `Task.Run` — offload CPU work without blocking |
| 2f | `ContinueWith` — task pipelines and conditional continuations |

```csharp
// PLINQ — just add .AsParallel()
var result = numbers
    .AsParallel()
    .WithDegreeOfParallelism(Environment.ProcessorCount)
    .Where(n => n % 2 == 0)
    .Select(n => n * n)
    .Sum();
```

### ✅ Objective 3 — ThreadPool & BackgroundWorker

| Demo | Concept |
|---|---|
| 3a | `Thread` class — foreground vs background, priority, join |
| 3b | `ThreadPool.QueueUserWorkItem` — reusable thread pool |
| 3c | `BackgroundWorker` — `ReportProgress`, `CancelAsync`, `RunWorkerCompleted` |
| 3d | `ThreadLocal<T>` — per-thread isolated state |
| 3e | `PeriodicTimer` — modern async-friendly background scheduler |

```csharp
// BackgroundWorker with progress reporting
worker.DoWork += (sender, e) => {
    for (int i = 1; i <= 10; i++) {
        if (bw.CancellationPending) { e.Cancel = true; return; }
        Thread.Sleep(150);
        bw.ReportProgress(i * 10, $"Step {i} of 10");
    }
};
worker.ProgressChanged += (s, e) => Console.WriteLine($"{e.ProgressPercentage}%");
worker.RunWorkerAsync();
```

### ✅ Objective 4 — Race Conditions & Deadlocks

| Demo | Concept |
|---|---|
| 4a | Race condition demo + fix with `Interlocked` |
| 4b | `lock` keyword — mutual exclusion (BankAccount example) |
| 4c | `Monitor.Wait/Pulse` — producer/consumer |
| 4d | `SemaphoreSlim` — limit concurrent API calls |
| 4e | `ReaderWriterLockSlim` — concurrent reads, exclusive writes |
| 4f | Deadlock explanation + `Monitor.TryEnter` prevention |
| 4g | Thread-safe collections: `ConcurrentDictionary`, `ConcurrentQueue`, `BlockingCollection` |
| 4h | `System.Threading.Channels` — modern async pipelines |

```csharp
// ❌ Race condition
int counter = 0;
Parallel.For(0, 10000, _ => counter++);   // loses updates!

// ✔ Fix with Interlocked (atomic)
int counter = 0;
Parallel.For(0, 10000, _ => Interlocked.Increment(ref counter));  // always 10000

// ✔ Fix with lock
lock (_lockObj) { counter++; }
```

#### Deadlock Prevention Rules
```
1. Consistent lock ordering   — always acquire locks in the same order
2. Monitor.TryEnter(timeout)  — don't wait forever; handle failure
3. Minimize lock scope        — hold lock as briefly as possible
4. Avoid calls while locked   — never call unknown code inside a lock
5. Use async primitives       — SemaphoreSlim can't deadlock a thread
```

---

## 🚀 How to Run

```bash
cd Task2_AsyncMultithreading
dotnet run
```

Each objective runs in sequence — press any key to advance.

---

## 🛠️ Technologies Used

| Feature | .NET API |
|---|---|
| Async/Await | `Task`, `Task<T>`, `ValueTask`, `async`/`await` |
| Async Stream | `IAsyncEnumerable<T>`, `await foreach` |
| Cancellation | `CancellationToken`, `CancellationTokenSource` |
| Data Parallelism | `Parallel.For`, `Parallel.ForEach`, `Parallel.Invoke` |
| Parallel LINQ | `.AsParallel()`, `.WithDegreeOfParallelism()` |
| Thread Pool | `ThreadPool.QueueUserWorkItem`, `Task.Run` |
| Thread Management | `Thread`, `BackgroundWorker`, `ThreadLocal<T>` |
| Scheduling | `PeriodicTimer` |
| Synchronization | `lock`, `Monitor`, `Mutex`, `Interlocked` |
| Limiting | `SemaphoreSlim`, `ReaderWriterLockSlim` |
| Safe Collections | `ConcurrentDictionary`, `ConcurrentQueue`, `BlockingCollection` |
| Pipelines | `System.Threading.Channels` |

---

*Built with ❤️ for the Codveda Technology .NET Development Internship*
`#CodvedaJourney` `#CodvedaExperience` `#FutureWithCodveda`
