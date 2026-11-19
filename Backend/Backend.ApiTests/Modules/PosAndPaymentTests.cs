using System.Net;
using System.Net.Http.Json;
using Backend.Core.Common;
using Backend.Core.Data;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.ApiTests.Modules;

/// <summary>
/// Endpoint tests for POS checkout workflows and payment processing.
/// </summary>
public static class PosAndPaymentTests
{
    #region Test Execution

    /// <summary>
    /// Runs all POS checkout and payment endpoint tests.
    /// </summary>
    public static async Task RunAllAsync(
        TestRunner runner,
        HttpClient client,
        TestWebApplicationFactory factory
    )
    {
        runner.PrintSectionHeader(
            "Point of Sale & Payments (/api/Pos, /api/Payments)"
        );

        long productId = 0;
        long serviceId = 0;

        await runner.ExecuteTestAsync(
            "POST /api/Pos/checkout (Physical Product)",
            "Checks out 3 units of a stocked product and verifies stock reduction and change.",
            async () =>
            {
                Product product = new()
                {
                    Name = "Bottled Water 500ml",
                    Cost = 10.00m,
                    Price = 20.00m,
                    Sku = "BW-500",
                    IsForSale = true,
                    TotalQuantity = 0m
                };

                HttpResponseMessage createRes = await client.PostAsJsonAsync(
                    "/api/Products",
                    product
                );
                await TestRunner.AssertStatusCodeAsync(
                    createRes,
                    HttpStatusCode.Created
                );

                Product? created =
                    await createRes.Content.ReadFromJsonAsync<Product>();
                productId = created!.Id;

                CreateStockRequest stockReq = new()
                {
                    ProductId = productId,
                    Quantity = 10m,
                    Cost = 10.00m
                };
                HttpResponseMessage stockRes = await client.PostAsJsonAsync(
                    "/api/ProductStocks/create-stock",
                    stockReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    stockRes,
                    HttpStatusCode.Created
                );

                CheckoutRequest checkoutReq = new(
                    Items: [new CartItemRequest(productId, "Product", 3m)],
                    AmountTendered: 100.00m,
                    PaymentMethod: PaymentMethod.Cash,
                    ReferenceNote: "POS product test",
                    UserId: 1
                );

                HttpResponseMessage checkoutRes = await client.PostAsJsonAsync(
                    "/api/Pos/checkout",
                    checkoutReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    checkoutRes,
                    HttpStatusCode.OK
                );

                CheckoutResponse? result = await checkoutRes.Content
                    .ReadFromJsonAsync<CheckoutResponse>();
                TestRunner.AssertTrue(
                    result is not null,
                    "Checkout response was empty."
                );
                TestRunner.AssertEqual(60.00m, result!.TotalAmount, "TotalAmount");
                TestRunner.AssertEqual(40.00m, result.ChangeAmount, "ChangeAmount");

                HttpResponseMessage getStockRes = await client.GetAsync(
                    $"/api/ProductStocks/{productId}"
                );
                GetStockResponse? updatedStock = await getStockRes.Content
                    .ReadFromJsonAsync<GetStockResponse>();
                TestRunner.AssertEqual(
                    7m,
                    updatedStock!.TotalQuantity,
                    "Remaining Stock"
                );

                return "Product checkout succeeded: total 60.00, change 40.00, stock reduced from 10 to 7.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/Pos/checkout (Service Item)",
            "Checks out a service item with unlimited stock and verifies no stock check blocks it.",
            async () =>
            {
                using (IServiceScope scope = factory.Services.CreateScope())
                {
                    AppDbContext db = scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();
                    Service service = new()
                    {
                        Name = "Phone Repair Diagnostic",
                        Cost = 0m,
                        Price = 150.00m,
                        Sku = "SRV-DIAG",
                        IsForSale = true,
                        QuantityType = QuantityType.Unlimited
                    };
                    await db.Services.AddAsync(service);
                    await db.SaveChangesAsync();
                    serviceId = service.Id;
                }

                CheckoutRequest checkoutReq = new(
                    Items: [new CartItemRequest(serviceId, "Service", 2m)],
                    AmountTendered: 300.00m,
                    PaymentMethod: PaymentMethod.DigitalWallet,
                    ReferenceNote: "Service checkout test",
                    UserId: 1
                );

                HttpResponseMessage checkoutRes = await client.PostAsJsonAsync(
                    "/api/Pos/checkout",
                    checkoutReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    checkoutRes,
                    HttpStatusCode.OK
                );

                CheckoutResponse? result = await checkoutRes.Content
                    .ReadFromJsonAsync<CheckoutResponse>();
                TestRunner.AssertEqual(300.00m, result!.TotalAmount, "TotalAmount");
                TestRunner.AssertEqual(0.00m, result.ChangeAmount, "ChangeAmount");
                TestRunner.AssertEqual(
                    "Service",
                    result.Items[0].ItemType,
                    "ItemType"
                );

                return "Service item checkout succeeded for 2 units totaling 300.00 without stock deduction.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/Pos/checkout (Insufficient Stock)",
            "Attempts to check out 100 units when only 7 remain and expects HTTP 409 Conflict.",
            async () =>
            {
                CheckoutRequest checkoutReq = new(
                    Items: [new CartItemRequest(productId, "Product", 100m)],
                    AmountTendered: 5000.00m,
                    PaymentMethod: PaymentMethod.Cash,
                    ReferenceNote: "Over-stock test",
                    UserId: 1
                );

                HttpResponseMessage checkoutRes = await client.PostAsJsonAsync(
                    "/api/Pos/checkout",
                    checkoutReq
                );
                await TestRunner.AssertStatusCodeAsync(
                    checkoutRes,
                    HttpStatusCode.Conflict
                );

                HttpResponseMessage getStockRes = await client.GetAsync(
                    $"/api/ProductStocks/{productId}"
                );
                GetStockResponse? stockAfter = await getStockRes.Content
                    .ReadFromJsonAsync<GetStockResponse>();
                TestRunner.AssertEqual(
                    7m,
                    stockAfter!.TotalQuantity,
                    "Stock After Rollback"
                );

                return "Insufficient stock was rejected with HTTP 409 and stock remained intact at 7.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/Payments/process (Valid Cash Payment)",
            "Processes a 150.00 payment with 200.00 tendered and checks change calculation.",
            async () =>
            {
                ProcessPaymentRequest request = new(
                    Amount: 150.00m,
                    AmountTendered: 200.00m,
                    Method: PaymentMethod.Cash,
                    ReferenceNote: "Cash counter 1"
                );

                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "/api/Payments/process",
                    request
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                ProcessPaymentResponse? result = await response.Content
                    .ReadFromJsonAsync<ProcessPaymentResponse>();
                TestRunner.AssertEqual(50.00m, result!.ChangeAmount, "ChangeAmount");

                return "Payment recorded successfully with 50.00 change returned.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/Payments/process (Insufficient Tendered Amount)",
            "Attempts to pay 150.00 with only 100.00 tendered and expects HTTP 400 Bad Request.",
            async () =>
            {
                ProcessPaymentRequest request = new(
                    Amount: 150.00m,
                    AmountTendered: 100.00m,
                    Method: PaymentMethod.Cash,
                    ReferenceNote: "Underpaid test"
                );

                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "/api/Payments/process",
                    request
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.BadRequest
                );

                return "Underpaid transaction was correctly rejected with HTTP 400.";
            }
        );
    }

    #endregion
}
