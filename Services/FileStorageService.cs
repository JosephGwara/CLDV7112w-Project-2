using Azure.Storage.Files.Shares;
using CLDV7112w_Project_2.Models;
using System.Text.Json;

namespace CLDV7112w_Project_2.Services
{
    public class FileStorageService : IFileStorageService
    {

        private readonly ShareClient _shareClient;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            string connectionString = configuration.GetConnectionString("AzureTableStorage");
            string shareName = "products"; 

            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();

            _logger = logger;
        }

        public async Task UploadProductAsync(Product product)
        {
            try
            {
                
                string productJson = JsonSerializer.Serialize(product);

               
                string directoryName = "products";
                string fileName = $"product-{product.RowKey}.json";

               
                ShareDirectoryClient directoryClient = _shareClient.GetDirectoryClient(directoryName);
                await directoryClient.CreateIfNotExistsAsync();

                
                ShareFileClient fileClient = directoryClient.GetFileClient(fileName);

            
                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(productJson)))
                {
                    await fileClient.CreateAsync(stream.Length);
                    await fileClient.UploadAsync(stream);
                }

                _logger.LogInformation($"Product {product.RowKey} uploaded to Azure Files as {fileName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload product {product.RowKey} to Azure Files.");
              
                throw;
            }
        }
        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                string directoryName = "products";
                string fileName = $"product-{rowKey}.json";

                ShareDirectoryClient directoryClient = _shareClient.GetDirectoryClient(directoryName);
                ShareFileClient fileClient = directoryClient.GetFileClient(fileName);

                await fileClient.DeleteIfExistsAsync();

                _logger.LogInformation($"Product file {fileName} deleted from Azure Files.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete product file {rowKey} from Azure Files.");
                throw;
            }
        }

    }
}
