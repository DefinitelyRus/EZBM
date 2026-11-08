# Tasks 261108-02

## Current Status

Phase 1 is complete. The application starts with a single command, applies SQLite migrations on launch, serves the dashboard from `wwwroot`, and processes product stock additions. Phase 2 introduces inventory audit trails, payment processing, and Point of Sale (POS) cart workflows.

## Tasks Summary

1. Implement Product Transactions Query Endpoints
2. Build Inventory Audit Log Interface in `wwwroot`
3. Implement Payment Models and Processing Service
4. Build Payment Method UI in `wwwroot`
5. Implement POS Cart Checkout and Stock Reduction Service
6. Build Vanilla HTML/JS Point of Sale Interface in `wwwroot`

## Tasks

### Task 1: Implement Product Transactions Query Endpoints

The API cannot retrieve existing stock transaction records. Endpoints in `ProductTransactionsController.cs` return `501 Not Implemented`.

Implement `GET /api/ProductTransactions` and `GET /api/ProductTransactions/{id}` in `ProductTransactionsController.cs` to return stored transaction entries.

1. Open `Backend/Backend.DesktopHost/Controllers/ProductTransactionController.cs`.
2. Implement `GetProductTransactions`:
    - Query `_context.ProductTransactions`.
    - Include associated `Product` and `Batch` data.
    - Return `200 OK` with the list of transactions.
3. Implement `GetProductTransaction(long id)`:
    - Query `_context.ProductTransactions` by identifier.
    - Return `404 Not Found` when the transaction does not exist.
    - Return `200 OK` with the found transaction.
4. Verify endpoints through Swagger UI or PowerShell request tests.

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<ProductTransaction>>> GetProductTransactions()
{
    List<ProductTransaction> transactions = await _context.ProductTransactions
        .Include(t => t.Product)
        .Include(t => t.Batch)
        .OrderByDescending(t => t.Timestamp)
        .ToListAsync();

    return Ok(transactions);
}
```

#### Things to Consider

- Prevent circular reference issues in JSON serialization when including related entities.
- Consider creating a dedicated DTO record for transaction query responses.

### Task 2: Build Inventory Audit Log Interface in `wwwroot`

Users cannot view past stock movements or transaction audit records in the browser.

Add an inventory audit table to `wwwroot/index.html` and connect it with `app.js` to show all movements.

1. Open `Backend/Backend.DesktopHost/wwwroot/index.html`.
2. Add a new `<fieldset>` titled "Inventory Audit Log".
3. Add a "Refresh Audit Log" button and a `<table>` with headers:
    - ID
    - Timestamp
    - Product
    - Type
    - Quantity
    - Notes
4. Open `Backend/Backend.DesktopHost/wwwroot/js/app.js`.
5. Add `loadTransactions()`:
    - Send a `GET` request to `/api/ProductTransactions`.
    - Populate table rows dynamically.
6. Trigger `loadTransactions()` on initial page load and after every stock update.

#### Things to Consider

- Format ISO timestamps into local date-time strings for user readability.
- Add basic CSS styling to distinguish purchase movements from sales.

### Task 3: Implement Payment Models and Processing Service

The backend cannot record customer payment details or split payments across payment methods.

Create payment models, update database migrations, and build a payment processing service in `Backend.Core` and `Backend.DesktopHost`.

1. Open `Backend/Backend.Core/Common/` and create `PaymentMethod.cs` (e.g. `Cash`, `Card`, `EWallet`).
2. Open `Backend/Backend.Core/Models/` and create `PaymentRecord.cs`:
    - Add properties for transaction ID, method, amount tendered, amount charged, change, and timestamp.
3. Register `PaymentRecords` DbSet in `Backend/Backend.Core/Data/AppDbContext.cs`.
4. Create an EF Core migration:
    - Run `dotnet ef migrations add AddPaymentRecords`.
5. Create `PaymentService.cs` in `Backend/Backend.DesktopHost/Services/`.
6. Add `PaymentsController.cs` in `Backend/Backend.DesktopHost/Controllers/` with `POST /api/Payments/process`.

```csharp
namespace Backend.Core.Models;

