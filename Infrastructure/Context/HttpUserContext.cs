using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Context;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Context;

internal class HttpUserContext(IHttpContextAccessor accessor) : IUserContext
{
    public string UserIdentifier
        => accessor.HttpContext?.User?
               .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
           ?? accessor.HttpContext?.User?
               .FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? "Anonymous";

    public string RequestPath  => accessor.HttpContext?.Request?.Path.Value ?? "/";
    public string RequestMethod => accessor.HttpContext?.Request?.Method ?? "GET";
    
}