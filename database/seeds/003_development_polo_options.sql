BEGIN;

-- Replace the placeholder Polo variant with real Color x Size combinations.
UPDATE product_variants
SET is_active = false,
    updated_at = CURRENT_TIMESTAMP
WHERE product_id = 'ec739637-9142-4428-9f00-84feb6e81336';

UPDATE inventory
SET available_quantity = 0,
    row_version = row_version + 1,
    updated_at = CURRENT_TIMESTAMP
WHERE variant_id = '68f86ffc-99d1-40a6-ba12-4d1b529e2c5d';

INSERT INTO product_options (
    option_id,
    product_id,
    name,
    display_order,
    is_required
)
VALUES
    (gen_random_uuid(), 'ec739637-9142-4428-9f00-84feb6e81336', 'สี', 1, true),
    (gen_random_uuid(), 'ec739637-9142-4428-9f00-84feb6e81336', 'ขนาด', 2, true)
ON CONFLICT (product_id, name) DO UPDATE
SET display_order = EXCLUDED.display_order,
    is_required = EXCLUDED.is_required;

WITH color_values(value, swatch_hex, display_order) AS (
    VALUES
        ('แดง', '#D62828', 1),
        ('น้ำเงินราชวงศ์', '#243B8F', 2),
        ('ฟ้าอ่อน', '#9EDAF0', 3),
        ('เหลือง', '#F6C431', 4),
        ('ขาว', '#F4F4F4', 5),
        ('น้ำเงินเข้ม', '#19233F', 6),
        ('ดำ', '#171717', 7),
        ('เขียวเข้ม', '#147A4A', 8),
        ('ส้ม', '#F05A28', 9),
        ('เทา', '#777D85', 10)
)
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
    cv.value,
    cv.swatch_hex,
    cv.display_order,
    true
FROM product_options AS po
CROSS JOIN color_values AS cv
WHERE po.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
  AND po.name = 'สี'
ON CONFLICT (option_id, value) DO UPDATE
SET swatch_hex = EXCLUDED.swatch_hex,
    display_order = EXCLUDED.display_order,
    is_active = EXCLUDED.is_active;

UPDATE product_option_values AS pov
SET value = CASE pov.value
        WHEN 'M (คำแนะนำ 50-55 กก.)' THEN 'M'
        WHEN 'L (คำแนะนำ 55-60 กก.)' THEN 'L'
        WHEN 'XL (คำแนะนำ 60-65 กก.)' THEN 'XL'
        WHEN 'XXL (คำแนะนำ 65-70 กก.)' THEN 'XXL'
        WHEN 'XXXL (คำแนะนำ 70-80 กก.)' THEN 'XXXL'
        WHEN 'XXXXL (คำแนะนำ 80-85 กก.)' THEN 'XXXXL'
    END
FROM product_options AS po
WHERE po.option_id = pov.option_id
  AND po.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
  AND po.name = 'ขนาด'
  AND pov.value IN (
      'M (คำแนะนำ 50-55 กก.)',
      'L (คำแนะนำ 55-60 กก.)',
      'XL (คำแนะนำ 60-65 กก.)',
      'XXL (คำแนะนำ 65-70 กก.)',
      'XXXL (คำแนะนำ 70-80 กก.)',
      'XXXXL (คำแนะนำ 80-85 กก.)'
  );

WITH size_values(value, display_order) AS (
    VALUES
        ('M', 1),
        ('L', 2),
        ('XL', 3),
        ('XXL', 4),
        ('XXXL', 5),
        ('XXXXL', 6)
)
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
    sv.value,
    NULL,
    sv.display_order,
    true
FROM product_options AS po
CROSS JOIN size_values AS sv
WHERE po.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
  AND po.name = 'ขนาด'
ON CONFLICT (option_id, value) DO UPDATE
SET swatch_hex = EXCLUDED.swatch_hex,
    display_order = EXCLUDED.display_order,
    is_active = EXCLUDED.is_active;

WITH colors(code) AS (
    VALUES
        ('RED'), ('ROYAL'), ('SKY'), ('YELLOW'), ('WHITE'),
        ('NAVY'), ('BLACK'), ('GREEN'), ('ORANGE'), ('GRAY')
),
sizes(code) AS (
    VALUES ('M'), ('L'), ('XL'), ('2XL'), ('3XL'), ('4XL')
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
    'ec739637-9142-4428-9f00-84feb6e81336'::uuid,
    'POLO-' || colors.code || '-' || sizes.code,
    790.00,
    'color=' || colors.code || '|size=' || sizes.code,
    true
FROM colors
CROSS JOIN sizes
ON CONFLICT (sku) DO UPDATE
SET product_id = EXCLUDED.product_id,
    base_price = EXCLUDED.base_price,
    combination_key = EXCLUDED.combination_key,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

DELETE FROM product_variant_values
WHERE product_id = 'ec739637-9142-4428-9f00-84feb6e81336';

WITH colors(code, value) AS (
    VALUES
        ('RED', 'แดง'),
        ('ROYAL', 'น้ำเงินราชวงศ์'),
        ('SKY', 'ฟ้าอ่อน'),
        ('YELLOW', 'เหลือง'),
        ('WHITE', 'ขาว'),
        ('NAVY', 'น้ำเงินเข้ม'),
        ('BLACK', 'ดำ'),
        ('GREEN', 'เขียวเข้ม'),
        ('ORANGE', 'ส้ม'),
        ('GRAY', 'เทา')
),
sizes(code, value) AS (
    VALUES
        ('M', 'M'),
        ('L', 'L'),
        ('XL', 'XL'),
        ('2XL', 'XXL'),
        ('3XL', 'XXXL'),
        ('4XL', 'XXXXL')
),
variant_map AS (
    SELECT
        pv.variant_id,
        colors.value AS color_value,
        sizes.value AS size_value
    FROM product_variants AS pv
    CROSS JOIN colors
    CROSS JOIN sizes
    WHERE pv.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
      AND pv.combination_key = 'color=' || colors.code || '|size=' || sizes.code
)
INSERT INTO product_variant_values (
    product_id,
    variant_id,
    option_id,
    option_value_id
)
SELECT
    'ec739637-9142-4428-9f00-84feb6e81336'::uuid,
    vm.variant_id,
    po.option_id,
    pov.option_value_id
FROM variant_map AS vm
JOIN product_options AS po
  ON po.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
 AND po.name = 'สี'
JOIN product_option_values AS pov
  ON pov.option_id = po.option_id
 AND pov.value = vm.color_value
UNION ALL
SELECT
    'ec739637-9142-4428-9f00-84feb6e81336'::uuid,
    vm.variant_id,
    po.option_id,
    pov.option_value_id
FROM variant_map AS vm
JOIN product_options AS po
  ON po.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
 AND po.name = 'ขนาด'
JOIN product_option_values AS pov
  ON pov.option_id = po.option_id
 AND pov.value = vm.size_value
ON CONFLICT (variant_id, option_id) DO UPDATE
SET option_value_id = EXCLUDED.option_value_id;

INSERT INTO inventory (variant_id, available_quantity)
SELECT pv.variant_id, 1
FROM product_variants AS pv
WHERE pv.product_id = 'ec739637-9142-4428-9f00-84feb6e81336'
  AND pv.is_active = true
ON CONFLICT (variant_id) DO UPDATE
SET available_quantity = EXCLUDED.available_quantity,
    row_version = inventory.row_version + 1,
    updated_at = CURRENT_TIMESTAMP;

COMMIT;
