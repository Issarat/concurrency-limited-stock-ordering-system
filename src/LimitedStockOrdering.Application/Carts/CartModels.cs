namespace LimitedStockOrdering.Application.Carts;

public sealed class AddCartItemRequest
{
    public Guid VariantId { get; set; }
    public Guid? FlashSaleId { get; set; }
    public int Quantity { get; set; }
}

public sealed class UpdateCartItemQuantityRequest
{
    public int Quantity { get; set; }
}

public sealed class CartResponse
{
    public Guid? CartId { get; set; }
    public Guid UserId { get; set; }
    public bool CanCheckout { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CartItemResponse> Items { get; set; } = [];
}

public sealed class CartItemResponse
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public Guid? FlashSaleId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<CartItemOptionResponse> Options { get; set; } = [];
}

public sealed class CartItemOptionResponse
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public enum CartMutationStatus
{
    Success,
    UserNotFound,
    VariantNotFound,
    FlashSaleUnavailable,
    ItemNotFound
}
