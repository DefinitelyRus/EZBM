using System.Net;
using System.Net.Http.Json;
using Backend.Core.Common;
using Backend.Core.Models;
using Backend.DesktopHost.DTOs;

namespace Backend.ApiTests.Modules;

/// <summary>
/// Endpoint tests for products, stock batches, and transaction history queries.
/// </summary>
public static class ProductsAndStockTests
{
    #region Test Execution

    /// <summary>
    /// Runs all product, stock, and product transaction endpoint tests.
    /// </summary>
    public static async Task RunAllAsync(
        TestRunner runner,
        HttpClient client
    )
    {
        runner.PrintSectionHeader(
            "Products, Stock & Transactions (/api/Products, /api/ProductStocks, /api/ProductTransactions)"
        );

        long createdProductId = 0;
        long createdTransactionId = 0;

        await runner.ExecuteTestAsync(
            "POST /api/Products",
            "Creates a new physical product and expects HTTP 201 Created.",
            async () =>
            {
                Product newProduct = new()
                {
                    Name = "Iced Milk Tea 500ml",
                    ShortDescription = "Classic wintermelon milk tea",
                    Cost = 45.00m,
                    Price = 95.00m,
                    Sku = "MT-WM-500",
                    IsForSale = true,
                    TotalQuantity = 0m,
                    QuantityType = QuantityType.Pieces
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "/api/Products",
                    newProduct
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.Created
                );

                Product? created =
                    await response.Content.ReadFromJsonAsync<Product>();
                TestRunner.AssertTrue(
                    created is not null && created.Id > 0,
                    "Created product ID was not returned."
                );

                createdProductId = created!.Id;
                return $"Created product '{created.Name}' with ID {createdProductId}.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/Products and GET /api/Products/{id}",
            "Fetches the product list and looks up the newly created product by ID.",
            async () =>
            {
                HttpResponseMessage listRes =
                    await client.GetAsync("/api/Products");
                await TestRunner.AssertStatusCodeAsync(
                    listRes,
                    HttpStatusCode.OK
                );

                List<Product>? products =
                    await listRes.Content.ReadFromJsonAsync<List<Product>>();
                TestRunner.AssertTrue(
                    products is not null &&
                    products.Any(p => p.Id == createdProductId),
                    "Created product was missing from GET /api/Products."
                );

                HttpResponseMessage itemRes = await client.GetAsync(
                    $"/api/Products/{createdProductId}"
                );
                await TestRunner.AssertStatusCodeAsync(
                    itemRes,
                    HttpStatusCode.OK
                );

                Product? fetched =
                    await itemRes.Content.ReadFromJsonAsync<Product>();
                TestRunner.AssertEqual(
                    "Iced Milk Tea 500ml",
                    fetched!.Name,
                    "Product Name"
                );

                return "Product list and single-product lookup returned expected data.";
            }
        );

        await runner.ExecuteTestAsync(
            "POST /api/ProductStocks/create-stock",
            "Adds 25 units of stock for the product and expects a purchase transaction.",
            async () =>
            {
                CreateStockRequest request = new()
                {
                    ProductId = createdProductId,
                    Quantity = 25m,
                    Cost = 42.00m
                };

                HttpResponseMessage response = await client.PostAsJsonAsync(
                    "/api/ProductStocks/create-stock",
                    request
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.Created
                );

                ProductTransaction? tx = await response.Content
                    .ReadFromJsonAsync<ProductTransaction>();
                TestRunner.AssertTrue(
                    tx is not null && tx.Id > 0,
                    "Stock creation did not return a valid transaction."
                );
                TestRunner.AssertEqual(25m, tx!.Quantity, "Transaction Quantity");

                createdTransactionId = tx.Id;
                return $"Added 25 units of stock (Transaction ID {createdTransactionId}).";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/ProductStocks/{productId}",
            "Verifies total quantity and batch records after adding stock.",
            async () =>
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"/api/ProductStocks/{createdProductId}"
                );
                await TestRunner.AssertStatusCodeAsync(
                    response,
                    HttpStatusCode.OK
                );

                GetStockResponse? stock = await response.Content
                    .ReadFromJsonAsync<GetStockResponse>();
                TestRunner.AssertTrue(
                    stock is not null,
                    "Stock response body was empty."
                );
                TestRunner.AssertEqual(25m, stock!.TotalQuantity, "TotalQuantity");
                TestRunner.AssertTrue(
                    stock!.Batches is not null && stock.Batches.Count >= 1,
                    "Expected at least one product batch."
                );

                return "Stock query confirmed 25 units available across 1 batch.";
            }
        );

        await runner.ExecuteTestAsync(
            "GET /api/ProductTransactions (Paginated & Filtered) and GET /{id}",
            "Queries transaction history with pagination/date filters and fetches a single record.",
            async () =>
            {
                string from = Uri.EscapeDataString(
                    DateTime.UtcNow.AddHours(-1).ToString("O")
                );
                string url =
                    $"/api/ProductTransactions?page=1&pageSize=10" +
                    $"&productId={createdProductId}&from={from}";

                HttpResponseMessage listRes = await client.GetAsync(url);
                await TestRunner.AssertStatusCodeAsync(
                    listRes,
                    HttpStatusCode.OK
                );

                List<ProductTransactionDto>? list = await listRes.Content
                    .ReadFromJsonAsync<List<ProductTransactionDto>>();
                TestRunner.AssertTrue(
                    list is not null &&
                    list.Any(t => t.Id == createdTransactionId),
                    "Paginated transaction query did not include the new transaction."
                );

                HttpResponseMessage singleRes = await client.GetAsync(
                    $"/api/ProductTransactions/{createdTransactionId}"
                );
                await TestRunner.AssertStatusCodeAsync(
                    singleRes,
                    HttpStatusCode.OK
                );

                ProductTransactionDto? single = await singleRes.Content
                    .ReadFromJsonAsync<ProductTransactionDto>();
                TestRunner.AssertTrue(
                    single is not null,
                    "Single transaction response was empty."
                );
                TestRunner.AssertEqual(
                    "Purchase",
                    single!.TransactionType,
                    "TransactionType"
                );

                return "Transaction history pagination, filtering, and ID lookup succeeded.";
            }
        );

        await runner.ExecuteTestAsync(
            "DELETE /api/Products/{id} (Disposable Product)",
            "Creates and deletes a temporary product to verify HTTP 204 No Content.",
            async () =>
            {
                Product tempProduct = new()
                {
                    Name = "Disposable Test Item",
                    Cost = 10m,
                    Price = 20m,
                    Sku = "TEMP-DEL",
                    IsForSale = false
                };

                HttpResponseMessage createRes = await client.PostAsJsonAsync(
                    "/api/Products",
                    tempProduct
                );
                await TestRunner.AssertStatusCodeAsync(
                    createRes,
                    HttpStatusCode.Created
                );

                Product? created =
                    await createRes.Content.ReadFromJsonAsync<Product>();
                HttpResponseMessage delRes = await client.DeleteAsync(
                    $"/api/Products/{created!.Id}"
                );
                await TestRunner.AssertStatusCodeAsync(
                    delRes,
                    HttpStatusCode.NoContent
                );

                return $"Temporary product ID {created.Id} was created and deleted cleanly.";
            }
        );
    }

    #endregion
}
