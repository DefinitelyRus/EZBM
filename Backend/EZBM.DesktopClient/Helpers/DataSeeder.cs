using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;

namespace EZBM.DesktopClient.Helpers;

/// <summary>
/// Seeds the SQLite database with 10 wildly different, realistic entries for all entities.
/// </summary>
public static class DataSeeder
{

    #region Database Seeding

    /// <summary>
    /// Resets the database and populates 10 distinct records for all system entities.
    /// </summary>
    public static void Seed()
    {
        // Clear active SQLite database connections
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        using AppDbContext context = new();

        // Clear existing tables
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Generate 10 Staff Members
        List<Staff> staffMembers =
        [
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "cashier_alice",
                payFrequency: Staff.Frequency.Hourly,
                payRate: 15.50f,
                password: "password123",
                firstName: "Alice",
                lastName: "Smith",
                email: "alice@ezbm.com",
                phoneNumber: "555-0101",
                position: "Junior Cashier"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "manager_bob",
                payFrequency: Staff.Frequency.Monthly,
                payRate: 4500.00f,
                password: "secure_pass",
                firstName: "Bob",
                lastName: "Jones",
                email: "bob@ezbm.com",
                phoneNumber: "555-0102",
                position: "General Store Manager"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "janitor_charlie",
                payFrequency: Staff.Frequency.Weekly,
                payRate: 500.00f,
                password: "clean_pass",
                firstName: "Charlie",
                lastName: "Brown",
                email: "charlie@ezbm.com",
                phoneNumber: "555-0103",
                position: "Lead Custodian"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "guard_david",
                payFrequency: Staff.Frequency.Daily,
                payRate: 140.00f,
                password: "guard_pass",
                firstName: "David",
                lastName: "Miller",
                email: "david@ezbm.com",
                phoneNumber: "555-0104",
                position: "Security Guard"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "ceo_emma",
                payFrequency: Staff.Frequency.Biweekly,
                payRate: 7500.00f,
                password: "ceo_exclusive",
                firstName: "Emma",
                lastName: "Davis",
                email: "emma@ezbm.com",
                phoneNumber: "555-0105",
                position: "Chief Executive Officer"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "baker_frank",
                payFrequency: Staff.Frequency.Hourly,
                payRate: 19.00f,
                password: "bake_bread",
                firstName: "Frank",
                lastName: "Wilson",
                email: "frank@ezbm.com",
                phoneNumber: "555-0106",
                position: "Head Pastry Chef"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "accountant_grace",
                payFrequency: Staff.Frequency.Weekly,
                payRate: 1250.00f,
                password: "audit_pass",
                firstName: "Grace",
                lastName: "Thomas",
                email: "grace@ezbm.com",
                phoneNumber: "555-0107",
                position: "Senior Accountant"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "driver_henry",
                payFrequency: Staff.Frequency.Hourly,
                payRate: 17.25f,
                password: "drive_safe",
                firstName: "Henry",
                lastName: "Taylor",
                email: "henry@ezbm.com",
                phoneNumber: "555-0108",
                position: "Delivery Courier"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "it_irene",
                payFrequency: Staff.Frequency.Monthly,
                payRate: 5200.00f,
                password: "sys_admin",
                firstName: "Irene",
                lastName: "Anderson",
                email: "irene@ezbm.com",
                phoneNumber: "555-0109",
                position: "IT Support Specialist"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "marketing_jack",
                payFrequency: Staff.Frequency.Biweekly,
                payRate: 2300.00f,
                password: "promo_pass",
                firstName: "Jack",
                lastName: "White",
                email: "jack@ezbm.com",
                phoneNumber: "555-0110",
                position: "Marketing Lead"
            ),
            new Staff(
                id: Utils.GenerateEntityId(),
                username: "teto",
                payFrequency: Staff.Frequency.Hourly,
                payRate: 25.00f,
                password: "teto41",
                firstName: "Teto",
                lastName: "Kasane",
                email: "teto@ezbm.com",
                phoneNumber: "555-0401",
                position: "Testing Specialist"
            )
        ];

        context.Staff.AddRange(staffMembers);
        context.SaveChanges();

