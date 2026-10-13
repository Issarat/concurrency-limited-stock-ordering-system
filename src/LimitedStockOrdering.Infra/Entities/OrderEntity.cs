namespace LimitedStockOrdering.Infra.Entities;

internal sealed class OrderEntity
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid FlashSaleId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
