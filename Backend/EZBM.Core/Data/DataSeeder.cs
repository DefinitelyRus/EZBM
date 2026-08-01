using EZBM.Core.Data;
using EZBM.Core.Entities;
using EZBM.Core.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EZBM.Core.Data;

/// <summary>
/// Seeds the SQLite database with randomized, realistic test data for all entities.
/// </summary>
public static class DataSeeder
{
    #region Database Seeding

    /// <summary>
    /// Seeds the database. If forceSeed is true, resets the data.
    /// </summary>
    /// <param name="forceSeed">Whether to force database recreation and seeding.</param>
    public static void Seed(
        bool forceSeed = false
    )
    {
        using AppDbContext context = new();
        context.Database.EnsureCreated();

        if (!forceSeed)
        {
            return;
        }

        if (context.Staff.Any())
        {
            DbManager.Reset();
        }

        // Clear active pools to prevent SQLite locks during seeding
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Random rand = new();

        // 1. Seed Roles
        List<Role> roles = new()
        {
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
        };

        // Custom roles (random count between 1 and 3)
        int customRolesCount = rand.Next(1, 4);
        string[] customRoleNames = { "Security", "Clerk", "Operator", "Host" };
        for (int i = 0; i < customRolesCount; i++)
        {
            roles.Add(new Role(
                id: Utils.GenerateEntityId(),
                name: customRoleNames[i],
                permissionsJson: "{\"Checkout\":1,\"ApplyDiscounts\":0,\"Refunds\":0,\"ViewInventory\":1,\"ModifyInventory\":0,\"ViewSensitiveInventoryCost\":0,\"ViewStaffInfo\":0,\"ManageStaff\":0,\"ManageAccess\":0,\"ManagePayroll\":0,\"ViewLogs\":0,\"DeleteLogs\":0,\"ManageSettings\":0}"
            ));
        }
        context.Role.AddRange(roles);
        context.SaveChanges();

        // 2. Seed Staff
        List<Staff> staffMembers = new();
        string[] firstNames = { "Alice", "Bob", "Charlie", "David", "Emma", "Frank", "Grace", "Henry", "Irene", "Jack" };
        string[] lastNames = { "Smith", "Jones", "Brown", "Miller", "Davis", "Wilson", "Thomas", "Taylor", "Anderson", "White" };
        string[] positions = { "Junior Cashier", "Lead Custodian", "Security Guard", "Delivery Courier", "IT Support Specialist", "Marketing Lead" };
        
        int staffCount = rand.Next(5, 11);
        for (int i = 0; i < staffCount; i++)
        {
            string username = $"{firstNames[i].ToLower()}_{lastNames[rand.Next(lastNames.Length)].ToLower()}_{rand.Next(10, 99)}";
            Staff.Frequency payFreq = (Staff.Frequency)rand.Next(Enum.GetValues<Staff.Frequency>().Length);
            float payRate = payFreq switch
            {
                Staff.Frequency.Hourly => (float)Math.Round(12.0f + (float)rand.NextDouble() * 15.0f, 2),
                Staff.Frequency.Daily => (float)Math.Round(100.0f + (float)rand.NextDouble() * 80.0f, 2),
                Staff.Frequency.Weekly => (float)Math.Round(500.0f + (float)rand.NextDouble() * 300.0f, 2),
                Staff.Frequency.Biweekly => (float)Math.Round(1200.0f + (float)rand.NextDouble() * 800.0f, 2),
                Staff.Frequency.Monthly => (float)Math.Round(3000.0f + (float)rand.NextDouble() * 2000.0f, 2),
                _ => 15.0f
            };

            // Optional properties randomized (some entries omit email/phone/last name)
            string? email = rand.Next(10) < 8 ? $"{username}@ezbm.com" : null;
            string? phone = rand.Next(10) < 8 ? $"555-01{rand.Next(10, 99)}" : null;
            string? lastName = rand.Next(10) < 9 ? lastNames[rand.Next(lastNames.Length)] : null;

            Staff staff = new(
                id: Utils.GenerateEntityId(),
                username: username,
                payFrequency: payFreq,
                payRate: payRate,
                password: "password123",
                firstName: firstNames[i],
                lastName: lastName,
                email: email,
                phoneNumber: phone,
                position: positions[rand.Next(positions.Length)]
            );

            // Assign unique 10 digit RFID
            staff.RfidCardId = $"{rand.Next(100, 999)}{rand.Next(100, 999)}{rand.Next(1000, 9999)}";

            // Assign random role (not Admin)
            Role assignedRole = roles[rand.Next(1, roles.Count)];
            staff.Roles.Add(assignedRole);

            staffMembers.Add(staff);
        }

        // Always seed teto with the required test configuration
        Staff tetoStaff = new(
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
        );
        tetoStaff.RfidCardId = "2853591044";
        tetoStaff.Roles.Add(roles[0]); // Admin
        staffMembers.Add(tetoStaff);

        // Always seed cashier_alice with required test RFID
        Staff aliceStaff = new(
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
        );
        aliceStaff.RfidCardId = "3144185349";
        aliceStaff.Roles.Add(roles[2]); // Cashier
        staffMembers.Add(aliceStaff);

        // Always seed manager_bob with Manager role
        Staff bobStaff = new(
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
        );
        bobStaff.Roles.Add(roles[1]); // Manager
        staffMembers.Add(bobStaff);

        // Hash passwords
        foreach (Staff s in staffMembers)
        {
            s.Password = EZBM.Core.Services.AuthenticationService.HashPassword(s.Password ?? "password");
        }
        context.Staff.AddRange(staffMembers);
        context.SaveChanges();

        // 3. Seed Customers
        List<Customer> customers = new();
        string[] customerFirst = { "Liam", "Noah", "Oliver", "Elijah", "James", "William", "Benjamin", "Lucas", "Henry", "Alexander", "Mason", "Michael", "Ethan", "Daniel", "Jacob", "Logan", "Jackson", "Levi", "Sebastian", "Jack" };
        string[] customerLast = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };
        int customerCount = rand.Next(15, 31);
        for (int i = 0; i < customerCount; i++)
        {
            string first = customerFirst[rand.Next(customerFirst.Length)];
            string last = customerLast[rand.Next(customerLast.Length)];
            string? email = rand.Next(10) < 7 ? $"{first.ToLower()}.{last.ToLower()}@example.com" : null;
            string? phone = rand.Next(10) < 7 ? $"555-02{rand.Next(10, 99)}" : null;
            string? rfid = rand.Next(10) < 8 ? $"{rand.Next(100, 999)}{rand.Next(100, 999)}{rand.Next(1000, 9999)}" : null;
            DateTime? expiry = rand.Next(10) < 8 ? DateTime.UtcNow.AddDays(rand.Next(30, 365)) : null;

            Customer cust = new(
                id: Utils.GenerateEntityId(),
                firstName: first,
                lastName: last,
                phoneNumber: phone,
                email: email
            )
            {
                RfidCardId = rfid,
                ExpirationDate = expiry,
                AccessType = rand.Next(2) == 0 ? AccessCardType.Member : AccessCardType.OneTime
            };
            customers.Add(cust);
        }
        context.Customer.AddRange(customers);
        context.SaveChanges();

