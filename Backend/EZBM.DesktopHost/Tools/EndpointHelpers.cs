using static EZBM.Core.Tools.Utils;

namespace EZBM.DesktopHost.Tools;

/// <summary>
/// Helper methods for endpoint mapping and conversion.
/// <br/><br/>
/// <i>Documented by: Google Antigravity</i>
/// </summary>
public static class EndpointHelpers
{
    /// <summary>
    /// Converts a RequestResult to an IResult.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// IResult result = EndpointHelpers.ToIResult(myRequestResult);
    /// </code>
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="result">The RequestResult to convert.</param>
    public static IResult ToIResult<TData>(RequestResult<TData> result)
    {
        return result.Type switch
        {
            Result.Success => Results.Ok(result.Data),
            Result.Success_NoResults => Results.NotFound(),
            Result.Failed_NoResults => Results.NotFound(result.Message),
            Result.Failed_Unauthorized => Results.Unauthorized(),
            Result.Failed_UnhandledException => Results.Problem(result.Message),
            _ => Results.Problem(result.Message)
        };
    }

    /// <summary>
    /// Converts a basic RequestResult to an IResult.
    /// <br/><br/>
    /// <i>Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="result">The RequestResult to convert.</param>
    /// <returns>An IResult representing the outcome.</returns>
    public static IResult ToIResult(RequestResult result)
    {
        return result.Type switch
        {
            Result.Success => Results.Ok(),
            Result.Success_NoResults => Results.NotFound(),
            Result.Failed_NoResults => Results.NotFound(result.Message),
            Result.Failed_Unauthorized => Results.Unauthorized(),
            Result.Failed_UnhandledException => Results.Problem(result.Message),
            _ => Results.Problem(result.Message)
        };
    }
}