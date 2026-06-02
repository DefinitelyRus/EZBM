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
/// <i>Documented by: Google Gemini</i>
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
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    {
        // Generate a random long integer

        // Query the database to check if the generated number matches any existing values

        // If there is a match, keep generating another number and checking until there is no match

        // If none, return the generated number

        return 0;
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
