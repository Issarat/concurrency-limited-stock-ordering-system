BEGIN;

ALTER TABLE users
    DROP CONSTRAINT uq_users_username;

CREATE UNIQUE INDEX ux_users_admin_username
    ON users (lower(username))
    WHERE is_anonymous = false;

COMMIT;
