# High-Concurrency Limited Stock Ordering System — Purchase Flow

> Scope: Purchase confirmation, transactional write behavior, concurrency invariants, seed-data consistency, deletion rules, and real-time stock updates.
>
> Related database structure: [`er-diagram.md`](./er-diagram.md)

## Purchase confirmation and write flow

The application does not perform payment processing. Selecting **Purchase** redirects the buyer to a confirmation page. The write operation begins only after the buyer confirms the product and quantity.

```text
Authentication
  → PartitionedRateLimiter
  → Purchase confirmation endpoint
  → Dapper
  → Purchase stored procedure
      → Validate user and requested quantity
      → Find or recognize (user_id, idempotency_key)
      → Validate flash-sale status and UTC time window
      → Validate the product belongs to the sale
      → Validate and update the per-user quota
      → Atomically decrement inventory
      → Create order and order item
      → Commit or roll back all changes
  → Publish the committed stock value through SignalR
```

The stored procedure updates these data sets in one SQL transaction:

```text
purchase_quotas
inventory
orders + order_items
```

If the quota would be exceeded, stock is insufficient, the user or product is inactive, or the sale is unavailable, the procedure rolls back the entire operation. Rate limiting protects the API from excessive traffic but does not enforce the per-user purchase limit.

### Concurrency invariants

- `inventory.available_quantity` never becomes negative.
- A user's `purchased_quantity` never exceeds `flash_sale_products.per_user_limit`.
- A successful order, its item, the quota change, and the inventory decrement are committed together.
- Retrying the same `(user_id, idempotency_key)` does not create a second order or decrement stock again.
- SignalR broadcasts only committed inventory values and is not a source of truth.

---

## Seed-data consistency

Development data should include an administrator, ordinary buyers, buyers at `9/10` and `10/10` quota, a disabled user, products with normal, low, and zero stock, and Active, Upcoming, and Ended sale examples. Every nonzero seeded `purchase_quotas.purchased_quantity` value must be backed by matching order and order-item records.

Load-test data should contain 500–1,000 buyers, one active flash-sale product, an inventory quantity of 100, a per-user limit of 10, and no initial orders or quota consumption. A valid test run must demonstrate that the total successful quantity is at most 100, inventory never becomes negative, and idempotent retries never create duplicate orders.

---

## Delete behavior

- Product, user, flash-sale, and order records referenced by purchase history should use restricted deletion so audit data cannot be removed accidentally.
- Catalog and account availability should normally be controlled through `is_active` rather than physical deletion.
- Removing a category assignment may physically delete the corresponding `product_categories` row without affecting the product or category.
- Cancelling or closing a flash sale changes its status; it does not restore inventory or delete completed orders automatically.
- Order cancellation behavior and any optional stock-restoration workflow must be explicit application operations, not cascading database deletes.

## Real-time stock

SignalR publishes the latest committed `inventory.available_quantity` after a successful purchase. Clients treat these events as display updates only and refresh from the API after reconnecting. Database constraints and the purchase stored procedure remain responsible for correctness when SignalR delivery is delayed, duplicated, or missed.
