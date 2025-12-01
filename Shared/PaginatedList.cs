using Microsoft.EntityFrameworkCore;

namespace Shared;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    
    public int PageNumber { get; }
    public int PageSize { get; }
    
    public int TotalPages { get; }
    public int TotalCount { get; }
    
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber=1, int pageSize=20,
        CancellationToken ct = default)
    {
        var count = await source.CountAsync(cancellationToken: ct);
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}