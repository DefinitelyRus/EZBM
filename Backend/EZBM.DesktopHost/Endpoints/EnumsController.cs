using EZBM.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace EZBM.DesktopHost.Endpoints;

/// <summary>
/// Exposes endpoints to retrieve lists of static enums and tags used in the system.
/// </summary>
public static class EnumsController
{
    /// <summary>
    /// Retrieves all enums and static lists in a single object.
    /// </summary>
    public static IResult GetEnums()
    {
        return Results.Ok(new
        {
            Units = Enum.GetNames<Item.Unit>(),
            Tags = Item.DefaultTags,
            AccessCardTypes = Enum.GetNames<AccessCardType>(),
            PayFrequencies = Enum.GetNames<Staff.Frequency>(),
            StockTransactionTypes = Enum.GetNames<ItemTransaction.Type>(),
            TransactionTypes = Enum.GetNames<Transaction.Type>(),
            PaymentMethods = Enum.GetNames<Transaction.PayMethod>()
        });
    }

    /// <summary>
    /// Retrieves the list of available units of measurement.
    /// </summary>
    public static IResult GetUnits()
    {
        return Results.Ok(Enum.GetNames<Item.Unit>());
    }

    /// <summary>
    /// Retrieves the list of default tags.
    /// </summary>
    public static IResult GetTags()
    {
        return Results.Ok(Item.DefaultTags);
    }

    /// <summary>
    /// Retrieves the list of access card types.
    /// </summary>
    public static IResult GetAccessCardTypes()
    {
        return Results.Ok(Enum.GetNames<AccessCardType>());
    }

    /// <summary>
    /// Retrieves the list of pay frequencies.
    /// </summary>
    public static IResult GetPayFrequencies()
    {
        return Results.Ok(Enum.GetNames<Staff.Frequency>());
    }

    /// <summary>
    /// Retrieves the list of stock transaction types.
    /// </summary>
    public static IResult GetStockTransactionTypes()
    {
        return Results.Ok(Enum.GetNames<ItemTransaction.Type>());
    }

    /// <summary>
    /// Retrieves the list of financial transaction types.
    /// </summary>
    public static IResult GetTransactionTypes()
    {
        return Results.Ok(Enum.GetNames<Transaction.Type>());
    }

    /// <summary>
    /// Retrieves the list of payment methods.
    /// </summary>
    public static IResult GetPaymentMethods()
    {
        return Results.Ok(Enum.GetNames<Transaction.PayMethod>());
    }
}
