using System.Net;
using Application.Contracts;
using Application.ResultFactory;
using Domain;
using Domain.Interfaces;
using Domain.TaxonomyRecords;
using FluentResults;

namespace Application.Products.PublishNewVersionProductSendObjectProduct;

public class PublishNewVersionProductSendObjectProductHandler : ICommandHandler<PublishNewVersionProductSendObjectProduct>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductsRepository _repository;

    public PublishNewVersionProductSendObjectProductHandler(IUnitOfWork unitOfWork, IProductsRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }
   
    public async Task<Result> Handle(PublishNewVersionProductSendObjectProduct command, CancellationToken ct = default)
    {
        var productId = Guid.Parse(command.IdProduct);
        var product = await _repository.GetProductByIdNoTrack(productId, ct);
        
        if(product == null)
            return ResultFailFactory.Fail("Product not found.", ErrCodeEnum.NotFound);


        //creating taxonomy
        var kind = Kind.Create(command.Kind);
        
        if(kind.IsFailed)
            return ResultFailFactory.Fail($"Cannot create product with kind {command.Kind}.", ErrCodeEnum.Conflict,  causedBy: kind.Errors);
        
        var productCategory = ProductCategory.Create(command.ProductCategory);
        
        if(productCategory.IsFailed)
            return ResultFailFactory.Fail($"Cannot create product with product category {command.ProductCategory}.",  ErrCodeEnum.Conflict, causedBy: productCategory.Errors);
        
        var generationRecord = GenerationRecord.Create(command.GenerationRecord);

        if(generationRecord.IsFailed)
            return ResultFailFactory.Fail($"Cannot create product with product generation {command.GenerationRecord}.", ErrCodeEnum.Conflict,  causedBy: generationRecord.Errors);
        
        var taxonomy = Taxonomy.Create(productCategory.Value, generationRecord.Value, kind.Value);
   
        if(taxonomy.IsFailed)
            return ResultFailFactory.Fail($"Cannot create product with product taxonomies.",  ErrCodeEnum.Conflict, causedBy: taxonomy.Errors);
        
        
        //creating spec
        var spec = ProductSpecification.GenerateProductSpecification(command.Price, taxonomy.Value, command.Amount);
        
        if (spec.IsFailed)
            return ResultFailFactory.Fail("Cannot create specification.",  ErrCodeEnum.Conflict, causedBy:spec.Errors);
        
        foreach (var r in command.Resources)
        {
            var res = Resource.CreateResource(r.Key, r.Value);
            if(res.IsFailed)
                return ResultFailFactory.Fail("Invalid resource.",  ErrCodeEnum.Conflict, causedBy: res.Errors);
            
            spec.Value.AddResource(res.Value);
        }
        
        //add new version
        var newVersion = product.PublishNewVersion(command.Name, spec.Value);
        
        if(newVersion.IsFailed)
            return ResultFailFactory.Fail("When try to create new version product, we got the problem.",  ErrCodeEnum.Unexpected, causedBy: newVersion.Errors);

        await _repository.PublishNewVersionProduct(product, ct);

        await _unitOfWork.CommitAsync(ct);
        
        return Result.Ok();
    }
}