# Database

PostgreSQL is the transactional source of truth for carts, inventory, purchase quotas, and orders.

- `migrations/` contains ordered schema changes. Docker executes these scripts alphabetically only when it initializes an empty data volume.
- `seeds/001_development_products.sql` contains repeatable local catalog data based on the images in `src/LimitedStockOrdering.Web/src/assets/image`.
- `seeds/002_development_regular_order.sql` contains one completed regular order for sales-summary development.
- `seeds/003_development_polo_options.sql` adds Color and Size options plus 60 SKU variants for Classic Color Polo.
- `seeds/004_development_all_product_options.sql` adds Color and Size options plus SKU variants for every remaining development product.

## Create the local database

From the repository root:

```powershell
Copy-Item .env.example .env
docker compose up -d postgres
docker compose ps
```

`001_initial_schema.sql` creates the original tables. `002_add_regular_orders.sql` adds Regular and FlashSale order types. `003_add_carts.sql` adds the non-reserving cart tables. Docker then applies the product and regular-order development seeds.

Verify the schema:

```powershell
docker compose exec postgres psql -U limited_stock -d limited_stock -c "\dt"
```

Apply or reset the development catalog and example order on an existing local volume:

```powershell
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/100_development_products.sql
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/110_development_regular_order.sql
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/120_development_polo_options.sql
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/130_development_all_product_options.sql
```

The seeds are repeatable. They create five categories and eight active products. Every product has Color and Size options backed by SKU variants and variant-level inventory.

Apply the regular-order migration and its example seed to an existing local volume:

```powershell
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/002_add_regular_orders.sql
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/110_development_regular_order.sql
```

The regular-order seed creates `demo.buyer`, one Completed order for two White Football Jerseys, and a consistent remaining inventory quantity of 98. Its password hash is intentionally a placeholder until authentication seeding is implemented.

Apply the cart migration to an existing local volume:

```powershell
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/003_add_carts.sql
```

The migration creates `carts` and `cart_items`. Cart items reference sellable variants and optionally reference the exact Flash Sale variant context. Adding an item to a cart does not decrement inventory.

If the `postgres-data` volume already existed before a new migration was added, Docker will not execute initialization scripts again. For disposable local data only, recreate the database with:

```powershell
docker compose down -v
docker compose up -d postgres
```

`docker compose down -v` permanently removes the local PostgreSQL volume. Do not use it when the data must be preserved.
