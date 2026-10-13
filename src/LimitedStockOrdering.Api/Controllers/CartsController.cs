using LimitedStockOrdering.Application.Carts;
using LimitedStockOrdering.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LimitedStockOrdering.Api.Controllers;

[ApiController]
[Route("api/carts/{userId:guid}")]
public sealed class CartsController(ICartRepository cartRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CartResponse>> Get(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(CreateProblem("UserId ต้องเป็น UUID ที่ถูกต้อง"));
        }

        var cart = await cartRepository.GetAsync(userId, cancellationToken);
        return Ok(cart ?? new CartResponse { UserId = userId });
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem(
        Guid userId,
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty || request.VariantId == Guid.Empty)
        {
            return BadRequest(CreateProblem("UserId และ VariantId ต้องเป็น UUID ที่ถูกต้อง"));
        }

        if (request.Quantity is < 1 or > 10)
        {
            return BadRequest(CreateProblem("Quantity ต้องอยู่ระหว่าง 1 ถึง 10"));
        }

        var result = await cartRepository.AddItemAsync(
            userId,
            request.VariantId,
            request.FlashSaleId,
            request.Quantity,
            cancellationToken);

        var failure = MapMutationFailure(result);
        if (failure is not null)
        {
            return failure;
        }

        return Ok(await cartRepository.GetAsync(userId, cancellationToken));
    }

    [HttpPatch("items/{variantId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateQuantity(
        Guid userId,
        Guid variantId,
        [FromBody] UpdateCartItemQuantityRequest request,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty || variantId == Guid.Empty)
        {
            return BadRequest(CreateProblem("UserId และ VariantId ต้องเป็น UUID ที่ถูกต้อง"));
        }

        if (request.Quantity is < 1 or > 10)
        {
            return BadRequest(CreateProblem("Quantity ต้องอยู่ระหว่าง 1 ถึง 10"));
        }

        var result = await cartRepository.UpdateQuantityAsync(
            userId,
            variantId,
            request.Quantity,
            cancellationToken);

        var failure = MapMutationFailure(result);
        if (failure is not null)
        {
            return failure;
        }

        return Ok(await cartRepository.GetAsync(userId, cancellationToken));
    }

    [HttpDelete("items/{variantId:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(
        Guid userId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty || variantId == Guid.Empty)
        {
            return BadRequest(CreateProblem("UserId และ VariantId ต้องเป็น UUID ที่ถูกต้อง"));
        }

        var result = await cartRepository.RemoveItemAsync(
            userId,
            variantId,
            cancellationToken);

        var failure = MapMutationFailure(result);
        if (failure is not null)
        {
            return failure;
        }

        return Ok(await cartRepository.GetAsync(userId, cancellationToken));
    }

    private ObjectResult? MapMutationFailure(CartMutationStatus status) => status switch
    {
        CartMutationStatus.Success => null,
        CartMutationStatus.UserNotFound => NotFound(CreateProblem("ไม่พบผู้ใช้ที่พร้อมใช้งาน", 404)),
        CartMutationStatus.VariantNotFound => NotFound(CreateProblem("ไม่พบ Variant ที่พร้อมขาย", 404)),
        CartMutationStatus.FlashSaleUnavailable => Conflict(
            CreateProblem("Flash Sale สิ้นสุดแล้วหรือ Variant ไม่ได้อยู่ในรอบขาย", 409)),
        CartMutationStatus.ItemNotFound => NotFound(CreateProblem("ไม่พบสินค้าในตะกร้า", 404)),
        _ => StatusCode(
            StatusCodes.Status500InternalServerError,
            CreateProblem("ไม่สามารถแก้ไขตะกร้าได้", 500))
    };

    private static ProblemDetails CreateProblem(string detail, int status = 400) => new()
    {
        Status = status,
        Title = "ไม่สามารถดำเนินการกับตะกร้าได้",
        Detail = detail
    };
}
