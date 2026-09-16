using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Backend.DesktopHost.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductStocksController(ProductTransactionService service) : ControllerBase
{
    private readonly ProductTransactionService _service = service;

    [HttpPost("create-stock")]
    public async Task<ActionResult> CreateStock([FromBody] CreateStockRequest request)
    {
        try
        {
            ProductTransaction? transaction = await _service.CreateStockAsync(request);
            return CreatedAtAction(
                nameof(ProductTransactionsController.GetProductTransaction),
                nameof(ProductTransactionsController).Replace("Controller", ""),
                new { id = transaction.Id },
                transaction
            );
        }

        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }

        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("update-stock")]
    public async Task<ActionResult> UpdateStock([FromBody] UpdateStockRequest request)
    {
        try
        {
            List<ProductTransaction> transactions = [.. await _service.UpdateStockAsync(request)];
            return StatusCode(StatusCodes.Status201Created, transactions);
        }

        catch (ArgumentException e)
        {
            return BadRequest($"{e.Message} ProductId={request.ProductId}");
        }
        
        catch (Exception e)
        {
            return Problem($"{e.Message} ProductId={request.ProductId}");
        }
    }

    [HttpGet("get-stock")]
    public async Task<ActionResult<GetStockRequest>> GetStock(long productId)
    {
        // TODO: Implement GetStock
        return Problem(statusCode: 501);
    }
}