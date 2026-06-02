namespace EZBM.Core.Entities;

/// <summary>
/// Represents a staff member in the system, including their personal information, employment details, and current status.
/// <br/><br/>
/// <i>Author(s): DefinitelyRus<br/>
/// Editors(s): None<br/>
/// Documented by: Google Gemini</i>
/// </summary>
public class Staff : Entity
{
    /// <summary>
    /// Defines the frequency or method of payment.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public enum PaymentType { Hourly, Daily, Weekly, Biweekly, Monthly, Invalid }

    /// <summary>
    /// The unique username used for authentication.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password for the account. Note: Currently stored in plain text.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Password { get; set; } //TEMP: Uses plain text password, should be hashed in production

    /// <summary>
    /// The first name of the staff member.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The last name of the staff member.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// The contact email address.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The contact phone number.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The job title or role of the staff member.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// The frequency at which the staff member is paid.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public PaymentType PayType { get; set; }

    /// <summary>
    /// The monetary value paid based on the payment type.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    public float PayRate { get; set; }

    /// <summary>
    /// Initializes a new instance of the Staff class.
    /// <br/><br/>
    /// <i>Documented by: Google Gemini</i>
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="username">The staff's username.</param>
    /// <param name="payType">The frequency of payment.</param>
    /// <param name="payRate">The rate of pay.</param>
    /// <param name="password">The account password.</param>
    /// <param name="firstName">The staff's first name.</param>
    /// <param name="lastName">The staff's last name.</param>
    /// <param name="email">The staff's email address.</param>
    /// <param name="phoneNumber">The staff's phone number.</param>
    /// <param name="position">The staff's job position.</param>
    public Staff(
        int id,
        string username,
        PaymentType payType,
        float payRate,
        string? password = null,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? phoneNumber = null,
        string? position = null)
    {
        Id = id;

        Username = username ?? $"user_{Id}";
        Password = password;

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;

        Position = position;

        PayType = payType;
        PayRate = payRate;
    }
}
