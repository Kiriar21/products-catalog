using System.Diagnostics;
using System.Text;
using Composition.API;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog.Enrichers.Span;

var typeEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
if (typeEnv == "Development")
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(envPath)) Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables(); 

builder.Services.InstallProductsModule(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    o.MapInboundClaims = false;
    var key = builder.Configuration["Jwt:SigningKey"] ?? Environment.GetEnvironmentVariable("JWT_SIGNING_KEY");
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            if (string.IsNullOrEmpty(ctx.Token))
            {
                var cookie = ctx.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(cookie))
                    ctx.Token = cookie;
            }
            return Task.CompletedTask;
        }
    };
})
.AddCookie("External", o =>
{
    o.Cookie.Name = "ext.auth";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.None;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.SlidingExpiration = false;
    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
})
.AddGoogle("Google", o =>
{
    var id = Environment.GetEnvironmentVariable("CLIENT_ID_GOOGLE_OAUTH2");
    var secret = Environment.GetEnvironmentVariable("CLIENT_SECRET_GOOGLE_OAUTH2");
    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(secret))
    {
        o.ClientId = "none"; o.ClientSecret = "none";
        o.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
        {
            OnRedirectToAuthorizationEndpoint = ctx => { ctx.Response.Redirect("/error?reason=oauth_not_provided"); return Task.CompletedTask; }
        };
        return;
    }
    o.ClientId = id!; o.ClientSecret = secret!;
    o.SignInScheme = "External";
    o.UsePkce = true;
    o.Scope.Add("openid"); o.Scope.Add("profile"); o.Scope.Add("email");
    o.CorrelationCookie.HttpOnly = true;
    o.CorrelationCookie.IsEssential = true;
    o.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
    o.CorrelationCookie.SameSite = SameSiteMode.None;
});

builder.Services.AddSwaggerGen(x =>
{
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Composition.xml");
    x.IncludeXmlComments(xmlPath);
    
    x.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NSIX project - Catalog of Products",
        Version = "v1",
        Description = "API for NSIX project. Catalog of Products, can add, modify and delete products with versions.",
    });
});

builder.Services.AddAuthorization();

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "NSIX project");
    x.RoutePrefix = string.Empty; 
    x.DocumentTitle = "Catalog of Products";
    x.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // unfold lists
    x.DisplayOperationId();
});

app.UseSerilogRequestLogging(opt =>
{
    opt.EnrichDiagnosticContext = (ctx, http) =>
    {
        ctx.Set("ReqId", http.TraceIdentifier);
    };
});

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapProductEndpoints();
app.ErrorEndpoints();

app.Run();
