// ============================================================
//  Program.cs — Application Entry Point
//  Codveda Internship · Level 2 · Task 2: ASP.NET Core MVC
// ============================================================
//  This file does TWO things:
//   1. Register services into the DI container (builder phase)
//   2. Configure the HTTP middleware pipeline (app phase)
// ============================================================

using CodvedaMVC.Data;
using CodvedaMVC.Middleware;
using CodvedaMVC.Services;

// ?? STEP 1: Create the builder ????????????????????????????
var builder = WebApplication.CreateBuilder(args);

// ?? STEP 2: Register services (Dependency Injection) ??????
// AddControllersWithViews registers MVC services:
//   - Controller discovery & routing
//   - Razor view engine
//   - Model binding & validation
//   - Tag Helpers & HTML helpers
builder.Services.AddControllersWithViews();

// Register our custom services:
//   AddSingleton  ? one instance for the lifetime of the app
//   AddScoped     ? one instance per HTTP request
//   AddTransient  ? new instance every time it's requested
//
//   IProductRepository ? InMemoryProductRepository
//   (swap this line with a real EF Core repo in production)
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();

//   IProductService ? ProductService  (scoped per request)
builder.Services.AddScoped<IProductService, ProductService>();

// ?? STEP 3: Build the app ?????????????????????????????????
var app = builder.Build();

// ?? STEP 4: Configure the HTTP middleware pipeline ????????
// Middleware order matters — each piece wraps the next.
// Request flows DOWN the pipeline, response flows back UP.

if (!app.Environment.IsDevelopment())
{
    // Production: show a friendly error page
    app.UseExceptionHandler("/Home/Error");

    // Add HTTP Strict Transport Security header
    app.UseHsts();
}
else
{
    // Development: show full exception details
    app.UseDeveloperExceptionPage();
}

// 1. Redirect HTTP ? HTTPS
app.UseHttpsRedirection();

// 2. Serve static files from wwwroot/ (CSS, JS, images)
app.UseStaticFiles();

// 3. Custom middleware: log every request with timing
app.UseRequestLogging();

// 4. Routing — match URLs to controllers/actions
app.UseRouting();

// 5. Authorization (no auth in this project, but correct position)
app.UseAuthorization();

// ?? STEP 5: Define routes ?????????????????????????????????
// Convention-based routing:  /Controller/Action/Id
// Default: / ? HomeController.Index()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

 //?? STEP 6: Start the server ??????????????????????????????
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("????????????????????????????????????????????????????????");
Console.WriteLine("?   Codveda Internship · Level 2 · Task 2             ?");
Console.WriteLine("?   ASP.NET Core MVC Application                     ?");
Console.WriteLine("????????????????????????????????????????????????????????");
Console.WriteLine("?   Running at: https://localhost:5001               ?");
Console.WriteLine("?   Running at: http://localhost:5000                ?");
Console.WriteLine("????????????????????????????????????????????????????????");
Console.ResetColor();

app.Run();