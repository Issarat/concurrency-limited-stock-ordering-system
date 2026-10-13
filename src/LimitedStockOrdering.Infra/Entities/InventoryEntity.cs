namespace LimitedStockOrdering.Infra.Entities;

internal sealed class InventoryEntity
{
    public Guid VariantId { get; set; }
    public int AvailableQuantity { get; set; }
    public long RowVersion { get; set; }
    public DateTime UpdatedAt { get; set; }
}
