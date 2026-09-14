using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Core.Data;
using Backend.Core.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;


    [HttpPost]
    public async Task<ActionResult> CreateProduct(Product createdProduct)
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
    public async Task<ActionResult> UpdateProduct(long id, Product updatedProduct)
    {
        if (id != updatedProduct.Id) return BadRequest("The target ID does not match the updated product's ID.");

        // TODO: Prevent direct quantity updates here.
        // Client should CREATE a new ProductTransaction instead.

        EntityEntry<Product> entry = _context.Entry(updatedProduct);
        entry.State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (DbUpdateConcurrencyException duce)
        {
            if (!_context.Products.Any(e => e.Id == id))
            {
                return NotFound();
            }

            else
            {
                return Problem(duce.Message);
            }
        }

        catch (Exception e)
        {
            return Problem(e.Message);
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