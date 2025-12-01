using Application.Contracts;
using Application.ResultFactory;
using Domain.Interfaces;
using FluentResults;

namespace Application.Products.ProductExist;

internal class ProductExistHandler : IQueryHandler<ProductExist, bool>
{
    private readonly IProductsRepository  _repository;

    public ProductExistHandler(IProductsRepository productsRepository)
    {
        _repository = productsRepository;
    }
    
    public async Task<Result<bool>> Handle(ProductExist query, CancellationToken ct = default)
    {
        var productId = Guid.Parse(query.Id);
        return Result.Ok(await _repository.ProductExists(productId, ct));;
    }
}