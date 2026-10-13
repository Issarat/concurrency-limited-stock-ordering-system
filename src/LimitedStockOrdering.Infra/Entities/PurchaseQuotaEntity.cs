namespace LimitedStockOrdering.Infra.Entities;

internal sealed class PurchaseQuotaEntity
{
    public Guid FlashSaleId { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public int PurchasedQuantity { get; set; }
    public DateTime UpdatedAt { get; set; }
}
