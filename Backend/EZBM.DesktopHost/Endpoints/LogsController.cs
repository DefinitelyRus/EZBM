using EZBM.Core.Entities;
using EZBM.Core.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints for accessing system audit and action logs.
/// </summary>
public static class LogsController
{
    /// <summary>
    /// Retrieves all system audit logs ordered by most recent.
    /// </summary>
    /// <returns>An HTTP result containing the list of action logs.</returns>
    public static async Task<IResult> GetActionLogs()
    {
        try
        {
            using AppDbContext context = new();
            List<ActionLog> logs = await context.ActionLog
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();

            return Results.Ok(logs);
        }

        catch (Exception ex)
        {
            return Results.Problem($"Failed to retrieve action logs: {ex.Message}");
        }
    }
}
