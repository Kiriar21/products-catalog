using Application.ClassDto;
using Application.Contracts;
using Shared;

namespace Application.Products.GetProductsWithPriceRange;

public record GetProductsWithPriceRange(decimal PriceMin, decimal PriceMax, int PageNumber = 1, int PageSize = 20, string? Queries = null) : IQuery<PaginatedList<ProductDto>>;