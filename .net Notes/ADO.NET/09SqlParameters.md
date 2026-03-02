
# 09 — SqlParameter & SQL Injection Prevention

---

## 🔷 What is SqlParameter?

* `SqlParameter` is a class used to **safely pass values into a SQL query or stored procedure**.
* Instead of building SQL by joining strings (dangerous), you create `@placeholders` in your SQL and fill them with `SqlParameter` objects.

> 💡 Think of it as a **sealed envelope** — you put your data inside. SQL Server opens it in a fully controlled, safe way. Any malicious content stays locked inside and is treated as plain data, never as SQL code.

---

## 🔷 The Problem — SQL Injection Attack

### What is SQL Injection?

SQL Injection is when a user types **malicious SQL code** into an input field, and your app accidentally **executes it as part of your query**.

### Example of the Attack

```csharp
// User types this into the "Name" field of your form:
string name = "'; DROP TABLE Employee; --";

// Your broken code builds this:
string query = "SELECT * FROM Employee WHERE Name = '" + name + "'";

// The actual SQL sent to SQL Server:
// SELECT * FROM Employee WHERE Name = ''; DROP TABLE Employee; --'
//                                         ↑↑↑ DISASTER! Table deleted!

SqlCommand cmd = new SqlCommand(query, con);
cmd.ExecuteNonQuery(); // Your Employee table is now GONE
```

**This is catastrophic.** The attacker's SQL runs as part of your query.

---

## 🔷 The Solution — Parameterized Queries ✅

```csharp
// Same attacker input...
string name = "'; DROP TABLE Employee; --";

// But with parameters:
string query = "SELECT * FROM Employee WHERE Name = @Name";

SqlCommand cmd = new SqlCommand(query, con);
cmd.Parameters.AddWithValue("@Name", name);

// SQL Server treats EVERYTHING in @Name as DATA — never as code.
// The attack becomes harmless. The table is safe.
```

**How it works internally:**

```
Your C#:      "SELECT * FROM Employee WHERE Id = @Id AND Name = @Name"
              @Id = 5, @Name = "Sumit"

SQL Server:   SELECT * FROM Employee WHERE Id = 5 AND Name = 'Sumit'
              @Id and @Name are NEVER treated as SQL code — only as data values.
```

---

## 🔷 The `@` Symbol

The `@` prefix in `@Name`, `@Id`, `@Salary` tells SQL Server:

> "This is a **placeholder** — wait for C# to provide the actual value."

- Must match **exactly** between SQL string and parameter name
- Case-insensitive: `@Name` and `@name` are the same
- Must always start with `@`

---

## 🔷 Why Use SqlParameter — Full Benefits

| Benefit               | Explanation                                                                     |
| --------------------- | ------------------------------------------------------------------------------- |
| **Security**    | Prevents SQL injection — input is always treated as data                       |
| **Type Safety** | C#`decimal` maps correctly to SQL `DECIMAL` — no guessing                  |
| **Performance** | SQL Server caches and reuses query plans (parameterized queries stay identical) |
| **Clean Code**  | No messy string concatenation with quotes and `+` signs                       |
| **Null Safety** | Standardized way to send `DBNull.Value` for nullable columns                  |

---

## 🔷 Three Ways to Add Parameters

### Method 1 — `AddWithValue()` (Quick, Common, Good for Learning)

```csharp
cmd.Parameters.AddWithValue("@Name",   "Sumit Tikone");
cmd.Parameters.AddWithValue("@Salary", 65000.00m);
cmd.Parameters.AddWithValue("@Id",     5);
```

- ✅ Short and easy to write
- ⚠️ SQL Server must **guess** the data type — can cause implicit conversion in some edge cases

---

### Method 2 — `Add()` with `SqlDbType` (Precise — Use in Production)

```csharp
cmd.Parameters.Add("@Name",   SqlDbType.NVarChar, 100).Value = "Sumit Tikone";
cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value       = 65000.00m;
cmd.Parameters.Add("@Id",     SqlDbType.Int).Value            = 5;
```

