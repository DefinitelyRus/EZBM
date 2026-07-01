using System;
using System.Collections.Generic;

namespace EZBM.Core.Entities;

/// <summary>
/// Represents a customer in the system, inheriting access features from User and maintaining their transaction history.
/// </summary>
public class Customer : User
{
    /// <summary>
    /// The ledger of all transactions performed by or associated with this customer.
    /// </summary>
    public virtual ICollection<Transaction> TransactionHistory { get; set; } = new List<Transaction>();


    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected Customer() : base() { }

    /// <summary>
    /// Initializes a new instance of the Customer class.
    /// </summary>
    public Customer(
        ulong id,
        string? firstName = null,
        string? lastName = null,
        string? phoneNumber = null,
        string? email = null)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        AccessType = AccessCardType.Member;
    }
}
