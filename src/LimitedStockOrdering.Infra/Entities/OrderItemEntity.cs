namespace LimitedStockOrdering.Infra.Entities;

internal sealed class OrderItemEntity
{
    public Guid OrderId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string SkuSnapshot { get; set; } = string.Empty;
    public string ProductNameSnapshot { get; set; } = string.Empty;
    public string OptionSummarySnapshot { get; set; } = string.Empty;
}
