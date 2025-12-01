using Shared;

namespace Application.Contracts;

public static class PaginatedListExtensions
{
    public static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize,
        CancellationToken ct = default)
    {
        return PaginatedList<T>.CreateAsync(source, pageNumber, pageSize, ct);
    }

    public static PaginatedList<TOut> MapTo<TIn, TOut>(this PaginatedList<TIn> source, Func<TIn, TOut> map)
    {
        var items = source.Items.Select(map).ToList();
        return new PaginatedList<TOut>(items, source.TotalCount, source.PageNumber, source.PageSize);
    }
}