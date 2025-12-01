using Domain.BusinessRules;
using Domain.Interfaces;
using Domain.VersionProductRecords;
using FluentResults;

namespace Domain;

//class of Version Product, inherit from EntityBase (ID, Validate forms) and IAuditableEntity (properties for modification)
public sealed class VersionProduct : EntityBase<Guid>, IAuditableEntity
{
    // =============== Version Product Properties ===============
    
    //name of version - value object
    public VersionName Name { get; private set; }
    //number of version - value object
    public VersionNumber VersionNumber { get; private set; }
    

    //ProductSpecification - information about product
    public ProductSpecification Spec { get; private set; }
    
    //flag of status version - default  false(is Active)
    public bool IsObsolete { get; private set; }

    
    //not important here
    public DateTimeOffset? CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; } 
    
    public DateTimeOffset? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    
        
    // ========================================================
    // =============== Building Version Product ===============
    // ========================================================

    //EF
    private VersionProduct():base(Guid.NewGuid()){}
    
    
    //basic constructor
    private VersionProduct(VersionName name, int versionNumber, bool isObsolete = false) : base(Guid.NewGuid())
    {
        Name = name;
        VersionNumber = VersionNumber.CreateNumber(versionNumber);
        IsObsolete = isObsolete;
    }

    //aggregate for creating and publishing new version of product with Product Specification
    public static Result<VersionProduct> PublishNewVersion(string name, int versionNumber,
        ProductSpecification? spec = null, bool isObsolete = false)
    {
        
        var tempName = VersionName.Create(name);
        
        if(tempName.IsFailed)
            return Result.Fail<VersionProduct>(tempName.Errors);
        
        var version = new VersionProduct(tempName.Value, versionNumber, isObsolete);
        
        if (spec != null)
            version.Spec = spec;
        
        return version;
    }

     
        
    // =================================================================
    // =============== Basic aggregates Version Product  ===============
    // =================================================================

    // set Version Number how to Active
    public Result ActivateOldVersionNumber()
    {
        var x = ValidateBusinessRule(new VersionMustBeObsoletedToBeActiveAgainBusinessRule(this));
        
        if (x.IsFailed)
            return x;
        
        IsObsolete = false;
        
        return x;
    }

    
    // set Version Number how to Obsolete
    public Result ObsoleteVersionNumber()
    {
        // if version is obsoleted - we can't do it again
        var x = ValidateBusinessRule(new VersionMustBeActiveToBecomeObsoletedBusinessRule(this));

        if (x.IsFailed)
            return x;
        
        IsObsolete = true;

        return x;
    }
    
          
    // ==================================================================
    // =============== Basic methods Version Product  ===================
    // ==================================================================
  
    
    // return information about number 
    public int GetNumberVersion()
    {
        return VersionNumber.GetNumber();
    }

    // return information about name 
    public string GetVersionName()
    {
        return Name.GetName();
    }
    
    // return information about status activity version
    public bool GetStatusVersion() => !IsObsolete;
    
    //information about Version without Product Specification
    public override string ToString()
    {
        return $"{Name}, {VersionNumber}, Is Obsolete? : {IsObsolete}";
    }
    
           
    // ==============================================================================
    // =============== Basic aggregate for ProductSpecification  ====================
    // ==============================================================================
    
    
    // add ProductSpecification 
    public void AddProductSpecification(ProductSpecification productSpecification) => Spec = productSpecification;
    
    // remove ProductSpecification
    public void RemoveProductSpecification() => Spec = null;
}