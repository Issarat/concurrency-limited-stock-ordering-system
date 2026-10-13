namespace LimitedStockOrdering.Infra.Entities;

internal sealed class ProductVariantValueEntity
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public Guid OptionId { get; set; }
    public Guid OptionValueId { get; set; }
}
