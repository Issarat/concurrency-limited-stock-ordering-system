namespace LimitedStockOrdering.Infra.Entities;

internal sealed class CartEntity
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
