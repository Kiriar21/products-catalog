
using Microsoft.OpenApi.Models;

namespace Composition.API;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/me", (HttpContext ctx) =>
        {
            var u = ctx.User;
            var authed = u?.Identity?.IsAuthenticated == true;

            if (!authed)
                return Results.Json(new { sub = "Anonymous", email = (string?)null, name = (string?)null, isAuthenticated = false });

            var sub   = u!.FindFirst("sub")?.Value;
            var email = u.FindFirst("email")?.Value;
            var name  = u.FindFirst("name")?.Value;

            return Results.Json(new { sub, email, name, isAuthenticated = true });
        })
        .AllowAnonymous()
        .WithOpenApi(op => new OpenApiOperation(op)
        {   
            Summary = "Get information about logged user.",
            Description = "Get information about logged user. Id user, name and email.",
        })
        .WithTags("Account");

        return endpoints;
    }
}