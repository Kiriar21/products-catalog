using System.Text;
using Domain.BusinessRules;
using Domain.Interfaces;
using Domain.ProductRecords;
using Domain.ProductSpecificationRecords;
using Domain.TaxonomyRecords;
using FluentResults;

namespace Domain;

//class inherits from main root interface EntityBase and others interfaces using to better and clean coding
public sealed class Product : EntityBase<Guid>, IAggregateRoot, IAuditableEntity
{
    // =============== Product Properties ===============
    
    //private list for adding and modification Products
    private readonly List<VersionProduct> _versions = [];
    
    // enums for life cycle and product type - better organization  
    public ProductType Type { get; private set; }
    public LifeCycleStatus LifeCycleStatus { get; private set; }
    
    //public read only list for checking our products - everything can use that to see all versions of product 
    public IReadOnlyCollection<VersionProduct> Versions => _versions.AsReadOnly();
    
    
    // not important here
    public DateTimeOffset? CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; } 
    
    public DateTimeOffset? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    
    
    
    // ================================================
    // =============== Building Product ===============
    // ================================================
    
    
    // EF
    private Product():base(Guid.NewGuid()){}
    
    // basic constructor
    private Product(ProductType type) : base(Guid.NewGuid())
    {
        Type = type;
        var lifeCycleStatus = LifeCycleStatus.Create("Draft").Value;
        LifeCycleStatus = lifeCycleStatus;
    }
    
    //basic class for generating new Product Draft - with type only
    public static Result<Product> CreateProduct(string type)
    {
        var productType = ProductType.Create(type);
        
        return productType.IsFailed ? Result.Fail<Product>(productType.Errors) : Result.Ok(new Product(productType.Value));
    }  

    //aggregate for generating new Product with own version and will be active already
    public static Result<Product> CreateProduct(VersionProduct versionProduct, string type)
    {
        var productType = ProductType.Create(type);

        if (productType.IsFailed)
            return Result.Fail<Product>(productType.Errors);
        
        var netProd = new Product(productType.Value);
        netProd._versions.Add(versionProduct);
        netProd.ActivateProduct();
        return netProd;
    }
    
     
    // ===========================================================
    // =============== Building Versions =========================
    // ===========================================================

    
    // publish new version of product
    public Result PublishNewVersion(string name, ProductSpecification? spec = null)
    {
        // test name - cannot be empty 
        var result = ValidateBusinessRule(new NameOfTheVersionCannotBeEmpty(name));
        
        if (result.IsFailed)
            return result;

        if (spec != null)
        {
            var testTaxonomy = TaxonomyRules.IsAllowed(spec.GetTaxonomy().GetCategory(), spec.GetTaxonomy().GetKind());
            if (testTaxonomy.IsFailed)
                return Result.Fail(testTaxonomy.Errors);
            
            var categoryCheck = ProductTaxonomyRules.IsAllowed(GetProductType,spec.GetTaxonomy().GetCategory());
            if (categoryCheck.IsFailed)
                return Result.Fail(categoryCheck.Errors);
        }
        

        var tempVersion = VersionProduct.PublishNewVersion(name, NextNumberVersion(), spec);
        
        if(tempVersion.IsFailed)
            return Result.Fail(tempVersion.Errors);
        
        _versions.Add(tempVersion.Value);
        
        var result2 = ActivateProduct();

        return result2.IsFailed ? result2 : result;
    }
    
    
    // ===========================================================
    // =============== Basic Aggregate for Product ===============
    // ===========================================================
    
    
    // set the product as active  
    private Result ActivateProduct()
    {
       var result = ValidateBusinessRule(new ProductMustHaveVersionBusinessRule(this));

       if (result.IsFailed)
           return result;
       
       var lifeCycleStatus = LifeCycleStatus.Create("Active").Value;
       LifeCycleStatus = lifeCycleStatus;
       return result;
    }

    // set the product as archived
    public Result DeactivateProduct()
    {
        // test - all versions are obsoleted
        var result = ValidateBusinessRule(new AllOfProductVersionsMustBeObsoletedBusinessRule(this));

        if (result.IsFailed)
            return result;
        
        var lifeCycleStatus = LifeCycleStatus.Create("Archived").Value;
        LifeCycleStatus = lifeCycleStatus;
        return result;
    }
    
    // set the product as draft
    public Result ProductAsADraft()
    {
        // test for that product doesn't have any active version 
        var result = ValidateBusinessRule(new AllOfProductVersionsMustBeObsoletedBusinessRule(this));
        
        if (result.IsFailed)
            return result;
        var lifeCycleStatus = LifeCycleStatus.Create("Draft").Value;
        LifeCycleStatus = lifeCycleStatus;
        return result;
    }
    
    
    // changing type of product
    public Result ChangingTypeOfProduct(string type)
    {
        var tempType = ProductType.Create(type);
        
        if(tempType.IsFailed)
            return Result.Fail(tempType.Errors);
        
        //test - existing type of product and the product changed type as not the same as the product now (for smaller amount of queries to DB)
        var result = ValidateBusinessRule(new ChangedTypeMustBeOnTheListOfProductTypesBusinessRule(tempType.Value), new ChangedTypeCannotBeTheSameTypeBusinessRule(this, tempType.Value));
        
        if (result.IsFailed)
            return result;
        
        Type = tempType.Value;
        return result;
    }
    
    // get information about Type
    public ProductType GetProductType => Type;
    
    // get information about Life Cycle Status
    public LifeCycleStatus GetLifeCycleStatus => LifeCycleStatus;
    
    // ====================================================================
    // =============== Basic aggregates for List of Versions ===============
    // ====================================================================
    
    // activate another version - rest of must be deactivated
    public Result ActivateAnotherVersion(int versionNumber)
    {
        // test for versionNumber - must be greater than 0
        // test for number of versions - must be gt 0
        // test for number of versions - must be existed 
        // test - number of version must be existed

        var result = ValidateBusinessRule(
            new ActivateNumberVersionMustBeGreaterThanZeroBusinessRule(versionNumber),
            new AmountOfVersionsMustBeGraterThanZeroBusinessRule(this),
            new ProductMustHaveVersionBusinessRule(this),
            new NumberOfVersionMustBeExistedBusinessRule(this, versionNumber));

        if (result.IsFailed)
        {
            return result;
        }
        
        //deactivate another version 
        DeactivateCurrentVersion();
        
        // activate currently selected version 
        var version =  Versions.FirstOrDefault(x => x.GetNumberVersion() == versionNumber);

        version?.ActivateOldVersionNumber();

        return result;
    }
    
    // obsoleting currently active version
    public Result DeactivateCurrentVersion()
    {
        // test for number of versions - must be gt 0
        var result = ValidateBusinessRule(new AmountOfVersionsMustBeGraterThanZeroBusinessRule(this));
        
        if (result.IsFailed)
            return result;
        
        var version = _versions.FirstOrDefault(x => !x.IsObsolete);

        if (version == null)
            return result;
        
        version.ObsoleteVersionNumber();
        
        //don't have any active or any versions - must be draft
        ProductAsADraft();
        
        return result;
    }
    
    // delete version
    public Result DeleteVersion(int versionNumber)
    {
        // test for versionNumber - must be greater than 0
        // test for number of versions - must be gt 0
        // test for number of versions - must be existed 
        // test - number of version must be existed
        
        var result = ValidateBusinessRule(
            new ActivateNumberVersionMustBeGreaterThanZeroBusinessRule(versionNumber),
            new AmountOfVersionsMustBeGraterThanZeroBusinessRule(this),
            new ProductMustHaveVersionBusinessRule(this),
            new NumberOfVersionMustBeExistedBusinessRule(this, versionNumber));
        
        if(result.IsFailed)
            return result;
        
        var version = Versions.FirstOrDefault(x => x.GetNumberVersion() == versionNumber);
        
        // test for obsolete version - if this is active version - do it DeactivateCurrentVersion

        result = ValidateBusinessRule(new VersionAboutNumberIsActiveBusinessRule(this, versionNumber));
        
        // must reverse the result - if our version is active we must be deactivated this  
        if (!result.IsFailed)
            DeactivateCurrentVersion();
        
        if (version != null) _versions.Remove(version);
        
        return result;
    }

    // ============================================
    // ==== Getting information about versions ====
    // ============================================
    


    // getting information about currently set version
    public Result<string> CurrentVersionInformation()
    {
        // test - number of versions - must be gt 0
        // test - must be set any active version
        var result = ValidateBusinessRule(new ProductMustHaveVersionBusinessRule(this), new AnyVersionIsActiveBusinessRule(this));

        return result.IsFailed ? result : CurrentVersion().ToString();
    }
    
    //getting information about version

    public Result<VersionProduct> GetVersionProduct(int versionNumber)
    {
        // test - number of versions - must be gt 0
        
        var result = ValidateBusinessRule(new ProductMustHaveVersionBusinessRule(this));
        
        return result.IsFailed ? result : Result.Ok(Versions.FirstOrDefault(x => x.GetNumberVersion() == versionNumber));
    }
    
    // getting currently active version
    public VersionProduct CurrentVersion() => Versions.FirstOrDefault(x => !x.IsObsolete)!;

    // getting information about all of version
    public Result<string> GettAllVersionsInformation()
    {
        // test - number of versions - must be gt 0
        var result = ValidateBusinessRule(new ProductMustHaveVersionBusinessRule(this));

        if (result.IsFailed)
            return result.ToString();
        
        var builder = new StringBuilder();
        
        foreach (var version in Versions)
        {
            builder.AppendLine(version.ToString());
            builder.AppendLine();
        }
        
        return builder.ToString();
    }
    
    // ====================================================
    // =============== Additional functions ===============
    // ====================================================
    
    // calculating the next number of version
    private int NextNumberVersion()
    {
        //find count and add one more to score or return 1 (first version)
        var numberOfVersion = Versions.Count > 0 ? Versions.Max(x => x.GetNumberVersion()) + 1 : 1 ; 
        
        //if we are publishing new version, we're doing obsolete to the last of active version
        if(Versions.Count > 0)
            DeactivateCurrentVersion();
        
        return numberOfVersion;
    }

    //return guid id
    public Guid GetGuidId() => Id;
    public string GetId() => Id.ToString();
    
}