BEGIN;

ALTER TABLE users
    ALTER COLUMN password_hash DROP NOT NULL;

ALTER TABLE users
    ADD COLUMN is_anonymous boolean NOT NULL DEFAULT false;

UPDATE users
SET password_hash = NULL,
    is_anonymous = true,
    updated_at = CURRENT_TIMESTAMP
WHERE role = 'Buyer';

ALTER TABLE users
    ADD CONSTRAINT ck_users_authentication_type CHECK (
        (
            is_anonymous = true
            AND role = 'Buyer'
            AND password_hash IS NULL
        )
        OR
        (
            is_anonymous = false
            AND role = 'Admin'
            AND password_hash IS NOT NULL
        )
    );

CREATE INDEX ix_users_role_active
    ON users (role, is_active)
    WHERE is_active = true;

COMMIT;
