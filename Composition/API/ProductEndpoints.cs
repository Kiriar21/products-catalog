using System.Net;
using Application.Products.AddNewProduct;
using Application.Products.CountProducts;
using Application.Products.GetAllProductsByStatus;
using Application.Products.GetPaginatedProducts;
using Application.Products.GetProductById;
using Application.Products.GetProductsWithPriceRange;
using Application.Products.ProductExist;
using Application.Products.PublishNewVersionProductByTracking;
using Composition.API.ApiMapper;
using Composition.API.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using DeleteProduct = Composition.API.Contracts.DeleteProduct;

namespace Composition.API;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        //get 
        
        //get products paginated
        endpoints.MapGet("products", async (IApplicationBus api, int pageSize = 20, int pageNumber = 1, string? query = null) =>
        {
            var result = await api.ExecuteQueryAsync(new GetPaginatedProducts(pageNumber, pageSize, query));
            return result.ToApi();
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {   
            Summary = "Get all products",
            Description = "Get all products in paginated list, default 20 items and first page.",
        }).WithTags("Products");

        //get products by status
        endpoints.MapGet("products/by-status/{status}", async (IApplicationBus api, string status, int pageSize = 20, int pageNumber = 1) =>
        {
            var result = await api.ExecuteQueryAsync(new GetAllProductsByStatus(status, pageNumber, pageSize));
            return result.ToApi();
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Get products with selected status",
            Description = "Get all products in paginated list with selected status (Draft/Active/Archived). Default 20 items and first page.",
        }).WithTags("Products");

            
        //get product by price range
        endpoints.MapGet("products/by-range/{priceLow}-{priceHigh}", async (IApplicationBus api, int priceLow = 1, int priceHigh = 2, int pageNumber = 1, int pageSize = 20, string? query = null) =>
        {
            var result = await api.ExecuteQueryAsync(new GetProductsWithPriceRange(priceLow, priceHigh, pageNumber, pageSize, query));
            return result.ToApi();
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Get products with selected min and max price",
            Description = "Get all products in paginated list with selected price range (min-max). Default 20 items and first page.",
        }).WithTags("Products");
        
        //get product bv id
        endpoints.MapGet("products/{id}", async (IApplicationBus api, string id) =>
        {
            var result = await api.ExecuteQueryAsync(new GetProductById(id));
            return result.ToApi();
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Get product with given id",
            Description = "Try to get product with given id.",
        }).WithTags("Products");
        
        //get info count product
        endpoints.MapGet("products/count", async (IApplicationBus api) =>
        {
            var result = await api.ExecuteQueryAsync(new CountProducts());
            return result.ToApi();
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Get count of products",
            Description = "Get count of all added products in database. (non specified for status).",
        }).WithTags("Products");
        
        //get info product exist
        endpoints.MapGet("products/{id}/exist", async (IApplicationBus api, string id) =>
        {
            var result = await api.ExecuteQueryAsync(new ProductExist(id));
            return result.ToApi();
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Information if product with given id exists",
            Description = "Get information about whether the product with given id exists in database.",
        }).WithTags("Products");
        
        //post
        
        //add product
        endpoints.MapPost("products", async ([FromServices] IApplicationBus api, [FromBody] AddProduct req) =>
        {
            var result = await api.ExecuteEmptyCommandAsync(
                new AddNewProduct(req.Type)
            );
            return result.ToApi(HttpStatusCode.Created);
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Add new product",
            Description = "Add new product with specified type of product (VirtualMachine/Software/Service).",
        }).WithTags("Products");


        //publish new version to product (using tracking)
        endpoints.MapPut("products/{id}/versions", async ([FromServices] IApplicationBus api, string id, 
            [FromBody]  AddVersion req ) =>
        {
            var result = await api.ExecuteEmptyCommandAsync(
                new PublishNewVersionProductByTracking(
                    id,
                    req.Name,
                    req.Price,
                    req.Amount,
                    req.Resources,
                    req.ProductCategory,
                    req.GenerationRecord,
                    req.Kind    
                ));
            return result.ToApi(HttpStatusCode.NoContent);
        })
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Add new version of product",
            Description = "Try to add new version of product with given id. Params should be: Amount how as a decimal (with m), ProductCategory - (VirtualMachine/Database/Os/License/Vpn/Firewall/Service), GenerationRecord - (G1/G2/G2E/G3) and  Kind (specified for ProductCategory): (Intel/Amd) for VirtualMachine, (MicrosoftStd/MicrosoftWeb) for Database, (Windows/Linus) fro OS, (Rds) for License, (OpenVPN) for Vpn, (PfSense/FireGuard) for Firewall and (ServiceImplementation/ExtendedSupport/Backup) for Service.",
        }).WithTags("Products");
        
        endpoints.MapDelete("products", async ( [FromServices] IApplicationBus api, [FromBody]  DeleteProduct req) =>
        {
            var result = await api.ExecuteEmptyCommandAsync(new Application.Products.DeleteProduct.DeleteProduct(req.Id));
            return result.ToApi(HttpStatusCode.NoContent);
        })        
        .WithOpenApi(op => new OpenApiOperation(op)
        {
            Summary = "Delete exist product",
            Description = "Try to find and delete product with given id.",
        }).WithTags("Products");
        
        return endpoints;
    }

    public static IEndpointRouteBuilder ErrorEndpoints(this IEndpointRouteBuilder endpoints)
    {

        endpoints.MapGet("/error", async ctx =>
        {
            var reason = ctx.Request.Query["reason"].ToString();

            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new
            {
                error = reason,
            });
        });
        
        endpoints.MapFallback(async ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsJsonAsync(new
            {
                error = "Not found page.",
                path = ctx.Request.Path.Value,
            });
        });
        
        return endpoints;
    }
}