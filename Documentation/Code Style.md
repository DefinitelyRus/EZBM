# EZBM Code Style Guidelines

This document outlines the coding standards, conventions, and style guidelines for the EZBM project. These styles should be strictly adhered to when modifying or adding code to the codebase.

---

## 1. Core General Rules

* **Strict Types:** Always use strict and explicit types where applicable.
* **Null Safety:** Ensure the code is null-safe (with nullable reference types enabled `?`), but avoid doing so excessively.
* **Follow Existing Style:** Follow the existing style of the file or component you are editing. If none exists, default to standard C# patterns.
* **Expression Splitting (80-Char Rule):**
  * When assigning values or passing arguments, split long expressions into smaller, easy-to-read assignments to dedicated local variables.
  * Only do this if the line would have otherwise been **over 80 characters long** (including whitespace).
  * **Exception:** Do not apply this rule to logging or printing statements.
  * *Example:*

    ```csharp
    // Avoid
    var average = (num1 + num2 + num3) / 3;

    // Prefer
    float sum = num1 + num2 + num3;
    float average = sum / 3;
    ```

---

## 2. Language & Syntax Style (C#)

### 2.1. Type Declarations

* **Prefer Explicit Types:** Avoid using `var`. Explicitly declare type names for all variables.

  ```csharp
  // Avoid
  var staff = await context.Staff.FindAsync(id);
  var message = "Hello";

  // Prefer
  Staff? staff = await context.Staff.FindAsync(id);
  string message = "Hello";
  ```

* **Target-Typed New Expressions:** Use target-typed `new()` when the type is explicitly specified on the left-hand side.

  ```csharp
  using AppDbContext context = new();
  Utils.RequestResult successResult = new(Utils.Result.Success, message);
  ```

### 2.2. Namespaces

* **File-Scoped Namespaces:** Always use file-scoped namespace declarations (without curly braces) at the top of the file.

  ```csharp
  namespace EZBM.Core.Services;
  ```

### 2.3. Null & Pattern Matching

* **Pattern Matching Null Checks:** Prefer `is null` and `is not null` operators over `== null` and `!= null`.

  ```csharp
  if (staff is null) { ... }
  if (request is not null) { ... }
  ```

### 2.4. Switch Expressions

* Use pattern-matching switch expressions for concise mappings and conversions.

  ```csharp
  return result.Type switch
  {
      Result.Success => Results.Ok(result.Data),
      Result.Failed_NoResults => Results.NotFound(result.Message),
      _ => Results.Problem(result.Message)
  };
  ```

---

## 3. Formatting & Layout

### 3.1. Brace Style (Allman)

* Use the **Allman** brace style where opening curly braces are placed on a new line, aligned with the statement.

  ```csharp
  public static async Task<Staff?> GetStaffAsync(GetStaffRequest request)
  {
      try
      {
          // ...
      }
      catch (Exception ex)
      {
          // ...
      }
  }
  ```

### 3.2. Blank Line Spacing

* **Namespace & Imports:** Place one blank line after using statements, and one blank line after the namespace declaration.
* **Try-Catch Blocks:** Place a blank line between the closing brace of a `try` block and the beginning of the `catch` statement.

  ```csharp
  try
  {
      await context.SaveChangesAsync();
  }

  catch (Exception ex)
  {
      // Catch block code
  }
  ```

* **Logical Spacing:** Use single empty lines to separate logically distinct blocks of code inside a method (e.g., separating database context creation, validations, mapping, database changes, and returns).

### 3.3. Wrapping & Line Breaks

* **Method Parameters & Arguments:**
  * When a method declaration or record contains multiple parameters, place each parameter on its own line.

    ```csharp
    public record UpdateStaffRequest(
        [Required] ulong Id,
        string? Username,
        float? PayRate
    );
    ```

  * Even for single-parameter methods, if the signature is long (approaching 80 characters), wrap the parameter to its own line.

    ```csharp
    public static async Task<Utils.RequestResult> CreateSaleAsync(
        CreateSaleRequest request
    )
    ```

* **Constructor Arguments:** When instantiating classes or records with many properties, place each argument on its own line and use **named arguments**.

  ```csharp
  Staff staff = new(
      id: Utils.GenerateEntityId(),
      username: request.Username,
      payRate: request.PayRate
  );
  ```

* **Method Chaining:** Wrap before the dot (`.`) and indent for chained calls (like EF Core LINQ operators).

  ```csharp
  Sale? sale = await context.Sale
      .Include(s => s.Staff)
      .FirstOrDefaultAsync(s => s.Id == request.Id);
  ```

### 3.4. Single-Line If Statements

* For simple guard clauses or single assignments, curly braces may be omitted if the statement fits on a single line.

  ```csharp
  if (!enabled) return;
  if (id == 0) continue;
  ```

* Even if formatted on multiple lines (e.g. LINQ query chains), braces can be omitted if there is only a single logical statement under the condition.

  ```csharp
  if (request.StaffId is not null)
      query = query.Where(a => a.Staff.Id == request.StaffId);
  ```

---

## 4. Member Organization & Regions

* Group class members logically (e.g., request type, entity type, action type) using `#region` and `#endregion` tags.
* Keep one blank line inside the region immediately after it starts and one immediately before it ends.

  ```csharp
  #region Staff Requests

  public static async Task CreateStaffAsync(...) { ... }

  #endregion
  ```

---

## 5. Naming Conventions

* **PascalCase:**
  * Class names, Interface names, Structs, Enums, Properties, Methods, and Public Fields (e.g., `StaffService`, `LoginAsync`, `AppDbContext`, `PayFrequency`).
* **camelCase:**
  * Method parameters and local variables (e.g., `request`, `message`, `successResult`).
* **Asynchronous Methods:** Always end asynchronous methods returning `Task` with the `Async` suffix (e.g., `GetStaffByUsernameAsync`).
* **Data Transfer Objects (DTOs):** Suffix request records with `Request` (e.g., `FindItemRequest`, `LoginRequest`).

---

## 6. Documentation Style (XML Comments)

All public classes, methods, and fields must have XML comments.

* Include `<summary>`, `<param>`, and `<returns>` tags where appropriate.
* Document authorship and tools using italicized HTML metadata tags at the end of the summary.

  ```xml
  /// <summary>
  /// Gets a staff profile by username.
  /// <br/><br/>
  /// <i>Author(s): DefinitelyRus<br/>
  /// Editor(s): None<br/>
  /// Documented by: Google Antigravity</i>
  /// </summary>
  ```

---

## 7. Logging & Error Handling

* **Logging Tool:** Use the custom `Log` utility classes:
  * `Log.Me(message)`: Standard informational trace logging.
  * `Log.Warn(message)`: Warnings that do not halt execution.
  * `Log.Err(message)`: Operational or unhandled errors.
* **Return Value Optimization:**
  * When preparing API responses via `RequestResult`, assign the result object to a local variable (e.g., `successResult`, `failResult`, `errorResult`) before returning it, rather than returning the constructor directly. This keeps constructor invocations clear and separate from the return statement.
  * Avoid duplicate string construction by storing messages in a local `string message;` variable, logging it, and then passing it directly to the response object.

  ```csharp
  string message = "Staff member clocked in successfully.";
  Log.Me(message);
  Utils.RequestResult successResult = new(Utils.Result.Success, message);
  return successResult;
  ```
