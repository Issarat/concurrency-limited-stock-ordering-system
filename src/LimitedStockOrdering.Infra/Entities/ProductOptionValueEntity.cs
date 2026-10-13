namespace LimitedStockOrdering.Infra.Entities;

internal sealed class ProductOptionValueEntity
{
    public Guid OptionValueId { get; set; }
    public Guid OptionId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? SwatchHex { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
