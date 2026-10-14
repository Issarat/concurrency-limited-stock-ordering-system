BEGIN;

INSERT INTO users (
    user_id,
    username,
    password_hash,
    first_name,
    last_name,
    role,
    is_anonymous,
    is_active
)
VALUES (
    '39bdc063-0eb6-49bc-98c7-d6c20b2f6cb6',
    'admin',
    '$pbkdf2-sha256$210000$7Pw6WOQKt6FMEvGMmz/TkQ==$ALiX7VIXO6TG54WzDKmDpmYaq9YRw2n56/0NdNvahvI=',
    'Demo',
    'Admin',
    'Admin',
    false,
    true
)
ON CONFLICT (user_id) DO UPDATE
SET username = EXCLUDED.username,
    password_hash = EXCLUDED.password_hash,
    first_name = EXCLUDED.first_name,
    last_name = EXCLUDED.last_name,
    role = EXCLUDED.role,
    is_anonymous = EXCLUDED.is_anonymous,
    is_active = EXCLUDED.is_active,
    updated_at = CURRENT_TIMESTAMP;

COMMIT;
