using Domain.TaxonomyRecords;
using FluentResults;

namespace Domain;

public class Taxonomy:EntityBase<Guid>
{
    // basic properties of taxonomy
    public ProductCategory ProductCategory { get; private set; }
    public GenerationRecord GenerationRecord { get; private set; }
    public Kind Kind { get; private set;}
    
    
    //EF
    private Taxonomy(Guid id) : base(id)
    {
    }

    //basic constructor
    private Taxonomy(ProductCategory productCategory, GenerationRecord generationRecord, Kind kind):base(Guid.NewGuid())
    {
        ProductCategory = productCategory;
        GenerationRecord = generationRecord;
        Kind = kind;
    }

    //public method to create object Taxonomy
    public static Result<Taxonomy> Create(ProductCategory productCategory, GenerationRecord generationRecord, Kind kind)
    {
        var result = TaxonomyRules.IsAllowed(productCategory, kind);

        return result.IsFailed 
            ? Result.Fail<Taxonomy>(result.Errors) 
            : Result.Ok(new Taxonomy(productCategory, generationRecord, kind));
    }
    
    //get Category
    public ProductCategory GetCategory() => ProductCategory;
    //get Generation
    public GenerationRecord GetGeneration() => GenerationRecord;
    //get Kind
    public Kind GetKind() => Kind;
    
    //return allowed kinds for category
    public static Result<IEnumerable<Kind>> GetAllowedKinds(ProductCategory productCategory)
    {
        var x = TaxonomyRules.GetAllowedKinds(productCategory);
        return x.IsFailed ? Result.Fail<IEnumerable<Kind>>(x.Errors) : Result.Ok(x.Value);
    }

    //return allowed name of kinds for category 
    public static Result<IEnumerable<string>> GetAllowedKindsNames(ProductCategory productCategory)
    {
        var x = TaxonomyRules.GetAllowedKinds(productCategory);
        return x.IsFailed 
            ? Result.Fail<IEnumerable<string>>(x.Errors) 
            : Result.Ok(x.Value.Select(kind => kind.ToString()));
    }

}