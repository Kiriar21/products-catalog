using Swashbuckle.AspNetCore.Annotations;

namespace Composition.API.Contracts;

/// <summary>
///  Add new Product with specified type.
/// </summary>
/// <param name="Type">VirtualMachine / Software / Service</param>

public record AddProduct
(
    string Type //type of product
);