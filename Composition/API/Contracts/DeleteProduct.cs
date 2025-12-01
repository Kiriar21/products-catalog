namespace Composition.API.Contracts;

/// <summary>
///  Remove product with given id.
/// </summary>
/// <param name="Id">only real Guid examples </param>
public record DeleteProduct
(
    string Id // guid
);