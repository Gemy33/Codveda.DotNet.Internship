// ============================================================
//  Services/RealWorldDemo.cs
//  Real-world scenario tying all 4 objectives together.
//
//  Simulates: an async order processing pipeline that uses
//   ✔ async/await  for I/O operations
//   ✔ TPL          for parallel order processing
//   ✔ ThreadPool   for background jobs
//   ✔ SemaphoreSlim to rate-limit external API calls
//   ✔ ConcurrentDictionary for thread-safe state
//   ✔ Channels     for producer/consumer pipeline
//   ✔ CancellationToken for graceful shutdown
// ============================================================

using CodvedaAsync.Helpers;
using CodvedaAsync.Models;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace CodvedaAsync.Services
{
    public static class RealWorldDemo
    {
        public static async Task RunAsync()
        {
            ConsoleHelper.Header(
                "REAL-WORLD: Async Order Processing Pipeline",
                "All 4 objectives combined");

            await RunOrderPipelineAsync();
        }

        static async Task RunOrderPipelineAsync()
        {
            using var cts = new CancellationTokenSource();

            // ── Shared state ───────────────────────────────
            var processedOrders = new ConcurrentDictionary<int, string>();

            // ── Rate limiter: max 3 concurrent API calls ───
            using var rateLimiter = new SemaphoreSlim(3, 3);

            // ── Async channel: orders flow from producer → consumer
            var channel = Channel.CreateBounded<Order>(capacity: 10);

            ConsoleHelper.Info("Starting order pipeline: Ingest → Validate → Process → Save");
            ConsoleHelper.Separator();

            // ── Stage 1: Order Ingestion (producer) ───────
            var ingester = Task.Run(async () =>
            {
                var orders = GenerateOrders(count: 8);
                foreach (var order in orders)
                {
                    await channel.Writer.WriteAsync(order, cts.Token);
                    ConsoleHelper.Info($"[INGEST]   Order #{order.Id:D2} — {order.Product} queued");
                    await Task.Delay(80, cts.Token);
                }
                channel.Writer.Complete();
                ConsoleHelper.Success("[INGEST]   All orders ingested.");
            }, cts.Token);

            // ── Stage 2: Validation + Processing (consumer) ─
            var processor = Task.Run(async () =>
            {
                var processingTasks = new List<Task>();

                await foreach (var order in channel.Reader.ReadAllAsync(cts.Token))
                {
                    var o = order;  // capture
                    processingTasks.Add(Task.Run(async () =>
                    {
                        // Validate
                        await ValidateOrderAsync(o, cts.Token);

                        // Rate-limited external payment API call
                        await rateLimiter.WaitAsync(cts.Token);
                        try
                        {
                            await CallPaymentApiAsync(o, cts.Token);
                        }
                        finally
                        {
                            rateLimiter.Release();
                        }

                        // Save result
                        processedOrders[o.Id] = o.Status;
                        ConsoleHelper.Success($"[COMPLETE] Order #{o.Id:D2} → {o.Status}");

                    }, cts.Token));
                }

                await Task.WhenAll(processingTasks);
                ConsoleHelper.Success("[PROCESSOR] All orders processed.");
            }, cts.Token);

            // ── Stage 3: Background health monitor ────────
            var monitor = Task.Run(async () =>
            {
                using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(400));
                try
                {
                    while (await timer.WaitForNextTickAsync(cts.Token))
                    {
                        ConsoleHelper.Warning(
                            $"[MONITOR]  Processed: {processedOrders.Count}/8 orders | " +
                            $"Semaphore slots free: {rateLimiter.CurrentCount}/3");
                    }
                }
                catch (OperationCanceledException) { }
            }, cts.Token);

            // ── Wait for pipeline to complete ──────────────
            await Task.WhenAll(ingester, processor);
            cts.Cancel();   // stop the monitor

            try { await monitor; } catch (OperationCanceledException) { }

            // ── Final summary ──────────────────────────────
            ConsoleHelper.Separator();
            ConsoleHelper.Header("Pipeline Summary");

            var grouped = processedOrders
                .GroupBy(kv => kv.Value)
                .Select(g => $"{g.Key}: {g.Count()}")
                .ToList();

            ConsoleHelper.Success($"Total orders processed: {processedOrders.Count}");
            grouped.ForEach(g => ConsoleHelper.Info($"  {g}"));
        }

        static List<Order> GenerateOrders(int count) =>
            Enumerable.Range(1, count).Select(i => new Order
            {
                Id = i,
                Product = new[] { "Laptop", "Phone", "Tablet", "Monitor", "Keyboard" }[i % 5],
                Quantity = i % 3 + 1,
                Price = Math.Round(i * 49.99, 2),
            }).ToList();

        static async Task ValidateOrderAsync(Order order, CancellationToken ct)
        {
            await Task.Delay(60, ct);   // simulate async validation (DB lookup)
            order.Status = order.Price > 0 ? "Validated" : "Invalid";
            ConsoleHelper.Thread($"[VALIDATE] Order #{order.Id:D2} → {order.Status}");
        }

        static async Task CallPaymentApiAsync(Order order, CancellationToken ct)
        {
            await Task.Delay(150, ct);  // simulate async HTTP call (rate-limited)
            order.Status = "Paid";
            ConsoleHelper.Thread($"[PAYMENT]  Order #{order.Id:D2} → charged ${order.Price:F2}");
        }
    }
}