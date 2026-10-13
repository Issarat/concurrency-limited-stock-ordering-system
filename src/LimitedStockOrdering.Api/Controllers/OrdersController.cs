using LimitedStockOrdering.Application.Orders;
using LimitedStockOrdering.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LimitedStockOrdering.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderRepository orderRepository) : ControllerBase
{
    [HttpPost("preview")]
    public async Task<ActionResult<OrderPreviewResponse>> Preview(
        [FromBody] OrderPreviewRequest request,
        CancellationToken cancellationToken)
    {
        if (request.ProductId == Guid.Empty || request.VariantId == Guid.Empty)
        {
            return BadRequest(CreateProblem("ProductId และ VariantId ต้องเป็น UUID ที่ถูกต้อง"));
        }

        if (request.Quantity is < 1 or > 10)
        {
            return BadRequest(CreateProblem("Quantity ต้องอยู่ระหว่าง 1 ถึง 10"));
        }

        var normalizedOrderType = request.OrderType?.Trim() ?? string.Empty;
        if (normalizedOrderType is not ("Regular" or "FlashSale"))
        {
            return BadRequest(CreateProblem("OrderType ต้องเป็น Regular หรือ FlashSale"));
        }

        var preview = await orderRepository.GetPreviewAsync(
            request.ProductId,
            request.VariantId,
            request.Quantity,
            normalizedOrderType,
            cancellationToken);

        return preview is null ? NotFound() : Ok(preview);
    }

    private static ProblemDetails CreateProblem(string detail) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Title = "ข้อมูล Order Preview ไม่ถูกต้อง",
        Detail = detail
    };
}
