# Chapter 3 — ADO.NET + Stored Procedures for the Login System

### Writing the Database Layer That Powers Authentication

---

## Prerequisites

| What You Need                        | Your File                                              |
| ------------------------------------ | ------------------------------------------------------ |
| SqlConnection, SqlCommand            | `ADO.NET / 06SqlConnection.md` + `07SqlCommand.md` |
| SqlDataReader                        | `ADO.NET / 08SqlDataReader.md`                       |
| SqlParameters                        | `ADO.NET / 09SqlParameters.md`                       |
| CRUD with stored procedures          | `ADO.NET / 14.CRUDOperationsInDA.md`                 |
| Chapter 1 + Chapter 2 of this series | ✓                                                     |

**This chapter is your existing ADO.NET knowledge applied to a specific task.**
You already know how to call stored procedures and read results.
Here you write the exact stored procedures and ADO.NET code your login
system needs. Nothing is new in technique — only the purpose is new.

---

## 1. The Database Table

Before any stored procedures, you need the Users table.
This is the exact table from the plan shared earlier.

```sql
CREATE TABLE Users
(
    UserId         NVARCHAR(50)  PRIMARY KEY,  -- login username
    PasswordHash   NVARCHAR(255) NOT NULL,     -- hashed password (never plain text)
    FailedAttempts INT           DEFAULT 0,    -- counts wrong password tries
    IsLocked       BIT           DEFAULT 0     -- 1 = account blocked
);
```

### Column Purpose

| Column             | Type              | Purpose                                       |
| ------------------ | ----------------- | --------------------------------------------- |
| `UserId`         | `NVARCHAR(50)`  | The username — primary key, must be unique   |
| `PasswordHash`   | `NVARCHAR(255)` | Hashed password stored here, NEVER plain text |
| `FailedAttempts` | `INT`           | Counts how many wrong passwords in a row      |
| `IsLocked`       | `BIT`           | 0 = active, 1 = locked after 3 failures       |

---

## 2. Stored Procedures — All Four You Need

You will write four stored procedures in SQL Server. Test each one
in SSMS before writing any C# code.

---

### SP 1 — sp_ValidateLogin

This is the main one. Called every time someone submits the login form.

**What it does:** Finds the user, returns their data so C# can check the password
and lock status.

```sql
CREATE PROCEDURE sp_ValidateLogin
    @UserId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserId,
        PasswordHash,
        FailedAttempts,
        IsLocked
    FROM Users
    WHERE UserId = @UserId;
END
```

**Why it only takes @UserId and not @Password:**
Password comparison happens in **C# code**, not SQL.
You fetch the stored hash, then compare in C# using your hash function.
This is the correct pattern — Chapter 4 explains why.

---

### SP 2 — sp_IncrementFailedAttempts

Called when the password is **wrong**. Adds 1 to the counter.
Also locks the account if this was the 3rd failure.

```sql
CREATE PROCEDURE sp_IncrementFailedAttempts
    @UserId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Add 1 to the failed count
    UPDATE Users
    SET FailedAttempts = FailedAttempts + 1
    WHERE UserId = @UserId;

    -- Lock the account if 3 or more failures
    UPDATE Users
    SET IsLocked = 1
    WHERE UserId = @UserId
      AND FailedAttempts >= 3;
END
```

**Both UPDATEs run together.** After the first UPDATE adds 1,
the second UPDATE immediately checks if the count is now >= 3 and locks.
No need for two separate SP calls from C#.

---

### SP 3 — sp_ResetFailedAttempts

Called when login is **successful**. Clears the failure counter.

```sql
CREATE PROCEDURE sp_ResetFailedAttempts
    @UserId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET FailedAttempts = 0
    WHERE UserId = @UserId;
END
```

Simple. One line of logic. But important — without this, a user who
eventually gets their password right would still be at FailedAttempts = 2
and one more wrong try would lock them.

---

### SP 4 — sp_UnlockUser (Admin Use)

Called by an admin to manually unlock a locked account.

```sql
CREATE PROCEDURE sp_UnlockUser
    @UserId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET FailedAttempts = 0,
        IsLocked       = 0
    WHERE UserId = @UserId;
END
```

---

### SP 5 — sp_GetAllUsers (For Admin Kendo Grid)

Returns all users for the admin panel grid. Added now so you have it ready.

```sql
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserId,
        FailedAttempts,
        IsLocked
    FROM Users
    ORDER BY UserId;
END
```

Note: `PasswordHash` is NOT selected here — never send password hashes to the UI.

---

## 3. The C# Model — UserRecord

