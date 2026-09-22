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
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            e.AddData(request.ProductId);
            return Problem(Esc.ExportMessage(e));
        }
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetStockResponse>>> GetStocks()
    {
        // TODO (Later): Implement GetStocks 
        return StatusCode(StatusCodes.Status501NotImplemented);
    }


    [HttpGet("{productId}")]
    public async Task<ActionResult<GetStockResponse>> GetStock(long productId)
    {
        // TODO: Implement GetStock
        try
        {
            GetStockResponse response = await _service.GetStockAsync(productId);
            return response;
        }

        catch (ArgumentException e)
        {
            e.AddData(productId);
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            e.AddData(productId);
            return Problem(Esc.ExportMessage(e));
        }
    }


    [HttpPost("{productId}")]
    public async Task<ActionResult> UpdateStock(long productId, [FromBody] UpdateStockRequest request)
    {
        if (productId != request.ProductId)
            return BadRequest($"The target product ID must match the product ID in the request. || productId={productId}");

        try
        {
            List<ProductTransaction> transactions = [.. await _service.UpdateStockAsync(request)];
            return StatusCode(StatusCodes.Status201Created, transactions);
        }

        catch (ArgumentException e)
        {
            e.AddData(productId);
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            e.AddData(productId);
            return Problem(Esc.ExportMessage(e));
        }
    }
}