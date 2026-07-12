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
        using AppDbContext context = new();
        context.Database.EnsureCreated();

        if (context.Staff.Any())
        {
            return;
        }

        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

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

        List<Role> defaultRoles =
        [
            new Role(
                id: Utils.GenerateEntityId(),
                name: "Admin",
                permissionsJson: "{\"Checkout\":1,\"ApplyDiscounts\":1,\"Refunds\":1,\"ViewInventory\":1,\"ModifyInventory\":1,\"ViewSensitiveInventoryCost\":1,\"ViewStaffInfo\":1,\"ManageStaff\":1,\"ManageAccess\":1,\"ManagePayroll\":1,\"ViewLogs\":1,\"DeleteLogs\":1,\"ManageSettings\":1}"
            ),
            new Role(
                id: Utils.GenerateEntityId(),
                name: "Manager",
                permissionsJson: "{\"Checkout\":1,\"ApplyDiscounts\":1,\"Refunds\":1,\"ViewInventory\":1,\"ModifyInventory\":1,\"ViewSensitiveInventoryCost\":1,\"ViewStaffInfo\":1,\"ManageStaff\":1,\"ManageAccess\":0,\"ManagePayroll\":1,\"ViewLogs\":1,\"DeleteLogs\":0,\"ManageSettings\":0}"
            ),
            new Role(
                id: Utils.GenerateEntityId(),
                name: "Cashier",
                permissionsJson: "{\"Checkout\":1,\"ApplyDiscounts\":0,\"Refunds\":0,\"ViewInventory\":1,\"ModifyInventory\":0,\"ViewSensitiveInventoryCost\":0,\"ViewStaffInfo\":0,\"ManageStaff\":0,\"ManageAccess\":0,\"ManagePayroll\":0,\"ViewLogs\":0,\"DeleteLogs\":0,\"ManageSettings\":0}"
            ),
            new Role(
                id: Utils.GenerateEntityId(),
                name: "Logistics",
                permissionsJson: "{\"Checkout\":0,\"ApplyDiscounts\":0,\"Refunds\":0,\"ViewInventory\":1,\"ModifyInventory\":1,\"ViewSensitiveInventoryCost\":0,\"ViewStaffInfo\":0,\"ManageStaff\":0,\"ManageAccess\":0,\"ManagePayroll\":0,\"ViewLogs\":1,\"DeleteLogs\":0,\"ManageSettings\":0}"
            )
        ];
        context.Role.AddRange(defaultRoles);

        staffMembers[0].RfidCardId = "3144185349";
        staffMembers[0].Roles.Add(defaultRoles[2]); // Cashier
        staffMembers[1].Roles.Add(defaultRoles[1]); // Manager
        staffMembers[4].Roles.Add(defaultRoles[0]); // Admin
        staffMembers[10].RfidCardId = "2853591044";
        foreach (var s in staffMembers)
        {
            s.Password = EZBM.Core.Services.AuthenticationService.HashPassword(s.Password ?? "password");
        }
        context.Staff.AddRange(staffMembers);
        context.SaveChanges();

        List<Item> itemsList =
        [
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 180.00f,
                name: "Pedigree Dog Food Dry (1kg)",
                description: "Nutritious dry food for adult dogs, beef flavor",
                tags: ["Food", "Consumable"],
                quantity: 120.0f,
                expirationDate: DateTime.UtcNow.AddDays(180),
                cost: 140.00f,
                targetStock: 200f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 35.00f,
                name: "Whiskas Cat Food Wet (85g)",
                description: "Wet food pouch for cats, tuna flavor in jelly",
                tags: ["Food", "Consumable"],
                quantity: 35.0f,
                expirationDate: DateTime.UtcNow.AddDays(90),
                cost: 25.00f,
                targetStock: 100f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Kilograms,
                isForSale: true,
                price: 280.00f,
                name: "Purina Friskies Cat Food Dry (1.1kg)",
                description: "Seafood sensations dry food for active cats",
                tags: ["Food", "Consumable"],
                quantity: 75.0f,
                expirationDate: DateTime.UtcNow.AddMonths(6),
                cost: 220.00f,
                targetStock: 100f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Liters,
                isForSale: true,
                price: 350.00f,
                name: "Kit Cat Soya Clump Cat Litter (7L)",
                description: "Eco-friendly, biodegradable soybean cat litter, original scent",
                tags: ["Consumable"],
                quantity: 3.0f,
                expirationDate: null,
                cost: 270.00f,
                targetStock: 15f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 160.00f,
                name: "Sleeky Dog Shampoo (350ml)",
                description: "Mild formula conditioning shampoo for dogs",
                tags: ["Hygiene", "Consumable"],
                quantity: 110.0f,
                expirationDate: null,
                cost: 110.00f,
                targetStock: 150f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 390.00f,
                name: "Saint Roche Dog Shampoo (628ml)",
                description: "Premium organic dog shampoo, sweet heaven scent",
                tags: ["Hygiene", "Consumable"],
                quantity: 250.0f,
                expirationDate: null,
                cost: 290.00f,
                targetStock: 300f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 75.00f,
                name: "Pet Collar Basic",
                description: "Adjustable nylon collar with safety bell",
                tags: ["Reusable"],
                quantity: 45.0f,
                expirationDate: null,
                cost: 35.00f,
                targetStock: 100f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 120.00f,
                name: "Pet Bowl Stainless Steel",
                description: "Non-slip stainless steel feeding bowl, medium size",
                tags: ["Reusable"],
                quantity: 2.0f,
                expirationDate: null,
                cost: 80.00f,
                targetStock: 10f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Count,
                isForSale: true,
                price: 50.00f,
                name: "Dog treats pack",
                description: "Crunchy beef flavor dog treats",
                tags: ["Food", "Consumable"],
                quantity: 1.0f,
                expirationDate: DateTime.UtcNow.AddMonths(12),
                cost: 35.00f,
                targetStock: 10f
            ),
            new Product(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Gallons,
                isForSale: false,
                price: null,
                name: "Industrial Cleaning Alcohol",
                description: "99% Isopropyl alcohol for store sanitization only",
                tags: ["Consumable"],
                quantity: 25.0f,
                expirationDate: null,
                cost: 320.00f,
                targetStock: 30f
            )
        ];

        itemsList[0].Barcode = "1234";
        itemsList[1].Barcode = "5678";
        itemsList.Add(new Service(Utils.GenerateEntityId(), Item.Unit.Unlimited, true, 400.00f, "Dog Grooming - Small", "Grooming package for small dog breeds under 10kg", null, 9999f, null, 0f, null, "silver_code"));
        itemsList.Add(new Service(Utils.GenerateEntityId(), Item.Unit.Unlimited, true, 550.00f, "Dog Grooming - Medium", "Grooming package for medium dog breeds 10-25kg", null, 9999f, null, 0f, null, "gold_code"));
        itemsList.Add(new Service(Utils.GenerateEntityId(), Item.Unit.Unlimited, true, 700.00f, "Dog Grooming - Large", "Grooming package for large dog breeds over 25kg", null, 9999f, null, 0f, null, "platinum_code"));
        itemsList.Add(new Service(Utils.GenerateEntityId(), Item.Unit.Unlimited, true, 500.00f, "Cat Grooming", "Standard bath and haircut package for cats", null, 9999f, null, 0f, null, "cat_grooming_code"));
        context.Item.AddRange(itemsList);
        context.SaveChanges();

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

        List<Sale> sales = [];
        List<SaleEntry> saleEntries = [];

        for (int i = 0; i < 10; i++)
        {
            DateTime saleTime = DateTime.UtcNow.AddDays(-i).AddHours(-2);
            int invoiceNum = Utils.GenerateInvoiceNumber(saleTime);

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

        Customer customer = new(Utils.GenerateEntityId(), "John", "Doe", "555-0202", "john.doe@example.com")
        {
            RfidCardId = "john_customer_card",
            ExpirationDate = DateTime.UtcNow.AddDays(30)
        };
        context.Customer.Add(customer);
        context.SaveChanges();
    }

    #endregion

}
