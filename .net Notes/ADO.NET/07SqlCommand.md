# 07 — SqlCommand

---

## 🔷 What is SqlCommand?

* `SqlCommand` is the class that **sends your SQL instructions to the database**.
* Once you have a `SqlConnection` open, `SqlCommand` is what you use to tell SQL Server *what to do* — run a SELECT, INSERT, UPDATE, DELETE, or call a Stored Procedure.

> 💡 If `SqlConnection` is the phone line, `SqlCommand` is **the actual words you speak** into the phone.

---

## 🔷 Basic Syntax

```csharp
SqlCommand cmd = new SqlCommand("SQL query here", con);
```

---

## 🔷 Constructors

| Constructor                                                      | When to Use                                          |
| ---------------------------------------------------------------- | ---------------------------------------------------- |
| `SqlCommand()`                                                 | Empty — set properties manually later               |
| `SqlCommand(string sql)`                                       | Query only — assign connection separately           |
| `SqlCommand(string sql, SqlConnection con)`                    | ✅**Most used** — query + connection together |
| `SqlCommand(string sql, SqlConnection con, SqlTransaction tx)` | Inside a transaction                                 |

```csharp
// ✅ Most common form:
using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con)) {
    // ...
}

// Empty constructor style:
SqlCommand cmd = new SqlCommand();
cmd.CommandText = "SELECT * FROM Employee";
cmd.Connection  = con;
```

---

## 🔷 Important Properties

| Property           | Type                       | Purpose                                |
| ------------------ | -------------------------- | -------------------------------------- |
| `CommandText`    | `string`                 | The SQL query or stored procedure name |
| `CommandType`    | `CommandType` enum       | `Text` or `StoredProcedure`        |
| `Connection`     | `SqlConnection`          | The open connection to use             |
| `CommandTimeout` | `int`                    | Max seconds to wait (default: 30)      |
| `Parameters`     | `SqlParameterCollection` | Safe parameter values                  |
| `Transaction`    | `SqlTransaction`         | Associated transaction                 |

---

## 🔷 CommandType — The Most Important Property Decision

`CommandType` tells ADO.NET whether `CommandText` is raw SQL or a stored procedure name.

---

### `CommandType.Text` — Inline SQL (Default)

```csharp
string query = "SELECT * FROM Employee WHERE Id = @Id";

using (SqlCommand cmd = new SqlCommand(query, con)) {
    cmd.CommandType = CommandType.Text; // This is the DEFAULT — can be omitted
    cmd.Parameters.AddWithValue("@Id", 5);
    // ...
}
```

> When to use: Writing SQL directly in your C# code.

---

### `CommandType.StoredProcedure` — Calling a Stored Procedure

```csharp
using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeById", con)) {
    cmd.CommandType = CommandType.StoredProcedure; // ← REQUIRED — must set this
    cmd.Parameters.AddWithValue("@Id", 5);
    // ...
}
```

> When to use: The SQL logic already exists as a stored procedure in SQL Server.

---

### Text vs StoredProcedure — Side by Side

