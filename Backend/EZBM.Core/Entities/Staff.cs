namespace EZBM.Core.Entities;

/// <summary>
/// Represents a staff member in the system, including their personal information, employment details, and current status.
/// </summary>
public class Staff : User
{
    #region Enums

    /// <summary>
    /// Defines the frequency or method of payment.
    /// </summary>
    public enum Frequency { Hourly, Daily, Weekly, Biweekly, Monthly, Invalid }

    #endregion

    #region Properties

    /// <summary>
    /// The unique username used for authentication.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password for the account. Note: Stored as hash/encrypted.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// The job title or role of the staff member.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// The frequency at which the staff member is paid.
    /// </summary>
    public Frequency PayFrequency { get; set; }

    /// <summary>
    /// The monetary value paid based on the payment type.
    /// </summary>
    public float PayRate { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected Staff() : base() { }
#pragma warning restore CS8618

    /// <summary>
    /// Initializes a new instance of the Staff class.
    /// </summary>
    /// <param name="id">The unique identifier for this entity.</param>
    /// <param name="username">The staff's username.</param>
    /// <param name="payFrequency">The frequency of payment.</param>
    /// <param name="payRate">The rate of pay.</param>
    /// <param name="password">The account password.</param>
    /// <param name="firstName">The staff's first name.</param>
    /// <param name="lastName">The staff's last name.</param>
    /// <param name="email">The staff's email address.</param>
    /// <param name="phoneNumber">The staff's phone number.</param>
    /// <param name="position">The staff's job position.</param>
    public Staff(
        ulong id,
        string username,
        Frequency payFrequency,
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

        PayFrequency = payFrequency;
        PayRate = payRate;
        AccessType = AccessCardType.Staff;
    }

    #endregion
}
