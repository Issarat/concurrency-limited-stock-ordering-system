namespace LimitedStockOrdering.Infra.Entities;

internal sealed class FlashSaleVariantEntity
{
    public Guid FlashSaleId { get; set; }
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public decimal SalePrice { get; set; }
}
