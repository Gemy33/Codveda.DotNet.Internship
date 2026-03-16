// ============================================================
//  Data/ProductRepository.cs
//  In-memory data store — simulates a real database.
//  In a real project: replace with EF Core + SQL Server.
// ============================================================



using Level2_ASPNetCore_MVC_TaskManager.Models;

namespace CodvedaMVC.Data
{
    // ── Interface (Dependency Injection contract) ─────────────
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetByCategory(string category);
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
        IEnumerable<string> GetCategories();
    }

    // ── In-Memory Implementation ──────────────────────────────
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products;
        private int _nextId = 7;

        public InMemoryProductRepository()
        {
            // Seed data
            _products = new List<Product>
            {
                new() { Id=1, Name="ASP.NET Core Handbook",  Description="Complete guide to ASP.NET Core 8 development.",   Price=49.99m,  Stock=100, Category="Books",       CreatedAt=DateTime.Now.AddDays(-30) },
                new() { Id=2, Name="C# Pro Course",          Description="Advanced C# programming from basics to expert.",  Price=199.99m, Stock=50,  Category="Courses",     CreatedAt=DateTime.Now.AddDays(-20) },
                new() { Id=3, Name="Mechanical Keyboard",    Description="Clicky RGB keyboard for developers.",             Price=89.99m,  Stock=30,  Category="Hardware",    CreatedAt=DateTime.Now.AddDays(-15) },
                new() { Id=4, Name="Standing Desk",          Description="Height-adjustable desk for better posture.",      Price=349.99m, Stock=0,   Category="Hardware",    CreatedAt=DateTime.Now.AddDays(-10) },
                new() { Id=5, Name="Code Review Checklist",  Description="Printable checklist for professional reviews.",   Price=9.99m,   Stock=200, Category="Templates",   CreatedAt=DateTime.Now.AddDays(-5)  },
                new() { Id=6, Name="Docker for .NET Devs",   Description="Containerise your .NET apps step by step.",      Price=79.99m,  Stock=75,  Category="Courses",     CreatedAt=DateTime.Now.AddDays(-2)  },
            };
        }

        public IEnumerable<Product> GetAll() => _products.OrderByDescending(p => p.CreatedAt);
        public IEnumerable<Product> GetByCategory(string cat) => _products.Where(p => p.Category == cat);
        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
        public IEnumerable<string> GetCategories() => _products.Select(p => p.Category).Distinct().OrderBy(c => c);

        public void Add(Product product)
        {
            product.Id = _nextId++;
            product.CreatedAt = DateTime.Now;
            _products.Add(product);
        }

        public void Update(Product product)
        {
            var existing = GetById(product.Id);
            if (existing is null) return;
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.Category = product.Category;
        }

        public void Delete(int id) =>
            _products.RemoveAll(p => p.Id == id);
    }
}