        // 4. Seed Items (Products & Services)
        List<Item> items = new();
        string[] petTypes = { "Dog", "Cat", "Bird", "Fish", "Small Animal" };
        string[] productCategories = { "Food", "Toy", "Accessory", "Litter", "Shampoo", "Collar", "Cage" };
        string[] brands = { "Pedigree", "Whiskas", "Purina", "Kit Cat", "Sleeky", "Saint Roche", "Royal Canin", "Friskies", "Orijen", "Acana" };
        
        int productCount = rand.Next(100, 121);
        for (int i = 0; i < productCount; i++)
        {
            string pet = petTypes[rand.Next(petTypes.Length)];
            string cat = productCategories[rand.Next(productCategories.Length)];
            string brand = brands[rand.Next(brands.Length)];
            string name = $"{brand} {pet} {cat} {rand.Next(1, 10)}";
            float cost = (float)Math.Round(1.0f + (float)rand.NextDouble() * 150.0f, 2);
            float price = (float)Math.Round(cost * (1.2f + (float)rand.NextDouble() * 0.5f), 2);
            float target = (float)rand.Next(10, 200);

            // Randomize stock limit: can be -1 (unlimited) or a random value up to target
            float quantity = rand.Next(10) < 2 ? -1.0f : (float)rand.Next(0, (int)target);
            Item.Unit unit = (Item.Unit)rand.Next(Enum.GetValues<Item.Unit>().Length);
            if (quantity == -1.0f)
            {
                unit = Item.Unit.Unlimited;
            }

            string? brandVal = rand.Next(10) < 8 ? brand : null;
            DateTime? expiry = rand.Next(10) < 4 ? DateTime.UtcNow.AddDays(rand.Next(30, 365)) : null;

            Product prod = new(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: unit,
                isForSale: true,
                price: price,
                name: name,
                description: $"High-quality {cat.ToLower()} for your pet.",
                tags: new List<string> { pet, cat },
                quantity: quantity,
                expirationDate: expiry,
                cost: cost,
                imageUrl: null,
                barcode: null,
                targetStock: target,
                lowStockThresholdPercentage: 0.20f,
                brand: brandVal
            );
            items.Add(prod);
        }

