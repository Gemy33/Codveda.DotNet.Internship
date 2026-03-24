# 🌐 Codveda .NET Internship — Level 2 | Task 2
## Web Development with ASP.NET Core MVC

> **Intern:** Mohamed Gamal
> **Company:** [Codveda Technology](https://www.codveda.com)
> **Domain:** .NET Development | **Level:** 2 (Intermediate)

---

## 📋 Task Objectives

| # | Objective | Status |
|---|-----------|--------|
| 1 | Set up an ASP.NET Core project with MVC architecture | ✅ |
| 2 | Implement controllers, views, and models | ✅ |
| 3 | Use Razor syntax for dynamic HTML rendering | ✅ |
| 4 | Apply middleware and dependency injection | ✅ |

---

## 📁 Project Structure

```
Task2_AspNetCoreMVC/
│
├── Controllers/
│   ├── HomeController.cs        ← Home, About, Contact pages
│   └── ProductController.cs     ← Full CRUD (List, Create, Edit, Delete)
│
├── Models/
│   ├── Product.cs               ← Entity with Data Annotations
│   └── HomeViewModel.cs         ← ViewModels (HomeViewModel, ContactViewModel)
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml         ← Hero, stats cards, featured products
│   │   ├── About.cshtml         ← MVC explanation + ViewBag/ViewData demo
│   │   └── Contact.cshtml       ← Razor form with server+client validation
│   ├── Product/
│   │   ├── Index.cshtml         ← Searchable product table
│   │   ├── Details.cshtml       ← Single product detail card
│   │   ├── Create.cshtml        ← Add product form
│   │   ├── Edit.cshtml          ← Edit product form
│   │   └── Delete.cshtml        ← Delete confirmation page
│   └── Shared/
│       ├── _Layout.cshtml       ← Master layout (navbar, footer, flash messages)
│       └── _ValidationScriptsPartial.cshtml
│
├── Data/
│   └── ProductRepository.cs     ← IProductRepository + InMemoryProductRepository
│
├── Services/
│   └── ProductService.cs        ← IProductService + ProductService (DI registered)
│
├── Middleware/
│   └── RequestLoggingMiddleware.cs ← Custom middleware (request timing + logging)
│
├── wwwroot/
│   ├── css/site.css             ← Custom styles on Bootstrap 5
│   └── js/site.js               ← Alert auto-dismiss, active nav highlight
│
├── Program.cs                   ← DI registration + middleware pipeline
├── appsettings.json
└── CodvedaMVC.csproj
```

---

## 🏗️ Architecture Overview

```
HTTP Request
     │
     ▼
┌─────────────────────────┐
│   Middleware Pipeline    │
│  ┌───────────────────┐  │
│  │ HTTPS Redirect    │  │
│  │ Static Files      │  │
│  │ Request Logging ← │──│── Custom Middleware
│  │ Routing           │  │
│  │ Authorization     │  │
│  └───────────────────┘  │
└──────────┬──────────────┘
           │
           ▼
┌──────────────────────┐
│     Controller       │  ← Receives request, calls Service
│  HomeController      │
│  ProductController   │
└──────────┬───────────┘
           │  injects via DI
           ▼
┌──────────────────────┐
│      Service         │  ← Business logic
│  IProductService     │
│  ProductService      │
└──────────┬───────────┘
           │  injects via DI
           ▼
┌──────────────────────┐
│    Repository        │  ← Data access (swap for EF Core)
│  IProductRepository  │
│  InMemoryRepository  │
└──────────────────────┘
           │
           │  returns Model
           ▼
┌──────────────────────┐
│       View           │  ← Razor .cshtml renders HTML
│  .cshtml + Layout    │
└──────────────────────┘
           │
           ▼
      HTTP Response
```

---

## ✨ Key Features Demonstrated

### 🎯 MVC Pattern
- **Models** — `Product` with `[Required]`, `[Range]`, `[StringLength]` Data Annotations
- **ViewModels** — `HomeViewModel`, `ContactViewModel` (shaped for the UI, not the DB)
- **Controllers** — `HomeController`, `ProductController` with full CRUD action methods
- **Views** — Razor `.cshtml` files with `@model`, `@foreach`, `@if`, Tag Helpers

### 🪟 Razor Syntax Used
```razor
@model Product                        <!-- strongly-typed model binding  -->
@foreach (var p in Model.Products)    <!-- iteration                      -->
@if (product.IsAvailable) { }         <!-- conditionals                   -->
@product.Price.ToString("C")          <!-- expression output              -->
asp-controller / asp-action           <!-- Tag Helper URL generation      -->
asp-for / asp-validation-for          <!-- Tag Helper form binding        -->
@Html.AntiForgeryToken()              <!-- CSRF protection                -->
@ViewBag.Title / @ViewData["Key"]     <!-- loose data from controller     -->
@TempData["SuccessMessage"]           <!-- flash messages across redirect  -->
@RenderBody() / @RenderSection()      <!-- layout composition             -->
```

### 💉 Dependency Injection
```csharp
// Program.cs — registering services
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Controller — receiving services via constructor injection
public ProductController(IProductService productService, ILogger<ProductController> logger)
{
    _productService = productService;
    _logger         = logger;
}
```

| Lifetime | Method | When to Use |
|----------|--------|-------------|
| Singleton | `AddSingleton` | One instance for the whole app (e.g. config, cache) |
| Scoped | `AddScoped` | One per HTTP request (e.g. DB context, services) |
| Transient | `AddTransient` | New instance every time (e.g. lightweight utilities) |

### 🔧 Custom Middleware
```csharp
public async Task InvokeAsync(HttpContext context)
{
    // Code here runs BEFORE the next middleware
    await _next(context);   // pass to next middleware
    // Code here runs AFTER the response is built
}
```

### 🛡️ Middleware Pipeline Order (Program.cs)
```
HTTPS Redirect → Static Files → Request Logging → Routing → Authorization → MVC
```

---

## 🚀 How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 **or** VS Code with C# Dev Kit

### Steps

```bash
# 1. Clone the repo
git clone https://github.com/YOUR_USERNAME/Codveda-DotNet-Internship.git

# 2. Navigate to Task 2
cd Task2_AspNetCoreMVC

# 3. Restore packages
dotnet restore

# 4. Run the app
dotnet run

# 5. Open in browser
# https://localhost:5001  or  http://localhost:5000
```

### Available Pages

| URL | Description |
|-----|-------------|
| `/` | Home page — hero, stats, featured products |
| `/Product` | Product list with search & category filter |
| `/Product/Create` | Add a new product |
| `/Product/Details/1` | View product detail |
| `/Product/Edit/1` | Edit a product |
| `/Product/Delete/1` | Delete confirmation |
| `/Home/About` | MVC architecture explainer |
| `/Home/Contact` | Contact form with validation |

---

## 🛠️ Technologies Used

| Technology | Purpose |
|---|---|
| ASP.NET Core 8 MVC | Web framework |
| Razor Pages | Server-side HTML rendering |
| Bootstrap 5 | UI styling |
| Bootstrap Icons | Icon library |
| In-Memory Repository | Data storage (no DB needed) |
| Microsoft.Extensions.Logging | Request logging |
| jQuery Validation | Client-side form validation |

---

## 📚 Resources

- [ASP.NET Core MVC Docs](https://docs.microsoft.com/aspnet/core/mvc)
- [Razor Syntax Reference](https://docs.microsoft.com/aspnet/core/mvc/views/razor)
- [Dependency Injection in .NET](https://docs.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Middleware in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/middleware)

---

*Built with ❤️ for the Codveda Technology .NET Development Internship*
`#CodvedaJourney` `#CodvedaExperience` `#FutureWithCodveda`
