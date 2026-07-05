# EZBM Code Style Guidelines

This document outlines the coding standards, conventions, and style guidelines for the EZBM project. These styles should be strictly adhered to when modifying or adding code to the codebase.

## Core General Rules

* Always use strict and clear types where you can.
* Make sure your code handles nulls safely, but don't overdo it with unnecessary checks.
* Follow the style of the file or code you are editing. If there is no style yet, just use standard C# coding styles.
* If a line of code is getting too long (over 80 characters), break it up by assigning parts of the expression to simple local variables first.
* Only do this if the line would have otherwise been **over 80 characters long** (including whitespace).
* You don't need to do this for logging or print statements.
* *Example:*

  ```csharp
  // Avoid
  var average = (num1 + num2 + num3) / 3;

  // Prefer
  float sum = num1 + num2 + num3;
  float average = sum / 3;
  ```

## Language & Syntax Style (C#)

### Type Declarations

Avoid using `var`. Always write out the type names for variables.

  ```csharp
  // Avoid
  var staff = await context.Staff.FindAsync(id);
  var message = "Hello";

  // Prefer
  Staff? staff = await context.Staff.FindAsync(id);
  string message = "Hello";
  ```

Use `new()` instead of repeating the type name when the type is already clearly declared on the left.

  ```csharp
  using AppDbContext context = new();
  Utils.RequestResult successResult = new(Utils.Result.Success, message);
  ```

### Namespaces

Always declare namespaces at the top of the file without using curly braces.

  ```csharp
  namespace EZBM.Core.Services;
  ```

### Null & Pattern Matching

Use `is null` and `is not null` instead of `== null` and `!= null` when checking for null.

  ```csharp
  if (staff is null) { ... }
  if (request is not null) { ... }
  ```

### Switch Expressions

Use switch expressions to keep your mapping and conversion logic short and clean.

  ```csharp
  return result.Type switch
  {
      Result.Success => Results.Ok(result.Data),
      Result.Failed_NoResults => Results.NotFound(result.Message),
      _ => Results.Problem(result.Message)
  };
  ```

## Formatting & Layout

### Brace Style (Allman)

Place opening curly braces on a new line, aligned with the statement (Allman style).

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

### Blank Line Spacing

* Put a blank line after your `using` statements, and another one after the namespace.
* Put a blank line between the end of a `try` block and the start of a `catch` block.

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

* Use empty lines to separate different parts of your code inside a method (like separating validation checks from database changes).

### Wrapping & Line Breaks

* If a class, method, or record has a long list of parameters that goes over 80 characters, put each parameter on a new line.

    ```csharp
    public record UpdateStaffRequest(
        [Required] ulong Id,
        string? Username,
        float? PayRate
    );
    ```

* If a method has only one parameter but the line is still close to 80 characters, wrap that parameter to its own line.

    ```csharp
    public static async Task<Utils.RequestResult> CreateSaleAsync(
        CreateSaleRequest request
    )
    ```

* When creating new objects with many parameters that go over 80 characters, put each parameter on its own line and name them.

  ```csharp
  Staff staff = new(
      id: Utils.GenerateEntityId(),
      username: request.Username,
      payRate: request.PayRate
  );
  ```

* Put a line break before the dot (`.`) and indent when chaining multiple method calls (like LINQ queries).

  ```csharp
  Sale? sale = await context.Sale
      .Include(s => s.Staff)
      .FirstOrDefaultAsync(s => s.Id == request.Id);
  ```

### Single-Line If Statements

* For simple checks or assignments, you can leave out curly braces if the statement fits on one line.

  ```csharp
  if (!enabled) return;
  if (id == 0) continue;
  ```

* You can also omit braces if there is only one logical statement under a condition, even if it wraps across multiple lines.

  ```csharp
  if (request.StaffId is not null) query = query.Where(a => a.Staff.Id == request.StaffId);
  ```

## Member Organization & Regions

* Group related code in your classes using `#region` and `#endregion` tags.
* Leave a blank line right after a `#region` starts, and another blank line right before it ends.

  ```csharp
  #region Staff Requests

  public static async Task CreateStaffAsync(...) { ... }

  #endregion
  ```

## Naming Conventions

* Use PascalCase for class names, interface names, properties, and methods.
* Use camelCase for parameters and local variables.
* Add `async` to the end of any method name that runs asynchronously and returns a `Task`.
* Always end request record names with `Request`.

## Documentation Style (XML Comments)

All public classes, methods, and fields must have XML comments.

* Include summary, parameter, and return descriptions where needed.

  ```xml
  /// <summary>
  /// Gets a staff profile by username.
  /// </summary>
  ```

## Logging & Error Handling

* Use our custom logging classes:
  * `Log.Me(message)` for regular info logs.
  * `Log.Warn(message)` for warning logs.
  * `Log.Err(message)` for error logs.
* When creating API responses, save the result to a variable (like `successResult` or `failResult`) first instead of returning the new object directly. This keeps the return statement simple and readable.
* Avoid writing the same message string multiple times. Store it in a `message` variable, log it, and then use that variable in the response.

  ```csharp
  string message = "Staff member clocked in successfully.";
  Log.Me(message);
  Utils.RequestResult successResult = new(Utils.Result.Success, message);
  return successResult;
  ```
