BEGIN;

INSERT INTO categories (category_id, name, is_active)
VALUES
    ('1eca8d84-b4b9-4748-8cf6-a4210f3351cd', 'Sportswear', true),
    ('a816dbe9-779e-4b70-be95-f37b888be007', 'Tops', true),
    ('ad8756d8-8a7c-4589-a460-c58870ee882c', 'Bottoms', true),
    ('293cd6cd-d152-496d-a213-663d06fd8584', 'Footwear', true),
    ('bfaa1943-c2f3-4df4-80f4-dc6dc65c098d', 'Accessories', true)
ON CONFLICT (category_id) DO UPDATE
SET name = EXCLUDED.name,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

INSERT INTO products (
    product_id,
    name,
    description,
    image_url,
    is_active
)
VALUES
    (
        'e42aa923-6158-42ee-80e5-0c776e792abb',
        'White Football Jersey',
        'Lightweight white football jersey with contrast shoulder stripes.',
        '/src/assets/image/football-shirt.jpg',
        true
    ),
    (
        '97970765-3594-40ea-9d58-deb8fb05727c',
        'Colorful Chino Shorts',
        'Classic chino shorts shown in a bright multi-color collection.',
        '/src/assets/image/pants.jpg',
        true
    ),
    (
        'ec739637-9142-4428-9f00-84feb6e81336',
        'Classic Color Polo',
        'Everyday polo shirt available in a wide range of colors.',
        '/src/assets/image/polo.jpg',
        true
    ),
    (
        'b283beb4-d2ae-441a-82b8-7a7c8bf7e70c',
        'Pink Character Clogs',
        'Comfortable pink clogs decorated with playful character charms.',
        '/src/assets/image/sandals.jpg',
        true
    ),
    (
        '2d178257-3a1f-4813-abf3-b9b936823299',
        'Black Canvas Sneakers',
        'Black low-top canvas sneakers with white laces and platform soles.',
        '/src/assets/image/shoes.jpg',
        true
    ),
    (
        'fded97d2-345f-40e1-ad6f-87916a7804c4',
        'White Performance Runners',
        'Breathable white running shoes designed for everyday training.',
        '/src/assets/image/shoes2.jpg',
        true
    ),
    (
        '408559e3-0a82-4c57-a871-7ae6db0c23f2',
        'Everyday Crew Socks',
        'Soft ribbed crew socks in calm neutral and pastel colors.',
        '/src/assets/image/sock.jpg',
        true
    ),
    (
        '333ea216-de6b-49c4-9f13-61930e01db88',
        'Sailboat Print Shirt',
        'Casual short-sleeve shirt with an all-over sailboat print.',
        '/src/assets/image/women-shirt.jpg',
        true
    )
ON CONFLICT (product_id) DO UPDATE
SET name = EXCLUDED.name,
    description = EXCLUDED.description,
    image_url = EXCLUDED.image_url,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

INSERT INTO product_categories (product_id, category_id)
VALUES
    ('e42aa923-6158-42ee-80e5-0c776e792abb', '1eca8d84-b4b9-4748-8cf6-a4210f3351cd'),
    ('e42aa923-6158-42ee-80e5-0c776e792abb', 'a816dbe9-779e-4b70-be95-f37b888be007'),
    ('97970765-3594-40ea-9d58-deb8fb05727c', 'ad8756d8-8a7c-4589-a460-c58870ee882c'),
    ('ec739637-9142-4428-9f00-84feb6e81336', 'a816dbe9-779e-4b70-be95-f37b888be007'),
    ('b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', '293cd6cd-d152-496d-a213-663d06fd8584'),
    ('2d178257-3a1f-4813-abf3-b9b936823299', '293cd6cd-d152-496d-a213-663d06fd8584'),
    ('fded97d2-345f-40e1-ad6f-87916a7804c4', '293cd6cd-d152-496d-a213-663d06fd8584'),
    ('408559e3-0a82-4c57-a871-7ae6db0c23f2', 'bfaa1943-c2f3-4df4-80f4-dc6dc65c098d'),
    ('333ea216-de6b-49c4-9f13-61930e01db88', 'a816dbe9-779e-4b70-be95-f37b888be007')
ON CONFLICT (product_id, category_id) DO NOTHING;

INSERT INTO product_variants (
    variant_id,
    product_id,
    sku,
    base_price,
    combination_key,
    is_active
)
VALUES
    ('c51342fc-d5d0-4093-aba8-38bb6e6e8e2d', 'e42aa923-6158-42ee-80e5-0c776e792abb', 'JERSEY-WHITE-DEFAULT', 1290.00, 'default', true),
    ('aea52f46-6697-443a-8727-673769c1e7e1', '97970765-3594-40ea-9d58-deb8fb05727c', 'CHINO-COLOR-DEFAULT', 890.00, 'default', true),
    ('68f86ffc-99d1-40a6-ba12-4d1b529e2c5d', 'ec739637-9142-4428-9f00-84feb6e81336', 'POLO-COLOR-DEFAULT', 790.00, 'default', true),
    ('e7da26bd-04aa-4bb4-be5a-8c17d05433ae', 'b283beb4-d2ae-441a-82b8-7a7c8bf7e70c', 'CLOG-PINK-DEFAULT', 590.00, 'default', true),
    ('0eeddbe1-9fb1-4b21-8a7c-533b5ce3f28b', '2d178257-3a1f-4813-abf3-b9b936823299', 'CANVAS-BLACK-DEFAULT', 1490.00, 'default', true),
    ('4e393dbd-639d-40f2-95b9-a7379cc3c6dd', 'fded97d2-345f-40e1-ad6f-87916a7804c4', 'RUNNER-WHITE-DEFAULT', 2490.00, 'default', true),
    ('82e8560f-b394-40c6-b96d-c5c11d10d85f', '408559e3-0a82-4c57-a871-7ae6db0c23f2', 'SOCK-CREW-DEFAULT', 290.00, 'default', true),
    ('b37996a9-3a1c-4a91-bffd-05a4ce063365', '333ea216-de6b-49c4-9f13-61930e01db88', 'SHIRT-SAILBOAT-DEFAULT', 990.00, 'default', true)
ON CONFLICT (variant_id) DO UPDATE
SET product_id = EXCLUDED.product_id,
    sku = EXCLUDED.sku,
    base_price = EXCLUDED.base_price,
    combination_key = EXCLUDED.combination_key,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

INSERT INTO inventory (variant_id, available_quantity)
VALUES
    ('c51342fc-d5d0-4093-aba8-38bb6e6e8e2d', 100),
    ('aea52f46-6697-443a-8727-673769c1e7e1', 5),
    ('68f86ffc-99d1-40a6-ba12-4d1b529e2c5d', 60),
    ('e7da26bd-04aa-4bb4-be5a-8c17d05433ae', 25),
    ('0eeddbe1-9fb1-4b21-8a7c-533b5ce3f28b', 0),
    ('4e393dbd-639d-40f2-95b9-a7379cc3c6dd', 8),
    ('82e8560f-b394-40c6-b96d-c5c11d10d85f', 100),
    ('b37996a9-3a1c-4a91-bffd-05a4ce063365', 35)
ON CONFLICT (variant_id) DO UPDATE
SET available_quantity = EXCLUDED.available_quantity,
    row_version = inventory.row_version + 1,
    updated_at = CURRENT_TIMESTAMP;

COMMIT;