A simple class to hold the data returned by `sp_ValidateLogin`.

```csharp
// Models/UserRecord.cs
public class UserRecord
{
    public string UserId         { get; set; } = string.Empty;
    public string PasswordHash   { get; set; } = string.Empty;
    public int    FailedAttempts { get; set; }
    public bool   IsLocked       { get; set; }
}
```

This is just a plain C# class — same as any model you've already written.

---

## 4. The Repository Class — UserRepository

In your architecture, the ADO.NET code goes in a **Repository** class,
not directly in the controller. The controller stays clean.

```
Controller (LoginController)
    │
    └── calls → UserRepository
                    │
                    └── calls → SQL Server via ADO.NET (stored procedures)
```

```csharp
// Data/UserRepository.cs
using System.Data;
using System.Data.SqlClient;

public class UserRepository
{
    private readonly string _connStr;

    // Connection string injected from appsettings.json
    public UserRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("Default")!;
    }

    // ── Get user by ID (for login validation) ───────────────────────────
    public UserRecord? GetUserById(string userId)
    {
        UserRecord? user = null;

        using (var conn = new SqlConnection(_connStr))
        using (var cmd  = new SqlCommand("sp_ValidateLogin", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);

            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())   // Read() returns true if a row was found
                {
                    user = new UserRecord
                    {
                        UserId         = reader["UserId"].ToString()!,
                        PasswordHash   = reader["PasswordHash"].ToString()!,
                        FailedAttempts = Convert.ToInt32(reader["FailedAttempts"]),
                        IsLocked       = Convert.ToBoolean(reader["IsLocked"])
                    };
                }
            }
        }

        return user;   // null if user not found
    }

    // ── Increment failed attempts (and auto-lock at 3) ───────────────────
    public void IncrementFailedAttempts(string userId)
    {
        using (var conn = new SqlConnection(_connStr))
        using (var cmd  = new SqlCommand("sp_IncrementFailedAttempts", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    // ── Reset failed attempts on successful login ─────────────────────────
    public void ResetFailedAttempts(string userId)
    {
        using (var conn = new SqlConnection(_connStr))
        using (var cmd  = new SqlCommand("sp_ResetFailedAttempts", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    // ── Admin: unlock a user ──────────────────────────────────────────────
    public void UnlockUser(string userId)
    {
        using (var conn = new SqlConnection(_connStr))
        using (var cmd  = new SqlCommand("sp_UnlockUser", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    // ── Admin: get all users (for Kendo grid) ────────────────────────────
    public List<UserRecord> GetAllUsers()
    {
        var users = new List<UserRecord>();

        using (var conn = new SqlConnection(_connStr))
        using (var cmd  = new SqlCommand("sp_GetAllUsers", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new UserRecord
                    {
                        UserId         = reader["UserId"].ToString()!,
                        FailedAttempts = Convert.ToInt32(reader["FailedAttempts"]),
                        IsLocked       = Convert.ToBoolean(reader["IsLocked"])
                    });
                }
            }
        }

        return users;
    }
}
```

---

## 5. Registering the Repository in Program.cs

For the controller to receive `UserRepository` via Dependency Injection,
you register it in Program.cs. You already know DI from your API notes.

```csharp
// Program.cs — in the services section (before builder.Build())
builder.Services.AddScoped<UserRepository>();
```

**Why Scoped?** One instance per HTTP request. Login is a one-request operation.
Scoped is the right lifetime for anything that touches the database.

---

## 6. Handling DBNull — A Real Issue with IsLocked

SQL Server's `BIT` column can sometimes return `DBNull` if a row was inserted
without setting the value. Safe way to read it:

```csharp
// Safe IsLocked read — handles potential DBNull
IsLocked = reader["IsLocked"] != DBNull.Value
           && Convert.ToBoolean(reader["IsLocked"]),

// Safe int read
FailedAttempts = reader["FailedAttempts"] == DBNull.Value
                 ? 0
                 : Convert.ToInt32(reader["FailedAttempts"]),
```

Your `09SqlParameters.md` notes cover this pattern. Use it any time a column
could theoretically be null.

---

## 7. The Login Logic Flow in the Controller

