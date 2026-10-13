namespace LimitedStockOrdering.Application.Orders;

public sealed class OrderPreviewRequest
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
    public string OrderType { get; set; } = "Regular";
}

public sealed class OrderPreviewResponse
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public int AvailableQuantity { get; set; }
    public bool HasSufficientStock { get; set; }
    public bool IsOnFlashSale { get; set; }
    public List<OrderPreviewOptionResponse> Options { get; set; } = [];
}

public sealed class OrderPreviewOptionResponse
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