        // Add 10-15 Services
        int serviceCount = rand.Next(10, 16);
        string[] serviceNames = { "Pet Grooming Basic", "Grooming Deluxe", "Nail Clipping", "Ear Cleaning", "Basic Obedience Training", "Behavior Modification", "Pet Boarding Per Night", "Daycare Pass", "Vet Checkup General", "Vaccination Booster" };
        for (int i = 0; i < serviceCount; i++)
        {
            string name = serviceNames[i % serviceNames.Length] + (i >= serviceNames.Length ? $" #{i}" : "");
            float cost = (float)Math.Round(10.0f + (float)rand.NextDouble() * 100.0f, 2);
            float price = (float)Math.Round(cost * 1.5f, 2);

            Service serv = new(
                id: Utils.GenerateEntityId(),
                unitOfMeasurement: Item.Unit.Unlimited,
                isForSale: true,
                price: price,
                name: name,
                description: "Professional pet care service by certified specialists.",
                tags: new List<string> { "Service" },
                quantity: 0.0f,
                expirationDate: null,
                cost: cost,
                imageUrl: null,
                barcode: null
            );
            items.Add(serv);
        }
        context.Item.AddRange(items);
        context.SaveChanges();

        // 5. Seed Attendance Logs (last 14 days)
        List<Attendance> attendances = new();
        int attendanceCount = rand.Next(30, 61);
        for (int i = 0; i < attendanceCount; i++)
        {
            Staff staff = staffMembers[rand.Next(staffMembers.Count)];
            DateTime timeIn = DateTime.UtcNow.AddDays(-rand.Next(1, 14)).AddHours(-rand.Next(0, 24));
            DateTime? timeOut = rand.Next(10) < 9 ? timeIn.AddHours(rand.Next(4, 11)) : null;

            Attendance att = new(
                id: Utils.GenerateEntityId(),
                staff: staff,
                timeIn: timeIn,
                timeOut: timeOut
            );
            attendances.Add(att);
        }
        context.Attendance.AddRange(attendances);
        context.SaveChanges();

        // 6. Seed Staff Adjustments (Bonuses, Advances, Deductions)
        List<StaffAdjustment> adjustments = new();
        int adjustmentCount = rand.Next(20, 41);
        string[] adjTypes = { "Bonus", "Cash Advance", "Deduction" };
        for (int i = 0; i < adjustmentCount; i++)
        {
            Staff staff = staffMembers[rand.Next(staffMembers.Count)];
            string type = adjTypes[rand.Next(adjTypes.Length)];
            float amount = (float)Math.Round(10.0f + (float)rand.NextDouble() * 150.0f, 2);

            StaffAdjustment adj = new(
                id: Utils.GenerateEntityId(),
                staffId: staff.Id,
                adjustmentType: type,
                amount: amount,
                deductFromCurrentPayroll: type != "Bonus",
                isPaid: rand.Next(2) == 0,
                timestamp: DateTime.UtcNow.AddDays(-rand.Next(1, 30)),
                notes: $"Mock {type.ToLower()} adjustment record."
            );
            adjustments.Add(adj);
        }
        context.StaffAdjustment.AddRange(adjustments);
        context.SaveChanges();

        // 7. Seed Payroll Logs
        List<Payroll> payrolls = new();
        int payrollCount = rand.Next(10, 26);
        for (int i = 0; i < payrollCount; i++)
        {
            Staff staff = staffMembers[rand.Next(staffMembers.Count)];
            DateTime start = DateTime.UtcNow.AddDays(-rand.Next(15, 60));
            DateTime end = start.AddDays(7);
            float hours = (float)rand.Next(10, 50);
            float gross = (float)Math.Round(hours * staff.PayRate, 2);
            float modifiers = (float)Math.Round(-50.0f + (float)rand.NextDouble() * 100.0f, 2);
            float net = (float)Math.Round(Math.Max(0.0f, gross + modifiers), 2);

            Payroll pay = new(
                id: Utils.GenerateEntityId(),
                staff: staff,
                periodStart: start,
                periodEnd: end,
                totalHours: hours,
                grossAmount: gross,
                modifiers: modifiers,
                netAmount: net,
                payDate: end.AddDays(1),
                notes: "Automated periodic payroll payout entry."
            );
            payrolls.Add(pay);
        }
        context.Payroll.AddRange(payrolls);
        context.SaveChanges();