- ✅ You specify the exact SQL type — no guessing
- ✅ Better performance
- ✅ Recommended for production code

---

### Method 3 — Full `SqlParameter` Object (Maximum Control)

```csharp
SqlParameter param = new SqlParameter();
param.ParameterName = "@Name";
param.SqlDbType     = SqlDbType.NVarChar;
param.Size          = 100;
param.Value         = "Sumit Tikone";

cmd.Parameters.Add(param);
```

- Use when you need full control: `Direction`, `Precision`, `Scale`, `SourceColumn`

---

## 🔷 SqlParameter Properties

| Property          | Type                   | Purpose                                    |
| ----------------- | ---------------------- | ------------------------------------------ |
| `ParameterName` | `string`             | The `@Name` placeholder                  |
| `Value`         | `object`             | The actual data to pass                    |
| `SqlDbType`     | `SqlDbType` enum     | Explicit SQL Server data type              |
| `Size`          | `int`                | Max characters for string/binary           |
| `Direction`     | `ParameterDirection` | Input / Output / InputOutput / ReturnValue |
| `Precision`     | `byte`               | Total digits for decimals                  |
| `Scale`         | `byte`               | Decimal places for decimals                |
| `SourceColumn`  | `string`             | Maps to DataTable column (for DataAdapter) |
| `SourceVersion` | `DataRowVersion`     | Original or Current version of row         |

---

## 🔷 C# Type → SqlDbType Mapping

| C# Type      | SqlDbType                      | SQL Server Type      |
| ------------ | ------------------------------ | -------------------- |
| `int`      | `SqlDbType.Int`              | `INT`              |
| `string`   | `SqlDbType.NVarChar`         | `NVARCHAR`         |
| `decimal`  | `SqlDbType.Decimal`          | `DECIMAL`          |
| `DateTime` | `SqlDbType.DateTime`         | `DATETIME`         |
| `bool`     | `SqlDbType.Bit`              | `BIT`              |
| `double`   | `SqlDbType.Float`            | `FLOAT`            |
| `Guid`     | `SqlDbType.UniqueIdentifier` | `UNIQUEIDENTIFIER` |
| `byte[]`   | `SqlDbType.VarBinary`        | `VARBINARY`        |

---

## 🔷 Output Parameters — Getting Values Back from Stored Procedures

**SQL Server SP:**

```sql
CREATE PROCEDURE sp_InsertEmployee
    @Name    NVARCHAR(100),
    @Salary  DECIMAL(18,2),
    @NewId   INT OUTPUT         -- ← output parameter
AS
BEGIN
    INSERT INTO Employee(Name, Salary) VALUES(@Name, @Salary)
    SET @NewId = SCOPE_IDENTITY() -- returns the new ID
END
```

**C# Code:**

```csharp
using (SqlConnection con = new SqlConnection(cs)) {
    con.Open();

    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee", con)) {
        cmd.CommandType = CommandType.StoredProcedure;

        // Input parameters
        cmd.Parameters.AddWithValue("@Name",   "Sumit Tikone");
        cmd.Parameters.AddWithValue("@Salary", 65000.00m);

        // Output parameter — direction must be set
        SqlParameter outputParam = new SqlParameter("@NewId", SqlDbType.Int);
        outputParam.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(outputParam);

        cmd.ExecuteNonQuery();

        // Read output value AFTER execution
        int newId = (int)outputParam.Value;
        Console.WriteLine($"New Employee ID: {newId}");
    }
}
```

---

## 🔷 Handling NULL Values

```csharp
string middleName = null;

// ❌ Cannot pass C# null directly to SQL — use DBNull.Value
cmd.Parameters.AddWithValue("@MiddleName", DBNull.Value); // explicit NULL

// ✅ Conditional pattern (most readable)
cmd.Parameters.AddWithValue("@MiddleName",
    (object)middleName ?? DBNull.Value);
```

