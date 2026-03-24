// ============================================================
//  RaceConditions/RaceConditionDemo.cs
//  OBJECTIVE 4: Race Conditions & Deadlocks
//
//  Covers:
//   ✔ Race condition — demonstration + fix with Interlocked
//   ✔ lock keyword — mutual exclusion
//   ✔ Monitor (Enter/Exit/Wait/Pulse) — advanced locking
//   ✔ Mutex — cross-process synchronization
//   ✔ Semaphore / SemaphoreSlim — limit concurrency
//   ✔ ReaderWriterLockSlim — concurrent reads, exclusive writes
//   ✔ Deadlock — demonstration + prevention strategies
//   ✔ Thread-safe collections (ConcurrentDictionary etc.)
// ============================================================

using CodvedaAsync.Helpers;
using System.Collections.Concurrent;

namespace CodvedaAsync.RaceConditions
{
    public static class RaceConditionDemo
    {
        public static async Task RunAsync()
        {
            ConsoleHelper.Header(
                "OBJECTIVE 4: Race Conditions & Deadlocks",
                "Synchronization primitives & safe patterns");

            Demo1_RaceConditionAndFix();
            Demo2_LockKeyword();
            Demo3_Monitor();
            await Demo4_SemaphoreSlimAsync();
            Demo5_ReaderWriterLock();
            Demo6_DeadlockAndPrevention();
            Demo7_ThreadSafeCollections();
            await Demo8_ChannelAsync();
        }

