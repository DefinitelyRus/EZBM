using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.DesktopHost.Controllers;

/// <summary>
/// API controller for reading product transactions and audit records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductTransactionsController(
    AppDbContext context
) : ControllerBase
{
    private readonly AppDbContext _context = context;

    #region Query Endpoints

    /// <summary>
    /// Retrieves all recorded product transactions.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductTransactionDto>>>
        GetProductTransactions()
    {
        List<ProductTransactionDto> transactions = await _context
            .ProductTransactions
            .Include(t => t.Product)
            .Include(t => t.Batch)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => new ProductTransactionDto(
                t.Id,
                t.Product.Id,
                t.Product.Name,
                t.Batch != null ? t.Batch.Id : null,
                t.Quantity,
                t.TransactionType.ToString(),
                t.Timestamp
            ))
            .ToListAsync();

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves a specific product transaction by its unique identifier.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductTransactionDto>>
        GetProductTransaction(long id)
    {
        ProductTransaction? transaction = await _context.ProductTransactions
            .Include(t => t.Product)
            .Include(t => t.Batch)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction is null)
        {
            return NotFound($"Product transaction with ID {id} was not found.");
        }

        ProductTransactionDto dto = new(
            transaction.Id,
            transaction.Product.Id,
            transaction.Product.Name,
            transaction.Batch != null ? transaction.Batch.Id : null,
            transaction.Quantity,
            transaction.TransactionType.ToString(),
            transaction.Timestamp
        );

        return Ok(dto);
    }

    #endregion

    #region Deprecated Stubs

    [HttpPost]
    [Obsolete("Please use ProductStocksController.CreateStock instead.")]
    public async Task<ActionResult> CreateProductTransaction(
        ProductTransaction createdTransaction
    )
    {
        return StatusCode(statusCode: StatusCodes.Status410Gone);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductTransaction(
        long id,
        ProductTransaction updatedTransaction
    )
    {
        return StatusCode(statusCode: StatusCodes.Status200OK);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductTransaction(long id)
    {
        return StatusCode(statusCode: StatusCodes.Status200OK);
    }

    #endregion
}
