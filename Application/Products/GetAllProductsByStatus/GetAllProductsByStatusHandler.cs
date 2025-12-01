using System.Net;
using Application.ClassDto;
using Application.Contracts;
using Application.Mappers;
using Application.Products.ClassDto;
using Application.ResultFactory;
using Domain;
using Domain.Interfaces;
using Domain.ProductRecords;
using FluentResults;
using Shared;


namespace Application.Products.GetAllActiveProducts;

internal class GetAllProductsByStatusHandler : IQueryHandler<GetAllProductsByStatus.GetAllProductsByStatus, PaginatedList<ProductDto>>
{
    private readonly IProductsRepository _repository;

    public GetAllProductsByStatusHandler(IProductsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<PaginatedList<ProductDto>>> Handle(GetAllProductsByStatus.GetAllProductsByStatus query, CancellationToken ct = default)
    {
        var userLifeCycleStatus = LifeCycleStatus.Create(query.Status);
        
        if (userLifeCycleStatus.IsFailed)
            return ResultFailFactory.Fail<PaginatedList<ProductDto>>($"Cannot get products with status {query.Status}.", ErrCodeEnum.Conflict, causedBy: userLifeCycleStatus.Errors);

        var products = await _repository.GetProductsByStatus(query.Status,query.PageNumber, query.PageSize , query.Queries, ct);

        var productsDto = products.MapTo(p => p.ToDto());
        
        return Result.Ok(productsDto);
    }
}