Now you can see exactly how the LoginController will use UserRepository.
(Password hashing is `/* Chapter 4 */` for now — you'll fill it in next.)

```csharp
[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    // ── Step 1: Find user in database ────────────────────────────────
    UserRecord? user = _userRepo.GetUserById(model.UserId);

    if (user == null)
    {
        ModelState.AddModelError("", "Invalid user ID or password.");
        return View(model);
    }

    // ── Step 2: Check if account is locked ───────────────────────────
    if (user.IsLocked)
    {
        ModelState.AddModelError("", "Account is locked. Contact your administrator.");
        return View(model);
    }

    // ── Step 3: Verify password ── (Chapter 4 fills this in)
    bool passwordCorrect = /* PasswordHelper.Verify(model.Password, user.PasswordHash) */
                           false; // placeholder

    // ── Step 4: Handle wrong password ────────────────────────────────
    if (!passwordCorrect)
    {
        _userRepo.IncrementFailedAttempts(model.UserId);

        // Reload user to check if they just got locked
        var updated = _userRepo.GetUserById(model.UserId);
        if (updated != null && updated.IsLocked)
            ModelState.AddModelError("", "Too many failed attempts. Account is now locked.");
        else
            ModelState.AddModelError("", "Invalid user ID or password.");

        return View(model);
    }

    // ── Step 5: Login success ─────────────────────────────────────────
    _userRepo.ResetFailedAttempts(model.UserId);

    // ── Step 6: Create auth cookie ── (Chapter 5 fills this in)
    // await HttpContext.SignInAsync(...)

    // ── Step 7: Set session ── (Chapter 6 fills this in)
    // HttpContext.Session.SetString("UserId", user.UserId)

    return RedirectToAction("Index", "Home");
}
```

This is the **complete login flow** you read about in Chapter 1, now in actual code.
Each `// Chapter X fills this in` comment becomes real code as you read on.

---

## 8. Test Your Stored Procedures First

**Before touching C#, test every SP in SSMS.**
This saves hours of debugging.

```sql
-- Insert a test user (plain text password for now — Chapter 4 changes this)
INSERT INTO Users (UserId, PasswordHash, FailedAttempts, IsLocked)
VALUES ('sumit', 'test_hash_placeholder', 0, 0);

-- Test sp_ValidateLogin
EXEC sp_ValidateLogin @UserId = 'sumit';
-- Expected: returns one row with sumit's data

-- Test sp_IncrementFailedAttempts (run 3 times, then check IsLocked)
EXEC sp_IncrementFailedAttempts @UserId = 'sumit';
SELECT UserId, FailedAttempts, IsLocked FROM Users WHERE UserId = 'sumit';
-- After 3rd run: FailedAttempts=3, IsLocked=1

-- Test sp_ResetFailedAttempts
EXEC sp_ResetFailedAttempts @UserId = 'sumit';
SELECT UserId, FailedAttempts, IsLocked FROM Users WHERE UserId = 'sumit';
-- Expected: FailedAttempts=0

-- Test sp_UnlockUser
EXEC sp_UnlockUser @UserId = 'sumit';
SELECT UserId, FailedAttempts, IsLocked FROM Users WHERE UserId = 'sumit';
-- Expected: FailedAttempts=0, IsLocked=0
```

If the SPs work in SSMS, your C# code will work too.
If something is wrong, SQL is much easier to debug than C#.

---

## 9. Connection String in appsettings.json

You already have this from your ADO.NET notes. Same as always.

```json
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=LoginDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## Chapter Summary

| What                                  | How                                                            |
| ------------------------------------- | -------------------------------------------------------------- |
| **Users table**                 | `UserId`, `PasswordHash`, `FailedAttempts`, `IsLocked` |
| **sp_ValidateLogin**            | SELECT user row by UserId — password check happens in C#      |
| **sp_IncrementFailedAttempts**  | UPDATE +1, then lock if >= 3 — both in one SP                 |
| **sp_ResetFailedAttempts**      | UPDATE to 0 on successful login                                |
| **sp_UnlockUser**               | Admin use — reset attempts AND unlock                         |
| **UserRepository**              | ADO.NET code lives here, not in the controller                 |
| **Scoped lifetime**             | Register `UserRepository` as Scoped in Program.cs            |
| **Test in SSMS first**          | Always verify SPs before writing C#                            |
| **DBNull safety**               | Check before converting BIT and INT columns                    |
| **CommandType.StoredProcedure** | Required when calling SPs — same as your CRUD notes           |

---

## What's Next — Chapter 4

**Password Hashing and Security**

Your stored procedure stores `PasswordHash`, not the plain password.
Chapter 4 explains why storing plain text passwords is catastrophic,
what hashing is, how SHA256 works in C#, and why you should use
BCrypt instead — and gives you the exact `PasswordHelper` class
that fills in the `/* Chapter 4 */` placeholder you saw in the login controller.

---

*Chapter 3 of 8 — Sumit's Login System — ASP.NET Core MVC*
