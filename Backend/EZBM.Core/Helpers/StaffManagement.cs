using EZBM.Core.Tools;
using EZBM.Core.Entities;

namespace EZBM.Core.Helpers;

public static class StaffManagement
{
    public static void AddStaff(string json)
    {
        Log.Me(() => "Attempting to add staff...");

        // Parse JSON
        Dictionary<string, object>? data = Utils.ConvertFromJson(json);

        // Return if invalid
        if (data == null)
        {
            Log.Err(() => "Unable to parse JSON.");
            return;
        }

        // ID
        object idObj = data["id"];
        if (idObj is null)
        {
            Log.Err(() => "'id' is missing from the input JSON string.");
            return;
        }
        int id = (int) idObj;

        // Username
        object usernameObj = data["username"];
        if (usernameObj is null)
        {
            Log.Err(() => "'username' is missing from the input JSON string.");
            return;
        }
        string username = (string) usernameObj;

        // Payment Type
        object payTypeObj = data["payType"];
        if (payTypeObj is null)
        {
            Log.Err(() => "'payType' is missing from the input JSON string.");
            return;
        }
        Staff.PaymentType payType = (string) payTypeObj switch
        {
            "Hourly" => Staff.PaymentType.Hourly,
            "Daily" => Staff.PaymentType.Daily,
            "Biweekly" => Staff.PaymentType.Biweekly,
            "Monthly" => Staff.PaymentType.Monthly,
            _ => Staff.PaymentType.Invalid
        };

        // Pay Rate
        object payRateObj = data["payRate"];
        if (payRateObj is null)
        {
            Log.Err(() => "'payRate' is missing from the input JSON string.");
            return;
        }
        float payRate = (float) payRateObj;

        // Password
        object passwordObj = data["password"];
        string? password = (string) passwordObj;

        // First Name
        object firstNameObj = data["firstName"];
        string? firstName = (string) firstNameObj;

        // Last Name
        object lastNameObj = data["lastName"];
        string? lastName = (string) lastNameObj;

        // Email
        object emailObj = data["email"];
        string? email = (string) emailObj;

        // Phone Number
        object phoneNumberObj = data["phoneNumber"];
        string? phoneNumber = (string) phoneNumberObj;

        // Position
        object positionObj = data["position"];
        string? position = (string) positionObj;

        Staff staff = new(id, username, payType, payRate, password, firstName, lastName, email, phoneNumber, position);
        //DatabaseManager.Write(...) // TODO!!!

        Log.Me(() => "Done!");
    }
}