using EZBM.Core.Entities;
using EZBM.Core.Services;
using EZBM.Core.Tools;
using Microsoft.AspNetCore.Mvc;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for managing staff payroll records.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public static class PayrollController
{
    /// <summary>
    /// Creates a new payroll record.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing payroll details.</param>
    /// <returns>An HTTP result indicating the status of the payroll creation.</returns>
    public static async Task<IResult> CreatePayroll([FromBody] CreatePayrollRequest request)
    {
        Utils.RequestResult result = await StaffService.CreatePayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Retrieves a specific payroll record by its identifier.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing the payroll ID.</param>
    /// <returns>An HTTP result with the payroll details if found.</returns>
    public static async Task<IResult> GetPayroll([FromBody] GetPayrollRequest request)
    {
        Utils.RequestResult<Payroll> result = await StaffService.GetPayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Finds payroll records matching the specified query filters.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing search filters.</param>
    /// <returns>An HTTP result with the list of matching payroll records.</returns>
    public static async Task<IResult> FindPayroll([FromBody] FindPayrollRequest request)
    {
        Utils.RequestResult<List<Payroll>> result = await StaffService.FindPayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }

    /// <summary>
    /// Deletes a specific payroll record by its identifier.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="request">The request parameters containing the payroll ID to delete.</param>
    /// <returns>An HTTP result indicating the status of the deletion.</returns>
    public static async Task<IResult> DeletePayroll([FromBody] DeletePayrollRequest request)
    {
        Utils.RequestResult result = await StaffService.DeletePayrollAsync(request);
        return EndpointHelpers.ToIResult(result);
    }
}
