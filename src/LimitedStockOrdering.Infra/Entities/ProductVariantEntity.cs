namespace LimitedStockOrdering.Infra.Entities;

internal sealed class ProductVariantEntity
{
    public Guid VariantId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string CombinationKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
