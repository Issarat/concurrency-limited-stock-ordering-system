BEGIN;

ALTER TABLE orders
    ADD COLUMN order_type varchar(20);

-- Before this migration every order belongs to a flash sale.
UPDATE orders
SET order_type = 'FlashSale';

ALTER TABLE orders
    ALTER COLUMN order_type SET DEFAULT 'Regular',
    ALTER COLUMN order_type SET NOT NULL,
    ALTER COLUMN flash_sale_id DROP NOT NULL;

ALTER TABLE orders
    ADD CONSTRAINT ck_orders_order_type
        CHECK (order_type IN ('Regular', 'FlashSale')),
    ADD CONSTRAINT ck_orders_flash_sale_consistency
        CHECK (
            (order_type = 'Regular' AND flash_sale_id IS NULL)
            OR
            (order_type = 'FlashSale' AND flash_sale_id IS NOT NULL)
        );

COMMIT;
