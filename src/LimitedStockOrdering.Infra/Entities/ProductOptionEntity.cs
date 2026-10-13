namespace LimitedStockOrdering.Infra.Entities;

internal sealed class ProductOptionEntity
{
    public Guid OptionId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
}
