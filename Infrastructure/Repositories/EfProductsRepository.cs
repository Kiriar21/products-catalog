using Application.Contracts;
using Domain;
using Domain.Interfaces;
using FluentResults;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure.Repositories;

public class EfProductsRepository(AppDbContext context) : IProductsRepository
{
    private readonly AppDbContext _context = context;

    public async Task AddProduct(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
    }

    // ok??  
    public Task PublishNewVersionProduct(Product product, CancellationToken ct = default)
    {
        _context.Products.Attach(product);
        return Task.CompletedTask;
    }

    public async Task DeleteProduct(Guid id, CancellationToken ct = default)
    {
        var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == id, ct);
        if (product is not null) _context.Products.Remove(product);
    }

    public Task<Product?> GetProductById(Guid id, CancellationToken ct = default)
    {
        return _context.Products.Include(p => p.Versions)
            .ThenInclude(v => v!.Spec)
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }
    public Task<Product?> GetProductByIdNoTrack(Guid id, CancellationToken ct = default)
    {
        return _context.Products.Include(p => p.Versions)
            .ThenInclude(v => v!.Spec).AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    
    
    public async Task<PaginatedList<Product>> GetProductsByStatus(string status, int pageNumber, int pageSize,  string? query = null, CancellationToken ct = default)
    {
        var queryTemp =  _context.Products.AsQueryable()
            .Where(p => p.LifeCycleStatus.Name == status)
            .OrderBy(p => p.Id);
        
        return await PaginatedList<Product>.CreateAsync(queryTemp.AsNoTracking(), pageNumber, pageSize, ct);
    }

    public async Task<PaginatedList<Product>> GetProductsWithPriceRange(decimal priceLow, decimal priceHigh, int pageNumber, int pageSize, string? query = null, CancellationToken ct = default)
    {
        var queryTemp = _context.Products
            .Include(v => v.Versions)
            .ThenInclude(s => s!.Spec)
            .Where(v =>
                v.Versions
                    .Any(v => !v.IsObsolete &&
                              v.Spec!.Price.Value >= priceLow &&
                              v.Spec!.Price.Value <= priceHigh))
            .OrderBy(p => p.Id);

        return await PaginatedList<Product>.CreateAsync(queryTemp.AsNoTracking(), pageNumber,pageSize, ct);
    }

    public Task<bool> ProductExists(Guid id, CancellationToken ct = default) =>
         _context.Products.AnyAsync(p => p.Id == id, ct);

    public Task<int> CountProducts(CancellationToken ct = default) =>
         _context.Products.CountAsync(ct);

    public Task<PaginatedList<Product>> GetPaginatedProductsList(int pageNumber = 1, int pageSize = 20, string? query = null,
        CancellationToken ct = default)
    {
        return _context.Products.Include(v => v.Versions).AsQueryable().OrderBy(p => p.Id).ToPaginatedListAsync(pageNumber, pageSize, ct);
    }
}