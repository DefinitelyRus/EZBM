# Actionable Task List

> This file outlines the immediate next steps for the EZBM prototype, focusing on solving the data handling mystery and fleshing out the backend services before moving to the frontend.

### `feat(db): implement database repository services`
**What:** Create service classes (e.g., `StaffService`, `InventoryService`, `SalesService`) that encapsulate Entity Framework Core DB operations.
**How:**
- Create a `Services` (or `Repositories`) folder in `EZBM.Core`.
- Inject `AppDbContext` into these services.
- Write strict-typed, null-safe methods for reading/writing data (e.g., `CreateStaffMember(string username, string role, ...)`, `GetAllItems()`, `LogAttendance(...)`).
- Prefer splitting long expressions inside these methods.
**Complete when:** The application has a clear, programmatic way to insert, read, and update data in SQLite without directly touching `AppDbContext` from the web API controllers.

### `feat(core): implement entity constructors and factory methods`
**What:** Add clear constructors or factory methods to the domain entities (like `Staff`, `Item`, `Sale`).
**How:** 
- Open the entity models in `EZBM.Core.Entities`.
- Add standard constructors that take required fields, making sure all non-nullable properties are properly initialized to avoid null-safety issues.
- If an entity is best created from a JSON DTO request (e.g., adding a staff member), create a factory method or a mapping utility.
**Complete when:** Entities can be instantiated cleanly and safely via `new Staff(...)` or `Staff.Create(...)` with all necessary properties assigned.

### `feat(api): build authentication endpoints`
**What:** Create the `POST /api/auth/login` endpoint.
**How:**
- Create an `AuthController` in `EZBM.DesktopHost`.
- Use the newly created `StaffService` to query the database and match the username and plaintext password.
- Return a simple user object or a 401 Unauthorized status.
**Complete when:** The API successfully accepts a login request and returns the appropriate response based on the DB records.

### `feat(api): build inventory endpoints`
**What:** Create `GET /api/items` and `POST /api/items`.
**How:**
- Create an `InventoryController`.
- Use the `InventoryService` to handle fetching items and adding new ones.
- Add basic validation before passing data to the service.
**Complete when:** You can fetch the list of items and add a new Product/Service via REST client/Swagger.

### `feat(api): build POS and attendance endpoints`
**What:** Create endpoints for completing sales (`POST /api/sales`) and clocking in/out (`POST /api/attendance`).
**How:**
- Create `SalesController` and `AttendanceController`.
- Hook them up to their respective services. Ensure transaction logic (e.g., reducing stock when a sale occurs) is handled in the service layer using `AppDbContext.SaveChanges()` in a single transaction.
**Complete when:** A sale can be recorded (deducting stock) and an attendance record can be appended via the API.

### `chore(config): setup default settings.json`
**What:** Create the root `settings.json` file mentioned in the PRD.
**How:**
- Create `settings.json` in the workspace root.
- Define basic schema properties (e.g., `{"storeName": "My Store", "currency": "PHP"}`).
**Complete when:** The file exists and is easily parseable.

### `init(frontend): bootstrap desktop-first React app`
**What:** Initialize the frontend codebase.
**How:**
- Run `npm create vite@latest . -- --template react-ts` inside the `Frontend` directory (or similar tool).
- Install Axios.
- Set up the basic layout.
**Complete when:** `npm run dev` in the frontend folder opens a default React page on `localhost`.