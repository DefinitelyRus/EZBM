using System.Text.Json;
using EZBM.Core.Entities;

namespace EZBM.Core.Tools;

/// <summary>
/// Provides utility functions for ID generation, JSON handling, and file operations.
/// <br/><br/>
/// Example:
/// <code>
/// string json = Utils.ConvertToJson(data);
/// </code>
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editor(s): None<br/>
/// Documented by: Antigravity</i>
/// </summary>
public static class Utils
{

    #region ID Handling

    /// <summary>
    /// Generates a random ID then checks if any existing entity shares the same ID.
    /// If it does, it will generate a new one until a unique ID is found.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// this.Id = Utils.GenerateEntityId();
    /// </code>
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus, Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static ulong GenerateEntityId()
    {
        using Data.AppDbContext context = new();
        while (true)
        {
            byte[] bytes = new byte[8];
            Random.Shared.NextBytes(bytes);
            ulong id = BitConverter.ToUInt64(bytes, 0);
            if (id == 0) continue;

            // Check all DB sets to see if this ID exists
            bool exists = context.Staff.Any(s => s.Id == id) ||
                          context.Attendance.Any(a => a.Id == id) ||
                          context.Payroll.Any(p => p.Id == id) ||
                          context.Item.Any(i => i.Id == id) ||
                          context.ItemTransaction.Any(it => it.Id == id) ||
                          context.Sale.Any(s => s.Id == id) ||
                          context.SaleEntry.Any(se => se.Id == id) ||
                          context.Transaction.Any(t => t.Id == id);

            if (!exists) return id;
        }
    }

    /// <summary>
    /// Generates a sequential invoice number for a transaction based on the given timestamp.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// int num = Utils.GenerateInvoiceNumber(DateTime.UtcNow);
    /// </code>
    /// <br/><br/>
    /// <i>Author(s): DefinitelyRus, Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="timestamp">The timestamp of the transaction.</param>
    public static int GenerateInvoiceNumber(DateTime timestamp)
    {
        using Data.AppDbContext context = new();
        DateTime datePrefix = timestamp.Date;

        int? maxInvoice = context.Transaction
            .Where(t => t.Timestamp.Date == datePrefix)
            .Select(t => (int?)t.InvoiceNumber)
            .Max();

        return maxInvoice.HasValue ? maxInvoice.Value + 1 : 1;
    }

    #endregion

    #region JSON Helpers

    /// <summary>
    /// Safely gets an object as a ulong value.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static ulong? GetAsUlong(object? obj)
    {
        if (obj is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetUInt64(out ulong val))
                return val;
            if (element.ValueKind == JsonValueKind.String && ulong.TryParse(element.GetString(), out ulong parsed))
                return parsed;
        }

        return null;
    }

    /// <summary>
    /// Safely gets an object as a string.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static string? GetAsString(object? obj)
    {
        if (obj is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String) return element.GetString();
            if (element.ValueKind == JsonValueKind.Null) return null;
            return element.GetRawText();
        }

        return obj?.ToString();
    }

    /// <summary>
    /// Safely gets an object as a float value.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static float? GetAsFloat(object? obj)
    {
        if (obj is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetSingle(out float val))
                return val;
            if (element.ValueKind == JsonValueKind.String && float.TryParse(element.GetString(), out float parsed))
                return parsed;
        }

        return null;
    }

    /// <summary>
    /// Safely gets an object as a boolean.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static bool? GetAsBool(object? obj)
    {
        if (obj is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.True) return true;
            if (element.ValueKind == JsonValueKind.False) return false;
            if (element.ValueKind == JsonValueKind.String && bool.TryParse(element.GetString(), out bool parsed))
                return parsed;
        }

        return null;
    }

    /// <summary>
    /// Safely gets an object as a DateTime.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static DateTime? GetAsDateTime(object? obj)
    {
        if (obj is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String && element.TryGetDateTime(out DateTime dt))
                return dt;
        }

        return null;
    }

    /// <summary>
    /// Safely gets a list of Item tags from a JsonElement array.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    public static List<Item.Tag>? GetAsTagsList(object? obj)
    {
        if (obj is JsonElement element && element.ValueKind == JsonValueKind.Array)
        {
            List<Item.Tag> list = [];
            foreach (JsonElement item in element.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    if (Enum.TryParse<Item.Tag>(item.GetString(), true, out Item.Tag tag))
                    {
                        list.Add(tag);
                    }
                }
            }

            return list;
        }

        return null;
    }

    #endregion

    #region JSON Handling

    /// <summary>
    /// Deserializes a JSON string into a dictionary object.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// var data = Utils.ConvertFromJson("{\"key\":\"value\"}");
    /// </code>
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="jsonString">The JSON string to parse.</param>
    public static Dictionary<string, object>? ConvertFromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
    }

    /// <summary>
    /// Serializes a dictionary object into a JSON string.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// string json = Utils.ConvertToJson(myDictionary);
    /// </code>
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="data">The dictionary data to serialize.</param>
    public static string ConvertToJson(Dictionary<string, object>? data)
    {
        return JsonSerializer.Serialize(data);
    }

    #endregion

    #region File Handling

    /// <summary>
    /// Gets the path to the user's documents folder.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// string path = Utils.UserSavePath;
    /// </code>
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public static string UserSavePath { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    /// Writes string content to a specified file asynchronously in the user's documents folder.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// await Utils.WriteFileAsync("data.txt", "Hello World", true);
    /// </code>
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="filename">The filename including the file extension.</param>
    /// <param name="content">The text content to write to the file.</param>
    /// <param name="overwriteExisting">Set to true to overwrite the file if it already exists.</param>
    public static async Task WriteFileAsync(string filename, string? content, bool overwriteExisting = false)
    {
        string filePath = Path.Combine(UserSavePath, filename);

        // Check if the file already exists
        if (!overwriteExisting && File.Exists(filePath))
        {
            Log.Warn($"The file '{filename}' already exists in path '{UserSavePath}'. Re-run the method with `overwriteExisting` set to true to proceed anyway.");
            return;
        }

        try
        {
            await File.WriteAllTextAsync(filePath, content ?? string.Empty);
        }
        catch (Exception e)
        {
            Log.Warn($"Failed to write '{filename}' to path '{UserSavePath}':\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// Reads text content from a specified file asynchronously from the user's documents folder.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// string? content = await Utils.ReadFileAsync("data.txt");
    /// </code>
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="filename">The filename including the file extension.</param>
    public static async Task<string?> ReadFileAsync(string filename)
    {
        string filePath = Path.Combine(UserSavePath, filename);

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Log.Warn($"The file '{filename}' does not exist in path '{UserSavePath}'.");
            return null;
        }

        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch (Exception e)
        {
            Log.Warn($"Failed to read file '{filename}':\n{e.Message}");
            return null;
        }
    }

    #endregion

}
