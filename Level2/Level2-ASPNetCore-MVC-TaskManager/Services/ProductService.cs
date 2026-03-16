// ============================================================
//  Services/ProductService.cs
//  Business logic layer — sits between Controller and Data.
//  Registered via Dependency Injection in Program.cs.
// ============================================================

using CodvedaMVC.Data;
using Level2_ASPNetCore_MVC_TaskManager.Models;

namespace CodvedaMVC.Services
{
    // ── Interface ─────────────────────────────────────────────
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> SearchProducts(string? query, string? category, decimal? maxPrice);
        Product? GetProductById(int id);
        void CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(int id);
        IEnumerable<string> GetCategories();
        HomeViewModel BuildHomeViewModel();
    }

    // ── Implementation ────────────────────────────────────────
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        // DI constructor injection — repo is provided by the DI container
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Product> GetAllProducts() => _repo.GetAll();

        public IEnumerable<Product> SearchProducts(string? query, string? category, decimal? maxPrice)
        {
            var products = _repo.GetAll();

            if (!string.IsNullOrWhiteSpace(query))
                products = products.Where(p =>
                    p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(category))
                products = products.Where(p => p.Category == category);

            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice.Value);

            return products;
        }

        public Product? GetProductById(int id) => _repo.GetById(id);

        public void CreateProduct(Product product) => _repo.Add(product);

        public bool UpdateProduct(Product product)
        {
            if (_repo.GetById(product.Id) is null) return false;
            _repo.Update(product);
            return true;
        }

        public bool DeleteProduct(int id)
        {
            if (_repo.GetById(id) is null) return false;
            _repo.Delete(id);
            return true;
        }

        public IEnumerable<string> GetCategories() => _repo.GetCategories();

        public HomeViewModel BuildHomeViewModel()
        {
            var all = _repo.GetAll().ToList();
            var categories = _repo.GetCategories().ToList();

            return new HomeViewModel
            {
                WelcomeMessage = "Welcome to Codveda Store",
                TotalProducts = all.Count,
                TotalCategories = categories.Count,
                FeaturedProducts = all.Take(3).ToList(),
                Categories = categories,
            };
        }
    }
}