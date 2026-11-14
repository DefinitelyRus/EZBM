# Tasks 261114-03

## Current Status

Phase 2 backend implementation delivers POS and transaction workflows. Phase 3 establishes single-user authentication and security safeguards. All front-end tasks are excluded.

## Tasks Summary

1. Implement Single-User Authentication Models and Password Hashing
2. Implement Authentication Service and Token / Session Verification
3. Implement Authentication API Endpoints
4. Verify Business Settings Backend Configuration

## Tasks

### Task 1: Implement Single-User Authentication Models and Password Hashing

The application needs backend domain models and secure credential storage for single-user authentication.

Create user authentication entity and password hashing service in `Backend.Core`.

1. Open `Backend/Backend.Core/Models/` and create `User.cs`:
    - Add properties for `Username`, `PasswordHash`, `Salt`, and `Role`.
2. Register `Users` DbSet in `Backend/Backend.Core/Data/AppDbContext.cs`.
3. Add cryptographic password hashing helper in `Backend/Backend.Core/Common/` using PBKDF2 (`Rfc2898DeriveBytes`) or BCrypt.
4. Add EF Core migration for the user entity.

```csharp
namespace Backend.Core.Models;

public class User : Entity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin";
}
```

#### Things to Consider

- Seed a default admin credential on initial startup if no user exists.
- Store random cryptographic salts per user.

### Task 2: Implement Authentication Service and Token / Session Verification

The application needs backend logic to authenticate credentials and verify ongoing sessions or tokens.

Create `AuthService.cs` in `Backend.DesktopHost/Services/`.

1. Add method `LoginAsync(string username, string password)` to verify password hash and generate an auth token/session key.
2. Add method `VerifySessionAsync(string token)` to validate active sessions.
3. Add method `UpdatePasswordAsync(string username, string oldPassword, string newPassword)` to change credentials.

#### Things to Consider

- Single-user mode means simple in-memory or database-backed active session tokens.
- Keep session validation lightweight for desktop use.

### Task 3: Implement Authentication API Endpoints

The API needs HTTP endpoints to handle login, session validation, and password changes.

Create `AuthController.cs` in `Backend.DesktopHost/Controllers/`.

1. Add `POST /api/Auth/login` accepting username and password, returning authentication token.
2. Add `GET /api/Auth/verify` to validate current token.
3. Add `POST /api/Auth/change-password` to update credentials.

```csharp
namespace Backend.DesktopHost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Login, verify, and change password actions
}
```

#### Things to Consider

- Return standard HTTP 401 Unauthorized on invalid credentials.

### Task 4: Verify Business Settings Backend Configuration

Business settings endpoints and dynamic database path resolution are already implemented in `SettingsController.cs` and `BusinessSettingsService.cs`.

Verify settings retrieval and update behavior to ensure compatibility with Phase 3 goals.

1. Review `Backend/Backend.DesktopHost/Controllers/SettingsController.cs`.
2. Ensure path validation verifies `.db` file extension and directory accessibility.
3. Confirm dynamic SQLite resolution loads default path `<user>/Documents/EZBM/business_data/data.db`.

## Next Step(s)

1. Implement Task 1 (User model and password hashing).
2. Implement Task 2 (Auth service and token management).
3. Implement Task 3 (Auth controller endpoints).
4. Verify Task 4 (Business settings validation).

## Affected File(s)

1. `Backend/Backend.Core/Common/PasswordHasher.cs`
2. `Backend/Backend.Core/Data/AppDbContext.cs`
3. `Backend/Backend.Core/Models/User.cs`
4. `Backend/Backend.DesktopHost/Controllers/AuthController.cs`
5. `Backend/Backend.DesktopHost/DTOs/AuthDtos.cs`
6. `Backend/Backend.DesktopHost/Services/AuthService.cs`
7. `Documentation/roadmap.md`
