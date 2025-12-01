using Application.ClassDto;
using Application.Contracts;
using Application.Mappers;
using Application.ResultFactory;
using Domain.Interfaces;
using FluentResults;
using Shared;

namespace Application.Products.GetProductsWithPriceRange;

internal class GetProductsWithPriceRangeHandler : IQueryHandler<GetProductsWithPriceRange, PaginatedList<ProductDto>>
{
    private readonly IProductsRepository _repository;

    public GetProductsWithPriceRangeHandler(IProductsRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<PaginatedList<ProductDto>>> Handle(Products.GetProductsWithPriceRange.GetProductsWithPriceRange query, CancellationToken ct = default)
    {
        var products = await _repository
            .GetProductsWithPriceRange(query.PriceMin, query.PriceMax, query.PageNumber, query.PageSize, query.Queries, ct);

        var productsDto = products.MapTo(p => p.ToDto());
        
        return Result.Ok(productsDto);
        
    }
}