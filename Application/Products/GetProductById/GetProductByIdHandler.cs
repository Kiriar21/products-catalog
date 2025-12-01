using Application.ClassDto;
using Application.Contracts;
using Application.Mappers;
using Application.Products.ClassDto;
using Application.ResultFactory;
using Domain;
using Domain.Interfaces;
using FluentResults;

namespace Application.Products.GetProductById;

internal class GetProductByIdHandler : IQueryHandler<GetProductById,ProductDto>
{
    private readonly IProductsRepository _repository;

    public GetProductByIdHandler(IProductsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProductDto>> Handle(GetProductById query, CancellationToken ct = default)
    {
        var productId = Guid.Parse(query.Id);
        var product = await _repository.GetProductById(productId, ct);
        
        if(product == null)
            return ResultFailFactory.Fail<ProductDto>("Product not found.",  ErrCodeEnum.NotFound);
        
        var productDto = product.ToDto();
        
        return Result.Ok(productDto);
    }
}