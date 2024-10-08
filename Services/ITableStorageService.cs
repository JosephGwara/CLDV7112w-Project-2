using CLDV7112w_Project_2.Models;

namespace CLDV7112w_Project_2.Services
{
    public interface ITableStorageService
    {
        Task AddProductAsync(Product product);
        Task<Product> GetProductAsync(string partitionKey, string rowKey);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(string partitionKey, string rowKey);
    }
}
