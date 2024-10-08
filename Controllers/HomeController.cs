using CLDV7112w_Project_2.Models;
using CLDV7112w_Project_2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CLDV7112w_Project_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ITableStorageService tableStorageService, ILogger<HomeController> logger)
        {
            _tableStorageService = tableStorageService;
            _logger = logger;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _tableStorageService.GetProductsAsync();

                if (products == null)
                {
                    _logger.LogWarning("GetProductsAsync returned null.");
                    products = new List<Product>(); // Initialize to prevent null reference in view
                }

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                // Optionally, you can return a specific error view or a user-friendly message
                return View("Error");
            }
        }

        // Optional: Other default actions like Privacy, Error, etc.
        public IActionResult Privacy()
        {
            return View();
        }

        // Optional: Error handling action
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
    