|                          | `CommandType.Text` | `CommandType.StoredProcedure`   |
| ------------------------ | -------------------- | --------------------------------- |
| `CommandText` contains | Full SQL query       | Just the SP name                  |
| SQL lives in             | C# code file         | SQL Server database               |
| Performance              | Good                 | Slightly better (SP pre-compiled) |
| Use for                  | Quick queries        | Complex business logic            |
| Must set CommandType?    | No (it's default)    | ✅ Yes — mandatory               |

---

## 🔷 The Three Execute Methods

```
Your SQL Goal
│
├── SELECT — multiple rows ────→  ExecuteReader()    returns SqlDataReader
│
├── INSERT / UPDATE / DELETE ──→  ExecuteNonQuery()  returns int (rows affected)
│
└── SELECT — single value ─────→  ExecuteScalar()    returns object (cast it)
    (COUNT, MAX, MIN, SUM, etc.)
```

---

### ExecuteReader() — SELECT multiple rows

```csharp
using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con)) {
    using (SqlDataReader reader = cmd.ExecuteReader()) {
        while (reader.Read()) {
            Console.WriteLine(reader["Name"]);
        }
    }
}
```

---

### ExecuteNonQuery() — INSERT / UPDATE / DELETE

```csharp
using (SqlCommand cmd = new SqlCommand(
    "INSERT INTO Employee(Name, Salary) VALUES(@Name, @Salary)", con)) {
    cmd.Parameters.AddWithValue("@Name",   "Sumit Tikone");
    cmd.Parameters.AddWithValue("@Salary", 65000);

    int rowsAffected = cmd.ExecuteNonQuery();
    Console.WriteLine($"{rowsAffected} row(s) inserted.");
}
```

---

### ExecuteScalar() — Single value

```csharp
using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employee", con)) {
    int count = (int)cmd.ExecuteScalar();
    Console.WriteLine($"Total: {count}");
}
```

---

## 🔷 CommandType.Text — Complete CRUD Example

```csharp
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SqlCommandDemo {
    class Program {
        static string cs = @"Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; TrustServerCertificate=True;";

        static void Main(string[] args) {
            InsertEmployee("Sumit Tikone", "Developer", "sumit@example.com", 65000);
            UpdateSalary(id: 1, newSalary: 70000);
            DeleteEmployee(id: 2);
            int total = GetCount();
            Console.WriteLine($"Total employees: {total}");
        }

        // ── INSERT ─────────────────────────────────────────────────────
        static void InsertEmployee(string name, string role, string email, decimal salary) {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                string sql = @"INSERT INTO Employee(Name, Role, Email, Salary)
                               VALUES(@Name, @Role, @Email, @Salary)";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    cmd.Parameters.AddWithValue("@Name",   name);
                    cmd.Parameters.AddWithValue("@Role",   role);
                    cmd.Parameters.AddWithValue("@Email",  email);
                    cmd.Parameters.AddWithValue("@Salary", salary);

                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"✅ Inserted — {rows} row(s) affected.");
                }
            }
        }

        // ── UPDATE ─────────────────────────────────────────────────────
        static void UpdateSalary(int id, decimal newSalary) {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                string sql = "UPDATE Employee SET Salary = @Salary WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    cmd.Parameters.AddWithValue("@Salary", newSalary);
                    cmd.Parameters.AddWithValue("@Id",     id);

                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"✅ Updated — {rows} row(s) affected.");
                }
            }
        }

        // ── DELETE ─────────────────────────────────────────────────────
        static void DeleteEmployee(int id) {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                string sql = "DELETE FROM Employee WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"🗑️ Deleted — {rows} row(s) affected.");
                }
            }
        }

        // ── COUNT (ExecuteScalar) ───────────────────────────────────────
        static int GetCount() {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employee", con)) {
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
```

---

## 🔷 CommandType.StoredProcedure — Example

**First create this in SQL Server:**

```sql
CREATE PROCEDURE sp_GetEmployeeById
    @Id INT
AS
BEGIN
    SELECT Id, Name, Role, Salary FROM Employee WHERE Id = @Id
END
```

**C# Code:**

```csharp
static void GetEmployeeBySP(int id) {
    using (SqlConnection con = new SqlConnection(cs)) {
        con.Open();

        using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeById", con)) {
            cmd.CommandType = CommandType.StoredProcedure; // ← Must set this

            cmd.Parameters.AddWithValue("@Id", id);

            using (SqlDataReader reader = cmd.ExecuteReader()) {
                if (reader.Read()) {
                    Console.WriteLine($"{reader["Name"]} — {reader["Salary"]}");
                }
            }
        }
    }
}
```

---

## 🔷 SqlCommand with Transaction

```csharp
using (SqlConnection con = new SqlConnection(cs)) {
    con.Open();
    SqlTransaction transaction = con.BeginTransaction();

    using (SqlCommand cmd = new SqlCommand(
        "INSERT INTO Employee(Name, Salary) VALUES(@Name, @Salary)",
        con,
        transaction)) {
        try {
            cmd.Parameters.AddWithValue("@Name",   "Amit Sharma");
            cmd.Parameters.AddWithValue("@Salary", 55000);
            cmd.ExecuteNonQuery();

            transaction.Commit();
            Console.WriteLine("✅ Transaction committed.");
        } catch (Exception ex) {
            transaction.Rollback();
            Console.WriteLine($"❌ Rolled back: {ex.Message}");
        }
    }
}
```

---

## 🔷 Best Practices

- ✅ Always use `using` block for `SqlCommand`
- ✅ Always use **parameterized queries** (never string concatenation)
- ✅ Open connection **just before** executing the command
- ✅ Set `CommandType = CommandType.StoredProcedure` when calling SPs
- ✅ Use `ExecuteScalar()` for single values, not `ExecuteReader()`

---

## 🔷 One-Line Summary

> **`SqlCommand` sends SQL queries or stored procedures to the database — choose `ExecuteReader()`, `ExecuteNonQuery()`, or `ExecuteScalar()` based on what you need back.**

---

## ⭐ Interview Quick-Fire

| Question                          | Answer                                                    |
| --------------------------------- | --------------------------------------------------------- |
| What does SqlCommand do?          | Sends SQL to the database for execution                   |
| Three execute methods?            | `ExecuteReader`, `ExecuteNonQuery`, `ExecuteScalar` |
| Which for SELECT multiple rows?   | `ExecuteReader()`                                       |
| Which for INSERT/UPDATE/DELETE?   | `ExecuteNonQuery()`                                     |
| Which for COUNT or MAX?           | `ExecuteScalar()`                                       |
| Default CommandType?              | `CommandType.Text`                                      |
| CommandText for StoredProcedure?  | Just the procedure name (no SQL)                          |
| Must you set CommandType for SPs? | ✅ Yes — mandatory                                       |
