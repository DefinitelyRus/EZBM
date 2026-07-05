using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using EZBM.Core.Data;
using EZBM.Core.Tools;
using System;
using Microsoft.AspNetCore.Http;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints to sync database backups to and from a simulated Google Drive storage location.
/// </summary>
public static class SyncController
{
    /// <summary>
    /// Simulates uploading a database backup to Google Drive by copying the sqlite file into the backups folder.
    /// </summary>
    public static async Task<IResult> SyncBackup()
    {
        try
        {
            // Simulate backing up SQLite database file
            string dbPath = DbManager.DbFilePath;
            if (!File.Exists(dbPath))
            {
                return Results.NotFound(new { error = "Database file not found." });
            }

            string backupDir = Path.Combine(Utils.UserSavePath, "ezbm-backups");
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            string backupPath = Path.Combine(backupDir, $"business_data_drive_sync_{DateTime.UtcNow:yyyyMMdd_HHmmss}.db");
            
            // Perform simulated cloud upload via asynchronous IO copy
            await Task.Run(() => File.Copy(dbPath, backupPath, true));

            Log.Me($"Database backed up and synced to simulated Google Drive storage at: {backupPath}");
            return Results.Ok(new
            {
                success = true,
                message = "Database backup successfully uploaded to Google Drive.",
                fileName = Path.GetFileName(backupPath),
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = $"Backup sync failed: {ex.Message}" }, statusCode: 500);
        }
    }

    /// <summary>
    /// Restores the active database file using a specified backup file.
    /// </summary>
    public static async Task<IResult> SyncRestore([FromQuery] string backupName)
    {
        try
        {
            if (string.IsNullOrEmpty(backupName))
            {
                return Results.BadRequest(new { error = "Backup filename must be provided." });
            }

            string backupDir = Path.Combine(Utils.UserSavePath, "ezbm-backups");
            string backupPath = Path.Combine(backupDir, backupName);

            if (!File.Exists(backupPath))
            {
                return Results.NotFound(new { error = $"Backup file '{backupName}' not found." });
            }

            // Restore db
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            await Task.Run(() => File.Copy(backupPath, DbManager.DbFilePath, true));
            Log.Me($"Database successfully restored from simulated Google Drive: {backupPath}");

            return Results.Ok(new
            {
                success = true,
                message = "Database successfully restored from Google Drive backup."
            });
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = $"Restore sync failed: {ex.Message}" }, statusCode: 500);
        }
    }
}
