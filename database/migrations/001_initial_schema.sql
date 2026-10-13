BEGIN;

SET TIME ZONE 'UTC';

CREATE TABLE users (
    user_id uuid PRIMARY KEY,
    username varchar(50) NOT NULL,
    password_hash varchar(255) NOT NULL,
    first_name varchar(50) NOT NULL,
    last_name varchar(50) NOT NULL,
    role varchar(20) NOT NULL DEFAULT 'Buyer',
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT uq_users_username UNIQUE (username),
    CONSTRAINT ck_users_role CHECK (role IN ('Admin', 'Buyer'))
);

CREATE TABLE products (
    product_id uuid PRIMARY KEY,
    name varchar(100) NOT NULL,
    description varchar(500) NULL,
    image_url text NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL
);

CREATE TABLE categories (
    category_id uuid PRIMARY KEY,
    name varchar(50) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT uq_categories_name UNIQUE (name)
);

CREATE TABLE product_categories (
    product_id uuid NOT NULL,
    category_id uuid NOT NULL,
    CONSTRAINT pk_product_categories PRIMARY KEY (product_id, category_id),
    CONSTRAINT fk_product_categories_product
        FOREIGN KEY (product_id) REFERENCES products (product_id),
    CONSTRAINT fk_product_categories_category
        FOREIGN KEY (category_id) REFERENCES categories (category_id)
);

CREATE INDEX ix_product_categories_category_product
    ON product_categories (category_id, product_id);

CREATE TABLE product_options (
    option_id uuid PRIMARY KEY,
    product_id uuid NOT NULL,
    name varchar(50) NOT NULL,
    display_order integer NOT NULL DEFAULT 0,
    is_required boolean NOT NULL DEFAULT true,
    CONSTRAINT fk_product_options_product
        FOREIGN KEY (product_id) REFERENCES products (product_id),
    CONSTRAINT uq_product_options_product_name UNIQUE (product_id, name),
    CONSTRAINT uq_product_options_product_option UNIQUE (product_id, option_id),
    CONSTRAINT ck_product_options_display_order CHECK (display_order >= 0)
);

CREATE TABLE product_option_values (
    option_value_id uuid PRIMARY KEY,
    option_id uuid NOT NULL,
    value varchar(50) NOT NULL,
    swatch_hex varchar(7) NULL,
    display_order integer NOT NULL DEFAULT 0,
    is_active boolean NOT NULL DEFAULT true,
    CONSTRAINT fk_product_option_values_option
        FOREIGN KEY (option_id) REFERENCES product_options (option_id),
    CONSTRAINT uq_product_option_values_option_value UNIQUE (option_id, value),
    CONSTRAINT uq_product_option_values_option_option_value
        UNIQUE (option_id, option_value_id),
    CONSTRAINT ck_product_option_values_swatch_hex
        CHECK (swatch_hex IS NULL OR swatch_hex ~ '^#[0-9A-Fa-f]{6}$'),
    CONSTRAINT ck_product_option_values_display_order CHECK (display_order >= 0)
);

CREATE TABLE product_option_value_images (
    image_id uuid PRIMARY KEY,
    option_value_id uuid NOT NULL,
    image_url text NOT NULL,
    alt_text varchar(200) NULL,
    display_order integer NOT NULL DEFAULT 0,
    is_primary boolean NOT NULL DEFAULT false,
    CONSTRAINT fk_product_option_value_images_option_value
        FOREIGN KEY (option_value_id)
        REFERENCES product_option_values (option_value_id),
    CONSTRAINT ck_product_option_value_images_display_order
        CHECK (display_order >= 0)
);

CREATE INDEX ix_product_option_value_images_option_value_order
    ON product_option_value_images (option_value_id, display_order);

CREATE UNIQUE INDEX ux_product_option_value_images_primary
    ON product_option_value_images (option_value_id)
    WHERE is_primary = true;

CREATE TABLE product_variants (
    variant_id uuid PRIMARY KEY,
    product_id uuid NOT NULL,
    sku varchar(50) NOT NULL,
    base_price numeric(12, 2) NOT NULL,
    combination_key varchar(300) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT fk_product_variants_product
        FOREIGN KEY (product_id) REFERENCES products (product_id),
    CONSTRAINT uq_product_variants_sku UNIQUE (sku),
    CONSTRAINT uq_product_variants_product_combination
        UNIQUE (product_id, combination_key),
    CONSTRAINT uq_product_variants_product_variant
        UNIQUE (product_id, variant_id),
    CONSTRAINT ck_product_variants_base_price CHECK (base_price >= 0)
);

CREATE TABLE product_variant_values (
    product_id uuid NOT NULL,
    variant_id uuid NOT NULL,
    option_id uuid NOT NULL,
    option_value_id uuid NOT NULL,
    CONSTRAINT pk_product_variant_values PRIMARY KEY (variant_id, option_id),
    CONSTRAINT fk_product_variant_values_variant
        FOREIGN KEY (product_id, variant_id)
        REFERENCES product_variants (product_id, variant_id),
    CONSTRAINT fk_product_variant_values_option
        FOREIGN KEY (product_id, option_id)
        REFERENCES product_options (product_id, option_id),
    CONSTRAINT fk_product_variant_values_option_value
        FOREIGN KEY (option_id, option_value_id)
        REFERENCES product_option_values (option_id, option_value_id)
);

CREATE INDEX ix_product_variant_values_option_value_variant
    ON product_variant_values (option_value_id, variant_id);

