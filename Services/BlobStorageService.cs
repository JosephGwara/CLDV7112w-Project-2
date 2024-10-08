using Azure.Storage.Blobs;
using CLDV7112w_Project_2.Models;
using System.Text.Json;

namespace CLDV7112w_Project_2.Services
{
    public class BlobStorageService : IBlobStorageService
    {

        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            string connectionString = configuration.GetConnectionString("AzureTableStorage");
            string containerName = "products"; 

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(); 

            _logger = logger;
        }
        public async Task UploadProductAsync(Product product)
        {
            try
            {
               
                string productJson = JsonSerializer.Serialize(product);

             
                string blobName = $"product-{product.RowKey}.json";

               
                BlobClient blobClient = _containerClient.GetBlobClient(blobName);

                
                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(productJson)))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                _logger.LogInformation($"Product {product.RowKey} uploaded to Blob Storage as {blobName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload product {product.RowKey} to Blob Storage.");
               
                throw;
            }
        }

        public async Task DeleteProductBlobAsync(string rowKey)
        {
            try
            {
                string blobName = $"product-{rowKey}.json";
                BlobClient blobClient = _containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();

                _logger.LogInformation($"Blob {blobName} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete blob for product {rowKey}.");
              
                throw;
            }
        }


    }
}
    
