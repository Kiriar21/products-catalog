using Application.ClassDto;
using Application.Contracts;
using FluentResults;
using Shared;

namespace Application.Products.GetPaginatedProducts;

public sealed record GetPaginatedProducts(int PageNumber, int PageSize, string? queries = null) : IQuery<PaginatedList<ProductDto>>
{ }