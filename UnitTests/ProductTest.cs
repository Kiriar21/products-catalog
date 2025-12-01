using Domain;
using Domain.ProductRecords;
using Domain.ProductSpecificationRecords;
using Domain.TaxonomyRecords;
using FluentAssertions;

namespace UnitTests;

public class ProductTest
{
    //=================================================================================
    //==================================Product Tests==================================
    //=================================================================================
    
    [Fact]
    public void CreateProductWithoutVersion_ShouldReturnProductWithoutVersion()
    {
        // arrange
        const string type = "VirtualMachine";

        // act
        var product = Product.CreateProduct(type).Value;

        // assert
        product.GetProductType.GetProductTypeName().Should().Be(type);
    }

    [Fact]
    public void CreateProduct_ShouldReturnProductWithVersion()
    {
        // arrange
        var product = Product.CreateProduct("VirtualMachine").Value;

        // act
        product.PublishNewVersion("test");

        // assert
        product.Versions.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public void CreateProductAfterActivation_ShouldReturnProductWithActivation()
    {
        // arrange
        var product = Product.CreateProduct("VirtualMachine").Value;
        var lifeCycleStatus = LifeCycleStatus.Create("Active").Value;

        // act
        product.PublishNewVersion("test");

        // assert
        product.GetLifeCycleStatus.Should().Be(lifeCycleStatus);

    }
    
    // deactivate product - without versions 
    [Fact]
    public void DeactivateProduct_ShouldReturnDeactivatedProduct()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        var lifeCycleStatus = LifeCycleStatus.Create("Archived").Value;
        product.PublishNewVersion("test");
        product.DeactivateCurrentVersion();
        product.DeactivateProduct();
    
        product.GetLifeCycleStatus.Should().Be(lifeCycleStatus);
    }
    
