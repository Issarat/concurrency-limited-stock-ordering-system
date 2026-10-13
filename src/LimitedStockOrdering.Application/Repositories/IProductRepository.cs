using LimitedStockOrdering.Application.Products;

namespace LimitedStockOrdering.Application.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductListResponse>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDetailResponse?> GetByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
