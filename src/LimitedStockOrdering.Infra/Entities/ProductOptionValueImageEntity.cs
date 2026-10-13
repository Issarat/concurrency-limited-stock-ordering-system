namespace LimitedStockOrdering.Infra.Entities;

internal sealed class ProductOptionValueImageEntity
{
    public Guid ImageId { get; set; }
    public Guid OptionValueId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
