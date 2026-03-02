# Connection String in ADO.NET

A **connection string** is a **text string** that contains information required to  **connect an application to a database** .

It tells :

- **Where** is the database server?
- **Which** database to use?
- **How** to authenticate (Windows login or SQL login)?

```csharp
string cs = @"Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; TrustServerCertificate=True;";
SqlConnection con = new SqlConnection(cs);
```

---

## 🔷 Connection String — All Keywords Explained

### 1️⃣ Server (Where is SQL Server?)

```text
Server=localhost;               ← Local machine, default instance
Server=.;                       ← Same as localhost
Server=.\SQLEXPRESS;            ← Local machine, named instance SQLEXPRESS
Server=DESKTOP-123\SQLEXPRESS;  ← Specific PC + named instance
Server=192.168.1.10;            ← Remote server by IP
Server=myserver.com,1433;       ← Remote server with custom port
```

> 📌 `Server` and `Data Source` are **identical** — both work.

---

### 2️⃣ Database (Which database?)

```text
Database=EmployeeDB;
Initial Catalog=EmployeeDB;     ← Same thing — both work
```

---

### 3️⃣ Authentication — Two Methods

#### ✅ Method A — Windows Authentication (Best for Development)

```text
Trusted_Connection=True;
```

or

```text
Integrated Security=True;       ← Same as Trusted_Connection
```

- Uses your **Windows login** — no username/password needed
- ❌ Do NOT use `User Id` / `Password` with this

**Example:**

```text
Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; Encrypt=True; TrustServerCertificate=True;
```

---

#### ✅ Method B — SQL Server Authentication (For Production / Remote)

```text
User Id=sa; Password=YourPassword;
```

- Uses a SQL Server login (username + password)
- ❌ Do NOT use `Trusted_Connection=True` with this

**Example:**

```text
Server=.\SQLEXPRESS; Database=EmployeeDB; User Id=sa; Password=123; Encrypt=True;
```

---

#### ❌ Never Mix Both — Will Cause Error

```text
Trusted_Connection=True; User Id=sa; Password=123;
```

🚫 **Invalid — throws exception**

---

### 4️⃣ Security Keywords

| Keyword                         | Purpose                                            |
| ------------------------------- | -------------------------------------------------- |
| `Encrypt=True`                | Encrypts data sent to SQL Server                   |
| `TrustServerCertificate=True` | Skips SSL certificate validation (use only in dev) |
| `Persist Security Info=False` | Prevents exposing credentials after connection     |

---

### 5️⃣ Timeout

|                              | Keyword                     | Where to Set         | Default    |
| ---------------------------- | --------------------------- | -------------------- | ---------- |
| **Connection Timeout** | `Connection Timeout=30`   | In connection string | 15 seconds |
| **Command Timeout**    | `cmd.CommandTimeout = 60` | On SqlCommand object | 30 seconds |

```csharp
// Command timeout — NOT in connection string
SqlCommand cmd = new SqlCommand(query, con);
cmd.CommandTimeout = 60; // Wait 60 seconds before giving up
```

---

### 6️⃣ Connection Pooling

* Pooling **reuses database connections** to improve performance.
* Pooling is a process of reusing expensive-to-create resources instead of repeatedly creating and destroying them, which significantly improves application performance and reduces resource overhead

```text
Pooling=True;           ← Default: ON
Min Pool Size=0;        ← Minimum connections to keep alive
Max Pool Size=100;      ← Maximum connections in pool (default: 100)
```

**How pooling works:**

```
First request:   con.Open()  → Creates new physical connection
                 con.Close() → Returns to POOL (not actually destroyed)

Second request:  con.Open()  → Reuses connection from POOL (fast!)
                 con.Close() → Back to pool again
```

> ✅ Always close connections (use `using` block) so they return to the pool and remain available for other requests.

---

## Complete Real Examples

### ✔ Windows Authentication (Best Practice)

```text
Server=localhost;
Database=EmployeeDB;
Trusted_Connection=True;
Encrypt=True;
TrustServerCertificate=True;
```

---

### ✔ SQL Authentication

```text
Server=localhost;
Database=EmployeeDB;
User Id=sa;
Password=123;
Encrypt=True;
```

---


## 🔷 Connection String in ASP.NET Core MVC (Correct Pattern)

**`appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> 📌 In JSON, backslashes must be **doubled** (`\\`) because `\` is a JSON escape character.

**Repository:**

```csharp
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class EmployeeRepository {
    private readonly string _cs;

    public EmployeeRepository(IConfiguration config) {
        _cs = config.GetConnectionString("DefaultConnection");
    }

    public int GetEmployeeCount() {
        using (SqlConnection con = new SqlConnection(_cs)) {
            con.Open();

            if (con.State == ConnectionState.Open) {
                Console.WriteLine($"Connected to: {con.Database}");
            }

            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employee", con)) {
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
```

**What does this Constructor  do?**

* This constructor uses dependency injection to access IConfiguration and retrieve the connection string from appsettings.json, storing it in a read-only variable for database operations.

**`Program.cs`:**

```csharp
builder.Services.AddScoped<EmployeeRepository>();
```

---

## 🔷 Complete Real Connection String Examples

```text
✅ Windows Auth (Development):
Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; Encrypt=True; TrustServerCertificate=True;

✅ SQL Auth (Production/Remote):
Server=192.168.1.10; Database=EmployeeDB; User Id=sa; Password=SecurePass; Encrypt=True; TrustServerCertificate=True;

✅ Local Machine Short Form:
Server=.; Database=EmployeeDB; Trusted_Connection=True; TrustServerCertificate=True;
```
