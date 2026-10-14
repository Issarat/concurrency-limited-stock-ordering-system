using LimitedStockOrdering.Application.Authentication;

namespace LimitedStockOrdering.Application.Repositories;

public interface IAuthenticationRepository
{
    Task<AuthUser?> FindAdminByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<AuthUser> CreateGuestAsync(
        CancellationToken cancellationToken = default);
}
