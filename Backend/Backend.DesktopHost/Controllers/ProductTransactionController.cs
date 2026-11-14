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
    /// Retrieves product transactions with pagination and date filters.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductTransactionDto>>>
        GetProductTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] long? productId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null
        )
    {
        int clampedPage = Math.Max(1, page);
        int clampedPageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<ProductTransaction> query = _context
            .ProductTransactions
            .AsNoTracking();

        if (productId.HasValue)
        {
            query = query.Where(t => t.Product.Id == productId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(t => t.Timestamp >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.Timestamp <= to.Value);
        }

        List<ProductTransactionDto> transactions = await query
            .Include(t => t.Product)
            .Include(t => t.Batch)
            .OrderByDescending(t => t.Timestamp)
            .Skip((clampedPage - 1) * clampedPageSize)
            .Take(clampedPageSize)
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
