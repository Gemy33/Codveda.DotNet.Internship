// ============================================================
//  Controllers/HomeController.cs
//  Handles requests for the home page and contact form.
//  Controller = the "C" in MVC — receives requests,
//  calls services, and returns Views with data.
// ============================================================

using CodvedaMVC.Services;
using Level2_ASPNetCore_MVC_TaskManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodvedaMVC.Controllers
{
    public class HomeController : Controller
    {
        // ?? Dependency Injection ??????????????????????????????
        // ASP.NET Core automatically provides IProductService
        // because we registered it in Program.cs
        private readonly IProductService _productService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductService productService, ILogger<HomeController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // ?? GET / ?????????????????????????????????????????????
        // Action method ? returns a View with a ViewModel
        public IActionResult Index()
        {
            _logger.LogInformation("Home page requested.");
            var viewModel = _productService.BuildHomeViewModel();
            return View(viewModel);                  // ? Views/Home/Index.cshtml
        }

        // ?? GET /Home/About ???????????????????????????????????
        public IActionResult About()
        {
            // ViewBag: dynamic bag to pass small pieces of data
            ViewBag.Title = "About Codveda Store";
            ViewBag.Description = "A demo ASP.NET Core MVC application built for the Codveda Internship.";

            // ViewData: dictionary alternative to ViewBag
            ViewData["Tech"] = ".NET 9 · ASP.NET Core MVC · Razor · DI · Middleware";

            return View();
        }

        // ?? GET /Home/Contact ?????????????????????????????????
        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        // ?? POST /Home/Contact ????????????????????????????????
        [HttpPost]
        [ValidateAntiForgeryToken]       // CSRF protection — always use on POST actions
        public IActionResult Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)     // server-side validation
            {
                return View(model);      // re-display form with validation messages
            }

            // In a real app: send email, save to DB, etc.
            _logger.LogInformation("Contact form submitted by {Name} ({Email})", model.Name, model.Email);

            model.IsSubmitted = true;
            TempData["SuccessMessage"] = $"Thank you, {model.Name}! We'll be in touch soon.";

            return RedirectToAction(nameof(Contact));
        }

        
       
    }
}