---

## 🔷 Complete CRUD Example with SqlParameter

```csharp
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace EmployeeManagement {
    class Program {
        static string cs = @"Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; TrustServerCertificate=True;";

        static void Main(string[] args) {
            Console.WriteLine("--- ADO.NET Parameters Demo ---");

            InsertEmployee("Amit Kumar", "Senior Developer", "amit@dev.com", 75000.50m);
            UpdateEmployeeSalary(1, 82000.00m);
            DeleteEmployee(5);
            ShowAllRecords();

            Console.ReadKey();
        }

        // ── INSERT ─────────────────────────────────────────────────────
        public static void InsertEmployee(string name, string role, string email, decimal salary) {
            using (SqlConnection con = new SqlConnection(cs)) {
                string sql = "INSERT INTO Employee(Name, Role, Email, Salary) VALUES(@Name, @Role, @Email, @Salary)";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    // Method 1: Full SqlParameter object
                    SqlParameter p1 = new SqlParameter("@Name", name);
                    cmd.Parameters.Add(p1);

                    // Method 2: AddWithValue — compact
                    cmd.Parameters.AddWithValue("@Role",   role);
                    cmd.Parameters.AddWithValue("@Email",  email);
                    cmd.Parameters.AddWithValue("@Salary", salary);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("✅ Record Inserted.");
                }
            }
        }

        // ── UPDATE ─────────────────────────────────────────────────────
        public static void UpdateEmployeeSalary(int id, decimal newSalary) {
            using (SqlConnection con = new SqlConnection(cs)) {
                string sql = "UPDATE Employee SET Salary = @NewSal WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    cmd.Parameters.AddWithValue("@NewSal", newSalary);
                    cmd.Parameters.AddWithValue("@Id",     id);

                    con.Open();
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0) {
                        Console.WriteLine($"✅ Employee ID {id} updated successfully.");
                    }
                }
            }
        }

        // ── DELETE ─────────────────────────────────────────────────────
        public static void DeleteEmployee(int id) {
            using (SqlConnection con = new SqlConnection(cs)) {
                string sql = "DELETE FROM Employee WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    int count = cmd.ExecuteNonQuery();
                    if (count > 0) {
                        Console.WriteLine($"🗑️ Employee ID {id} deleted.");
                    }
                }
            }
        }

        // ── READ ───────────────────────────────────────────────────────
        public static void ShowAllRecords() {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con)) {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            Console.WriteLine($"{reader["Id"]}\t{reader["Name"]}\t{reader["Salary"]}");
                        }
                    }
                }
            }
        }
    }
}
```

---

## 🔷 AddWithValue vs Add — Summary

|             | `AddWithValue`       | `Add` with `SqlDbType` |
| ----------- | ---------------------- | -------------------------- |
| Code        | Short                  | Slightly longer            |
| Type safety | SQL Server guesses     | You specify exactly        |
| Performance | Can be slower          | Better                     |
| Best for    | Learning / prototyping | Production code            |

---

## 🔷 One-Line Summary

> **`SqlParameter` prevents SQL injection by treating all input as data — never code — while giving you full control over data types, sizes, and output values from stored procedures.**

---

## ⭐ Interview Quick-Fire

| Question                             | Answer                                                  |
| ------------------------------------ | ------------------------------------------------------- |
| What is SQL injection?               | User input executed as SQL code                         |
| How do parameters prevent it?        | Input treated as data, never as SQL                     |
| Syntax for placeholder in SQL?       | `@ParameterName`                                      |
| Two ways to add parameters?          | `AddWithValue()` and `Add(name, SqlDbType)`         |
| How to pass null to SQL?             | `DBNull.Value`                                        |
| How to get output from SP?           | `ParameterDirection.Output`                           |
| Is AddWithValue safe from injection? | ✅ Yes — but `Add` with SqlDbType is better practice |
