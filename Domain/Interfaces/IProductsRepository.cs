using Shared;

namespace Domain.Interfaces;

public interface IProductsRepository
{
    Task AddProduct(Product product, CancellationToken ct = default);
    Task PublishNewVersionProduct(Product product, CancellationToken ct = default); 
    Task DeleteProduct(Guid id, CancellationToken ct = default);
    
    Task<Product?> GetProductById(Guid id, CancellationToken ct = default); 
    Task<Product?> GetProductByIdNoTrack(Guid id, CancellationToken ct = default); 
    
    Task<PaginatedList<Product>> GetProductsByStatus(string status, int pageNumber=1, int pageSize = 20, string? query = null, CancellationToken ct = default);
    
    Task<PaginatedList<Product>> GetProductsWithPriceRange(decimal priceLow, decimal priceHigh, int pageNumber=1, int pageSize = 20, string? query = null, CancellationToken ct = default);
    
    Task<bool> ProductExists(Guid id, CancellationToken ct = default); 
    Task<int> CountProducts(CancellationToken ct = default);
    
    Task<PaginatedList<Product>> GetPaginatedProductsList(int pageNumber=1, int pageSize = 20, string? query = null, CancellationToken ct = default);
   
}