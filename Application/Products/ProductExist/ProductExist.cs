using Application.Contracts;

namespace Application.Products.ProductExist;

public record ProductExist(string Id) : IQuery<bool>;