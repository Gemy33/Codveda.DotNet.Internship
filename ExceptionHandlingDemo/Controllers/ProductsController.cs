using ExceptionHandlingDemo.Exceptions;
using ExceptionHandlingDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ExceptionHandlingDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop" },
            new Product { Id = 2, Name = "Mouse" }
        };

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                Log.Information("Fetching product with ID {ProductId}", id);

                var product = products.FirstOrDefault(p => p.Id == id);

                if (product == null)
                    throw new ProductNotFoundException("Product not found");

                return Ok(product);
            }
            catch (ProductNotFoundException ex)
            {
                Log.Warning(ex, "Product not found");

                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}