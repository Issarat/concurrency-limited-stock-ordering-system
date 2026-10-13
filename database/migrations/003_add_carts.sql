BEGIN;

CREATE TABLE carts (
    cart_id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT fk_carts_user
        FOREIGN KEY (user_id) REFERENCES users (user_id),
    CONSTRAINT uq_carts_user UNIQUE (user_id)
);

CREATE TABLE cart_items (
    cart_id uuid NOT NULL,
    variant_id uuid NOT NULL,
    flash_sale_id uuid NULL,
    quantity integer NOT NULL,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT pk_cart_items PRIMARY KEY (cart_id, variant_id),
    CONSTRAINT fk_cart_items_cart
        FOREIGN KEY (cart_id)
        REFERENCES carts (cart_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_cart_items_variant
        FOREIGN KEY (variant_id)
        REFERENCES product_variants (variant_id),
    CONSTRAINT fk_cart_items_flash_sale_variant
        FOREIGN KEY (flash_sale_id, variant_id)
        REFERENCES flash_sale_variants (flash_sale_id, variant_id),
    CONSTRAINT ck_cart_items_quantity CHECK (quantity > 0)
);

CREATE INDEX ix_cart_items_variant_cart
    ON cart_items (variant_id, cart_id);

CREATE INDEX ix_cart_items_flash_sale_cart
    ON cart_items (flash_sale_id, cart_id)
    WHERE flash_sale_id IS NOT NULL;

COMMIT;
