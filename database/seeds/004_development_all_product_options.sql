BEGIN;

CREATE TEMP TABLE seed_products (
    product_id uuid PRIMARY KEY,
    sku_prefix varchar(30) NOT NULL,
    base_price numeric(12, 2) NOT NULL,
    total_stock integer NOT NULL
) ON COMMIT DROP;

CREATE TEMP TABLE seed_option_values (
    product_id uuid NOT NULL,
    option_name varchar(50) NOT NULL,
    option_order integer NOT NULL,
    value_code varchar(30) NOT NULL,
    value_label varchar(50) NOT NULL,
    swatch_hex varchar(7) NULL,
    value_order integer NOT NULL
) ON COMMIT DROP;

INSERT INTO seed_products (product_id, sku_prefix, base_price, total_stock)
VALUES
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'JERSEY', 1290.00, 98),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'CHINO', 890.00, 5),
    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'CLOG', 590.00, 25),
    ('2d178257-3a1f-4813-abf3-b9b936823299', 'CANVAS', 1490.00, 0),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'RUNNER', 2490.00, 8),
    ('408559e3-0a82-4c57-a871-7ae6db0c23f2', 'SOCK', 290.00, 100),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'SAILBOAT', 990.00, 35);

-- Color values
INSERT INTO seed_option_values
    (product_id, option_name, option_order, value_code, value_label, swatch_hex, value_order)
VALUES
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'สี', 1, 'WHITE', 'ขาว', '#F4F4F4', 1),

    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'BLACK', 'ดำ', '#171717', 1),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'NAVY', 'กรมท่า', '#19233F', 2),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'BURGUNDY', 'แดงเข้ม', '#7D1830', 3),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'RED', 'แดง', '#D62828', 4),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'BROWN', 'น้ำตาล', '#9A572E', 5),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'KHAKI', 'กากี', '#B7A17B', 6),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'YELLOW', 'เหลือง', '#F6C431', 7),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'SKY', 'ฟ้าอ่อน', '#9EDAF0', 8),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'สี', 1, 'WHITE', 'ขาว', '#F4F4F4', 9),

    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'สี', 1, 'PINK', 'ชมพู', '#EF9FBE', 1),
    ('2d178257-3a1f-4813-abf3-b9b936823299', 'สี', 1, 'BLACK', 'ดำ', '#171717', 1),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'สี', 1, 'WHITE', 'ขาว', '#F4F4F4', 1),

    ('408559e3-0a82-4c57-a871-7ae6db0c23f2', 'สี', 1, 'BEIGE', 'เบจ', '#C9B693', 1),
    ('408559e3-0a82-4c57-a871-7ae6db0c23f2', 'สี', 1, 'BLUE', 'ฟ้า', '#6B9DCA', 2),
    ('408559e3-0a82-4c57-a871-7ae6db0c23f2', 'สี', 1, 'PURPLE', 'ม่วง', '#8567A8', 3),

    ('333ea216-de6b-49c4-9f13-61930e01db88', 'สี', 1, 'WHITE', 'ขาว', '#F4F4F4', 1),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'สี', 1, 'BLACK', 'ดำ', '#171717', 2),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'สี', 1, 'GRAY', 'เทา', '#6D6F73', 3),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'สี', 1, 'RED', 'แดง', '#C92F2F', 4);

-- Size values
INSERT INTO seed_option_values
    (product_id, option_name, option_order, value_code, value_label, swatch_hex, value_order)
