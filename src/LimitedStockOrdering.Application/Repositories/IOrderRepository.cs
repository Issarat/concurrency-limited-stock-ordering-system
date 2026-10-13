using LimitedStockOrdering.Application.Orders;

namespace LimitedStockOrdering.Application.Repositories;

public interface IOrderRepository
{
    Task<OrderPreviewResponse?> GetPreviewAsync(
        Guid productId,
        Guid variantId,
        int quantity,
        string orderType,
        CancellationToken cancellationToken = default);
}
