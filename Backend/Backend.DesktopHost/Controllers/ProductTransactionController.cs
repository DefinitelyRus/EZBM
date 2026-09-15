using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTransactionsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    #region Unused

    [HttpPost]
    public async Task<ActionResult> CreateProductTransaction(ProductTransaction createdTransaction)
    {
        await _context.ProductTransactions.AddAsync(createdTransaction);

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return CreatedAtAction(
            nameof(CreateProductTransaction),
            new { id = createdTransaction.Id },
            createdTransaction
        );
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductTransaction>> GetProductTransaction(long id)
    {
        // TODO: Implement GetProductTransaction.
        return Problem(statusCode: 501);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductTransaction>>> GetProductTransactions()
    {
        // TODO: Implement GetProductTransactions.
        return Problem(statusCode: 501);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateProductTransaction(
        long id,
        ProductTransaction updatedTransaction
    )
    {
        // TODO: Implement UpdateProductTransaction.
        return Problem(statusCode: 501);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductTransaction(long id)
    {
        // TODO: Implement DeleteProductTransaction.
        return Problem(statusCode: 501);
    }

    #endregion

}