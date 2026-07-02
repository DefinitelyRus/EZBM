using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using EZBM.DesktopHost.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing sales and sale entries.
/// </summary>
public static class SalesController
{
    #region Sale Endpoints

    /// <summary>
    /// Creates a new sale record.
    /// </summary>
    /// <param name="request">The request parameters containing sale details.</param>
    /// <returns>An HTTP result indicating the status of the sale creation.</returns>
    public static async Task<IResult> CreateSale(
        [FromBody] CreateSaleRequest request)
    {
        Utils.RequestResult<ulong> result = await SalesService.CreateSaleAsync(request);

        if (result.Type == Utils.Result.Success)
        {
            return Results.Ok(new
            {
                success = true,
                saleId = result.Data
            });
        }

        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific sale record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the sale ID.</param>
    /// <returns>An HTTP result with the sale details if found.</returns>
    public static async Task<IResult> GetSale(
        [FromBody] GetSaleRequest request)
    {
        Utils.RequestResult<Sale> result = await SalesService.GetSaleAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds sale records matching the specified query filters.
    /// </summary>
    /// <param name="request">The request parameters containing search filters.</param>
    /// <returns>An HTTP result with the list of matching sales.</returns>
    public static async Task<IResult> FindSales(
        [FromBody] FindSaleRequest request)
    {
        Utils.RequestResult<List<Sale>> result = await SalesService.FindSalesAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific sale record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the sale ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteSale(
        [FromBody] DeleteSaleRequest request)
    {
        Utils.RequestResult result = await SalesService.DeleteSaleAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion

    #region Sale Entry Endpoints

    /// <summary>
    /// Creates a new sale entry record.
    /// </summary>
    /// <param name="request">The request parameters containing sale entry details.</param>
    /// <returns>An HTTP result indicating the status of the sale entry creation.</returns>
    public static async Task<IResult> CreateSaleEntry(
        [FromBody] CreateSaleEntryRequest request)
    {
        Utils.RequestResult result = await SalesService.CreateSaleEntryAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific sale entry record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the sale entry ID.</param>
    /// <returns>An HTTP result with the sale entry details if found.</returns>
    public static async Task<IResult> GetSaleEntry(
        [FromBody] GetSaleEntryRequest request)
    {
        Utils.RequestResult<SaleEntry> result = await SalesService.GetSaleEntryAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds sale entry records matching the specified query filters.
    /// </summary>
    /// <param name="request">The request parameters containing search filters.</param>
    /// <returns>An HTTP result with the list of matching sale entries.</returns>
    public static async Task<IResult> FindSaleEntries(
        [FromBody] FindSaleEntryRequest request)
    {
        Utils.RequestResult<List<SaleEntry>> result = await SalesService.FindSaleEntriesAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific sale entry record by its identifier.
    /// </summary>
    /// <param name="request">The request parameters containing the sale entry ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeleteSaleEntry(
        [FromBody] DeleteSaleEntryRequest request)
    {
        Utils.RequestResult result = await SalesService.DeleteSaleEntryAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    #endregion

}
