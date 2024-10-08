using Azure;
using Azure.Data.Tables;
using CLDV7112w_Project_2.Models;

namespace CLDV7112w_Project_2.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableClient _tableClient;

        public TableStorageService(IConfiguration configuration)
        {

            string connectionString = configuration.GetConnectionString("AzureTableStorage");
            _tableClient = new TableClient(connectionString, "Products");
            _tableClient.CreateIfNotExists();
        }
        public async Task AddProductAsync(Product product)
        {
            await _tableClient.AddEntityAsync(product);
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<Product> GetProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<Product>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _tableClient.UpdateEntityAsync(product, product.ETag, TableUpdateMode.Replace);
        }
    }
}
