using Application.ClassDto;
using Application.Contracts;
using Application.Mappers;
using Application.ResultFactory;
using Domain.Interfaces;
using FluentResults;
using Shared;

namespace Application.Products.GetPaginatedProducts;

internal class GetPaginatedProductsHandler : IQueryHandler<GetPaginatedProducts, PaginatedList<ProductDto>>
{
    private readonly IProductsRepository _repository;

    public GetPaginatedProductsHandler(IProductsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<PaginatedList<ProductDto>>> Handle(GetPaginatedProducts query, CancellationToken ct = default)
    {
        var products = await _repository.GetPaginatedProductsList(query.PageNumber, query.PageSize, query.queries, ct);
        var productsDto = products.MapTo(p => p.ToDto());
        
        return Result.Ok(productsDto);
    }
}