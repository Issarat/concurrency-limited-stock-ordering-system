using Dapper;
using LimitedStockOrdering.Application.Carts;
using LimitedStockOrdering.Application.Repositories;
using Npgsql;

namespace LimitedStockOrdering.Infra.Repositories;

internal sealed class CartRepository(NpgsqlDataSource dataSource) : ICartRepository
{
    public async Task<CartResponse?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            """
            SELECT
                c.cart_id AS "CartId",
                c.user_id AS "UserId"
            FROM carts AS c
            WHERE c.user_id = @UserId;

            WITH cart_lines AS (
                SELECT
                    ci.variant_id,
                    ci.flash_sale_id,
                    ci.quantity,
                    p.product_id,
                    p.name AS product_name,
                    p.description,
                    p.image_url,
                    p.is_active AS product_is_active,
                    pv.sku,
                    pv.base_price,
                    pv.is_active AS variant_is_active,
                    COALESCE(i.available_quantity, 0)::integer AS available_quantity,
                    fsv.sale_price,
                    fs.status AS flash_sale_status,
                    fs.starts_at,
                    fs.ends_at,
                    fsp.per_user_limit,
                    COALESCE(pq.purchased_quantity, 0)::integer AS purchased_quantity,
                    CASE
                        WHEN ci.flash_sale_id IS NULL THEN ci.quantity
                        ELSE SUM(ci.quantity) OVER (
                            PARTITION BY ci.cart_id, ci.flash_sale_id, pv.product_id
                        )::integer
                    END AS requested_product_quantity
                FROM carts AS c
                JOIN cart_items AS ci ON ci.cart_id = c.cart_id
                JOIN product_variants AS pv ON pv.variant_id = ci.variant_id
                JOIN products AS p ON p.product_id = pv.product_id
                LEFT JOIN inventory AS i ON i.variant_id = pv.variant_id
                LEFT JOIN flash_sale_variants AS fsv
                    ON fsv.flash_sale_id = ci.flash_sale_id
                   AND fsv.variant_id = ci.variant_id
                   AND fsv.product_id = pv.product_id
                LEFT JOIN flash_sales AS fs ON fs.flash_sale_id = ci.flash_sale_id
                LEFT JOIN flash_sale_products AS fsp
                    ON fsp.flash_sale_id = ci.flash_sale_id
                   AND fsp.product_id = pv.product_id
                LEFT JOIN purchase_quotas AS pq
                    ON pq.flash_sale_id = ci.flash_sale_id
                   AND pq.product_id = pv.product_id
                   AND pq.user_id = c.user_id
                WHERE c.user_id = @UserId
            ),
            evaluated_lines AS (
                SELECT
                    cl.*,
                    COALESCE(cl.sale_price, cl.base_price) AS unit_price,
                    CASE
                        WHEN NOT cl.product_is_active OR NOT cl.variant_is_active
                            THEN 'VariantInactive'
                        WHEN cl.flash_sale_id IS NOT NULL
                             AND (
                                 cl.flash_sale_status IS DISTINCT FROM 'Active'
                                 OR CURRENT_TIMESTAMP < cl.starts_at
                                 OR CURRENT_TIMESTAMP >= cl.ends_at
                                 OR cl.sale_price IS NULL
                             )
                            THEN 'SaleEnded'
                        WHEN cl.available_quantity <= 0
                            THEN 'OutOfStock'
                        WHEN cl.available_quantity < cl.quantity
                            THEN 'InsufficientStock'
                        WHEN cl.flash_sale_id IS NOT NULL
                             AND cl.per_user_limit IS NOT NULL
                             AND cl.purchased_quantity + cl.requested_product_quantity > cl.per_user_limit
                            THEN 'QuotaExceeded'
                        ELSE 'Available'
                    END AS item_status
                FROM cart_lines AS cl
            )
            SELECT
                el.product_id AS "ProductId",
                el.variant_id AS "VariantId",
                el.flash_sale_id AS "FlashSaleId",
                el.product_name AS "ProductName",
                el.description AS "Description",
                el.image_url AS "ImageUrl",
                el.sku AS "Sku",
                el.quantity AS "Quantity",
                el.available_quantity AS "AvailableQuantity",
                el.unit_price AS "UnitPrice",
                el.unit_price * el.quantity AS "LineTotal",
                el.item_status AS "Status",
                CASE el.item_status
                    WHEN 'Available' THEN 'พร้อมสั่งซื้อ'
                    WHEN 'OutOfStock' THEN 'สินค้าหมด กรุณาลบออกจากรายการ'
                    WHEN 'InsufficientStock' THEN 'สต๊อกไม่เพียงพอ กรุณาลดจำนวน'
                    WHEN 'SaleEnded' THEN 'Flash Sale สิ้นสุดแล้ว กรุณาลบออกจากรายการ'
                    WHEN 'VariantInactive' THEN 'ตัวเลือกสินค้านี้ปิดการขายแล้ว'
                    WHEN 'QuotaExceeded' THEN 'จำนวนสินค้าเกินโควตา Flash Sale'
                    ELSE 'ไม่สามารถสั่งซื้อรายการนี้ได้'
                END AS "Message"
            FROM evaluated_lines AS el
            ORDER BY el.product_name, el.sku;

            SELECT
                ci.variant_id AS "VariantId",
                po.name AS "Name",
                pov.value AS "Value"
            FROM carts AS c
            JOIN cart_items AS ci ON ci.cart_id = c.cart_id
            JOIN product_variant_values AS pvv ON pvv.variant_id = ci.variant_id
            JOIN product_options AS po
                ON po.product_id = pvv.product_id
               AND po.option_id = pvv.option_id
            JOIN product_option_values AS pov
                ON pov.option_id = pvv.option_id
               AND pov.option_value_id = pvv.option_value_id
            WHERE c.user_id = @UserId
            ORDER BY ci.variant_id, po.display_order;
            """;

        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { UserId = userId },
            cancellationToken: cancellationToken);

        using var grid = await connection.QueryMultipleAsync(command);
        var cart = await grid.ReadSingleOrDefaultAsync<CartResponse>();

        if (cart is null)
        {
            return null;
        }

        cart.Items = (await grid.ReadAsync<CartItemResponse>()).AsList();
        var optionRows = (await grid.ReadAsync<CartItemOptionRow>()).AsList();
        var optionsByVariant = optionRows
            .GroupBy(row => row.VariantId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(row => new CartItemOptionResponse
                    {
                        Name = row.Name,
                        Value = row.Value
                    })
                    .ToList());

        foreach (var item in cart.Items)
        {
            if (optionsByVariant.TryGetValue(item.VariantId, out var options))
            {
                item.Options = options;
            }
        }

        cart.CanCheckout = cart.Items.Count > 0
            && cart.Items.All(item => item.Status == "Available");
        cart.TotalQuantity = cart.Items.Sum(item => item.Quantity);
        cart.TotalAmount = cart.Items.Sum(item => item.LineTotal);

        return cart;
    }

    public async Task<CartMutationStatus> AddItemAsync(
        Guid userId,
        Guid variantId,
        Guid? flashSaleId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        const string validationSql =
            """
            SELECT
                EXISTS (
                    SELECT 1
                    FROM users
                    WHERE user_id = @UserId AND is_active = true
                ) AS "UserExists",
                EXISTS (
                    SELECT 1
                    FROM product_variants AS pv
                    JOIN products AS p ON p.product_id = pv.product_id
                    WHERE pv.variant_id = @VariantId
                      AND pv.is_active = true
                      AND p.is_active = true
                ) AS "VariantExists",
                (
                    @FlashSaleId IS NULL
                    OR EXISTS (
                        SELECT 1
                        FROM flash_sale_variants AS fsv
                        JOIN flash_sales AS fs ON fs.flash_sale_id = fsv.flash_sale_id
                        WHERE fsv.flash_sale_id = @FlashSaleId
                          AND fsv.variant_id = @VariantId
                          AND fs.status = 'Active'
                          AND CURRENT_TIMESTAMP >= fs.starts_at
                          AND CURRENT_TIMESTAMP < fs.ends_at
                    )
                ) AS "FlashSaleAvailable";
            """;

        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        var validation = await connection.QuerySingleAsync<CartItemValidation>(
            new CommandDefinition(
                validationSql,
                new { UserId = userId, VariantId = variantId, FlashSaleId = flashSaleId },
                cancellationToken: cancellationToken));

        if (!validation.UserExists)
        {
            return CartMutationStatus.UserNotFound;
        }

        if (!validation.VariantExists)
        {
            return CartMutationStatus.VariantNotFound;
        }

        if (!validation.FlashSaleAvailable)
        {
            return CartMutationStatus.FlashSaleUnavailable;
        }

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        var cartId = await connection.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(
                """
                SELECT cart_id
                FROM carts
                WHERE user_id = @UserId;
                """,
                new { UserId = userId },
                transaction,
                cancellationToken: cancellationToken));

        if (!cartId.HasValue)
        {
            cartId = await connection.QuerySingleOrDefaultAsync<Guid?>(
                new CommandDefinition(
                    """
                    INSERT INTO carts (cart_id, user_id)
                    VALUES (@CartId, @UserId)
                    ON CONFLICT (user_id) DO NOTHING
                    RETURNING cart_id;
                    """,
                    new { CartId = Guid.NewGuid(), UserId = userId },
                    transaction,
                    cancellationToken: cancellationToken));

            // Another request may have created the user's cart concurrently.
            cartId ??= await connection.QuerySingleAsync<Guid>(
                new CommandDefinition(
                    """
                    SELECT cart_id
                    FROM carts
                    WHERE user_id = @UserId;
                    """,
                    new { UserId = userId },
                    transaction,
                    cancellationToken: cancellationToken));
        }

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                INSERT INTO cart_items (
                    cart_id,
                    variant_id,
                    flash_sale_id,
                    quantity
                )
                VALUES (@CartId, @VariantId, @FlashSaleId, @Quantity)
                ON CONFLICT (cart_id, variant_id) DO UPDATE
                SET flash_sale_id = EXCLUDED.flash_sale_id,
                    quantity = EXCLUDED.quantity,
                    updated_at = CURRENT_TIMESTAMP;
                """,
                new
                {
                    CartId = cartId.Value,
                    VariantId = variantId,
                    FlashSaleId = flashSaleId,
                    Quantity = quantity
                },
                transaction,
                cancellationToken: cancellationToken));

        await connection.ExecuteAsync(
            new CommandDefinition(
                """
                UPDATE carts
                SET updated_at = CURRENT_TIMESTAMP
                WHERE cart_id = @CartId;
                """,
                new { CartId = cartId.Value },
                transaction,
                cancellationToken: cancellationToken));

        await transaction.CommitAsync(cancellationToken);
        return CartMutationStatus.Success;
    }

    public async Task<CartMutationStatus> UpdateQuantityAsync(
        Guid userId,
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            """
            WITH changed AS (
                UPDATE cart_items AS ci
                SET quantity = @Quantity,
                    updated_at = CURRENT_TIMESTAMP
                FROM carts AS c
                WHERE c.cart_id = ci.cart_id
                  AND c.user_id = @UserId
                  AND ci.variant_id = @VariantId
                RETURNING ci.cart_id
            )
            UPDATE carts AS c
            SET updated_at = CURRENT_TIMESTAMP
            WHERE c.cart_id IN (SELECT cart_id FROM changed)
            RETURNING c.cart_id;
            """;

        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        var cartId = await connection.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(
                sql,
                new { UserId = userId, VariantId = variantId, Quantity = quantity },
                cancellationToken: cancellationToken));

        return cartId.HasValue
            ? CartMutationStatus.Success
            : CartMutationStatus.ItemNotFound;
    }

    public async Task<CartMutationStatus> RemoveItemAsync(
        Guid userId,
        Guid variantId,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            """
            WITH removed AS (
                DELETE FROM cart_items AS ci
                USING carts AS c
                WHERE c.cart_id = ci.cart_id
                  AND c.user_id = @UserId
                  AND ci.variant_id = @VariantId
                RETURNING ci.cart_id
            )
            UPDATE carts AS c
            SET updated_at = CURRENT_TIMESTAMP
            WHERE c.cart_id IN (SELECT cart_id FROM removed)
            RETURNING c.cart_id;
            """;

        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        var cartId = await connection.QuerySingleOrDefaultAsync<Guid?>(
            new CommandDefinition(
                sql,
                new { UserId = userId, VariantId = variantId },
                cancellationToken: cancellationToken));

        return cartId.HasValue
            ? CartMutationStatus.Success
            : CartMutationStatus.ItemNotFound;
    }

    private sealed class CartItemValidation
    {
        public bool UserExists { get; set; }
        public bool VariantExists { get; set; }
        public bool FlashSaleAvailable { get; set; }
    }

    private sealed class CartItemOptionRow
    {
        public Guid VariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
