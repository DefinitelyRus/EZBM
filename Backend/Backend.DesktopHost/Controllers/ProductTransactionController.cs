using Backend.Core.Data;
using Backend.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTransactionsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost]
    [Obsolete("Please use ProductStocksController.CreateStock instead.")]
    public async Task<ActionResult> CreateProductTransaction(ProductTransaction createdTransaction)
    {
        return StatusCode(statusCode: StatusCodes.Status410Gone);
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


    // The use of this endpoint is allowed but heavily discouraged.
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductTransaction(
        long id,
        ProductTransaction updatedTransaction
    )
    {
        // TODO: Implement UpdateProductTransaction.
        return StatusCode(statusCode: StatusCodes.Status200OK);
    }


    // The use of this endpoint is allowed but heavily discouraged.
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductTransaction(long id)
    {
        // TODO: Implement DeleteProductTransaction.
        return StatusCode(statusCode: StatusCodes.Status200OK);
    }

}