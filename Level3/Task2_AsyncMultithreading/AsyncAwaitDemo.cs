// ============================================================
//  AsyncAwait/AsyncAwaitDemo.cs
//  OBJECTIVE 1: Implement async/await for non-blocking execution
//
//  Covers:
//   ✔ async/await basics — why and how
//   ✔ Task<T> return types
//   ✔ Sequential vs parallel async calls (performance difference)
//   ✔ ConfigureAwait(false) — avoid deadlocks in libraries
//   ✔ Async exception handling with try/catch
//   ✔ CancellationToken — graceful cancellation
//   ✔ ValueTask — lightweight async for hot paths
//   ✔ IAsyncEnumerable — async streaming
// ============================================================

using CodvedaAsync.Helpers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodvedaAsync.AsyncAwait
{
    public static class AsyncAwaitDemo
    {
        public static async Task RunAsync()
        {
            ConsoleHelper.Header(
                "OBJECTIVE 1: async / await",
                "Non-blocking code execution");

            await Demo1_BasicsAsync();
            await Demo2_SequentialVsParallelAsync();
            await Demo3_ExceptionHandlingAsync();
            await Demo4_CancellationTokenAsync();
            await Demo5_ValueTaskAsync();
            await Demo6_AsyncStreamAsync();
        }

        // ════════════════════════════════════════════════════
        //  Demo 1 — async/await Basics
        // ════════════════════════════════════════════════════
        static async Task Demo1_BasicsAsync()
        {
            ConsoleHelper.Section("1a. async/await — Basics");

            ConsoleHelper.Info("Main thread starts work...");

            // await releases the thread back to the thread pool
            // while waiting — no thread is blocked!
            string result = await FetchDataAsync("https://api.codveda.com/data");
            ConsoleHelper.Success($"Data received: {result}");

            // Chaining awaits — reads top to bottom, non-blocking
            var processed = await ProcessDataAsync(result);
            ConsoleHelper.Success($"Processed: {processed}");

            ConsoleHelper.Info("Main thread continues after await.");
        }

        // Simulates an async I/O operation (e.g., HTTP call, DB read)
        static async Task<string> FetchDataAsync(string url)
        {
            ConsoleHelper.Thread($"FetchDataAsync started  → url={url}");
            await Task.Delay(500);   // simulates network I/O (non-blocking)
            ConsoleHelper.Thread("FetchDataAsync completed");
            return "{ \"status\": \"ok\", \"records\": 42 }";
        }

        static async Task<string> ProcessDataAsync(string data)
        {
            ConsoleHelper.Thread("ProcessDataAsync started");
            await Task.Delay(300);   // simulates CPU/DB work
            return $"Processed({data.Length} chars)";
        }

        // ════════════════════════════════════════════════════
        //  Demo 2 — Sequential vs Parallel async
        // ════════════════════════════════════════════════════
        static async Task Demo2_SequentialVsParallelAsync()
        {
            ConsoleHelper.Section("1b. Sequential vs Parallel async calls");

            // ── Sequential: each await waits before starting next ──
            var sw = Stopwatch.StartNew();
            ConsoleHelper.Info("Sequential: starting 3 tasks one after another...");

            var r1 = await SimulateApiCallAsync("OrderService", 400);
            var r2 = await SimulateApiCallAsync("InventoryService", 300);
            var r3 = await SimulateApiCallAsync("PaymentService", 500);

            sw.Stop();
            ConsoleHelper.Warning($"Sequential total: {sw.ElapsedMilliseconds}ms " +
                                  $"(sum of all delays = ~1200ms)");

            // ── Parallel: all tasks fire simultaneously ────────────
            sw.Restart();
            ConsoleHelper.Info("\n  Parallel: starting 3 tasks simultaneously...");

            // Task.WhenAll fires all tasks at once and awaits ALL completions
            var (p1, p2, p3) = await RunParallelAsync();

            sw.Stop();
            ConsoleHelper.Success($"Parallel total:     {sw.ElapsedMilliseconds}ms " +
                                  $"(limited by slowest = ~500ms)  ← {(1200 / Math.Max(sw.ElapsedMilliseconds, 1))}x faster!");
        }

        static async Task<(string, string, string)> RunParallelAsync()
        {
            // Start all three — none awaited yet, all running concurrently
            var t1 = SimulateApiCallAsync("OrderService", 400);
            var t2 = SimulateApiCallAsync("InventoryService", 300);
            var t3 = SimulateApiCallAsync("PaymentService", 500);

            // Await all — continues when ALL are done
            await Task.WhenAll(t1, t2, t3);
            return (t1.Result, t2.Result, t3.Result);
        }

        static async Task<string> SimulateApiCallAsync(string service, int delayMs)
        {
            ConsoleHelper.Thread($"{service} → calling...");
            await Task.Delay(delayMs);
            ConsoleHelper.Thread($"{service} → responded in {delayMs}ms");
            return $"{service}:OK";
        }

        // ════════════════════════════════════════════════════
        //  Demo 3 — Exception handling in async code
        // ════════════════════════════════════════════════════
        static async Task Demo3_ExceptionHandlingAsync()
        {
            ConsoleHelper.Section("1c. Async Exception Handling");

            // ── Single task exception ──────────────────────
            try
            {
                await MightFailAsync(shouldFail: true);
            }
            catch (InvalidOperationException ex)
            {
                ConsoleHelper.Error($"Caught single exception: {ex.Message}");
            }

            // ── AggregateException from Task.WhenAll ───────
            // When multiple tasks fail, WhenAll wraps them all
            ConsoleHelper.Info("Running 3 tasks where some may fail...");
            var tasks = new[]
            {
                MightFailAsync(shouldFail: false),
                MightFailAsync(shouldFail: true),
                MightFailAsync(shouldFail: true),
            };

            try
            {
                await Task.WhenAll(tasks);
            }
            catch   // catches first exception, but ALL tasks still complete
            {
                // Inspect all exceptions via .Exception on each task
                var failed = tasks
                    .Where(t => t.IsFaulted)
                    .Select(t => t.Exception?.InnerException?.Message)
                    .ToList();

                ConsoleHelper.Error($"WhenAll: {failed.Count} task(s) failed:");
                failed.ForEach(msg => ConsoleHelper.Error($"  → {msg}"));
            }

            // ── Task.WhenAny — continue on first success ───
            ConsoleHelper.Info("WhenAny: first successful task wins...");
            var raceTasks = new[]
            {
                SimulateApiCallAsync("FastServer",  200),
                SimulateApiCallAsync("SlowServer",  800),
                SimulateApiCallAsync("MediumServer", 500),
            };

            var winner = await Task.WhenAny(raceTasks);
            ConsoleHelper.Success($"WhenAny winner: {await winner}");
        }

        static async Task MightFailAsync(bool shouldFail)
        {
            await Task.Delay(100);
            if (shouldFail)
                throw new InvalidOperationException("Simulated async failure.");
            ConsoleHelper.Success("Task completed successfully.");
        }

        // ════════════════════════════════════════════════════
        //  Demo 4 — CancellationToken
        // ════════════════════════════════════════════════════
        static async Task Demo4_CancellationTokenAsync()
        {
            ConsoleHelper.Section("1d. CancellationToken — Graceful Cancellation");

            using var cts = new CancellationTokenSource();

            // Cancel after 600ms
            cts.CancelAfter(TimeSpan.FromMilliseconds(600));

            ConsoleHelper.Info("Starting long operation (2s). Will cancel after 600ms...");

            try
            {
                await LongOperationAsync(cts.Token);
                ConsoleHelper.Success("Operation completed normally.");
            }
            catch (OperationCanceledException)
            {
                ConsoleHelper.Warning("Operation was cancelled gracefully! ✔");
            }

            // Linked tokens — cancel from multiple sources
            ConsoleHelper.Info("Linked CancellationTokens demo...");
            using var cts1 = new CancellationTokenSource();
            using var cts2 = new CancellationTokenSource();
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(
                                    cts1.Token, cts2.Token);

            // Cancel cts1 — linked token also gets cancelled
            cts1.Cancel();
            ConsoleHelper.Warning($"Linked token cancelled: {linked.Token.IsCancellationRequested}");
        }

        static async Task LongOperationAsync(CancellationToken ct)
        {
            for (int i = 1; i <= 10; i++)
            {
                // Check cancellation at each iteration
                ct.ThrowIfCancellationRequested();

                ConsoleHelper.Thread($"Step {i}/10 processing...");
                await Task.Delay(200, ct);   // Delay itself respects cancellation
            }
        }

        // ════════════════════════════════════════════════════
        //  Demo 5 — ValueTask (lightweight async)
        // ════════════════════════════════════════════════════
        static async Task Demo5_ValueTaskAsync()
        {
            ConsoleHelper.Section("1e. ValueTask — Zero-allocation async");

            // ValueTask avoids heap allocation when result is cached/sync
            // Use when the method frequently returns synchronously (hot path)
            var cache = new Dictionary<int, string>();

            for (int i = 0; i < 5; i++)
            {
                var result = await GetWithCacheAsync(i % 2, cache);
                ConsoleHelper.Info($"Got: {result}");
            }
        }

        // Returns ValueTask — synchronous path when cache hits (no allocation)
        static ValueTask<string> GetWithCacheAsync(int id, Dictionary<int, string> cache)
        {
            if (cache.TryGetValue(id, out var cached))
            {
                ConsoleHelper.Success($"Cache HIT for id={id} (synchronous, no heap alloc)");
                return ValueTask.FromResult(cached);   // no Task object allocated
            }

            return FetchAndCacheAsync(id, cache);      // actual async path
        }

        static async ValueTask<string> FetchAndCacheAsync(int id, Dictionary<int, string> cache)
        {
            ConsoleHelper.Warning($"Cache MISS for id={id} — fetching...");
            await Task.Delay(200);
            var value = $"Data-{id}";
            cache[id] = value;
            return value;
        }

        // ════════════════════════════════════════════════════
        //  Demo 6 — IAsyncEnumerable (async streaming)
        // ════════════════════════════════════════════════════
        static async Task Demo6_AsyncStreamAsync()
        {
            ConsoleHelper.Section("1f. IAsyncEnumerable — Async Streaming");

            ConsoleHelper.Info("Streaming records one-by-one (no need to wait for all):");

            // await foreach — consumes stream as items arrive
            await foreach (var record in GenerateRecordsAsync(count: 4))
            {
                ConsoleHelper.Success($"Received record: {record}");
                // Process immediately — no waiting for the full batch
            }

            ConsoleHelper.Info("Stream complete.");
        }

        // Yields items one at a time with delays — simulates DB cursor / SSE
        static async IAsyncEnumerable<string> GenerateRecordsAsync(
            int count,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            for (int i = 1; i <= count; i++)
            {
                await Task.Delay(250, ct);
                yield return $"Record #{i} — {DateTime.Now:HH:mm:ss.fff}";
            }
        }
    }
}