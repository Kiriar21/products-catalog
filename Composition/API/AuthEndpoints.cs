using System.Security.Claims;
using Application.Users.AddNewUser;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

namespace Composition.API;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/login", (HttpContext ctx) =>
            Results.Redirect("/auth/google"))
        .AllowAnonymous()
        .WithOpenApi(op => new OpenApiOperation(op)
        {   
            Summary = "Login account",
            Description = "First version of login. Redirect to authorization using oatuh2 pkce flow code with google.",
        })
        .WithTags("Authorization");

        endpoints.MapGet("/auth/google", (HttpContext ctx) =>
        {
            var props = new AuthenticationProperties { RedirectUri = "/auth/google/callback" };
            return Results.Challenge(props, new[] { "Google" });
        })
        .AllowAnonymous()
        .WithOpenApi(op => new OpenApiOperation(op)
        {   
            Summary = "Authorization by Google",
            Description = "Redirect to authorization by Google using oatuh2 pkce flow code.",
        })
        .WithTags("Authorization");

        endpoints.MapGet("/auth/google/callback",
            async (HttpContext ctx, IApplicationBus bus, IUserRepository users, IJwtTokenService jwt, CancellationToken ct) =>
        {
            var ext = await ctx.AuthenticateAsync("External");
            if (!ext.Succeeded || ext.Principal is null)
                return Results.BadRequest(new { error = "external_auth_failed" });

            var p = ext.Principal;
            var email = p.FindFirstValue(ClaimTypes.Email) ?? p.FindFirst("email")?.Value;
            var name = p.FindFirstValue(ClaimTypes.Name) ?? p.Identity?.Name ?? email ?? "User";
            var providerSub = p.FindFirstValue(ClaimTypes.NameIdentifier) ?? p.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(providerSub))
                return Results.BadRequest(new { error = "missing_claims" });

            var user = await users.GetByProviderAsync("Google", providerSub!, ct);
            if (user is null)
            {
                var cmd = new AddNewUser(email!, name!, "Google", providerSub!);
                var res = await bus.ExecuteEmptyCommandAsync(cmd, ct);
                if (res.IsFailed) return Results.BadRequest(new { error = "user_create_failed", res.Errors });
                user = await users.GetByProviderAsync("Google", providerSub!, ct);
            }

            await ctx.SignOutAsync("External"); 

            var (token, exp) = jwt.Create(user!.Id, user.Email, user.DisplayName);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,  
                Expires = exp,
                Path = "/"
            };
            ctx.Response.Cookies.Append("access_token", token, cookieOptions);
            return Results.Redirect("/"); 
        })
        .AllowAnonymous()
        .WithOpenApi(op => new OpenApiOperation(op)
        {   
            Summary = "Get authorization by Google",
            Description = "First version of login (by google oauth2 pkce flow code). Now allow anonymous account.",
        })
        .WithTags("Authorization");
        
        endpoints.MapPost("/logout", (HttpContext ctx) =>
            {
                if (ctx.Request.Cookies.ContainsKey("access_token"))
                    ctx.Response.Cookies.Delete("access_token", new CookieOptions { Path = "/" });
                return Results.Ok(new { logged_out = true });
            })
            .WithOpenApi(op => new OpenApiOperation(op)
            {   
                Summary = "Logout account",
                Description = "Logout account and remove token with JWT access information.",
            })
            .WithTags("Authorization");
        
        return endpoints;
    }
}