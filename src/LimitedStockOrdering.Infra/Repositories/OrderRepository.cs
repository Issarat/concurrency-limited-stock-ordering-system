using Dapper;
using LimitedStockOrdering.Application.Orders;
using LimitedStockOrdering.Application.Repositories;
using Npgsql;

namespace LimitedStockOrdering.Infra.Repositories;

internal sealed class OrderRepository(NpgsqlDataSource dataSource) : IOrderRepository
{
    public async Task<OrderPreviewResponse?> GetPreviewAsync(
        Guid productId,
        Guid variantId,
        int quantity,
        string orderType,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            """
            SELECT
                p.product_id AS "ProductId",
                pv.variant_id AS "VariantId",
                p.name AS "ProductName",
                p.description AS "Description",
                p.image_url AS "ImageUrl",
                pv.sku AS "Sku",
                @OrderType AS "OrderType",
                COALESCE(
                    CASE WHEN @OrderType = 'FlashSale' THEN active_sale.sale_price END,
                    pv.base_price
                ) AS "UnitPrice",
                @Quantity AS "Quantity",
                COALESCE(
                    CASE WHEN @OrderType = 'FlashSale' THEN active_sale.sale_price END,
                    pv.base_price
                ) * @Quantity AS "TotalAmount",
                COALESCE(i.available_quantity, 0)::integer AS "AvailableQuantity",
                (COALESCE(i.available_quantity, 0) >= @Quantity) AS "HasSufficientStock",
                (@OrderType = 'FlashSale' AND active_sale.sale_price IS NOT NULL) AS "IsOnFlashSale"
            FROM products AS p
            JOIN product_variants AS pv
                ON pv.product_id = p.product_id
               AND pv.variant_id = @VariantId
               AND pv.is_active = true
            LEFT JOIN inventory AS i ON i.variant_id = pv.variant_id
            LEFT JOIN LATERAL (
                SELECT MIN(fsv.sale_price) AS sale_price
                FROM flash_sale_variants AS fsv
                JOIN flash_sales AS fs ON fs.flash_sale_id = fsv.flash_sale_id
                WHERE fsv.product_id = p.product_id
                  AND fsv.variant_id = pv.variant_id
                  AND fs.status = 'Active'
                  AND CURRENT_TIMESTAMP >= fs.starts_at
                  AND CURRENT_TIMESTAMP < fs.ends_at
            ) AS active_sale ON true
            WHERE p.product_id = @ProductId
              AND p.is_active = true;

            SELECT
                po.name AS "Name",
                pov.value AS "Value"
            FROM product_variant_values AS pvv
            JOIN product_options AS po
                ON po.product_id = pvv.product_id
               AND po.option_id = pvv.option_id
            JOIN product_option_values AS pov
                ON pov.option_id = pvv.option_id
               AND pov.option_value_id = pvv.option_value_id
               AND pov.is_active = true
            WHERE pvv.product_id = @ProductId
              AND pvv.variant_id = @VariantId
            ORDER BY po.display_order;
            """;

        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                ProductId = productId,
                VariantId = variantId,
                Quantity = quantity,
                OrderType = orderType
            },
            cancellationToken: cancellationToken);

        using var grid = await connection.QueryMultipleAsync(command);
        var preview = await grid.ReadSingleOrDefaultAsync<OrderPreviewResponse>();

        if (preview is null)
        {
            return null;
        }

        preview.Options = (await grid.ReadAsync<OrderPreviewOptionResponse>()).AsList();
        return preview;
    }
}
