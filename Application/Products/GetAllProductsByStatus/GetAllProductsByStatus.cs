using Application.ClassDto;
using Application.Contracts;
using Shared;

namespace Application.Products.GetAllProductsByStatus;

public record GetAllProductsByStatus(string Status, int PageNumber = 1, int PageSize = 20,  string? Queries = null) : IQuery<PaginatedList<ProductDto>>;