        // Generate 10 Items
        List<Item> itemsList =
        [
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 0.99f,
                name: "Fresh Red Apple",
                description: "Sweet organic gala apple",
                tags: [Item.Tag.Food, Item.Tag.Consumable],
                quantity: 120.0f,
                expirationDate: DateTime.UtcNow.AddDays(14),
                cost: 0.25f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 8.49f,
                name: "Herbal Essence Shampoo",
                description: "Moisturizing hair shampoo, 400ml",
                tags: [Item.Tag.Hygiene, Item.Tag.Consumable],
                quantity: 35.0f,
                expirationDate: null,
                cost: 3.10f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Kilograms,
                isForSale: true,
                price: 2.80f,
                name: "Whole Wheat Flour",
                description: "Stoneground organic wheat flour",
                tags: [Item.Tag.Food, Item.Tag.Consumable],
                quantity: 75.0f,
                expirationDate: DateTime.UtcNow.AddMonths(6),
                cost: 1.05f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Liters,
                isForSale: true,
                price: 1.95f,
                name: "Organic Whole Milk",
                description: "Pasteurized farm fresh milk",
                tags: [Item.Tag.Food, Item.Tag.Consumable],
                quantity: 3.0f, // Low Stock!
                expirationDate: DateTime.UtcNow.AddDays(5),
                cost: 0.80f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 1.50f,
                name: "Recycled Paper Towels",
                description: "Eco-friendly 2-ply kitchen paper towel roll",
                tags: [Item.Tag.Consumable],
                quantity: 110.0f,
                expirationDate: null,
                cost: 0.45f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 2.00f,
                name: "Cotton Shopping Bag",
                description: "Reusable organic cotton tote bag",
                tags: [Item.Tag.Reusable],
                quantity: 250.0f,
                expirationDate: null,
                cost: 0.60f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Milliliters,
                isForSale: true,
                price: 0.04f,
                name: "Extra Virgin Olive Oil",
                description: "Cold-pressed Greek olive oil, sold per ml",
                tags: [Item.Tag.Food, Item.Tag.Consumable],
                quantity: 8000.0f,
                expirationDate: DateTime.UtcNow.AddYears(1),
                cost: 0.015f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Pounds,
                isForSale: true,
                price: 13.99f,
                name: "Premium Beef Ribeye",
                description: "USDA Choice ribeye steak",
                tags: [Item.Tag.Food, Item.Tag.Consumable],
                quantity: 2.0f, // Low Stock!
                expirationDate: DateTime.UtcNow.AddDays(3),
                cost: 6.50f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 3.50f,
                name: "Antibacterial Dish Soap",
                description: "Lemon scent liquid soap, 500ml",
                tags: [Item.Tag.Hygiene, Item.Tag.Consumable],
                quantity: 1.0f, // Low Stock!
                expirationDate: null,
                cost: 1.20f
            ),
            new Item(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Gallons,
                isForSale: false,
                price: null,
                name: "Industrial Cleaning Alcohol",
                description: "99% Isopropyl alcohol for store sanitization only",
                tags: [Item.Tag.Consumable],
                quantity: 25.0f,
                expirationDate: null,
                cost: 8.00f
            )
        ];

        context.Item.AddRange(itemsList);
        context.SaveChanges();

        // Generate 10 Attendances
        DateTime baseTime = DateTime.UtcNow.Date.AddDays(-5);
        List<Attendance> attendances =
        [
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[0],
                timeIn: baseTime.AddHours(8),
                timeOut: baseTime.AddHours(16)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[1],
                timeIn: baseTime.AddHours(9),
                timeOut: baseTime.AddHours(17)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[2],
                timeIn: baseTime.AddDays(1).AddHours(7),
                timeOut: baseTime.AddDays(1).AddHours(15)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[3],
                timeIn: baseTime.AddDays(1).AddHours(18),
                timeOut: baseTime.AddDays(2).AddHours(2)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[5],
                timeIn: baseTime.AddDays(2).AddHours(5),
                timeOut: baseTime.AddDays(2).AddHours(13)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[7],
                timeIn: baseTime.AddDays(3).AddHours(10),
                timeOut: baseTime.AddDays(3).AddHours(18)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[8],
                timeIn: baseTime.AddDays(3).AddHours(9),
                timeOut: baseTime.AddDays(3).AddHours(17)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[0],
                timeIn: baseTime.AddDays(4).AddHours(8),
                timeOut: baseTime.AddDays(4).AddHours(12)
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[0],
                timeIn: DateTime.UtcNow.AddHours(-2),
                timeOut: null
            ),
            new Attendance(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[5],
                timeIn: DateTime.UtcNow.AddHours(-4),
                timeOut: null
            )
        ];

        context.Attendance.AddRange(attendances);
        context.SaveChanges();

        // Generate 10 Payroll payments
        List<Payroll> payrolls =
        [
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[0],
                periodStart: baseTime.AddMonths(-1),
                periodEnd: baseTime,
                totalHours: 160.0f,
                grossAmount: 2480.00f,
                modifiers: -150.00f,
                netAmount: 2330.00f,
                payDate: baseTime.AddDays(1),
                notes: "Regular hourly salary"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[1],
                periodStart: baseTime.AddMonths(-1),
                periodEnd: baseTime,
                totalHours: 160.0f,
                grossAmount: 4500.00f,
                modifiers: 200.00f,
                netAmount: 4700.00f,
                payDate: baseTime.AddDays(1),
                notes: "Monthly salary + bonus"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[2],
                periodStart: baseTime.AddDays(-7),
                periodEnd: baseTime,
                totalHours: 40.0f,
                grossAmount: 500.00f,
                modifiers: 0f,
                netAmount: 500.00f,
                payDate: baseTime.AddDays(2),
                notes: "Weekly paycheck"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[3],
                periodStart: baseTime.AddDays(-7),
                periodEnd: baseTime,
                totalHours: 48.0f,
                grossAmount: 840.00f,
                modifiers: 50.00f,
                netAmount: 890.00f,
                payDate: baseTime.AddDays(2),
                notes: "Security shift pay"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[5],
                periodStart: baseTime.AddMonths(-1),
                periodEnd: baseTime,
                totalHours: 155.0f,
                grossAmount: 2945.00f,
                modifiers: -80.00f,
                netAmount: 2865.00f,
                payDate: baseTime.AddDays(1),
                notes: "Chef monthly pay"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[6],
                periodStart: baseTime.AddMonths(-1),
                periodEnd: baseTime,
                totalHours: 160.0f,
                grossAmount: 5000.00f,
                modifiers: -450.00f,
                netAmount: 4550.00f,
                payDate: baseTime.AddDays(1),
                notes: "Senior account monthly wage"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[7],
                periodStart: baseTime.AddDays(-14),
                periodEnd: baseTime,
                totalHours: 85.0f,
                grossAmount: 1466.25f,
                modifiers: 120.00f,
                netAmount: 1586.25f,
                payDate: baseTime.AddDays(2),
                notes: "Biweekly delivery pay"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[8],
                periodStart: baseTime.AddMonths(-1),
                periodEnd: baseTime,
                totalHours: 160.0f,
                grossAmount: 5200.00f,
                modifiers: -600.00f,
                netAmount: 4600.00f,
                payDate: baseTime.AddDays(1),
                notes: "IT Specialist salary"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[9],
                periodStart: baseTime.AddDays(-14),
                periodEnd: baseTime,
                totalHours: 80.0f,
                grossAmount: 2300.00f,
                modifiers: -200.00f,
                netAmount: 2100.00f,
                payDate: baseTime.AddDays(3),
                notes: "Biweekly marketing payout"
            ),
            new Payroll(
                id: Utils.GenerateEntityId(),
                staff: staffMembers[4],
                periodStart: baseTime.AddDays(-14),
                periodEnd: baseTime,
                totalHours: 80.0f,
                grossAmount: 7500.00f,
                modifiers: -950.00f,
                netAmount: 6550.00f,
                payDate: baseTime.AddDays(1),
                notes: "CEO executive salary"
            )
        ];

        context.Payroll.AddRange(payrolls);
        context.SaveChanges();

        // Generate 10 Sales & SaleEntries
        List<Sale> sales = [];
        List<SaleEntry> saleEntries = [];

        for (int i = 0; i < 10; i++)
        {
            DateTime saleTime = DateTime.UtcNow.AddDays(-i).AddHours(-2);
            int invoiceNum = Utils.GenerateInvoiceNumber(saleTime);

            // Generate total amount dynamically based on item index
            Item selectedItem = itemsList[i];
            float quantity = 2.0f;
            float unitPrice = selectedItem.SalePrice ?? 1.99f;
            float total = quantity * unitPrice;

            Transaction.PayMethod method = (Transaction.PayMethod)(i % 5);

            Sale sale = new(
                id: Utils.GenerateEntityId(),
                invoiceNumber: invoiceNum,
                amount: total,
                paymentMethod: method,
                staff: staffMembers[i % staffMembers.Count],
                timestamp: saleTime,
                notes: $"Customer test checkout number {i + 1}"
            );

            SaleEntry entry = new(
                sale: sale,
                item: selectedItem,
                quantity: quantity,
                unitPrice: unitPrice,
                subtotal: total
            );

            selectedItem.Quantity -= quantity;

            sales.Add(sale);
            saleEntries.Add(entry);
        }

        context.Sale.AddRange(sales);
        context.SaleEntry.AddRange(saleEntries);
        context.SaveChanges();

        // Generate 10 ItemTransactions
        List<ItemTransaction> itemTransactions =
        [
            new ItemTransaction(
                item: itemsList[0],
                transactionType: ItemTransaction.Type.NewStock,
                saleEntry: null,
                quantity: 100.0f,
                staff: staffMembers[1],
                timestamp: baseTime,
                note: "Initial intake of fresh apples"
            ),
            new ItemTransaction(
                item: itemsList[1],
                transactionType: ItemTransaction.Type.NewStock,
                saleEntry: null,
                quantity: 20.0f,
                staff: staffMembers[1],
                timestamp: baseTime.AddDays(1),
                note: "Shampoo restock shipment"
            ),
            new ItemTransaction(
                item: itemsList[2],
                transactionType: ItemTransaction.Type.Correction_Set,
                saleEntry: null,
                quantity: 80.0f,
                staff: staffMembers[6],
                timestamp: baseTime.AddDays(2),
                note: "Inventory audit count correction"
            ),
            new ItemTransaction(
                item: itemsList[3],
                transactionType: ItemTransaction.Type.Damaged_Lost_Expired,
                saleEntry: null,
                quantity: 2.0f,
                staff: staffMembers[2],
                timestamp: baseTime.AddDays(2),
                note: "Spoiled milk bottles discarded"
            ),
            new ItemTransaction(
                item: itemsList[4],
                transactionType: ItemTransaction.Type.Consumed,
                saleEntry: null,
                quantity: 5.0f,
                staff: staffMembers[2],
                timestamp: baseTime.AddDays(3),
                note: "Paper towels used for storefront cleaning"
            ),

            new ItemTransaction(
                item: itemsList[0],
                transactionType: ItemTransaction.Type.Sale,
                saleEntry: saleEntries[0],
                quantity: saleEntries[0].Quantity,
                staff: sales[0].Staff,
                timestamp: sales[0].Timestamp,
                note: $"Sold in Invoice {sales[0].InvoiceId}"
            ),
            new ItemTransaction(
                item: itemsList[1],
                transactionType: ItemTransaction.Type.Sale,
                saleEntry: saleEntries[1],
                quantity: saleEntries[1].Quantity,
                staff: sales[1].Staff,
                timestamp: sales[1].Timestamp,
                note: $"Sold in Invoice {sales[1].InvoiceId}"
            ),
            new ItemTransaction(
                item: itemsList[2],
                transactionType: ItemTransaction.Type.Sale,
                saleEntry: saleEntries[2],
                quantity: saleEntries[2].Quantity,
                staff: sales[2].Staff,
                timestamp: sales[2].Timestamp,
                note: $"Sold in Invoice {sales[2].InvoiceId}"
            ),
            new ItemTransaction(
                item: itemsList[3],
                transactionType: ItemTransaction.Type.Sale,
                saleEntry: saleEntries[3],
                quantity: saleEntries[3].Quantity,
                staff: sales[3].Staff,
                timestamp: sales[3].Timestamp,
                note: $"Sold in Invoice {sales[3].InvoiceId}"
            ),
            new ItemTransaction(
                item: itemsList[4],
                transactionType: ItemTransaction.Type.Sale,
                saleEntry: saleEntries[4],
                quantity: saleEntries[4].Quantity,
                staff: sales[4].Staff,
                timestamp: sales[4].Timestamp,
                note: $"Sold in Invoice {sales[4].InvoiceId}"
            )
        ];

        context.ItemTransaction.AddRange(itemTransactions);
        context.SaveChanges();
    }

    #endregion

}
