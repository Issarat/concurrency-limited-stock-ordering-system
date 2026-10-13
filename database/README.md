# Database

PostgreSQL is the transactional source of truth for inventory, purchase quotas, and orders.

- `migrations/` contains ordered schema changes. Docker executes these scripts alphabetically only when it initializes an empty data volume.
- `seeds/001_development_products.sql` contains repeatable local catalog data based on the images in `src/LimitedStockOrdering.Web/src/assets/image`.

## Create the local database

From the repository root:

```powershell
Copy-Item .env.example .env
docker compose up -d postgres
docker compose ps
```

`001_initial_schema.sql` creates all catalog, variant, inventory, flash-sale, quota, and order tables together with their foreign keys, checks, unique constraints, and lookup indexes. Docker then applies the development product seed as `100_development_products.sql`.

Verify the schema:

```powershell
docker compose exec postgres psql -U limited_stock -d limited_stock -c "\dt"
```

Apply or reset only the development products on an existing local volume:

```powershell
docker compose exec -T postgres psql -U limited_stock -d limited_stock -v ON_ERROR_STOP=1 -f /docker-entrypoint-initdb.d/100_development_products.sql
```

The seed is repeatable. It creates five categories and eight active products, each with a Default variant and inventory row.

If the `postgres-data` volume already existed before a new migration was added, Docker will not execute initialization scripts again. For disposable local data only, recreate the database with:

```powershell
docker compose down -v
docker compose up -d postgres
```

`docker compose down -v` permanently removes the local PostgreSQL volume. Do not use it when the data must be preserved.