CREATE TABLE inventory (
    variant_id uuid PRIMARY KEY,
    available_quantity integer NOT NULL DEFAULT 0,
    row_version bigint NOT NULL DEFAULT 0,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_inventory_variant
        FOREIGN KEY (variant_id) REFERENCES product_variants (variant_id),
    CONSTRAINT ck_inventory_available_quantity CHECK (available_quantity >= 0),
    CONSTRAINT ck_inventory_row_version CHECK (row_version >= 0)
);

CREATE TABLE flash_sales (
    flash_sale_id uuid PRIMARY KEY,
    name varchar(100) NOT NULL,
    starts_at timestamptz NOT NULL,
    ends_at timestamptz NOT NULL,
    status varchar(20) NOT NULL DEFAULT 'Draft',
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT ck_flash_sales_time_window CHECK (ends_at > starts_at),
    CONSTRAINT ck_flash_sales_status
        CHECK (status IN ('Draft', 'Active', 'Closed', 'Cancelled'))
);

CREATE INDEX ix_flash_sales_status_window
    ON flash_sales (status, starts_at, ends_at);

CREATE TABLE flash_sale_products (
    flash_sale_id uuid NOT NULL,
    product_id uuid NOT NULL,
    per_user_limit integer NOT NULL DEFAULT 10,
    CONSTRAINT pk_flash_sale_products PRIMARY KEY (flash_sale_id, product_id),
    CONSTRAINT fk_flash_sale_products_flash_sale
        FOREIGN KEY (flash_sale_id) REFERENCES flash_sales (flash_sale_id),
    CONSTRAINT fk_flash_sale_products_product
        FOREIGN KEY (product_id) REFERENCES products (product_id),
    CONSTRAINT ck_flash_sale_products_per_user_limit CHECK (per_user_limit > 0)
);

CREATE TABLE flash_sale_variants (
    flash_sale_id uuid NOT NULL,
    product_id uuid NOT NULL,
    variant_id uuid NOT NULL,
    sale_price numeric(12, 2) NOT NULL,
    CONSTRAINT pk_flash_sale_variants PRIMARY KEY (flash_sale_id, variant_id),
    CONSTRAINT fk_flash_sale_variants_sale_product
        FOREIGN KEY (flash_sale_id, product_id)
        REFERENCES flash_sale_products (flash_sale_id, product_id),
    CONSTRAINT fk_flash_sale_variants_product_variant
        FOREIGN KEY (product_id, variant_id)
        REFERENCES product_variants (product_id, variant_id),
    CONSTRAINT ck_flash_sale_variants_sale_price CHECK (sale_price >= 0)
);

CREATE INDEX ix_flash_sale_variants_variant_sale
    ON flash_sale_variants (variant_id, flash_sale_id);

CREATE TABLE purchase_quotas (
    flash_sale_id uuid NOT NULL,
    product_id uuid NOT NULL,
    user_id uuid NOT NULL,
    purchased_quantity integer NOT NULL DEFAULT 0,
    updated_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT pk_purchase_quotas
        PRIMARY KEY (flash_sale_id, product_id, user_id),
    CONSTRAINT fk_purchase_quotas_sale_product
        FOREIGN KEY (flash_sale_id, product_id)
        REFERENCES flash_sale_products (flash_sale_id, product_id),
    CONSTRAINT fk_purchase_quotas_user
        FOREIGN KEY (user_id) REFERENCES users (user_id),
    CONSTRAINT ck_purchase_quotas_purchased_quantity
        CHECK (purchased_quantity >= 0)
);

CREATE INDEX ix_purchase_quotas_user_sale_product
    ON purchase_quotas (user_id, flash_sale_id, product_id);

CREATE TABLE orders (
    order_id uuid PRIMARY KEY,
    user_id uuid NOT NULL,
    flash_sale_id uuid NOT NULL,
    idempotency_key varchar(100) NOT NULL,
    status varchar(20) NOT NULL DEFAULT 'Pending',
    total_amount numeric(12, 2) NOT NULL,
    created_at timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamptz NULL,
    CONSTRAINT fk_orders_user FOREIGN KEY (user_id) REFERENCES users (user_id),
    CONSTRAINT fk_orders_flash_sale
        FOREIGN KEY (flash_sale_id) REFERENCES flash_sales (flash_sale_id),
    CONSTRAINT uq_orders_user_idempotency UNIQUE (user_id, idempotency_key),
    CONSTRAINT ck_orders_status
        CHECK (status IN ('Pending', 'Completed', 'Cancelled', 'Failed')),
    CONSTRAINT ck_orders_total_amount CHECK (total_amount >= 0)
);

CREATE INDEX ix_orders_user_created
    ON orders (user_id, created_at DESC);

CREATE INDEX ix_orders_flash_sale_created
    ON orders (flash_sale_id, created_at DESC);

CREATE TABLE order_items (
    order_id uuid NOT NULL,
    variant_id uuid NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(12, 2) NOT NULL,
    sku_snapshot varchar(50) NOT NULL,
    product_name_snapshot varchar(100) NOT NULL,
    option_summary_snapshot varchar(200) NOT NULL,
    CONSTRAINT pk_order_items PRIMARY KEY (order_id, variant_id),
    CONSTRAINT fk_order_items_order
        FOREIGN KEY (order_id) REFERENCES orders (order_id),
    CONSTRAINT fk_order_items_variant
        FOREIGN KEY (variant_id) REFERENCES product_variants (variant_id),
    CONSTRAINT ck_order_items_quantity CHECK (quantity > 0),
    CONSTRAINT ck_order_items_unit_price CHECK (unit_price >= 0)
);

CREATE INDEX ix_order_items_variant_order
    ON order_items (variant_id, order_id);

COMMIT;
