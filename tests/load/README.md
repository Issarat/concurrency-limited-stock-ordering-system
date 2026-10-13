# Load tests

k6 scenarios will verify that concurrent requests never oversell inventory, never exceed per-user quotas, and never create duplicate orders for the same idempotency key.
