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

        // เพิ่มภายหลังเมื่อมี Repository
        // services.AddScoped<IProductRepository, ProductRepository>();
        // services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        return services;
    }
}
