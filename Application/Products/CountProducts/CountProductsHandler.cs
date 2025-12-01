using Application.Contracts;
using Application.Products.GetProductById;
using Application.ResultFactory;
using Domain.Interfaces;
using FluentResults;

namespace Application.Products.CountProducts;

internal class CountProductsHandler : IQueryHandler<CountProducts, int>
{
    private readonly IProductsRepository  _repository;

    public CountProductsHandler(IProductsRepository productsRepository)
    {
        _repository = productsRepository;
    }

    public async Task<Result<int>> Handle(CountProducts query, CancellationToken ct = default)
    {
        return Result.Ok(await _repository.CountProducts(ct));
    }
}