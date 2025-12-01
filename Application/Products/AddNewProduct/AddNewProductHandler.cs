using Application.Contracts;
using Domain;
using Domain.Interfaces;
using FluentResults;
using System.Runtime.CompilerServices;
using Application.ResultFactory;

[assembly: InternalsVisibleTo("UnitTests")]

namespace Application.Products.AddNewProduct;

internal class AddNewProductHandler : ICommandHandler<AddNewProduct>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductsRepository _repository;

    public AddNewProductHandler(IProductsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddNewProduct command, CancellationToken ct = default)
    {
        // 1. ETAP - tworzymy obiekt (encję) produktu)
        var product = Product.CreateProduct(command.Type);

        if(product.IsFailed)
            return ResultFailFactory.Fail("Product can't be build. Has error.", ErrCodeEnum.Conflict, causedBy: product.Errors);

        // 2. ETAP - dodajemy produkt do repo
        await _repository.AddProduct(product.Value, ct);
        
        // 3. ETAP - zapisanie zmian
        await _unitOfWork.CommitAsync(ct);
        
        return Result.Ok();
       
    }
}