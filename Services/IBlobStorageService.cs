using CLDV7112w_Project_2.Models;

namespace CLDV7112w_Project_2.Services
{
    public interface IBlobStorageService
    {
        Task UploadProductAsync(Product product);
        Task DeleteProductBlobAsync(string rowKey);
    }
}
