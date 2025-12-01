using Application.Contracts;

namespace Application.Products.PublishNewVersionProductByTracking;

public record PublishNewVersionProductByTracking(string IdProduct, string Name, decimal Price, int Amount, Dictionary<string, string> Resources, string ProductCategory, string GenerationRecord, string Kind) : ICommand
{ }
