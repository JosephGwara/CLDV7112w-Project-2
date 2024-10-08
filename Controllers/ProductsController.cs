using CLDV7112w_Project_2.Models;
using CLDV7112w_Project_2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CLDV7112w_Project_2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ITableStorageService tableStorageService,
                                  IBlobStorageService blobStorageService,
                                  IFileStorageService fileStorageService,
                                  ILogger<ProductsController> logger)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetProductsAsync();
            if (products == null)
            {
                _logger.LogWarning("GetProductsAsync returned null.");
                products = new List<Product>();
            }
            return View(products);
        }

        // GET: Products/Details/partitionKey/rowKey
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                return BadRequest();
            }

            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.PartitionKey = "Default"; 
                product.RowKey = Guid.NewGuid().ToString();

                try
                {
                    await _tableStorageService.AddProductAsync(product);
                    await _blobStorageService.UploadProductAsync(product);
                    await _fileStorageService.UploadProductAsync(product);

                    _logger.LogInformation($"Product {product.RowKey} created successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating product {product.RowKey}.");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                }
            }
            return View(product);
        }

        // GET: Products/Edit/partitionKey/rowKey
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                return BadRequest();
            }

            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _tableStorageService.UpdateProductAsync(product);
                    await _blobStorageService.UploadProductAsync(product); // Assuming re-uploading
                    await _fileStorageService.UploadProductAsync(product); // Re-upload to Azure Files

                    _logger.LogInformation($"Product {product.RowKey} updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating product {product.RowKey}.");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                }
            }
            return View(product);
        }

        // GET: Products/Delete/partitionKey/rowKey
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                return BadRequest();
            }

            var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            try
            {
                await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);
                await _blobStorageService.DeleteProductBlobAsync(rowKey);
                

                _logger.LogInformation($"Product {rowKey} deleted successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {rowKey}.");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
                var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
        }
    }
}
