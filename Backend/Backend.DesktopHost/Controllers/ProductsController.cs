using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;


    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] Product createdProduct)
    {
        await _context.Products.AddAsync(createdProduct);

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = createdProduct.Id },
            createdProduct
        );
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        Product? product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        return product;
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(long id, [FromBody] Product updatedProduct)
    {
        try
        {
            // ─── Checks And Assignments ──────────────────────────────────────
            if (id != updatedProduct.Id)
                throw Esc.New<ArgumentException>("Target ID does not match the updated product's ID.");

            Product? product = await _context.Products.FindAsync(id)
                ?? throw Esc.New<ArgumentException>("Product does not exist.", id);

            EntityEntry<Product> entry = _context.Entry(updatedProduct);
            entry.State = EntityState.Modified; // Allow data changes
            entry.Property(p => p.TotalQuantity).IsModified = false; // Ignore all quantity changes
            // TODO: Add documentation about quantity changes.
            // Quantity changes must only be made in ProductStocksController, except
            // during product creation (not batch creation).

            // ─── Apply Changes ───────────────────────────────────────────────
            await _context.SaveChangesAsync();
        }

        catch (ArgumentException e)
        {
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            return Problem(Esc.ExportMessage(e));
        }

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(long id)
    {
        Product? product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        _context.Products.Remove(product);

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return NoContent();
    }
}