using EZBM.Core.Tools;

namespace EZBM.Core.Services;

/// <summary>
/// A simulated mock cash register drawer service that outputs logs to the diagnostic trace console.
/// </summary>
public class MockCashRegisterService : ICashRegisterService
{
    /// <summary>
    /// Commands the cash drawer to physically kick or open, logging the event to diagnostic output.
    /// </summary>
    public void OpenDrawer()
    {
        Log.Me("Cash register drawer kicked/opened (Console Mock).");
    }
}
