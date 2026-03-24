// ============================================================
//  ThreadManagement/ThreadManagementDemo.cs
//  OBJECTIVE 3: Manage threads with ThreadPool & BackgroundWorker
//
//  Covers:
//   ✔ Thread class — creation, naming, priority, join
//   ✔ ThreadPool.QueueUserWorkItem — pool-based threading
//   ✔ ThreadPool size tuning and monitoring
//   ✔ BackgroundWorker — progress reporting & cancellation
//   ✔ Thread-local storage (ThreadLocal<T>)
//   ✔ Timer vs PeriodicTimer
// ============================================================

using CodvedaAsync.Helpers;
using CodvedaAsync.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace CodvedaAsync.ThreadManagement
{
    public static class ThreadManagementDemo
    {
        public static async Task RunAsync()
        {
            ConsoleHelper.Header(
                "OBJECTIVE 3: ThreadPool & BackgroundWorker",
                "Thread lifecycle management");

            Demo1_ThreadClass();
            Demo2_ThreadPool();
            Demo3_BackgroundWorker();
            Demo4_ThreadLocal();
            await Demo5_PeriodicTimerAsync();
        }

        // ════════════════════════════════════════════════════
        //  Demo 1 — Thread class (explicit thread management)
        // ════════════════════════════════════════════════════
        static void Demo1_ThreadClass()
        {
            ConsoleHelper.Section("3a. Thread Class — Explicit Thread Management");

            // ── Foreground thread ──────────────────────────
            var thread1 = new Thread(() =>
            {
                ConsoleHelper.Thread("Foreground thread running...");
                Thread.Sleep(200);
                ConsoleHelper.Thread("Foreground thread done.");
            })
            {
                Name = "CodvedaWorker-1",
                Priority = ThreadPriority.Normal,
                // IsBackground = false (default) — app waits for foreground threads
            };

            // ── Background thread ──────────────────────────
            var thread2 = new Thread(WorkerWithParam)
            {
                Name = "CodvedaWorker-2",
                IsBackground = true,   // app can exit without waiting for this
                Priority = ThreadPriority.BelowNormal,
            };

            ConsoleHelper.Info($"Main thread ID: {Thread.CurrentThread.ManagedThreadId}");
            ConsoleHelper.Info($"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}");

            thread1.Start();
            thread2.Start("Hello from thread param!");

            thread1.Join();    // block main thread until thread1 finishes
            thread2.Join(500); // wait max 500ms

            ConsoleHelper.Success("Both threads completed.");

            // ── Thread state ───────────────────────────────
            ConsoleHelper.Info($"thread1 state: {thread1.ThreadState}");
            ConsoleHelper.Info($"thread2 state: {thread2.ThreadState}");
        }

        static void WorkerWithParam(object? param)
        {
            ConsoleHelper.Thread($"Background thread received: '{param}'");
            Thread.Sleep(150);
            ConsoleHelper.Thread("Background thread finished.");
        }

        // ════════════════════════════════════════════════════
        //  Demo 2 — ThreadPool
        // ════════════════════════════════════════════════════
        static void Demo2_ThreadPool()
        {
            ConsoleHelper.Section("3b. ThreadPool — Reusing Threads Efficiently");

            // ── Monitor pool size ──────────────────────────
            ThreadPool.GetMinThreads(out int minWorker, out int minIO);
            ThreadPool.GetMaxThreads(out int maxWorker, out int maxIO);
            ConsoleHelper.Info($"Thread pool — Min workers: {minWorker}, Max workers: {maxWorker}");
            ConsoleHelper.Info($"Thread pool — Min IO:      {minIO}, Max IO:      {maxIO}");

            // ── QueueUserWorkItem ──────────────────────────
            // Reuses threads from the pool — no overhead of creating new threads
            int completed = 0;
            using var countdown = new CountdownEvent(6);

            for (int i = 0; i < 6; i++)
            {
                int taskId = i; // capture loop variable
                ThreadPool.QueueUserWorkItem(state =>
                {
                    ConsoleHelper.Thread($"WorkItem #{taskId} started");
                    Thread.Sleep(100 + taskId * 20);
                    Interlocked.Increment(ref completed);
                    ConsoleHelper.Thread($"WorkItem #{taskId} done — completed so far: {completed}");
                    countdown.Signal();  // decrement countdown
                });
            }

            // Wait for all work items to finish
            countdown.Wait();
            ConsoleHelper.Success($"All {completed} work items completed via ThreadPool.");

            // ── QueueUserWorkItem with WorkItem model ──────
            var workItems = Enumerable.Range(1, 4).Select(i => new WorkItem
            {
                Id = i,
                Description = $"Batch task #{i}",
                DurationMs = i * 60,
            }).ToList();

            using var done = new CountdownEvent(workItems.Count);

            foreach (var item in workItems)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    ProcessWorkItem(item);
                    done.Signal();
                });
            }

            done.Wait();
            ConsoleHelper.Success("All batch work items processed.");
        }

        static void ProcessWorkItem(WorkItem item)
        {
            ConsoleHelper.Thread($"Processing: [{item.Id}] {item.Description}");
            Thread.Sleep(item.DurationMs);
            ConsoleHelper.Success($"Completed:  [{item.Id}] {item.Description} ({item.DurationMs}ms)");
        }

        // ════════════════════════════════════════════════════
        //  Demo 3 — BackgroundWorker
        //  Best for: WinForms/WPF tasks with progress reporting
        // ════════════════════════════════════════════════════
        static void Demo3_BackgroundWorker()
        {
            ConsoleHelper.Section("3c. BackgroundWorker — Progress & Cancellation");

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,   // enables ReportProgress()
                WorkerSupportsCancellation = true,   // enables CancelAsync()
            };

            // ── DoWork: runs on background thread ─────────
            worker.DoWork += (sender, e) =>
            {
                var bw = (BackgroundWorker)sender!;

                for (int i = 1; i <= 10; i++)
                {
                    // Check for cancellation request
                    if (bw.CancellationPending)
                    {
                        ConsoleHelper.Warning("Worker: cancellation detected — stopping.");
                        e.Cancel = true;
                        return;
                    }

                    Thread.Sleep(150);

                    // Report progress back to the UI thread (ProgressChanged event)
                    bw.ReportProgress(i * 10, $"Step {i} of 10 complete");
                }

                e.Result = "All 10 steps finished successfully!";
            };

            // ── ProgressChanged: runs on calling thread ───
            worker.ProgressChanged += (sender, e) =>
            {
                ConsoleHelper.Info(
                    $"  Progress: {e.ProgressPercentage,3}% — {e.UserState}");
            };

            // ── RunWorkerCompleted: runs on calling thread ─
            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Cancelled)
                    ConsoleHelper.Warning("Worker was cancelled.");
                else if (e.Error != null)
                    ConsoleHelper.Error($"Worker error: {e.Error.Message}");
                else
                    ConsoleHelper.Success($"Worker result: {e.Result}");
            };

            // Start the worker
            worker.RunWorkerAsync();

            // Simulate cancellation after 800ms
            Thread.Sleep(800);
            if (worker.IsBusy)
            {
                ConsoleHelper.Warning("Main thread: requesting cancellation...");
                worker.CancelAsync();
            }

            // Wait for worker to finish
            while (worker.IsBusy)
                Thread.Sleep(50);

            ConsoleHelper.Success("BackgroundWorker demo complete.");
        }

        // ════════════════════════════════════════════════════
        //  Demo 4 — ThreadLocal<T>
        // ════════════════════════════════════════════════════
        static void Demo4_ThreadLocal()
        {
            ConsoleHelper.Section("3d. ThreadLocal<T> — Per-Thread State");

            // Each thread gets its own independent copy of the value
            // Great for: random number generators, formatters, connections
            using var threadLocalRng = new ThreadLocal<Random>(
                valueFactory: () =>
                {
                    var seed = Thread.CurrentThread.ManagedThreadId;
                    ConsoleHelper.Thread($"Creating Random for thread {seed}");
                    return new Random(seed);
                });

            using var done = new CountdownEvent(4);

            for (int i = 0; i < 4; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    // Each thread has its OWN Random — no sharing, no locks needed!
                    var rng = threadLocalRng.Value!;
                    var number = rng.Next(1, 1000);
                    ConsoleHelper.Thread($"Thread-local Random → {number}");
                    done.Signal();
                });
            }

            done.Wait();
            ConsoleHelper.Success("ThreadLocal demo complete — no race conditions on Random!");
        }

        // ════════════════════════════════════════════════════
        //  Demo 5 — PeriodicTimer (.NET 6+)
        // ════════════════════════════════════════════════════
        static async Task Demo5_PeriodicTimerAsync()
        {
            ConsoleHelper.Section("3e. PeriodicTimer — Modern Background Scheduling");

            // PeriodicTimer is async-friendly and avoids timer drift
            // Prefer over System.Threading.Timer for async background tasks
            using var cts = new CancellationTokenSource();
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(300));

            cts.CancelAfter(TimeSpan.FromMilliseconds(1100));   // run for ~1 sec

            int ticks = 0;
            ConsoleHelper.Info("PeriodicTimer firing every 300ms...");

            try
            {
                while (await timer.WaitForNextTickAsync(cts.Token))
                {
                    ticks++;
                    ConsoleHelper.Thread($"Timer tick #{ticks} — {DateTime.Now:HH:mm:ss.fff}");
                    // Do async background work here
                }
            }
            catch (OperationCanceledException)
            {
                ConsoleHelper.Success($"PeriodicTimer stopped after {ticks} ticks.");
            }
        }
    }
}