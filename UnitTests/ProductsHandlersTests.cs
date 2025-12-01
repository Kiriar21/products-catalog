using Application.Contracts;
using Application.Products.AddNewProduct;
using Application.Products.PublishNewVersionProductSendObjectProduct;
using Domain;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Shared;

namespace UnitTests;

public class ProductsHandlersTests
{
    [Fact]
    public async Task AddNewProductHandler_AddsTwoProducts_AndCommitsTwice()
    {
        var repoMock = new Mock<IProductsRepository>();
        var uowMock = new Mock<IUnitOfWork>();
    
        var added = new List<Product>();
        repoMock.Setup(r => r.AddProduct(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns((Product p, CancellationToken ct) =>
            {
                added.Add(p);
                return Task.CompletedTask;
            });
    
        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
    
        var handler = new AddNewProductHandler(repoMock.Object, uowMock.Object);
    
        var res1 = await handler.Handle(new AddNewProduct("VirtualMachine"));
        var res2 = await handler.Handle(new AddNewProduct("Service"));
    
        res1.IsSuccess.Should().BeTrue();
        res2.IsSuccess.Should().BeTrue();
    
        added.Count.Should().Be(2);
        repoMock.Verify(r => r.AddProduct(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
   
    }
    
    [Fact]
    public async Task PublishNewVersionProductHandler_PublishesTwoVersions_OnProduct_AndCommits()
    {
        var repoMock = new Mock<IProductsRepository>();
        var uowMock = new Mock<IUnitOfWork>();

        var product = Product.CreateProduct("VirtualMachine").Value;

        repoMock.Setup(r => r.GetProductByIdNoTrack(product.GetGuidId(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        repoMock.Setup(r => r.PublishNewVersionProduct(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var handler = new PublishNewVersionProductSendObjectProductHandler(uowMock.Object, repoMock.Object);

        var cmd1 = new PublishNewVersionProductSendObjectProduct(
            IdProduct: product.GetId(),
            Name: "Virtual Machine G1 AMD",
            Price: 100m,
            Amount: 1,
            Resources: new Dictionary<string, string>(),
            ProductCategory: "VirtualMachine",
            GenerationRecord: "G1",
            Kind: "Amd"
        );

        var cmd2 = cmd1 with { Name = "Virtual Machine G1+ AMD" };

        var r1 = await handler.Handle(cmd1);
        var r2 = await handler.Handle(cmd2);

        r1.IsSuccess.Should().BeTrue();
        r2.IsSuccess.Should().BeTrue();

        product.Versions.Count.Should().Be(2);

        repoMock.Verify(r => r.PublishNewVersionProduct(
            It.Is<Product>(p => p.GetGuidId() == product.GetGuidId()), It.IsAny<CancellationToken>()), Times.Exactly(2));

        uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    
        [Fact]
        public async Task GetPaginatedProductsList_FirstPage_ReturnsCorrectItemsAndMetadata()
        {
            var data = Enumerable.Range(1, 5)
                .Select(_ => Product.CreateProduct("VirtualMachine").Value)
                .OrderBy(p => p.Id) 
                .ToList();

            const int pageNumber = 1;
            const int pageSize = 2;

            var expectedItems = data.Take(pageSize).ToList();
            var expectedPaginated = new PaginatedList<Product>(expectedItems, data.Count, pageNumber, pageSize);

            var repoMock = new Mock<IProductsRepository>();
            repoMock
                .Setup(r => r.GetPaginatedProductsList(pageNumber, pageSize, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPaginated);

            var page = await repoMock.Object.GetPaginatedProductsList(pageNumber, pageSize, null, CancellationToken.None);

            page.Items.Should().HaveCount(pageSize);
            page.TotalCount.Should().Be(5);
            page.PageNumber.Should().Be(pageNumber);
            page.PageSize.Should().Be(pageSize);
            page.TotalPages.Should().Be(3);
            page.HasPreviousPage.Should().BeFalse();
            page.HasNextPage.Should().BeTrue();

            repoMock.Verify(r => r.GetPaginatedProductsList(pageNumber, pageSize, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPaginatedProductsList_LastPage_ReturnsRemainderAndFlags()
        {
            var data = Enumerable.Range(1, 5)
                .Select(_ => Product.CreateProduct("VirtualMachine").Value)
                .OrderBy(p => p.Id)
                .ToList();

            const int pageNumber = 3;
            const int pageSize = 2;

            var expectedItems = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); // powinien zwrócić 1 element
            var expectedPaginated = new PaginatedList<Product>(expectedItems, data.Count, pageNumber, pageSize);

            var repoMock = new Mock<IProductsRepository>();
            repoMock
                .Setup(r => r.GetPaginatedProductsList(pageNumber, pageSize, It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPaginated);

            var page = await repoMock.Object.GetPaginatedProductsList(pageNumber, pageSize, null, CancellationToken.None);

            page.Items.Should().HaveCount(1);
            page.TotalCount.Should().Be(5);
            page.PageNumber.Should().Be(pageNumber);
            page.PageSize.Should().Be(pageSize);
            page.TotalPages.Should().Be(3);
            page.HasPreviousPage.Should().BeTrue();
            page.HasNextPage.Should().BeFalse();

            repoMock.Verify(r => r.GetPaginatedProductsList(pageNumber, pageSize, null, It.IsAny<CancellationToken>()), Times.Once);
        }

}