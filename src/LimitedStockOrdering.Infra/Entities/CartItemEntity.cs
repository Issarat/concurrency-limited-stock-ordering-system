namespace LimitedStockOrdering.Infra.Entities;

internal sealed class CartItemEntity
{
    public Guid CartId { get; set; }
    public Guid VariantId { get; set; }
    public Guid? FlashSaleId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
