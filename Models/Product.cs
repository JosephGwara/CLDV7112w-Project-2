using Azure;
using Azure.Data.Tables;

namespace CLDV7112w_Project_2.Models
{
    public class Product : ITableEntity
    {
        public string? PartitionKey { get ; set ; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
}
