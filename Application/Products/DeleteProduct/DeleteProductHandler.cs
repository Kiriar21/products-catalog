using System.Data;
using System.Net;
using Application.Contracts;
using Application.ResultFactory;
using Domain.Interfaces;
using FluentResults;

namespace Application.Products.DeleteProduct;

internal class DeleteProductHandler : ICommandHandler<DeleteProduct>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductsRepository _repository;
    
    public DeleteProductHandler(IProductsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(DeleteProduct command, CancellationToken ct = default)
    {
        // get product 
        var productId = Guid.Parse(command.Id);
        var product = await _repository.GetProductById(productId,ct);
        
        // if product not exist - result fail
        if (product == null)
            return ResultFailFactory.Fail("Product not found",  ErrCodeEnum.NotFound);
        
        //1. try delete product
        await _repository.DeleteProduct(productId, ct);

        //2. try save changes
        await _unitOfWork.CommitAsync(ct);
        
        //3. return status ok
        return Result.Ok();
    }
}