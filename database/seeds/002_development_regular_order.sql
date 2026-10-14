BEGIN;

INSERT INTO users (
    user_id,
    username,
    password_hash,
    first_name,
    last_name,
    role,
    is_anonymous,
    is_active
)
VALUES (
    '4df29112-6160-41a5-88a1-8f85532b0231',
    'demo.buyer',
    NULL,
    'Demo',
    'Buyer',
    'Buyer',
    true,
    true
)
ON CONFLICT (user_id) DO UPDATE
SET username = EXCLUDED.username,
    password_hash = EXCLUDED.password_hash,
    first_name = EXCLUDED.first_name,
    last_name = EXCLUDED.last_name,
    role = EXCLUDED.role,
    is_anonymous = EXCLUDED.is_anonymous,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

INSERT INTO orders (
    order_id,
    user_id,
    order_type,
    flash_sale_id,
    idempotency_key,
    status,
    total_amount
)
VALUES (
    '0c4f3e40-661a-4e3d-941e-6eca91d910fd',
    '4df29112-6160-41a5-88a1-8f85532b0231',
    'Regular',
    NULL,
    'development-regular-order-001',
    'Completed',
    2580.00
)
ON CONFLICT (order_id) DO UPDATE
SET order_type = EXCLUDED.order_type,
    flash_sale_id = EXCLUDED.flash_sale_id,
    status = EXCLUDED.status,
    total_amount = EXCLUDED.total_amount,
    updated_at = CURRENT_TIMESTAMP;

INSERT INTO order_items (
    order_id,
    variant_id,
    quantity,
    unit_price,
    sku_snapshot,
    product_name_snapshot,
    option_summary_snapshot
)
VALUES (
    '0c4f3e40-661a-4e3d-941e-6eca91d910fd',
    'c51342fc-d5d0-4093-aba8-38bb6e6e8e2d',
    2,
    1290.00,
    'JERSEY-WHITE-DEFAULT',
    'White Football Jersey',
    'Default'
)
ON CONFLICT (order_id, variant_id) DO UPDATE
SET quantity = EXCLUDED.quantity,
    unit_price = EXCLUDED.unit_price,
    sku_snapshot = EXCLUDED.sku_snapshot,
    product_name_snapshot = EXCLUDED.product_name_snapshot,
    option_summary_snapshot = EXCLUDED.option_summary_snapshot;

UPDATE inventory
SET available_quantity = 98,
    row_version = row_version + 1,
    updated_at = CURRENT_TIMESTAMP
WHERE variant_id = 'c51342fc-d5d0-4093-aba8-38bb6e6e8e2d'
  AND available_quantity <> 98;

COMMIT;
