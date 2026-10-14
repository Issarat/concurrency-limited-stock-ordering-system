using Dapper;
using LimitedStockOrdering.Application.Authentication;
using LimitedStockOrdering.Application.Repositories;
using Npgsql;

namespace LimitedStockOrdering.Infra.Repositories;

internal sealed class AuthenticationRepository(NpgsqlDataSource dataSource)
    : IAuthenticationRepository
{
    private static readonly string[] Adjectives =
    [
        "Brave",
        "Bright",
        "Calm",
        "Clever",
        "Curious",
        "Friendly",
        "Gentle",
        "Happy",
        "Kind",
        "Lucky",
        "Quick",
        "Wise"
    ];

    private static readonly string[] Animals =
    [
        "Bear",
        "Cat",
        "Deer",
        "Dolphin",
        "Falcon",
        "Fox",
        "Koala",
        "Otter",
        "Owl",
        "Panda",
        "Rabbit",
        "Tiger"
    ];

    public async Task<AuthUser?> FindAdminByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            """
            SELECT
                user_id AS "UserId",
                username AS "Username",
                password_hash AS "PasswordHash",
                first_name AS "FirstName",
                last_name AS "LastName",
                role AS "Role",
                is_anonymous AS "IsAnonymous"
            FROM users
            WHERE lower(username) = lower(@Username)
              AND role = 'Admin'
              AND is_anonymous = false
              AND is_active = true;
            """;

        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<AuthUser>(
            new CommandDefinition(
                sql,
                new { Username = username },
                cancellationToken: cancellationToken));
    }

    public async Task<AuthUser> CreateGuestAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql =
            """
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
                @UserId,
                @Username,
                NULL,
                @FirstName,
                @LastName,
                'Buyer',
                true,
                true
            )
            RETURNING
                user_id AS "UserId",
                username AS "Username",
                password_hash AS "PasswordHash",
                first_name AS "FirstName",
                last_name AS "LastName",
                role AS "Role",
                is_anonymous AS "IsAnonymous";
            """;

        var userId = Guid.NewGuid();
        var firstName = Adjectives[Random.Shared.Next(Adjectives.Length)];
        var lastName = Animals[Random.Shared.Next(Animals.Length)];
        await using var connection =
            await dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleAsync<AuthUser>(
            new CommandDefinition(
                sql,
                new
                {
                    UserId = userId,
                    Username = $"{firstName}{lastName}",
                    FirstName = firstName,
                    LastName = lastName
                },
                cancellationToken: cancellationToken));
    }
}
