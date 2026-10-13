using Dapper;
using LimitedStockOrdering.Application.Products;
using LimitedStockOrdering.Application.Repositories;
using Npgsql;

namespace LimitedStockOrdering.Infra.Repositories;

internal sealed class ProductRepository : IProductRepository
{
    private const string ProductAggregateCtes =
        """
        WITH variant_summary AS (
            SELECT
                pv.product_id,
                MIN(pv.base_price) AS starting_price,
                COALESCE(SUM(i.available_quantity), 0)::integer AS available_quantity
            FROM product_variants AS pv
            LEFT JOIN inventory AS i ON i.variant_id = pv.variant_id
            WHERE pv.is_active = true
            GROUP BY pv.product_id
        ),
        sales_summary AS (
            SELECT
                pv.product_id,
                COALESCE(SUM(oi.quantity), 0)::integer AS sold_quantity
            FROM order_items AS oi
            JOIN orders AS o ON o.order_id = oi.order_id
            JOIN product_variants AS pv ON pv.variant_id = oi.variant_id
            WHERE o.status = 'Completed'
            GROUP BY pv.product_id
        ),
        active_flash_sale_summary AS (
            SELECT
                fsv.product_id,
                MIN(fsv.sale_price) AS flash_sale_starting_price
            FROM flash_sale_variants AS fsv
            JOIN flash_sales AS fs ON fs.flash_sale_id = fsv.flash_sale_id
            JOIN product_variants AS pv ON pv.variant_id = fsv.variant_id
            WHERE fs.status = 'Active'
              AND CURRENT_TIMESTAMP >= fs.starts_at
              AND CURRENT_TIMESTAMP < fs.ends_at
              AND pv.is_active = true
            GROUP BY fsv.product_id
        )
        """;

    private const string ProductProjection =
        """
        SELECT
            p.product_id AS "ProductId",
            p.name AS "Name",
            p.description AS "Description",
            p.image_url AS "ImageUrl",
            COALESCE(vs.starting_price, 0) AS "StartingPrice",
            afs.flash_sale_starting_price AS "FlashSaleStartingPrice",
            (afs.product_id IS NOT NULL) AS "IsOnFlashSale",
            COALESCE(vs.available_quantity, 0) AS "AvailableQuantity",
            COALESCE(ss.sold_quantity, 0) AS "SoldQuantity"
        FROM products AS p
        LEFT JOIN variant_summary AS vs ON vs.product_id = p.product_id
        LEFT JOIN sales_summary AS ss ON ss.product_id = p.product_id
        LEFT JOIN active_flash_sale_summary AS afs ON afs.product_id = p.product_id
        """;

    private readonly NpgsqlDataSource _dataSource;

    public ProductRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyList<ProductListResponse>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql =
            ProductAggregateCtes
            + "\n"
            + ProductProjection
            + "\n"
            + """
              WHERE p.is_active = true
              ORDER BY p.name;
              """;

        await using var connection =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var products = await connection.QueryAsync<ProductListResponse>(command);
        return products.AsList();
    }

    public async Task<ProductDetailResponse?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        #region sql quey
        const string sql =
            ProductAggregateCtes
            + "\n"
            + ProductProjection
            + "\n"
            + """
              WHERE p.product_id = @ProductId
                AND p.is_active = true;

              SELECT
                  po.option_id AS "OptionId",
                  po.name AS "OptionName",
                  po.display_order AS "OptionDisplayOrder",
                  pov.option_value_id AS "OptionValueId",
                  pov.value AS "OptionValue",
                  pov.swatch_hex AS "SwatchHex",
                  pov.display_order AS "ValueDisplayOrder"
              FROM product_options AS po
              LEFT JOIN product_option_values AS pov
                  ON pov.option_id = po.option_id
                 AND pov.is_active = true
              WHERE po.product_id = @ProductId
              ORDER BY po.display_order, pov.display_order;

              WITH active_sale_prices AS (
                  SELECT
                      fsv.variant_id,
                      MIN(fsv.sale_price) AS sale_price
                  FROM flash_sale_variants AS fsv
                  JOIN flash_sales AS fs
                      ON fs.flash_sale_id = fsv.flash_sale_id
                  WHERE fsv.product_id = @ProductId
                    AND fs.status = 'Active'
                    AND CURRENT_TIMESTAMP >= fs.starts_at
                    AND CURRENT_TIMESTAMP < fs.ends_at
                  GROUP BY fsv.variant_id
              )
              SELECT
                  pv.variant_id AS "VariantId",
                  pv.sku AS "Sku",
                  pv.base_price AS "BasePrice",
                  asp.sale_price AS "SalePrice",
                  COALESCE(i.available_quantity, 0) AS "AvailableQuantity"
              FROM product_variants AS pv
              LEFT JOIN inventory AS i ON i.variant_id = pv.variant_id
              LEFT JOIN active_sale_prices AS asp ON asp.variant_id = pv.variant_id
              WHERE pv.product_id = @ProductId
                AND pv.is_active = true
              ORDER BY pv.sku;

              SELECT
                  pvv.variant_id AS "VariantId",
                  pvv.option_value_id AS "OptionValueId"
              FROM product_variant_values AS pvv
              JOIN product_variants AS pv ON pv.variant_id = pvv.variant_id
              WHERE pvv.product_id = @ProductId
                AND pv.is_active = true
              ORDER BY pvv.variant_id, pvv.option_id;
              """;
        #endregion

        await using var connection =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { ProductId = productId },
            cancellationToken: cancellationToken);

        using var grid = await connection.QueryMultipleAsync(command);
        var product = await grid.ReadSingleOrDefaultAsync<ProductDetailResponse>();

        if (product is null)
        {
            return null;
        }

        var optionRows = (await grid.ReadAsync<ProductOptionRow>()).AsList();
        product.Options = optionRows
            .GroupBy(row => new
            {
                row.OptionId,
                row.OptionName,
                row.OptionDisplayOrder
            })
            .Select(group => new ProductOptionResponse
            {
                OptionId = group.Key.OptionId,
                Name = group.Key.OptionName,
                DisplayOrder = group.Key.OptionDisplayOrder,
                Values = group
                    .Where(row => row.OptionValueId.HasValue)
                    .Select(row => new ProductOptionValueResponse
                    {
                        OptionValueId = row.OptionValueId!.Value,
                        Value = row.OptionValue ?? string.Empty,
                        SwatchHex = row.SwatchHex,
                        DisplayOrder = row.ValueDisplayOrder ?? 0
                    })
                    .ToList()
            })
            .ToList();

        product.Variants = (await grid.ReadAsync<ProductVariantResponse>()).AsList();

        var optionValueIdsByVariant = (await grid.ReadAsync<ProductVariantValueRow>())
            .GroupBy(row => row.VariantId)
            .ToDictionary(
                group => group.Key,
                group => group.Select(row => row.OptionValueId).ToList());

        foreach (var variant in product.Variants)
        {
            if (optionValueIdsByVariant.TryGetValue(variant.VariantId, out var valueIds))
            {
                variant.OptionValueIds = valueIds;
            }
        }

        return product;
    }

    private sealed class ProductOptionRow
    {
        public Guid OptionId { get; set; }
        public string OptionName { get; set; } = string.Empty;
        public int OptionDisplayOrder { get; set; }
        public Guid? OptionValueId { get; set; }
        public string? OptionValue { get; set; }
        public string? SwatchHex { get; set; }
        public int? ValueDisplayOrder { get; set; }
    }

    private sealed class ProductVariantValueRow
    {
        public Guid VariantId { get; set; }
        public Guid OptionValueId { get; set; }
    }
}