public class PaymentRecord : Entity
{
    public decimal Amount { get; set; }
    public decimal AmountTendered { get; set; }
    public decimal ChangeAmount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

#### Things to Consider

- Ensure split payments validate that total paid covers the total balance.
- Prevent negative payment amounts.

### Task 4: Build Payment Method UI in `wwwroot`

Users cannot test payment recording or split payments in the dashboard.

Add a payment processing section in `index.html` and wire it up via `app.js`.

1. Open `Backend/Backend.DesktopHost/wwwroot/index.html`.
2. Add a "Process Payment" fieldset.
3. Add inputs for total amount, payment method selector, tendered amount, and reference notes.
4. Add split payment controls to add secondary payment method inputs.
5. In `wwwroot/js/app.js`, add `processPayment()` to submit payment details to `/api/Payments/process`.
6. Display payment summary, change amount, and transaction identifier upon confirmation.

#### Things to Consider

- Calculate change due automatically when user types the tendered amount.

### Task 5: Implement POS Cart Checkout and Stock Reduction Service

The application has no sales checkout endpoint to reduce product quantities and log sales transactions in a single atomic step.

Implement POS order models and checkout logic that adjusts stock and creates sale transactions in SQLite.

1. Open `Backend/Backend.Core/Models/` and create `SaleOrder.cs` and `SaleOrderItem.cs`.
2. Add `SaleOrders` and `SaleOrderItems` DbSets in `AppDbContext.cs`.
3. Add EF Core migration for sales schema.
4. Create `PosService.cs` in `Backend/Backend.DesktopHost/Services/`:
    - Accept list of cart items and payment details.
    - Check stock availability for each item.
    - Reduce product total quantity.
    - Create `ProductTransaction` entries with `TransactionType.Sale`.
    - Record payment details.
    - Commit all changes in an atomic database transaction.
5. Create `PosController.cs` in `Backend/Backend.DesktopHost/Controllers/` with `POST /api/Pos/checkout`.

#### Things to Consider

- Roll back all changes if any item has insufficient stock.
- Support FEFO/FIFO batch deduction rules when reducing batch quantities.

### Task 6: Build Vanilla HTML/JS Point of Sale Interface in `wwwroot`

Users cannot scan/select items, manage an active cart, or complete sales from the test interface.

Add a POS interface section in `index.html` and implement cart management in `app.js`.

1. Open `Backend/Backend.DesktopHost/wwwroot/index.html`.
2. Add a "Point of Sale" fieldset:
    - Product selector or SKU quick-add input.
    - Cart item table showing item name, unit price, quantity input, subtotal, and remove button.
    - Cart summary section showing Total Due and Item Count.
    - Checkout button.
3. Open `Backend/Backend.DesktopHost/wwwroot/js/app.js`:
    - Maintain an in-memory `cart` array.
    - Add functions `addToCart(productId)`, `updateCartQuantity(productId, qty)`, and `removeFromCart(productId)`.
    - Implement `checkoutCart()` to post the order to `/api/Pos/checkout`.
    - Refresh product list and audit log after checkout completes.
4. Verify complete sale flow in browser:
    - Add product to cart.
    - Complete sale.
    - Confirm product stock decreases in the product table.
    - Confirm sale appears in the audit log.

#### Things to Consider

- Prevent adding more items than available in current stock.
- Clear cart and display receipt summary on successful checkout.

## Next Step(s)

1. Review and prioritize Task 1 (`ProductTransactionsController` query endpoints).
2. Follow up with Task 2 to provide immediate visibility into stock movement logs.
3. Implement payment models and cart checkout services (Tasks 3 through 6).

## Affected File(s)

1. `Backend/Backend.Core/Common/PaymentMethod.cs`
2. `Backend/Backend.Core/Data/AppDbContext.cs`
3. `Backend/Backend.Core/Models/PaymentRecord.cs`
4. `Backend/Backend.Core/Models/SaleOrder.cs`
5. `Backend/Backend.Core/Models/SaleOrderItem.cs`
6. `Backend/Backend.DesktopHost/Controllers/PaymentController.cs`
7. `Backend/Backend.DesktopHost/Controllers/PosController.cs`
8. `Backend/Backend.DesktopHost/Controllers/ProductTransactionController.cs`
9. `Backend/Backend.DesktopHost/Services/PaymentService.cs`
10. `Backend/Backend.DesktopHost/Services/PosService.cs`
11. `Backend/Backend.DesktopHost/wwwroot/index.html`
12. `Backend/Backend.DesktopHost/wwwroot/js/app.js`
13. `Documentation/roadmap.md`

