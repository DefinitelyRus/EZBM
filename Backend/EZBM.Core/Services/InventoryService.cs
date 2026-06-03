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
/// Service class for managing inventory Items and ItemTransactions.
/// <br/><br/>
/// <i>Author(s): Google Antigravity<br/>
/// Editor(s): None<br/>
/// Documented by: Google Antigravity</i>
/// </summary>
internal static class InventoryService
{
    #region Factory Methods

    /// <summary>
    /// Creates an Item instance from a JSON string.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing item details.</param>
    internal static Item? CreateItemInstance(string json)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as an Item object.");
            return null;
        }

        ulong id = 0;
        Item.QType unitOfMeasurement = Item.QType.Count;
        bool isForSale = false;
        float price = 0;
        string? name = null;
        string? description = null;
        List<Item.Tag>? tags = null;
        float quantity = 0;
        DateTime? expirationDate = null;
        float cost = 0;
        string? imageUrl = null;

        foreach (KeyValuePair<string, object> kvp in contents)
        {
            switch (kvp.Key.ToLowerInvariant())
            {
                case "id":
                    ulong? v_id = Utils.GetAsUlong(kvp.Value);
                    if (v_id.HasValue) id = v_id.Value;
                    break;

                case "unitofmeasurement":
                    string? uomStr = Utils.GetAsString(kvp.Value);
                    if (uomStr != null && Enum.TryParse<Item.QType>(uomStr, true, out Item.QType parsedUom))
                    {
                        unitOfMeasurement = parsedUom;
                    }
                    break;

                case "isforsale":
                    isForSale = Utils.GetAsBool(kvp.Value) ?? false;
                    break;

                case "saleprice" or "price":
                    price = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "name":
                    name = Utils.GetAsString(kvp.Value);
                    break;

                case "description":
                    description = Utils.GetAsString(kvp.Value);
                    break;

                case "tags":
                    tags = Utils.GetAsTagsList(kvp.Value);
                    break;

                case "quantity" or "stockquantity":
                    quantity = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "expirationdate":
                    expirationDate = Utils.GetAsDateTime(kvp.Value);
                    break;

                case "cost" or "costprice":
                    cost = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "imageurl":
                    imageUrl = Utils.GetAsString(kvp.Value);
                    break;

                default:
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in Item JSON. Skipping...");
                    break;
            }
        }

        return new Item(
            id,
            unitOfMeasurement,
            isForSale,
            price,
            name,
            description,
            tags,
            quantity,
            expirationDate,
            cost,
            imageUrl
        );
    }

    /// <summary>
    /// Creates an ItemTransaction instance from a JSON string, resolving relations from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="json">The JSON substring containing item transaction details.</param>
    /// <param name="context">The database context used to lookup relations.</param>
    internal static ItemTransaction? CreateItemTransactionInstance(string json, AppDbContext context)
    {
        Dictionary<string, object>? contents = Utils.ConvertFromJson(json);

        if (contents is null)
        {
            Log.Err(() => "Unable to parse the input JSON as an ItemTransaction object.");
            return null;
        }

        ulong id = 0;
        Item? item = null;
        ItemTransaction.Type transactionType = ItemTransaction.Type.NewStock;
        SaleEntry? saleEntry = null;
        float quantity = 0;
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

                case "itemid":
                    ulong? itemId = Utils.GetAsUlong(kvp.Value);
                    if (itemId.HasValue)
                    {
                        item = context.Item.Find(itemId.Value);
                    }
                    break;

                case "transactiontype" or "type":
                    string? typeStr = Utils.GetAsString(kvp.Value);
                    if (typeStr != null && Enum.TryParse<ItemTransaction.Type>(typeStr, true, out ItemTransaction.Type parsedType))
                    {
                        transactionType = parsedType;
                    }
                    break;

                case "saleentryid":
                    ulong? saleEntryId = Utils.GetAsUlong(kvp.Value);
                    if (saleEntryId.HasValue)
                    {
                        saleEntry = context.SaleEntry.Find(saleEntryId.Value);
                    }
                    break;

                case "quantity":
                    quantity = Utils.GetAsFloat(kvp.Value) ?? 0;
                    break;

                case "staffid":
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
                    Log.Warn(() => $"Invalid key '{kvp.Key}' detected in ItemTransaction JSON. Skipping...");
                    break;
            }
        }

        if (item is null)
        {
            Log.Err(() => "Item reference is required to create an ItemTransaction instance.");
            return null;
        }

        if (staff is null)
        {
            Log.Err(() => "Staff reference is required to create an ItemTransaction instance.");
            return null;
        }

        return new ItemTransaction(
            id,
            item,
            transactionType,
            saleEntry,
            quantity,
            staff,
            timestamp,
            notes
        );
    }

    #endregion

    #region CRUD Item Operations

    /// <summary>
    /// Retrieves all items from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<Item> GetAllItems()
    {
        using AppDbContext context = new();
        return [.. context.Item];
    }

    /// <summary>
    /// Retrieves a specific item by its unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static Item? GetItemById(ulong id)
    {
        using AppDbContext context = new();
        return context.Item.Find(id);
    }

    /// <summary>
    /// Adds a new item to the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool AddItem(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            Log.Err(() => "Item name cannot be empty.");
            return false;
        }
        if (item.SalePrice < 0 || item.Cost < 0)
        {
            Log.Err(() => "Item prices or costs cannot be negative.");
            return false;
        }

        try
        {
            using AppDbContext context = new();
            context.Item.Add(item);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error adding item: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Updates an existing item in the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool UpdateItem(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            Log.Err(() => "Item name cannot be empty.");
            return false;
        }
        if (item.SalePrice < 0 || item.Cost < 0)
        {
            Log.Err(() => "Item prices or costs cannot be negative.");
            return false;
        }

        try
        {
            using AppDbContext context = new();
            context.Item.Update(item);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error updating item: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deletes an item by its unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static bool DeleteItem(ulong id)
    {
        try
        {
            using AppDbContext context = new();
            Item? item = context.Item.Find(id);
            if (item is null) return false;
            context.Item.Remove(item);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error deleting item: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region CRUD ItemTransaction Operations

    /// <summary>
    /// Retrieves all item transactions from the database.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static List<ItemTransaction> GetAllItemTransactions()
    {
        using AppDbContext context = new();
        return [.. context.ItemTransaction
            .Include(it => it.Item)
            .Include(it => it.Staff)
            .Include(it => it.SaleEntry)];
    }

    /// <summary>
    /// Retrieves a specific item transaction by its unique ID.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    internal static ItemTransaction? GetItemTransactionById(ulong id)
    {
        using AppDbContext context = new();
        return context.ItemTransaction
            .Include(it => it.Item)
            .Include(it => it.Staff)
            .Include(it => it.SaleEntry)
            .FirstOrDefault(it => it.Id == id);
    }

    #endregion

    #region Stock Adjustment Operations

    /// <summary>
    /// Adjusts the stock quantity of an item and records an ItemTransaction log.
    /// <br/><br/>
    /// <i>Author(s): Google Antigravity<br/>
    /// Editor(s): None<br/>
    /// Documented by: Google Antigravity</i>
    /// </summary>
    /// <param name="itemId">The unique ID of the item.</param>
    /// <param name="quantityChange">The change in quantity (positive for additions, negative for reductions).</param>
    /// <param name="type">The type of stock movement.</param>
    /// <param name="staffId">The ID of the staff member performing the action.</param>
    /// <param name="notes">Optional remarks about the stock adjustment.</param>
    internal static bool AdjustStock(ulong itemId, float quantityChange, ItemTransaction.Type type, ulong staffId, string? notes = null)
    {
        try
        {
            using AppDbContext context = new();
            Item? item = context.Item.Find(itemId);
            if (item is null)
            {
                Log.Err(() => $"Item with ID {itemId} not found.");
                return false;
            }

            Staff? staff = context.Staff.Find(staffId);
            if (staff is null)
            {
                Log.Err(() => $"Staff member with ID {staffId} not found.");
                return false;
            }

            // Update item stock count
            item.Quantity += quantityChange;
            context.Item.Update(item);

            // Log item transaction
            ulong itId = Utils.GenerateEntityId();
            ItemTransaction it = new(itId, item, type, null, quantityChange, staff, DateTime.UtcNow, notes);
            context.Entry(it.Item).State = EntityState.Unchanged;
            context.Entry(it.Staff).State = EntityState.Unchanged;
            context.ItemTransaction.Add(it);

            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Log.Err(() => $"Error adjusting stock: {ex.Message}");
            return false;
        }
    }

    #endregion
}
