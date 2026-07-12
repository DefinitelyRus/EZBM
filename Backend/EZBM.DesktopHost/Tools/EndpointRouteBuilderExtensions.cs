using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;

namespace EZBM.DesktopHost.Tools;

/// <summary>
/// Contains extension methods for IEndpointRouteBuilder to map generic CRUD routes.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps generic CRUD endpoints under a given prefix pattern.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="prefix">The URL prefix pattern (e.g. "/api/staff").</param>
    /// <param name="createHandler">The delegate for create operation.</param>
    /// <param name="getHandler">The delegate for get operation.</param>
    /// <param name="findHandler">The delegate for find operation.</param>
    /// <param name="updateHandler">The delegate for update operation (optional).</param>
    /// <param name="deleteHandler">The delegate for delete operation.</param>
    public static void MapCrud(
        this IEndpointRouteBuilder endpoints,
        string prefix,
        Delegate createHandler,
        Delegate getHandler,
        Delegate findHandler,
        Delegate? updateHandler,
        Delegate deleteHandler
    )
    {
        endpoints.MapPost($"{prefix}/create", createHandler);
        endpoints.MapPost($"{prefix}/get", getHandler);
        endpoints.MapPost($"{prefix}/find", findHandler);
        if (updateHandler is not null)
        {
            endpoints.MapPost($"{prefix}/update", updateHandler);
        }
        endpoints.MapPost($"{prefix}/delete", deleteHandler);
    }
}
