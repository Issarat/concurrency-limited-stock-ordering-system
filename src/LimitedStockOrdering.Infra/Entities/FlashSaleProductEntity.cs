namespace LimitedStockOrdering.Infra.Entities;

internal sealed class FlashSaleProductEntity
{
    public Guid FlashSaleId { get; set; }
    public Guid ProductId { get; set; }
    public int PerUserLimit { get; set; }
}
