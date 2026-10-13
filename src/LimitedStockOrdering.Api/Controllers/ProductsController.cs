using LimitedStockOrdering.Application.Products;
using LimitedStockOrdering.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LimitedStockOrdering.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductRepository productRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductListResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await productRepository.GetActiveAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<ProductDetailResponse>> GetById(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }
}
