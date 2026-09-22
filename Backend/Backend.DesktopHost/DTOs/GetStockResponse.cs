using Backend.Core.Models;

namespace Backend.DesktopHost.DTOs;

public class GetStockResponse
{
	public long ProductId { get; set; }
	public decimal TotalQuantity { get; set; }
	public List<ProductBatch>? Batches { get; set; }
}