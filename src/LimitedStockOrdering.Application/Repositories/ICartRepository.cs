using LimitedStockOrdering.Application.Carts;

namespace LimitedStockOrdering.Application.Repositories;

public interface ICartRepository
{
    Task<CartResponse?> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<CartMutationStatus> AddItemAsync(
        Guid userId,
        Guid variantId,
        Guid? flashSaleId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<CartMutationStatus> UpdateQuantityAsync(
        Guid userId,
        Guid variantId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<CartMutationStatus> RemoveItemAsync(
        Guid userId,
        Guid variantId,
        CancellationToken cancellationToken = default);
}
