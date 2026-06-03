using EZBM.Core.Entities;
using EZBM.Core.Tools;
using EZBM.Core.Data;
using Microsoft.EntityFrameworkCore;

/*
 * For the record, I am quite annoyed at how AI tools keep generating
 * code that I did not ask it to generate.
 *
 * They're good and they're already there, so I won't remove it now,
 * but I'm writing the code manually for a reason, damn it.
 *
 * Now I have to go read through all this code to make sure
 * it actually does what I need it to do.
 * I'm dyslexic ffs!
 *
 * - DefinitelyRus
 */

namespace EZBM.Core.Services;

/// <summary>
/// Service class for managing Sale records, SaleEntries, and general financial Transactions.
/// <br/><br/>
/// <i>Author(s): Google Antigravity<br/>
/// Editor(s): None<br/>
/// Documented by: Google Antigravity</i>
/// </summary>
internal static class SalesService
{
    #region Factory Methods

    /// <summary>
    /// Creates a Sale instance from a JSON string, resolving the Staff relationship.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing sale details.</param>
    /// <param name="context">The database context used to lookup the Staff member.</param>
    internal static Sale? CreateSaleInstance(string json, AppDbContext context)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as a Sale object.");
            return null;
        }

        ulong id = 0;
        int invoiceNumber = 0;
        float amount = 0;
        Transaction.PayMethod paymentMethod = Transaction.PayMethod.Cash;
        Staff? staff = null;
        DateTime timestamp = DateTime.UtcNow;
        string? notes = null;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "id":
                    id = Utils.GetAsUlong(kvp.Value) ?? 0;
                    break;

                case "invoicenumber":
                    invoiceNumber = (int)(Utils.GetAsUlong(kvp.Value) ?? 0);
                    break;

                case "amount" or "totalamount":
                    amount = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "paymentmethod" or "paymethod":
                    string? payStr = Utils.GetAsString(kvp.Value);
                    if (payStr != null && Enum.TryParse<Transaction.PayMethod>(payStr, true, out Transaction.PayMethod parsedPay))
                    {
                        paymentMethod = parsedPay;
                    }
                    break;

                case "staffid" or "userid":
                    ulong? staffId = Utils.GetAsUlong(kvp.Value);
                    if (staffId.HasValue)
                    {
                        staff = context.Staff.Find(staffId.Value);
                    }
                    break;

                case "timestamp":
                    timestamp = Utils.GetAsDateTime(kvp.Value) ?? DateTime.UtcNow;
                    break;

                case "notes":
                    notes = Utils.GetAsString(kvp.Value);
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in Sale JSON. Skipping...");
                    break;
            }
        }

        if (staff is null)
        {
            Log.Err(() => "Staff reference is required to create a Sale instance.");
            return null;
        }

        if (invoiceNumber == 0)
        {
            invoiceNumber = Utils.GenerateInvoiceNumber(timestamp);
        }

        return new Sale(
            id,
            invoiceNumber,
            amount,
            paymentMethod,
            staff,
            timestamp,
            notes
        );
    }

    /// <summary>
    /// Creates a SaleEntry instance from a JSON string, resolving the Sale and Item relationships.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing sale entry details.</param>
    /// <param name="context">The database context used to lookup relations.</param>
    internal static SaleEntry? CreateSaleEntryInstance(string json, AppDbContext context)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as a SaleEntry object.");
            return null;
        }

        Sale? sale = null;
        Item? item = null;
        float quantity = 0;
        float unitPrice = 0;
        float subtotal = 0;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "saleid":
                    ulong? saleId = Utils.GetAsUlong(kvp.Value);
                    if (saleId.HasValue)
                    {
                        sale = context.Sale.Find(saleId.Value);
                    }
                    break;

                case "itemid":
                    ulong? itemId = Utils.GetAsUlong(kvp.Value);
                    if (itemId.HasValue)
                    {
                        item = context.Item.Find(itemId.Value);
                    }
                    break;

                case "quantity":
                    quantity = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "unitprice" or "price":
                    unitPrice = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "subtotal":
                    subtotal = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in SaleEntry JSON. Skipping...");
                    break;
            }
        }

        if (sale is null)
        {
            Log.Err(() => "Sale reference is required to create a SaleEntry instance.");
            return null;
        }

        if (item is null)
        {
            Log.Err(() => "Item reference is required to create a SaleEntry instance.");
            return null;
        }

        if (subtotal == 0)
        {
            subtotal = quantity * unitPrice;
        }

        return new SaleEntry(
            sale,
            item,
            quantity,
            unitPrice,
            subtotal
        );
    }

    /// <summary>
    /// Creates a generic Transaction instance from a JSON string, resolving the Staff relationship.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing transaction details.</param>
    /// <param name="context">The database context used to lookup the Staff member.</param>
    internal static Transaction? CreateTransactionInstance(string json, AppDbContext context)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as a Transaction object.");
            return null;
        }

        ulong id = 0;
        Transaction.Type transactionType = Transaction.Type.Income;
        float amount = 0;
        DateTime timestamp = DateTime.UtcNow;
        Staff? staff = null;
        Transaction.PayMethod? paymentMethod = null;
        int? invoiceNumber = null;
        string? invoicePrefix = "GENERIC";
        string? notes = null;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "id":
                    id = Utils.GetAsUlong(kvp.Value) ?? 0;
                    break;

                case "transactiontype" or "type":
                    string? typeStr = Utils.GetAsString(kvp.Value);
                    if (typeStr != null && Enum.TryParse<Transaction.Type>(typeStr, true, out Transaction.Type parsedType))
                    {
                        transactionType = parsedType;
                    }
                    break;

                case "amount":
                    amount = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "timestamp":
                    timestamp = Utils.GetAsDateTime(kvp.Value) ?? DateTime.UtcNow;
                    break;

                case "staffid":
                    ulong? staffId = Utils.GetAsUlong(kvp.Value);
                    if (staffId.HasValue)
                    {
                        staff = context.Staff.Find(staffId.Value);
                    }
                    break;

                case "paymentmethod" or "paymethod":
                    string? payStr = Utils.GetAsString(kvp.Value);
                    if (payStr != null && Enum.TryParse<Transaction.PayMethod>(payStr, true, out Transaction.PayMethod parsedPay))
                    {
                        paymentMethod = parsedPay;
                    }
                    break;

                case "invoicenumber":
                    invoiceNumber = (int)(Utils.GetAsUlong(kvp.Value) ?? 0);
                    break;

                case "invoiceprefix":
                    invoicePrefix = Utils.GetAsString(kvp.Value) ?? "GENERIC";
                    break;

                case "notes":
                    notes = Utils.GetAsString(kvp.Value);
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in Transaction JSON. Skipping...");
                    break;
            }
        }

        if (staff is null)
        {
            Log.Err(() => "Staff reference is required to create a Transaction instance.");
            return null;
        }

        return new Transaction(
            id,
            transactionType,
            amount,
            timestamp,
            staff,
            paymentMethod,
            invoiceNumber,
            invoicePrefix,
            notes
        );
    }

    #endregion

    #region CRUD Sale Operations

    /// <summary>
    /// Retrieves all sales from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Sale> GetAllSales()
    {
        using AppDbContext context = new();
        return [.. context.Sale.Include(s => s.Staff)];
    }

    /// <summary>
    /// Retrieves a specific sale by its unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static Sale? GetSaleById(ulong id)
    {
        using AppDbContext context = new();
        return context.Sale
            .Include(s => s.Staff)
            .FirstOrDefault(s => s.Id == id);
    }

    #endregion

    #region CRUD SaleEntry Operations

    /// <summary>
    /// Retrieves all sale entries from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<SaleEntry> GetAllSaleEntries()
    {
        using AppDbContext context = new();
        return [.. context.SaleEntry
            .Include(se => se.Sale)
            .Include(se => se.Item)];
    }

    /// <summary>
    /// Retrieves all sale entries associated with a specific sale.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<SaleEntry> GetSaleEntriesForSale(ulong saleId)
    {
        using AppDbContext context = new();
        return [.. context.SaleEntry
            .Include(se => se.Sale)
            .Include(se => se.Item)
            .Where(se => se.Sale.Id == saleId)];
    }

    #endregion

    #region CRUD Transaction Operations

    /// <summary>
    /// Retrieves all financial transactions from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Transaction> GetAllTransactions()
    {
        using AppDbContext context = new();
        return [.. context.Transaction.Include(t => t.Staff)];
    }

    /// <summary>
    /// Retrieves a specific transaction by its unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static Transaction? GetTransactionById(ulong id)
    {
        using AppDbContext context = new();
        return context.Transaction
            .Include(t => t.Staff)
            .FirstOrDefault(t => t.Id == id);
    }

    #endregion

    #region Add Sale Logic (Transaction)

    /// <summary>
    /// Completes a sale transaction, saving the Sale, its SaleEntries,
    /// deducting item stock levels for Products, and logging stock movement.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="sale">The Sale header record.</param>
    /// <param name="entries">The list of SaleEntry items purchased.</param>
    internal static bool AddSale(Sale sale, List<SaleEntry> entries)
    {
        using AppDbContext context = new();
        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction dbTransaction = context.Database.BeginTransaction();

        try
        {
            context.Entry(sale.Staff).State = EntityState.Unchanged;
            context.Sale.Add(sale);

            foreach (SaleEntry entry in entries)
            {
                context.Entry(entry.Item).State = EntityState.Unchanged;
                context.SaleEntry.Add(entry);

                Item? dbItem = context.Item.Find(entry.Item.Id);
                if (dbItem is null)
                {
                    Log.Err(() => $"Item with ID {entry.Item.Id} not found in database.");
                    dbTransaction.Rollback();
                    return false;
                }

                dbItem.Quantity -= entry.Quantity;
                context.Item.Update(dbItem);

                ulong itId = Utils.GenerateEntityId();
                ItemTransaction it = new(
                    itId,
                    dbItem,
                    ItemTransaction.Type.Sale,
                    entry,
                    -entry.Quantity,
                    sale.Staff,
                    DateTime.UtcNow,
                    $"Deduction from Sale ID {sale.Id}"
                );
                context.ItemTransaction.Add(it);
            }

            context.SaveChanges();
            dbTransaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            dbTransaction.Rollback();
            Log.Err(() => $"Error completing sale transaction: {ex.Message}");
            return false;
        }
    }

    #endregion
}
