// ============================================================
//  Controllers/ProductController.cs
//  Full CRUD for Products: List, Detail, Create, Edit, Delete.
//  Demonstrates: routing, model binding, validation, TempData.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using CodvedaMVC.Services;
using Level2_ASPNetCore_MVC_TaskManager.Models;

namespace CodvedaMVC.Controllers
{
    // Route prefix: /Product/...
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // ── GET /Product ──────────────────────────────────────
        // List all products with optional search/filter
        public IActionResult Index(string? search, string? category, decimal? maxPrice)
        {
            var products = _productService.SearchProducts(search, category, maxPrice);
            var categories = _productService.GetCategories();

            // Pass filter state back to the view via ViewBag
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Categories = categories;

            return View(products);
        }

        // ── GET /Product/Details/5 ────────────────────────────
        public IActionResult Details(int id)
        {
            var product = _productService.GetProductById(id);

            if (product is null)
            {
                _logger.LogWarning("Product {Id} not found.", id);
                return NotFound();         // 404
            }

            return View(product);
        }

        // ── GET /Product/Create ───────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View(new Product());
        }

        // ── POST /Product/Create ──────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _productService.GetCategories();
                return View(product);
            }

            _productService.CreateProduct(product);
            _logger.LogInformation("Product '{Name}' created (ID: {Id}).", product.Name, product.Id);

            TempData["SuccessMessage"] = $"✔ Product '{product.Name}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Product/Edit/5 ───────────────────────────────
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product is null) return NotFound();

            ViewBag.Categories = _productService.GetCategories();
            return View(product);
        }

        // ── POST /Product/Edit/5 ──────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _productService.GetCategories();
                return View(product);
            }

            var updated = _productService.UpdateProduct(product);
            if (!updated) return NotFound();

            _logger.LogInformation("Product '{Name}' (ID: {Id}) updated.", product.Name, id);
            TempData["SuccessMessage"] = $"✔ Product '{product.Name}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Product/Delete/5 ─────────────────────────────
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetProductById(id);
            if (product is null) return NotFound();
            return View(product);
        }

        // ── POST /Product/Delete/5 ────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productService.GetProductById(id);
            var name = product?.Name ?? $"ID {id}";

            var deleted = _productService.DeleteProduct(id);
            if (!deleted) return NotFound();

            _logger.LogInformation("Product '{Name}' (ID: {Id}) deleted.", name, id);
            TempData["SuccessMessage"] = $"🗑 Product '{name}' deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}