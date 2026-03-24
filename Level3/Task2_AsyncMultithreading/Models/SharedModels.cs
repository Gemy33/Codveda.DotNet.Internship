// ============================================================
//  Models/SharedModels.cs
//  Shared models and helpers used across all demo files.
// ============================================================

namespace CodvedaAsync.Models
{
    // ── Order: used in TPL & async demos ─────────────────────
    public class Order
    {
        public int Id { get; set; }
        public string Product { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    // ── DownloadResult: used in async HTTP demo ───────────────
    public class DownloadResult
    {
        public string Url { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public long Bytes { get; set; }
        public long ElapsedMs { get; set; }
        public bool Success { get; set; }
    }

    // ── WorkItem: used in ThreadPool demo ─────────────────────
    public class WorkItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int DurationMs { get; set; }
    }
}

// ============================================================
//  Helpers/ConsoleHelper.cs
//  Thread-safe colored console output used throughout demos.
// ============================================================

namespace CodvedaAsync.Helpers
{
    public static class ConsoleHelper
    {
        // Lock object — ensures only one thread writes at a time
        private static readonly object _consoleLock = new();

        public static void Info(string msg, string? prefix = null)
            => Write(msg, ConsoleColor.Cyan, prefix ?? "INFO");

        public static void Success(string msg, string? prefix = null)
            => Write(msg, ConsoleColor.Green, prefix ?? "✔");

        public static void Warning(string msg, string? prefix = null)
            => Write(msg, ConsoleColor.Yellow, prefix ?? "WARN");

        public static void Error(string msg, string? prefix = null)
            => Write(msg, ConsoleColor.Red, prefix ?? "✘");

        public static void Thread(string msg)
            => Write(msg, ConsoleColor.Magenta,
                $"T-{System.Threading.Thread.CurrentThread.ManagedThreadId:D2}");

        public static void Section(string title)
        {
            lock (_consoleLock)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  ── {title} ──");
                Console.ResetColor();
            }
        }

        public static void Header(string title, string subtitle = "")
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
                Console.WriteLine($"║  {title,-52}║");
                if (!string.IsNullOrEmpty(subtitle))
                    Console.WriteLine($"║  {subtitle,-52}║");
                Console.WriteLine("╚══════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
        }

        public static void Separator()
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  " + new string('─', 54));
                Console.ResetColor();
            }
        }

        private static void Write(string msg, ConsoleColor color, string prefix)
        {
            lock (_consoleLock)
            {
                var orig = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"  [{DateTime.Now:HH:mm:ss.fff}] ");
                Console.ForegroundColor = color;
                Console.Write($"[{prefix}] ");
                Console.ResetColor();
                Console.WriteLine(msg);
            }
        }
    }
}