        // ════════════════════════════════════════════════════
        //  Demo 1 — Race Condition: problem + fix
        // ════════════════════════════════════════════════════
        static void Demo1_RaceConditionAndFix()
        {
            ConsoleHelper.Section("4a. Race Condition — Problem & Fix");

            // ── PROBLEM: unsynchronized shared counter ─────
            int unsafeCounter = 0;
            var threads = new Thread[10];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 1000; j++)
                        unsafeCounter++;   // READ-MODIFY-WRITE — NOT atomic!
                });
            }

            foreach (var t in threads) t.Start();
            foreach (var t in threads) t.Join();

            ConsoleHelper.Error($"Unsafe counter: {unsafeCounter:N0}  " +
                                $"(expected 10,000 — lost {10000 - unsafeCounter:N0} updates due to race!)");

            // ── FIX 1: Interlocked (atomic operations) ────
            int atomicCounter = 0;
            var atomicThreads = new Thread[10];

            for (int i = 0; i < atomicThreads.Length; i++)
            {
                atomicThreads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 1000; j++)
                        Interlocked.Increment(ref atomicCounter);  // atomic — no race!
                });
            }

            foreach (var t in atomicThreads) t.Start();
            foreach (var t in atomicThreads) t.Join();

            ConsoleHelper.Success($"Interlocked counter: {atomicCounter:N0}  (always exactly 10,000 ✔)");

            // ── Interlocked methods ────────────────────────
            int val = 5;
            ConsoleHelper.Info($"Interlocked.Add:            {Interlocked.Add(ref val, 10)} (was 5)");
            ConsoleHelper.Info($"Interlocked.Exchange:       {Interlocked.Exchange(ref val, 99)} (swapped to 99)");
            ConsoleHelper.Info($"Interlocked.CompareExchange:{Interlocked.CompareExchange(ref val, 200, 99)} " +
                               $"(if 99 then set 200 → val={val})");
        }

        // ════════════════════════════════════════════════════
        //  Demo 2 — lock keyword
        // ════════════════════════════════════════════════════
        static void Demo2_LockKeyword()
        {
            ConsoleHelper.Section("4b. lock — Mutual Exclusion");

            var bankAccount = new BankAccount(balance: 1000m);
            var threads = new Thread[5];

            // Simulate 5 threads simultaneously withdrawing
            for (int i = 0; i < threads.Length; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    bool success = bankAccount.Withdraw(200m);
                    ConsoleHelper.Thread(
                        success
                            ? $"Thread {threadId}: withdrew $200 — balance: ${bankAccount.Balance:F2}"
                            : $"Thread {threadId}: insufficient funds!");
                });
            }

            foreach (var t in threads) t.Start();
            foreach (var t in threads) t.Join();

            ConsoleHelper.Success($"Final balance: ${bankAccount.Balance:F2}  " +
                                  $"(never went negative — lock protected it ✔)");
        }

        // ════════════════════════════════════════════════════
        //  Demo 3 — Monitor (Enter/Exit/Wait/Pulse)
        // ════════════════════════════════════════════════════
        static void Demo3_Monitor()
        {
            ConsoleHelper.Section("4c. Monitor — Producer / Consumer Pattern");

            var queue = new Queue<int>();
            var lockObj = new object();
            bool done = false;

            // ── Producer thread ────────────────────────────
            var producer = new Thread(() =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    lock (lockObj)
                    {
                        queue.Enqueue(i);
                        ConsoleHelper.Thread($"Produced item: {i}");
                        Monitor.Pulse(lockObj);  // notify consumer
                    }
                    Thread.Sleep(100);
                }

                lock (lockObj)
                {
                    done = true;
                    Monitor.PulseAll(lockObj);
                }
            });

            // ── Consumer thread ────────────────────────────
            var consumer = new Thread(() =>
            {
                while (true)
                {
                    int item;
                    lock (lockObj)
                    {
                        // Wait while queue is empty and producer isn't done
                        while (queue.Count == 0 && !done)
                            Monitor.Wait(lockObj);  // releases lock and waits for Pulse

                        if (queue.Count == 0 && done) break;

                        item = queue.Dequeue();
                    }

                    // Process outside the lock — minimize lock hold time!
                    ConsoleHelper.Thread($"Consumed item: {item}");
                    Thread.Sleep(150);
                }

                ConsoleHelper.Success("Consumer: all items consumed ✔");
            });

            producer.Start();
            consumer.Start();
            producer.Join();
            consumer.Join();
        }

        // ════════════════════════════════════════════════════
        //  Demo 4 — SemaphoreSlim (limit concurrency)
        // ════════════════════════════════════════════════════
        static async Task Demo4_SemaphoreSlimAsync()
        {
            ConsoleHelper.Section("4d. SemaphoreSlim — Limit Concurrent Operations");

            // Allow max 3 concurrent operations (e.g., DB connections, HTTP requests)
            using var semaphore = new SemaphoreSlim(initialCount: 3, maxCount: 3);

            var tasks = Enumerable.Range(1, 8).Select(async i =>
            {
                ConsoleHelper.Info($"Task {i}: waiting for semaphore slot...");

                await semaphore.WaitAsync();   // async wait — doesn't block thread

                try
                {
                    ConsoleHelper.Thread($"Task {i}: slot acquired — working...");
                    await Task.Delay(300);     // simulate work
                    ConsoleHelper.Success($"Task {i}: done — releasing slot");
                }
                finally
                {
                    semaphore.Release();       // always release in finally!
                }
            });

            await Task.WhenAll(tasks);
            ConsoleHelper.Success("All 8 tasks done — never more than 3 ran at once ✔");
        }

        // ════════════════════════════════════════════════════
        //  Demo 5 — ReaderWriterLockSlim
        // ════════════════════════════════════════════════════
        static void Demo5_ReaderWriterLock()
        {
            ConsoleHelper.Section("4e. ReaderWriterLockSlim — Concurrent Reads, Exclusive Writes");

            // Allows: multiple concurrent readers OR one exclusive writer
            // Perfect for caches, shared config, lookup tables
            var rwLock = new ReaderWriterLockSlim();
            var cache = new Dictionary<int, string>();

            // Seed cache
            for (int i = 1; i <= 5; i++) cache[i] = $"Value-{i}";

            var threads = new List<Thread>();

            // ── 4 reader threads ──────────────────────────
            for (int i = 0; i < 4; i++)
            {
                int id = i;
                threads.Add(new Thread(() =>
                {
                    rwLock.EnterReadLock();    // many readers can hold this simultaneously
                    try
                    {
                        ConsoleHelper.Thread($"Reader {id} reading — concurrent readers: allowed");
                        Thread.Sleep(100);
                        var val = cache.TryGetValue(id + 1, out var v) ? v : "not found";
                        ConsoleHelper.Success($"Reader {id} got: {val}");
                    }
                    finally
                    {
                        rwLock.ExitReadLock();
                    }
                }));
            }

            // ── 1 writer thread ───────────────────────────
            threads.Add(new Thread(() =>
            {
                Thread.Sleep(50);              // let readers start first
                rwLock.EnterWriteLock();       // exclusive — waits for all readers to finish
                try
                {
                    ConsoleHelper.Warning("Writer: acquired exclusive lock — updating cache");
                    Thread.Sleep(120);
                    cache[99] = "NewValue-99";
                    ConsoleHelper.Success("Writer: cache updated ✔");
                }
                finally
                {
                    rwLock.ExitWriteLock();
                }
            }));

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            rwLock.Dispose();

            ConsoleHelper.Success("ReaderWriterLockSlim demo complete.");
        }

        // ════════════════════════════════════════════════════
        //  Demo 6 — Deadlock: how it happens + prevention
        // ════════════════════════════════════════════════════
        static void Demo6_DeadlockAndPrevention()
        {
            ConsoleHelper.Section("4f. Deadlock — Detection & Prevention");

            ConsoleHelper.Warning("DEADLOCK SCENARIO (explained, not executed):");
            ConsoleHelper.Warning("  Thread A: locks resource1, waits for resource2");
            ConsoleHelper.Warning("  Thread B: locks resource2, waits for resource1");
            ConsoleHelper.Warning("  → Both wait forever = DEADLOCK");

            Console.WriteLine(@"
  // ❌ DEADLOCK-PRONE CODE:
  Thread A: lock(resource1) { lock(resource2) { ... } }
  Thread B: lock(resource2) { lock(resource1) { ... } }   ← opposite order!

  // ✔ FIX 1 — Consistent lock ordering (always lock in same order):
  Thread A: lock(resource1) { lock(resource2) { ... } }
  Thread B: lock(resource1) { lock(resource2) { ... } }   ← same order

  // ✔ FIX 2 — Use Monitor.TryEnter with timeout:
  if (Monitor.TryEnter(resource1, TimeSpan.FromSeconds(1)))
  {
      try   { ... }
      finally { Monitor.Exit(resource1); }
  }
  else { /* couldn't acquire lock — handle gracefully */ }

  // ✔ FIX 3 — Use SemaphoreSlim / higher-level async primitives
  //   (can't deadlock because they don't hold thread)

  // ✔ FIX 4 — Minimize lock scope (hold lock as briefly as possible)
  // ✔ FIX 5 — Avoid calling unknown code while holding a lock
");

            // ── Safe timeout-based locking demo ───────────
            ConsoleHelper.Info("Demonstrating Monitor.TryEnter with timeout...");
            var resource = new object();

            // Lock it first to simulate contention
            bool acquired1 = Monitor.TryEnter(resource, TimeSpan.FromMilliseconds(200));
            if (acquired1)
            {
                ConsoleHelper.Success("First TryEnter: lock acquired ✔");

                var thread = new Thread(() =>
                {
                    // Try to acquire the same lock — will timeout
                    bool acquired2 = Monitor.TryEnter(resource, TimeSpan.FromMilliseconds(100));
                    if (acquired2)
                    {
                        ConsoleHelper.Success("Second TryEnter: acquired (unexpected)");
                        Monitor.Exit(resource);
                    }
                    else
                    {
                        ConsoleHelper.Warning("Second TryEnter: timed out gracefully — no deadlock ✔");
                    }
                });

                thread.Start();
                thread.Join();
                Monitor.Exit(resource);
            }
        }

        // ════════════════════════════════════════════════════
        //  Demo 7 — Thread-safe collections
        // ════════════════════════════════════════════════════
        static void Demo7_ThreadSafeCollections()
        {
            ConsoleHelper.Section("4g. Thread-Safe Collections");

            // ── ConcurrentDictionary ──────────────────────
            var dict = new ConcurrentDictionary<string, int>();

            Parallel.For(0, 1000, i =>
            {
                // GetOrAdd, AddOrUpdate are atomic — no lock needed
                dict.AddOrUpdate(
                    key: $"Key-{i % 10}",
                    addValue: 1,
                    updateValueFactory: (_, old) => old + 1);
            });

            ConsoleHelper.Success($"ConcurrentDictionary: {dict.Count} keys, " +
                                  $"total value sum = {dict.Values.Sum()} (expected 1000)");

            // ── ConcurrentQueue ───────────────────────────
            var queue = new ConcurrentQueue<int>();
            Parallel.For(0, 20, i => queue.Enqueue(i));

            int dequeued = 0;
            while (queue.TryDequeue(out _)) dequeued++;
            ConsoleHelper.Success($"ConcurrentQueue: enqueued and dequeued {dequeued} items safely ✔");

            // ── ConcurrentBag ─────────────────────────────
            var bag = new ConcurrentBag<string>();
            Parallel.For(0, 10, i => bag.Add($"Item-{i}"));
            ConsoleHelper.Success($"ConcurrentBag: {bag.Count} items added in parallel ✔");

            // ── BlockingCollection ────────────────────────
            // Bounded producer/consumer queue
            using var bounded = new BlockingCollection<int>(boundedCapacity: 5);

            var producer = Task.Run(() =>
            {
                for (int i = 0; i < 8; i++)
                {
                    bounded.Add(i);    // blocks if full (capacity=5)
                    ConsoleHelper.Thread($"Produced {i} — queue size ≤5");
                }
                bounded.CompleteAdding();
            });

            var consumer = Task.Run(() =>
            {
                foreach (var item in bounded.GetConsumingEnumerable())
                    ConsoleHelper.Thread($"Consumed {item}");
            });

            Task.WaitAll(producer, consumer);
            ConsoleHelper.Success("BlockingCollection producer/consumer done ✔");
        }

        // ════════════════════════════════════════════════════
        //  Demo 8 — System.Threading.Channels (modern)
        // ════════════════════════════════════════════════════
        static async Task Demo8_ChannelAsync()
        {
            ConsoleHelper.Section("4h. System.Threading.Channels — Modern Async Pipelines");

            // Channels are the modern async alternative to BlockingCollection
            var channel = System.Threading.Channels.Channel.CreateBounded<string>(
                new System.Threading.Channels.BoundedChannelOptions(capacity: 4)
                {
                    FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait,
                });

            // ── Writer (producer) ──────────────────────────
            var writer = Task.Run(async () =>
            {
                for (int i = 1; i <= 6; i++)
                {
                    await channel.Writer.WriteAsync($"Message-{i}");
                    ConsoleHelper.Thread($"Channel: wrote Message-{i}");
                    await Task.Delay(80);
                }
                channel.Writer.Complete();
                ConsoleHelper.Info("Channel: writer complete.");
            });

            // ── Reader (consumer) ──────────────────────────
            var reader = Task.Run(async () =>
            {
                await foreach (var msg in channel.Reader.ReadAllAsync())
                {
                    ConsoleHelper.Thread($"Channel: read  {msg}");
                    await Task.Delay(120);
                }
                ConsoleHelper.Success("Channel: all messages consumed ✔");
            });

            await Task.WhenAll(writer, reader);
        }
    }

    // ── BankAccount: thread-safe with lock ───────────────────
    public class BankAccount
    {
        private decimal _balance;
        private readonly object _lock = new();

        public decimal Balance
        {
            get { lock (_lock) { return _balance; } }
        }

        public BankAccount(decimal balance) => _balance = balance;

        public bool Withdraw(decimal amount)
        {
            lock (_lock)   // only one thread in here at a time
            {
                if (_balance < amount) return false;

                // Simulate tiny delay (makes race condition more visible without lock)
                Thread.Sleep(10);
                _balance -= amount;
                return true;
            }
        }

        public void Deposit(decimal amount)
        {
            lock (_lock)
            {
                _balance += amount;
            }
        }
    }
}