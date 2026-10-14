using LimitedStockOrdering.Application.Repositories;
using LimitedStockOrdering.Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace LimitedStockOrdering.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddSingleton<NpgsqlDataSource>(_ =>
        {
            var dataSourceBuilder =
                new NpgsqlDataSourceBuilder(connectionString);

            return dataSourceBuilder.Build();
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

        return services;
    }
}
