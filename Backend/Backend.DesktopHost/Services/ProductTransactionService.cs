using Backend.Core.Data;
using static Backend.DesktopHost.Controllers.ProductsController;

namespace Backend.DesktopHost.Services;

public class ProductTransactionService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task ProcessTransactionAsync()
    {

    }
}