VALUES
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'ขนาด', 2, 'S', 'S', NULL, 1),
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'ขนาด', 2, 'M', 'M', NULL, 2),
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'ขนาด', 2, 'L', 'L', NULL, 3),
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'ขนาด', 2, 'XL', 'XL', NULL, 4),
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'ขนาด', 2, '2XL', 'XXL', NULL, 5),

    ('97970765-3594-40ea-9d58-deb8fb05727c', 'ขนาด', 2, 'S', 'S', NULL, 1),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'ขนาด', 2, 'M', 'M', NULL, 2),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'ขนาด', 2, 'L', 'L', NULL, 3),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'ขนาด', 2, 'XL', 'XL', NULL, 4),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'ขนาด', 2, '2XL', 'XXL', NULL, 5),

    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'ขนาด', 2, '36', '36', NULL, 1),
    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'ขนาด', 2, '37', '37', NULL, 2),
    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'ขนาด', 2, '38', '38', NULL, 3),
    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'ขนาด', 2, '39', '39', NULL, 4),
    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'ขนาด', 2, '40', '40', NULL, 5),

    ('2d178257-3a1f-4813-abf3-b9b936823299', 'ขนาด', 2, '38', '38', NULL, 1),
    ('2d178257-3a1f-4813-abf3-b9b936823299', 'ขนาด', 2, '39', '39', NULL, 2),
    ('2d178257-3a1f-4813-abf3-b9b936823299', 'ขนาด', 2, '40', '40', NULL, 3),
    ('2d178257-3a1f-4813-abf3-b9b936823299', 'ขนาด', 2, '41', '41', NULL, 4),
    ('2d178257-3a1f-4813-abf3-b9b936823299', 'ขนาด', 2, '42', '42', NULL, 5),

    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'ขนาด', 2, '38', '38', NULL, 1),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'ขนาด', 2, '39', '39', NULL, 2),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'ขนาด', 2, '40', '40', NULL, 3),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'ขนาด', 2, '41', '41', NULL, 4),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', 'ขนาด', 2, '42', '42', NULL, 5),

    ('408559e3-0a82-4c57-a871-7ae6db0c23f2', 'ขนาด', 2, 'FREE', 'Free Size', NULL, 1),

    ('333ea216-de6b-49c4-9f13-61930e01db88', 'ขนาด', 2, 'S', 'S', NULL, 1),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'ขนาด', 2, 'M', 'M', NULL, 2),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'ขนาด', 2, 'L', 'L', NULL, 3),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'ขนาด', 2, 'XL', 'XL', NULL, 4),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'ขนาด', 2, '2XL', 'XXL', NULL, 5);

UPDATE product_variants AS pv
SET is_active = false,
    updated_at = CURRENT_TIMESTAMP
FROM seed_products AS sp
WHERE sp.product_id = pv.product_id;

UPDATE inventory AS i
SET available_quantity = 0,
    row_version = i.row_version + 1,
    updated_at = CURRENT_TIMESTAMP
FROM product_variants AS pv
JOIN seed_products AS sp ON sp.product_id = pv.product_id
WHERE pv.variant_id = i.variant_id;

INSERT INTO product_options (option_id, product_id, name, display_order, is_required)
SELECT
    gen_random_uuid(),
    sov.product_id,
    sov.option_name,
    sov.option_order,
    true
FROM (
    SELECT DISTINCT product_id, option_name, option_order
    FROM seed_option_values
) AS sov
ON CONFLICT (product_id, name) DO UPDATE
SET display_order = EXCLUDED.display_order,
    is_required = EXCLUDED.is_required;

UPDATE product_option_values AS pov
SET is_active = false
FROM product_options AS po
JOIN seed_products AS sp ON sp.product_id = po.product_id
WHERE po.option_id = pov.option_id;

INSERT INTO product_option_values (
    option_value_id,
    option_id,
    value,
    swatch_hex,
    display_order,
    is_active
)
SELECT
    gen_random_uuid(),
    po.option_id,
    sov.value_label,
    sov.swatch_hex,
    sov.value_order,
    true
FROM seed_option_values AS sov
JOIN product_options AS po
  ON po.product_id = sov.product_id
 AND po.name = sov.option_name
ON CONFLICT (option_id, value) DO UPDATE
SET swatch_hex = EXCLUDED.swatch_hex,
    display_order = EXCLUDED.display_order,
    is_active = EXCLUDED.is_active;

