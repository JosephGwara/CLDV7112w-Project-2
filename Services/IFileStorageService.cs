using CLDV7112w_Project_2.Models;

namespace CLDV7112w_Project_2.Services
{
    public interface IFileStorageService
    {
        Task UploadProductAsync(Product product);
        Task DeleteProductAsync(string partitionKey, string rowKey);
    }
}