        // 8. Seed Sales & Transactions
        List<Sale> sales = new();
        List<Transaction> transactions = new();
        int saleCount = rand.Next(40, 81);
        
        List<Product> productsOnly = items.OfType<Product>().ToList();

        for (int i = 0; i < saleCount; i++)
        {
            Staff staff = staffMembers[rand.Next(staffMembers.Count)];
            Customer? customer = rand.Next(10) < 7 ? customers[rand.Next(customers.Count)] : null;
            DateTime timestamp = DateTime.UtcNow.AddDays(-rand.Next(1, 30));
            Transaction.PayMethod method = (Transaction.PayMethod)rand.Next(Enum.GetValues<Transaction.PayMethod>().Length);

            // Calculate totalSaleAmount first
            int entryCount = rand.Next(1, 6);
            float totalSaleAmount = 0.0f;
            
            List<Product> saleProducts = new();
            List<float> saleQtys = new();
            for (int j = 0; j < entryCount; j++)
            {
                Product prod = productsOnly[rand.Next(productsOnly.Count)];
                float qty = rand.Next(1, 5);
                totalSaleAmount += (float)Math.Round(qty * (prod.SalePrice ?? 10.0f), 2);
                saleProducts.Add(prod);
                saleQtys.Add(qty);
            }

            int invoiceNum = rand.Next(100000, 999999);
            Sale sale = new(
                id: Utils.GenerateEntityId(),
                invoiceNumber: invoiceNum,
                amount: (float)Math.Round(totalSaleAmount, 2),
                paymentMethod: method,
                staff: staff,
                timestamp: timestamp
            );
            sale.Customer = customer;

            for (int j = 0; j < saleProducts.Count; j++)
            {
                Product prod = saleProducts[j];
                float qty = saleQtys[j];
                float entryTotal = (float)Math.Round(qty * (prod.SalePrice ?? 10.0f), 2);

                SaleEntry entry = new(
                    sale: sale,
                    item: prod,
                    quantity: qty,
                    unitPrice: prod.SalePrice ?? 10.0f,
                    subtotal: entryTotal
                );
                sale.SaleEntries.Add(entry);
            }

            // Generate child Transaction record(s)
            if (method == Transaction.PayMethod.Mixed)
            {
                float cashPart = (float)Math.Round(totalSaleAmount * 0.4f, 2);
                float cardPart = (float)Math.Round(sale.Amount - cashPart, 2);

                Transaction parentTx = new(
                    id: Utils.GenerateEntityId(),
                    transactionType: Transaction.Type.Income,
                    amount: sale.Amount,
                    timestamp: timestamp,
                    staff: staff,
                    paymentMethod: Transaction.PayMethod.Mixed,
                    invoiceNumber: invoiceNum,
                    invoicePrefix: "SALE"
                );
                parentTx.Customer = customer;

                Transaction cashTx = new(
                    id: Utils.GenerateEntityId(),
                    transactionType: Transaction.Type.Income,
                    amount: cashPart,
                    timestamp: timestamp,
                    staff: staff,
                    paymentMethod: Transaction.PayMethod.Cash,
                    invoiceNumber: invoiceNum,
                    invoicePrefix: "SALE"
                );
                cashTx.ParentTransactionId = parentTx.Id;
                cashTx.Customer = customer;

                Transaction cardTx = new(
                    id: Utils.GenerateEntityId(),
                    transactionType: Transaction.Type.Income,
                    amount: cardPart,
                    timestamp: timestamp,
                    staff: staff,
                    paymentMethod: Transaction.PayMethod.Credit,
                    invoiceNumber: invoiceNum,
                    invoicePrefix: "SALE"
                );
                cardTx.ParentTransactionId = parentTx.Id;
                cardTx.Customer = customer;

                transactions.Add(parentTx);
                transactions.Add(cashTx);
                transactions.Add(cardTx);
                sale.ChildTransactions.Add(parentTx);
            }
            else
            {
                Transaction tx = new(
                    id: Utils.GenerateEntityId(),
                    transactionType: Transaction.Type.Income,
                    amount: sale.Amount,
                    timestamp: timestamp,
                    staff: staff,
                    paymentMethod: method,
                    invoiceNumber: invoiceNum,
                    invoicePrefix: "SALE"
                );
                tx.Customer = customer;
                transactions.Add(tx);
                sale.ChildTransactions.Add(tx);
            }

            sales.Add(sale);
        }

        context.Transaction.AddRange(transactions);
        context.Sale.AddRange(sales);
        context.SaveChanges();
    }

    #endregion
}