WITH color_values AS (
    SELECT * FROM seed_option_values WHERE option_name = 'สี'
),
size_values AS (
    SELECT * FROM seed_option_values WHERE option_name = 'ขนาด'
)
INSERT INTO product_variants (
    variant_id,
    product_id,
    sku,
    base_price,
    combination_key,
    is_active
)
SELECT
    gen_random_uuid(),
    sp.product_id,
    sp.sku_prefix || '-' || color_values.value_code || '-' || size_values.value_code,
    sp.base_price,
    'color=' || color_values.value_code || '|size=' || size_values.value_code,
    true
FROM seed_products AS sp
JOIN color_values ON color_values.product_id = sp.product_id
JOIN size_values ON size_values.product_id = sp.product_id
ON CONFLICT (sku) DO UPDATE
SET product_id = EXCLUDED.product_id,
    base_price = EXCLUDED.base_price,
    combination_key = EXCLUDED.combination_key,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

DELETE FROM product_variant_values AS pvv
USING seed_products AS sp
WHERE sp.product_id = pvv.product_id;

WITH color_values AS (
    SELECT * FROM seed_option_values WHERE option_name = 'สี'
),
size_values AS (
    SELECT * FROM seed_option_values WHERE option_name = 'ขนาด'
),
variant_map AS (
    SELECT
        sp.product_id,
        pv.variant_id,
        color_values.value_label AS color_value,
        size_values.value_label AS size_value
    FROM seed_products AS sp
    JOIN color_values ON color_values.product_id = sp.product_id
    JOIN size_values ON size_values.product_id = sp.product_id
    JOIN product_variants AS pv
      ON pv.product_id = sp.product_id
     AND pv.combination_key = 'color=' || color_values.value_code || '|size=' || size_values.value_code
)
INSERT INTO product_variant_values (product_id, variant_id, option_id, option_value_id)
SELECT
    vm.product_id,
    vm.variant_id,
    po.option_id,
    pov.option_value_id
FROM variant_map AS vm
JOIN product_options AS po ON po.product_id = vm.product_id AND po.name = 'สี'
JOIN product_option_values AS pov ON pov.option_id = po.option_id AND pov.value = vm.color_value
UNION ALL
SELECT
    vm.product_id,
    vm.variant_id,
    po.option_id,
    pov.option_value_id
FROM variant_map AS vm
JOIN product_options AS po ON po.product_id = vm.product_id AND po.name = 'ขนาด'
JOIN product_option_values AS pov ON pov.option_id = po.option_id AND pov.value = vm.size_value
ON CONFLICT (variant_id, option_id) DO UPDATE
SET option_value_id = EXCLUDED.option_value_id;

WITH ranked_variants AS (
    SELECT
        pv.variant_id,
        pv.product_id,
        pv.sku,
        sp.total_stock,
        ROW_NUMBER() OVER (PARTITION BY pv.product_id ORDER BY pv.sku) AS row_number,
        COUNT(*) OVER (PARTITION BY pv.product_id) AS variant_count
    FROM product_variants AS pv
    JOIN seed_products AS sp ON sp.product_id = pv.product_id
    WHERE pv.is_active = true
)
INSERT INTO inventory (variant_id, available_quantity)
SELECT
    rv.variant_id,
    CASE
        -- A constrained-stock example: only the black / M Sailboat variant remains.
        WHEN rv.product_id = '333ea216-de6b-49c4-9f13-61930e01db88'::uuid THEN
            CASE WHEN rv.sku = 'SAILBOAT-BLACK-M' THEN rv.total_stock ELSE 0 END
        ELSE
            ((rv.total_stock / rv.variant_count)
                + CASE
                    WHEN rv.row_number <= (rv.total_stock % rv.variant_count) THEN 1
                    ELSE 0
                  END)::integer
    END
FROM ranked_variants AS rv
ON CONFLICT (variant_id) DO UPDATE
SET available_quantity = EXCLUDED.available_quantity,
    row_version = inventory.row_version + 1,
    updated_at = CURRENT_TIMESTAMP;

COMMIT;
