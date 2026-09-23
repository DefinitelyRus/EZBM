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
        // ─── Add New Product ─────────────────────────────────────────────────
        try
        {
            await _context.Products.AddAsync(createdProduct);
            await _context.SaveChangesAsync();
        }

        catch (Exception e)
        {
            Esc.AddData(e, createdProduct.Id);
            return Problem(Esc.ExportMessage(e));
        }

        // ─── Return Created Product ──────────────────────────────────────────
        CreatedAtActionResult result = CreatedAtAction(
            nameof(GetProduct),
            new { id = createdProduct.Id },
            createdProduct
        );

        return result;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        try
        {
            // ─── Checks And Assignments ──────────────────────────────────────
            Product product = await _context.Products.FindAsync(id)
                ?? throw Esc.New<ArgumentException>("Product does not exist.", id);

            // ─── Return Result ───────────────────────────────────────────────
            return product;
        }

        catch (ArgumentException e)
        {
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            Esc.AddData(e, id);
            return Problem(Esc.ExportMessage(e));
        }
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

            // ─── Apply Changes ───────────────────────────────────────────────
            entry.State = EntityState.Modified; // Allow data changes
            entry.Property(p => p.TotalQuantity).IsModified = false; // Ignore all quantity changes
                                                                     // TODO: Add documentation about quantity changes.
                                                                     // Quantity changes must only be made in ProductStocksController, except
                                                                     // during product creation (not batch creation).
            await _context.SaveChangesAsync();
        }

        catch (ArgumentException e)
        {
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            Esc.AddData(e, id);
            return Problem(Esc.ExportMessage(e));
        }

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(long id)
    {
        try
        {
            // ─── Checks And Assignments ──────────────────────────────────────
            Product product = await _context.Products.FindAsync(id)
                ?? throw Esc.New<ArgumentException>("Product does not exist.", id);

            // ─── Apply Changes ───────────────────────────────────────────────
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        catch (ArgumentException e)
        {
            return BadRequest(Esc.ExportMessage(e));
        }

        catch (Exception e)
        {
            Esc.AddData(e, id);
            return Problem(Esc.ExportMessage(e));
        }

        return NoContent();
    }
}