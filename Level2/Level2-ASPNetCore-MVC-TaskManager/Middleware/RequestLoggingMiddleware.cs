// ============================================================
//  Middleware/RequestLoggingMiddleware.cs
//  Custom middleware — runs for every HTTP request.
//  Middleware forms a pipeline: each piece can inspect,
//  modify, or short-circuit the request/response.
// ============================================================

namespace CodvedaMVC.Middleware
{
    // ── Middleware Class ──────────────────────────────────────
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;   
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            _logger.LogInformation(
                "➡  [{Method}] {Path}{Query} — started at {Time}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                DateTime.Now.ToString("HH:mm:ss.fff"));

            await _next(context);

            stopwatch.Stop();
            _logger.LogInformation(
                "⬅  [{Method}] {Path} — {StatusCode} in {Elapsed}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestLoggingMiddleware>();
    }
}