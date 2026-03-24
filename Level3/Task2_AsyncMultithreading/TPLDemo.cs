// ============================================================
//  TPL/TPLDemo.cs
//  OBJECTIVE 2: Task Parallel Library (TPL)
//
//  Covers:
//   ✔ Parallel.For / Parallel.ForEach — data parallelism
//   ✔ Parallel.Invoke — action parallelism
//   ✔ PLINQ — parallel LINQ queries
//   ✔ Task.Run — offload CPU work to thread pool
//   ✔ Task continuations — ContinueWith
//   ✔ Task factories and chaining
//   ✔ ParallelOptions — degree of parallelism
// ============================================================

using CodvedaAsync.Helpers;
using CodvedaAsync.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CodvedaAsync.TPL
{
    public static class TPLDemo
    {
        public static async Task RunAsync()
        {
            ConsoleHelper.Header(
                "OBJECTIVE 2: Task Parallel Library (TPL)",
                "Concurrent & parallel programming");

            Demo1_ParallelFor();
            Demo2_ParallelForEach();
            Demo3_ParallelInvoke();
            Demo4_PLINQ();
            await Demo5_TaskRunAsync();
            await Demo6_ContinuationsAsync();
        }

        // ════════════════════════════════════════════════════
        //  Demo 1 — Parallel.For
        // ════════════════════════════════════════════════════
        static void Demo1_ParallelFor()
        {
            ConsoleHelper.Section("2a. Parallel.For — Data Parallelism");

            const int count = 8;

            // ── Sequential ────────────────────────────────
            var sw = Stopwatch.StartNew();
            var seqResults = new int[count];
            for (int i = 0; i < count; i++)
            {
                seqResults[i] = HeavyCompute(i);
            }
            sw.Stop();
            ConsoleHelper.Warning($"Sequential Parallel.For:  {sw.ElapsedMilliseconds}ms");

            // ── Parallel.For ──────────────────────────────
            sw.Restart();
            var parResults = new int[count];

            Parallel.For(0, count, new ParallelOptions
            {
                // Limit threads to logical CPU count — avoid over-subscription
                MaxDegreeOfParallelism = Environment.ProcessorCount
            },
            i =>
            {
                parResults[i] = HeavyCompute(i);
                ConsoleHelper.Thread($"Parallel.For i={i} done on thread");
            });

            sw.Stop();
            ConsoleHelper.Success($"Parallel Parallel.For:    {sw.ElapsedMilliseconds}ms");

            // ── Parallel.For with local state ─────────────
            // Safe accumulation — no shared mutable state across threads
            long total = 0;
            Parallel.For<long>(
                fromInclusive: 0,
                toExclusive: count,
                localInit: () => 0L,                         // init each thread's local value
                body: (i, state, local) => local + i,   // accumulate locally
                localFinally: local => Interlocked.Add(ref total, local) // merge safely
            );
            ConsoleHelper.Info($"Parallel sum with local state = {total}  (expected {count * (count - 1) / 2})");
        }

        static int HeavyCompute(int input)
        {
            // Simulate CPU-bound work
            Thread.Sleep(30);
            return input * input;
        }

        // ════════════════════════════════════════════════════
        //  Demo 2 — Parallel.ForEach
        // ════════════════════════════════════════════════════
        static void Demo2_ParallelForEach()
        {
            ConsoleHelper.Section("2b. Parallel.ForEach — Process Collections");

            var orders = Enumerable.Range(1, 10)
                .Select(i => new Order
                {
                    Id = i,
                    Product = $"Product-{i}",
                    Price = i * 9.99,
                    Quantity = i % 5 + 1,
                })
                .ToList();

            // Thread-safe collection for results
            var processed = new ConcurrentBag<Order>();
            var sw = Stopwatch.StartNew();

            Parallel.ForEach(orders,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                order =>
                {
                    // Simulate processing each order
                    Thread.Sleep(50);
                    order.Status = "Processed";
                    processed.Add(order);
                    ConsoleHelper.Thread($"Order #{order.Id:D2} processed → ${order.Price:F2}");
                });

            sw.Stop();
            ConsoleHelper.Success(
                $"All {processed.Count} orders processed in {sw.ElapsedMilliseconds}ms " +
                $"(sequential would be ~{orders.Count * 50}ms)");

            // ── Parallel.ForEach with partition ───────────
            // Partitioner groups items into chunks to reduce overhead
            var partitioner = Partitioner.Create(0, orders.Count, rangeSize: 3);
            Parallel.ForEach(partitioner, range =>
            {
                ConsoleHelper.Thread($"Partition [{range.Item1}–{range.Item2}] on thread");
                // Process range.Item1 to range.Item2 here
            });
        }

        // ════════════════════════════════════════════════════
        //  Demo 3 — Parallel.Invoke
        // ════════════════════════════════════════════════════
        static void Demo3_ParallelInvoke()
        {
            ConsoleHelper.Section("2c. Parallel.Invoke — Independent Actions");

            ConsoleHelper.Info("Running 4 independent operations simultaneously...");
            var sw = Stopwatch.StartNew();

            // Each action runs on a separate thread simultaneously
            Parallel.Invoke(
                () => { Thread.Sleep(200); ConsoleHelper.Thread("Action 1: Send email       ✔"); },
                () => { Thread.Sleep(150); ConsoleHelper.Thread("Action 2: Update database  ✔"); },
                () => { Thread.Sleep(300); ConsoleHelper.Thread("Action 3: Generate report  ✔"); },
                () => { Thread.Sleep(100); ConsoleHelper.Thread("Action 4: Clear cache      ✔"); }
            );

            sw.Stop();
            ConsoleHelper.Success($"All actions done in {sw.ElapsedMilliseconds}ms " +
                                  $"(slowest was 300ms — ~{(750.0 / sw.ElapsedMilliseconds):F1}x faster than sequential)");
        }

        // ════════════════════════════════════════════════════
        //  Demo 4 — PLINQ (Parallel LINQ)
        // ════════════════════════════════════════════════════
        static void Demo4_PLINQ()
        {
            ConsoleHelper.Section("2d. PLINQ — Parallel LINQ Queries");

            var numbers = Enumerable.Range(1, 1_000_000).ToList();

            // ── Sequential LINQ ───────────────────────────
            var sw = Stopwatch.StartNew();
            var seqResult = numbers
                .Where(n => n % 2 == 0)
                .Select(n => n * n)
                .Sum();
            sw.Stop();
            ConsoleHelper.Warning($"Sequential LINQ: {sw.ElapsedMilliseconds}ms (sum={seqResult:N0})");

            // ── PLINQ — just add .AsParallel() ───────────
            sw.Restart();
            var parResult = numbers
                .AsParallel()                                    // ← enables PLINQ
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Where(n => n % 2 == 0)
                .Select(n => n * n)
                .Sum();
            sw.Stop();
            ConsoleHelper.Success($"PLINQ:            {sw.ElapsedMilliseconds}ms (sum={parResult:N0})");

            // ── PLINQ with ordering preserved ─────────────
            // AsParallel() doesn't guarantee order — use AsOrdered() when needed
            var ordered = Enumerable.Range(1, 10)
                .AsParallel()
                .AsOrdered()
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .Select(n => n * 2)
                .ToList();
            ConsoleHelper.Info($"PLINQ ordered: [{string.Join(", ", ordered)}]");

            // ── PLINQ with ForAll (no result collection) ──
            var safeCount = 0;
            Enumerable.Range(1, 20)
                .AsParallel()
                .Where(n => n % 3 == 0)
                .ForAll(n => Interlocked.Increment(ref safeCount));   // thread-safe increment
            ConsoleHelper.Info($"PLINQ ForAll — multiples of 3 found: {safeCount}");
        }

        // ════════════════════════════════════════════════════
        //  Demo 5 — Task.Run (offload CPU work)
        // ════════════════════════════════════════════════════
        static async Task Demo5_TaskRunAsync()
        {
            ConsoleHelper.Section("2e. Task.Run — Offload CPU Work to Thread Pool");

            ConsoleHelper.Info("Offloading CPU-bound work without blocking main thread...");

            // Task.Run queues work to the ThreadPool
            // Use for CPU-bound work — NOT for I/O (use async/await instead)
            var cpuTask = Task.Run(() =>
            {
                ConsoleHelper.Thread("CPU work started on pool thread");
                long sum = 0;
                for (long i = 0; i < 50_000_000; i++) sum += i;
                ConsoleHelper.Thread($"CPU work done → sum={sum:N0}");
                return sum;
            });

            // Main thread is free to do other work
            ConsoleHelper.Info("Main thread is NOT blocked — doing other work...");
            await Task.Delay(100);
            ConsoleHelper.Info("Main thread still responding...");

            var result = await cpuTask;
            ConsoleHelper.Success($"CPU result received: {result:N0}");

            // ── Multiple Task.Run in parallel ─────────────
            ConsoleHelper.Info("Running 4 CPU tasks in parallel...");
            var sw = Stopwatch.StartNew();

            var cpuTasks = Enumerable.Range(1, 4)
                .Select(i => Task.Run(() =>
                {
                    Thread.Sleep(200);
                    ConsoleHelper.Thread($"CPU Task {i} complete");
                    return i * 100;
                }))
                .ToArray();

            var results = await Task.WhenAll(cpuTasks);
            sw.Stop();
            ConsoleHelper.Success(
                $"4 CPU tasks done in {sw.ElapsedMilliseconds}ms → [{string.Join(", ", results)}]");
        }

        // ════════════════════════════════════════════════════
        //  Demo 6 — Task Continuations (ContinueWith)
        // ════════════════════════════════════════════════════
        static async Task Demo6_ContinuationsAsync()
        {
            ConsoleHelper.Section("2f. Task Continuations — ContinueWith");

            // ContinueWith schedules a task to run after another completes
            // Prefer async/await for readability — ContinueWith for advanced scenarios
            ConsoleHelper.Info("Building task pipeline with continuations...");

            var pipeline = Task.Run(() =>
            {
                ConsoleHelper.Thread("Step 1: Fetch data");
                Thread.Sleep(150);
                return "raw_data";
            })
            .ContinueWith(prev =>
            {
                ConsoleHelper.Thread($"Step 2: Validate '{prev.Result}'");
                Thread.Sleep(100);
                return prev.Result.ToUpper();
            })
            .ContinueWith(prev =>
            {
                ConsoleHelper.Thread($"Step 3: Transform '{prev.Result}'");
                Thread.Sleep(100);
                return $"[{prev.Result}]";
            })
            .ContinueWith(prev =>
            {
                ConsoleHelper.Thread($"Step 4: Save '{prev.Result}'");
                Thread.Sleep(80);
                return true;
            });

            var saved = await pipeline;
            ConsoleHelper.Success($"Pipeline complete — saved: {saved}");

            // ── Conditional continuation ───────────────────
            var risky = Task.Run(() =>
            {
                throw new Exception("Something went wrong!");
                return "never";
            });

            risky.ContinueWith(t =>
                ConsoleHelper.Success("Continuation on success"),
                TaskContinuationOptions.OnlyOnRanToCompletion);

            risky.ContinueWith(t =>
                ConsoleHelper.Error($"Continuation on failure: {t.Exception?.InnerException?.Message}"),
                TaskContinuationOptions.OnlyOnFaulted);

            await Task.Delay(200);  // let continuations finish
        }
    }
}