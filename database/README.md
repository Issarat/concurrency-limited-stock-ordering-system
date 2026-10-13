# Database

PostgreSQL is the transactional source of truth for inventory, purchase quotas, and orders.

- `migrations/` contains ordered schema changes. Docker executes these scripts alphabetically only when it initializes an empty data volume.
- `seeds/001_development_products.sql` contains repeatable local catalog data based on the images in `src/LimitedStockOrdering.Web/src/assets/image`.
- `seeds/002_development_regular_order.sql` contains one completed regular order for sales-summary development.

## Create the local database

From the repository root:

```powershell
Copy-Item .env.example .env
docker compose up -d postgres
docker compose ps
```

`001_initial_schema.sql` creates all tables. `002_add_regular_orders.sql` adds Regular and FlashSale order types. Docker then applies the product and regular-order development seeds.

Verify the schema:

```powershell
docker compose exec postgres psql -U limited_stock -d limited_stock -c "\dt"
```

Apply or reset the development catalog and example order on an existing local volume:

```powershell
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/100_development_products.sql
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/110_development_regular_order.sql
```

The seed is repeatable. It creates five categories and eight active products, each with a Default variant and inventory row.

Apply the regular-order migration and its example seed to an existing local volume:

```powershell
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/002_add_regular_orders.sql
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/110_development_regular_order.sql
```

The regular-order seed creates `demo.buyer`, one Completed order for two White Football Jerseys, and a consistent remaining inventory quantity of 98. Its password hash is intentionally a placeholder until authentication seeding is implemented.

If the `postgres-data` volume already existed before a new migration was added, Docker will not execute initialization scripts again. For disposable local data only, recreate the database with:

```powershell
docker compose down -v
docker compose up -d postgres
```

`docker compose down -v` permanently removes the local PostgreSQL volume. Do not use it when the data must be preserved.
