using System.Reflection;
using Application.Context;
using Application.Contracts;
using Application.Decorators.Command;
using Application.Decorators.EmptyCommand;
using Application.Decorators.Queries;
using Domain.Interfaces;
using FluentValidation;
using Infrastructure.Auth;
using Infrastructure.Context;
using Infrastructure.Database;
using Infrastructure.Database.Configurations;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ICommand = Application.Contracts.ICommand;
// using ICommand = System.Windows.Input.ICommand;

namespace Infrastructure;

public static class Setup
{
    private static readonly Assembly ApplicationAssemblies = typeof(ICommand).Assembly;
    
    public static IServiceCollection InstallProductsModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Application
        services.AddScoped<IApplicationBus, ApplicationBus>();

        services.Scan(scan => scan.FromAssemblies(ApplicationAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces().WithTransientLifetime()
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), publicOnly:false)
            .AsImplementedInterfaces().WithTransientLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces().WithTransientLifetime());

        services.AddValidatorsFromAssembly(ApplicationAssemblies, includeInternalTypes: true);

        // 1a. Decorators

        // - decorators for validation
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandDecorator<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationEmptyCommandDecorator<>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationQueriesDecorator<,>));
        
        // - decorators for TryCatch
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(TryCatchCommandDecorator<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(TryCatchEmptyCommandDecorator<>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(TryCatchQueriesDecorator<,>));

        
        //infrastructure

        //interceptor - usercontextt
        services.AddScoped<AuditabilityEntityInterceptor>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, HttpUserContext>();

        // context db -connectionstring
        services.AddDbContext<AppDbContext>((sp, opt) =>
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .AddInterceptors(sp.GetRequiredService<AuditabilityEntityInterceptor>()));
        

        
        //repo
        services.AddScoped<IProductsRepository, EfProductsRepository>();
        services.AddScoped<IUserRepository, EfUsersRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        
        
        //auth
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    } 
}