namespace LimitedStockOrdering.Application.Products;

public abstract class ProductBaseResponse
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public sealed class ProductListResponse : ProductBaseResponse
{
    public decimal StartingPrice { get; set; }
    public decimal? FlashSaleStartingPrice { get; set; }
    public bool IsOnFlashSale { get; set; }
    public int AvailableQuantity { get; set; }
    public int SoldQuantity { get; set; }
}

public sealed class ProductDetailResponse : ProductBaseResponse
{
    public decimal StartingPrice { get; set; }
    public decimal? FlashSaleStartingPrice { get; set; }
    public bool IsOnFlashSale { get; set; }
    public int AvailableQuantity { get; set; }
    public int SoldQuantity { get; set; }
    public List<ProductOptionResponse> Options { get; set; } = [];
    public List<ProductVariantResponse> Variants { get; set; } = [];
}

public sealed class ProductOptionResponse
{
    public Guid OptionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public List<ProductOptionValueResponse> Values { get; set; } = [];
}

public sealed class ProductOptionValueResponse
{
    public Guid OptionValueId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? SwatchHex { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class ProductVariantResponse
{
    public Guid VariantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int AvailableQuantity { get; set; }
    public List<Guid> OptionValueIds { get; set; } = [];
}
