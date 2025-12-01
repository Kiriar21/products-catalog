using Application.Contracts;

namespace Application.Products.PublishNewVersionProductSendObjectProduct;

public record PublishNewVersionProductSendObjectProduct(
    string IdProduct,
    string Name,
    decimal Price,
    int Amount,
    Dictionary<string, string> Resources,
    string ProductCategory,
    string GenerationRecord,
    string Kind) : ICommand 
{
}