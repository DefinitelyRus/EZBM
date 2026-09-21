using Backend.Core.Common;
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
            e.AddData(request.ProductId);
            return BadRequest($"{Esc.ExportMessage(e)}");
        }

        catch (Exception e)
        {
            e.AddData(request.ProductId);
            return Problem($"{Esc.ExportMessage(e)}");
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
            e.AddData(request.ProductId);
            return BadRequest($"{Esc.ExportMessage(e)}");
        }

        catch (Exception e)
        {
            e.AddData(request.ProductId);
            return Problem($"{Esc.ExportMessage(e)}");
        }
    }

    [HttpGet("get-stock")]
    public async Task<ActionResult<GetStockRequest>> GetStock(long productId)
    {
        // TODO: Implement GetStock
        return Problem(statusCode: 501);
    }
}