    // product can be draft without any active versions
    [Fact]
    public void ProductCanBeDraft_ShouldReturnDraftProductWithoutVersion()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        var lifeCycleStatus = LifeCycleStatus.Create("Draft").Value;
        product.PublishNewVersion("test");
        product.DeactivateCurrentVersion();
        product.ProductAsADraft();
        product.GetLifeCycleStatus.Should().Be(lifeCycleStatus);
    }
    
    
    // changing type of product
    [Fact]
    public void TypeOfProductChanging_ShouldReturnChangedTypeOfProduct()
    {
        var product = Product.CreateProduct("Service").Value;
        product.PublishNewVersion("test");
        product.ChangingTypeOfProduct("VirtualMachine");
        var productTypes = ProductType.GetProductTypes();
        product.GetProductType.GetProductTypeName().Should().Be("VirtualMachine");
        }
    
    // deactivate current version - with one version
    [Fact]
    public void DeactivateProductWithOneVersion_ShouldReturnProductAsADraft()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        var lifeCycleStatus = LifeCycleStatus.Create("Draft").Value;
        product.PublishNewVersion("test");
        product.DeactivateCurrentVersion();
        product.GetLifeCycleStatus.Should().Be(lifeCycleStatus);
        
    }
    
    // new version is active version
    [Fact]
    public void NewVersionIsActiveVersion_ShouldReturnActiveVersion()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        product.PublishNewVersion("test");
        product.PublishNewVersion("test2");
        var version = product.CurrentVersion();
        version.GetNumberVersion().Should().Be(2);
    
    }
    
    // activate another version
    [Fact]
    public void ActivateAnotherVersion_ShouldReturnAnotherActiveVersion()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        product.PublishNewVersion("test");
        product.PublishNewVersion("test2");
        product.ActivateAnotherVersion(1);
        var version = product.CurrentVersion();
        version.GetNumberVersion().Should().Be(1);
    }
    
    // delete version (many version)
    [Fact]
    public void DeleteActiveVersion_ShouldReturnProductAsADraft()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        product.PublishNewVersion("test");
        product.PublishNewVersion("test2");
        product.ActivateAnotherVersion(1);
        product.DeleteVersion(2);
        var version = product.CurrentVersion();
        version.GetNumberVersion().Should().Be(1);
    }
    
    // delete version (last/one version - product must become a draft)
    [Fact]
    public void ProductCanBeDraft_ShouldReturnDraftProductWithoutVersion4()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        var lifeCycleStatus = LifeCycleStatus.Create("Draft").Value;
        product.PublishNewVersion("test");
        product.DeleteVersion(1);
        product.GetLifeCycleStatus.Should().Be(lifeCycleStatus);
        
    }
    
    // getting active version information
    [Fact]
    public void GettingInformationAboutVersion_ShouldReturnInformationAboutVersion()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        product.PublishNewVersion("test");
        var version = product.CurrentVersion();
        version.GetStatusVersion().Should().BeTrue();
    }
    
    // getting information about all of versions
    [Fact]
    public void GettingInformationAboutAllOfVersions_ShouldReturnInformationAboutAllOfVersions()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        product.PublishNewVersion("test");
        product.PublishNewVersion("test2");
        product.PublishNewVersion("test3");
        var information = product.GettAllVersionsInformation();
        information.IsSuccess.Should().BeTrue();
    }
    
    // getting version information without any version
    
    [Fact]
    public void GettingInformationAboutVersionWithoutVersion_ShouldReturnInformationAboutVersionWithoutVersion()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        product.CurrentVersionInformation().IsSuccess.Should().BeFalse();
    }
    
    //==================================================================================================
    //==================================Version Without Spec Tests==================================
    //==================================================================================================
    
    // creating version must have any gt 0 number
    [Fact]
    public void CreatedVersionMustHaveNumberGT0_ShouldReturnVersionWithNumberGreaterThanZero()
    {
        var version = VersionProduct.PublishNewVersion("test", 1).Value;
        version.GetNumberVersion().Should().BeGreaterThan(0);
    }
    
    // creating version with number 0 or below must be error
    [Fact]
    public void CreatedVersionWithNumberLT1_ShouldReturnError()
    {
        Assert.Throws<ArgumentException>(() => VersionProduct.PublishNewVersion("test", 0));
    }
    
    // version must return her number
    [Fact]
    public void VersionMustGiveHerNumber_ShouldReturnNumber()
    {
        var version = VersionProduct.PublishNewVersion("test", 1).Value;
        version.GetNumberVersion().Should().BeGreaterThan(0);
    }
    
    // version must return her name 
    [Fact]
    public void VersionMustGiveHerName_ShouldReturnName()
    {
        var version = VersionProduct.PublishNewVersion("test", 1).Value;
        version.GetVersionName().Should().Be("test");
    }
    
    // version after add must be active
    [Fact]
    public void VersionAfterAddMustBeActive_ShouldReturnActiveVersion()
    {
        var version = VersionProduct.PublishNewVersion("test", 1).Value;
        version.GetStatusVersion().Should().BeTrue();
    }
    
    //==================================================================================================
    //==================================Version With Spec Tests=====================================
    //==================================================================================================
    
    // add new version with spec
    [Fact]
    public void VersionWithoutSpecCanAddNewSpec_ShouldReturnVersionWithSpec()
    {
        var version = VersionProduct.PublishNewVersion("test", 1).Value;
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.AddResource("CPU", "2");
        spec.AddResource("RAM", "4");
        spec.AddResource("SSD", "100");
        version.AddProductSpecification(spec);
        version.Spec.Should().NotBeNull();
    }
    
    // remove spec from version
    [Fact]
    public void VersionWithSpecCanRemoveSpec_ShouldReturnVersionWithoutSpec()
    {
        var version = VersionProduct.PublishNewVersion("test", 1).Value;
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.AddResource("CPU", "2");
        spec.AddResource("RAM", "4");
        spec.AddResource("SSD", "100");
        version.AddProductSpecification(spec);
        version.RemoveProductSpecification();
        version.Spec.Should().BeNull();
    }
    
    // new version with spec and taxonomy should give access to create full product with correct Category and Type of product 
    [Fact]
    public void FullVersionWithCorrectTaxonomy_ShouldReturnVersionWithCorrectTaxonomy()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;

        var productCategory = ProductCategory.Create("VirtualMachine").Value;
        var productGeneration = GenerationRecord.Create("G1").Value;
        var productKind = Kind.Create("Amd").Value;
        var taxonomy = Taxonomy.Create(productCategory, productGeneration, productKind ).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        var test = product.PublishNewVersion("test", spec);
        
        test.IsSuccess.Should().BeTrue();
    }
    
    // new version with spec and taxonomy should give error to create full product with incorrect Category and Type of product 
    [Fact]
    public void FullVersionWithIncorrectTaxonomy_ShouldReturnError()
    {
        var product = Product.CreateProduct("VirtualMachine").Value;
        var taxonomy = Taxonomy.Create(ProductCategory.Create("Database").Value, GenerationRecord.Create("G1").Value, Kind.Create("MicrosoftStd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        var test = product.PublishNewVersion("test", spec);
        test.IsSuccess.Should().BeFalse();
    }
    
    //==================================================================================================
    //==================================Spec Tests==================================================
    //==================================================================================================
    
    //add new spec without resources
    [Fact]
    public void NewSpecWithoutResources_ShouldReturnSpecWithoutResources()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.GetResources().Should().BeEmpty();
    }
    
    //add new spec with one recourse as a value
    [Fact]
    public void NewSpecWithResourcesAsValue_ShouldReturnSpecWithResources()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.AddResource("CPU", "2"); 
        spec.GetResources().Should().NotBeEmpty();
    }
    
    //add new spec with one recourse as object
    [Fact]
    public void NewSpecWithResourcesAsObject_ShouldReturnSpecWithResources()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        var resource = Resource.CreateResource("CPU", "2").Value;
        spec.AddResource(resource);
        spec.GetResources().Should().NotBeEmpty();
    }
    
    //add new spec with list of resources
    [Fact]
    public void NewSpecWithResourcesAsArray_ShouldReturnSpecWithResources()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        List<Resource> resources =
        [
            Resource.CreateResource("CPU", "2").Value,
            Resource.CreateResource("RAM", "4").Value,
            Resource.CreateResource("SSD", "100").Value
        ];
    
        spec.AddResources(resources);
        spec.GetResources().Should().NotBeEmpty().And.HaveCount(3);
    }
    
    //remove one of the resource using name
    [Fact]
    public void RemovingOneOfTheResourceUsingValueKey_ShouldReturnSpecWithoutRemovedResource()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        List<Resource> resources =
        [
            Resource.CreateResource("CPU", "2").Value,
            Resource.CreateResource("RAM", "4").Value,
            Resource.CreateResource("SSD", "100").Value
        ];
    
        spec.AddResources(resources);
        spec.RemoveResource("RAM");
        spec.GetResources().Should().NotBeEmpty().And.HaveCount(2);
    }
    
    //remove one of the resource using object
    [Fact]
    public void RemovingOneOfTheResourceUsingObject_ShouldReturnSpecWithoutRemovedResource()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        spec.RemoveResource(resource2);
        spec.GetResources().Should().NotBeEmpty().And.HaveCount(2);
    }
    
    //remove all the resources
    [Fact]
    public void RemovingResourcesUsingArrayOfObject_ShouldReturnSpecWithoutResources()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        spec.RemoveAllResources();
        spec.GetResources().Should().BeEmpty().And.HaveCount(0);
    }
    
    //value of the product must be good
    [Fact]
    public void ValueOfProductMustBeCorrect_ShouldReturnCorrectValueOfProductUsingMultiple()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("Os").Value, GenerationRecord.Create("G2").Value, Kind.Create("Windows").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(65.0m,taxonomy,6).Value;
        spec.GetValueOfProduct().Should().Be(65.0m*6);
    }    
    
    // search resource by exist name 
    [Fact]
    public void SearchingForAResourceByExistName_ShouldReturnAResource()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        spec.GetResourceOfProduct("RAM").Should().NotBeNull();
    }
    
    // search resource by not exist name 
    [Fact]
    public void SearchingForAResourceByNotExistName_ShouldReturnNull()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        spec.GetResourceOfProduct("YEAR").Should().BeNull();
    }
    
    // product category must be the same as the following
    [Fact]
    public void ProductCategory_ShouldReturnTheSameCategory()
    {
        var productCategory = ProductCategory.Create("VirtualMachine").Value;
        var taxonomy = Taxonomy.Create(productCategory, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.GetTaxonomy().GetCategory().Should().Be(productCategory);
    }
    
    // product price must be the same as the following
    [Fact]
    public void ProductPrice_ShouldReturnTheSameCategory()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.GetPrice().Should().Be(100.50m);
    }
    
    // product amount must be the same as the following
    [Fact]
    public void ProductAmount_ShouldReturnTheSameCategory()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        spec.GetAmount().Should().Be(1);
    }
        
    // product value must be returning
    [Fact]
    public void ProductValueFindingByName_ShouldReturnAValueAndBeNotEmpty()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        spec.GetValueOfResource("CPU").Should().Be("2").And.NotBeNull();
    }
    
    // value of resource must be returned
    [Fact]
    public void ValueOfResourcesUsingObjectSearching_ShouldBeReturned()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        var value = ProductSpecification.GetResourcesOfProduct(resource);
        value.Key.Should().Be("CPU");
    }
    
    //create resource later is existed on the list 
    [Fact]
    public void CreateMoreResourcesLaterWillBeAddedToList_ShouldReturnListWithMoreResources()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
        
        var resource = Resource.CreateResource("CPU", "2").Value;
        var resource2 = Resource.CreateResource("RAM", "4").Value;
        var resource3 = Resource.CreateResource("SSD", "100").Value;
        
        List<Resource> resources =
        [
            resource,
            resource2,
            resource3
        ];
    
        spec.AddResources(resources);
        spec.AddResource("vLAN", "tak");
        spec.GetResources().Should().NotBeEmpty().And.HaveCount(4);
    
    }
    
    //==================================================================================================
    //==================================Taxonomy Tests==================================================
    //==================================================================================================
    
    // create taxonomy with correct category and kind of product
    [Fact]
    public void CreateTaxonomyHaveRightCategoryAndKind_ShouldReturnTaxonomyWithCorrectCategoryAndKind()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value);
        taxonomy.IsSuccess.Should().BeTrue();
    }
    
    // create taxonomy with incorrect category and kind of product
    [Fact]
    public void CreateTaxonomyWithNoCorrectCategoryAndKind_ShouldReturnError()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Linux").Value);
        taxonomy.IsFailed.Should().BeTrue();
    }
    
    
    
        
    //==================================================================================================
    //==================================Another Tests===================================================
    //==================================================================================================
    
    
    
    // test not equals resources (each has another Guid)
    [Fact]
    public void ResourcesWithTheSameKeyAndValue_ShouldBeNotEqual()
    {
        var r1 = Resource.CreateResource("CPU", "2").Value;
        var r2 = Resource.CreateResource("CPU", "2").Value;
        r1.Should().NotBe(r2);
    }
    
    // test to check throwing exception
    [Fact]
    public void CreatingPriceWithZero_ShouldThrowException()
    {
        var act = Price.CreatePrice(0);
        act.IsFailed.Should().BeTrue();
    }
    
    // test diagnostic - created 10_000 resources and added 10_000 more resources - it lasted lower than 2ms
    [Fact]
    public void AddingTenThousandResources_ShouldCompleteQuickly()
    {
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(100.50m,taxonomy,1).Value;
    
        var initialResources = new List<Resource>();
        for (var i = 0; i < 10_000; i++)
        {
            initialResources.Add(Resource.CreateResource($"Key{i}", $"{i}").Value);
        }
    
        spec.AddResources(initialResources);
        spec.GetResources().Should().HaveCount(10_000);
    
        var moreResources = new List<Resource>();
        for (var i = 10_000; i < 20_000; i++)
        {
            moreResources.Add(Resource.CreateResource($"Key{i}", $"{i}").Value);
        }
    
        var sw = System.Diagnostics.Stopwatch.StartNew();
        spec.AddResources(moreResources);
        sw.Stop();
    
        spec.GetResources().Should().HaveCount(20_000);
        sw.ElapsedMilliseconds.Should().BeLessThan(2); 
    }
    
    //==================================================================================================
    //==================================Main Tests=======================================================
    //==================================================================================================
    
    
    // main test - Product have the resource in spec after creating product without version and resources
    [Fact]
    public void ProductHaveSpecAndResourcesFirstlyWithout_ShouldReturnProductWithListResourcesFromSpec()
    {
        // create product
        var product = Product.CreateProduct("VirtualMachine").Value;
        
        // publish new version without spec - version number 1
        product.PublishNewVersion("Test server");
        
        // create spec
        var taxonomy = Taxonomy.Create(ProductCategory.Create("VirtualMachine").Value, GenerationRecord.Create("G1").Value, Kind.Create("Amd").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(369.00m, taxonomy, 1).Value;
        
        // create resources for spec
        spec.AddResource("CPU", "8");
        spec.AddResource("RAM", "32");
        spec.AddResource("SSD", "100");
        spec.AddResource("vLAN", "tak");
    
        // add spec to version
        var version2 = product.CurrentVersion();
        version2.AddProductSpecification(spec);
    
        //product with active version must have something on the list resources
        version2.Spec?.GetResources().Should().NotBeEmpty().And.HaveCount(4);
        product.Versions.Should().HaveCount(1);
        product.Versions.Should().Contain(version2);
    }
    
    // main test 2- Product have the resource in spec after creating product with full spec
    [Fact]
    public void ProductHaveSpecAndResources_ShouldReturnProductWithListResourcesFromSpec()
    {
        // create product
        var product = Product.CreateProduct("Software").Value;
        
        // create spec
        var taxonomy = Taxonomy.Create(ProductCategory.Create("Os").Value, GenerationRecord.Create("G2").Value, Kind.Create("Windows").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(65, taxonomy, 7).Value;
        
        // create resources for spec
        spec.AddResource("Name", "Windows Server 2025");
        spec.AddResource("Year", "2025");
        spec.AddResource("Company", "Microsoft Corporation");
        
        // publish new version with spec - version number 2
        product.PublishNewVersion("Test license", spec);
        
        // get current version
        var version2 = product.CurrentVersion();
    
        //product with active version must have something on the list resources
        version2.Spec?.GetResources().Should().NotBeEmpty().And.HaveCount(3);
        product.Versions.Should().HaveCount(1);
        product.Versions.Should().Contain(version2);
    }
    
    // main test 3- Product have a few versions, one is active and have resources
    [Fact]
    public void ProductHaveFewVersions_ShouldReturnOneActiveVersionWithResources()
    {
        // create product
        var product = Product.CreateProduct("Software").Value;
        
        // create spec
        var taxonomy = Taxonomy.Create(ProductCategory.Create("Os").Value, GenerationRecord.Create("G2").Value, Kind.Create("Windows").Value).Value;
        var spec = ProductSpecification.GenerateProductSpecification(65,taxonomy, 7).Value;
        
        // create resources for spec
        spec.AddResource("Name", "Windows Server 2025");
        spec.AddResource("Year", "2025");
        spec.AddResource("Company", "Microsoft Corporation");
        
        // publish new version with spec - version number 2
        product.PublishNewVersion("Test server", spec);
    
        var version1 = product.CurrentVersion();
        
        version1.Spec?.GetResources().Should().NotBeEmpty().And.HaveCount(3);
        
        product.PublishNewVersion("Licences Upgrade", spec);
        
        var versionOld = product.GetVersionProduct(1);
        versionOld.IsSuccess.Should().BeTrue();
        
        // get current version
        var version2 = product.CurrentVersion();
    
        version2.GetStatusVersion().Should().BeTrue();
        
        //product with active version must have something on the list resources
        version2.Spec?.GetResources().Should().NotBeEmpty().And.HaveCount(3);
        product.Versions.Should().HaveCount(2);
        
    }
}