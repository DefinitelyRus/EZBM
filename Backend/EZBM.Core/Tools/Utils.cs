using System.Text.Json;

namespace EZBM.Core.Tools;

public static class Utils
{
    
    // Generates a random ID then checks if any existing entity shares the same ID.
    // If it does, it will generate a new one until a unique ID is found.
    public static long GenerateId(Type type)
    {
        // TODO: Write this function
        return 0;
    }

    public static Dictionary<string, object>? ConvertFromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
    }

    public static string ConvertToJson(Dictionary<string, object>? data)
    {
        return JsonSerializer.Serialize(data);
    }
}