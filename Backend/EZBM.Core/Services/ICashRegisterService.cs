namespace EZBM.Core.Services;

/// <summary>
/// Defines interface operations for hardware cash register drawer kick actions.
/// </summary>
public interface ICashRegisterService
{
    /// <summary>
    /// Commands the cash drawer to physically kick or open.
    /// </summary>
    void OpenDrawer();
}
