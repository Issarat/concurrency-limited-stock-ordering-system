namespace LimitedStockOrdering.Infra.Entities;

internal sealed class CategoryEntity
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
