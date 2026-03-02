
# 02 — Namespaces & Key Classes in ADO.NET

---

## 🔷 Why Namespaces Matter

Before using any ADO.NET class, you must **import its namespace** at the top of your file.
Namespaces are like **organized folders** — they tell C# where to find the classes you want to use.

> ⚠️ Without the correct `using` statement, Visual Studio will not find `SqlConnection`, `DataTable`, or any other ADO.NET class.

---

## 🔷 The Two Lines Every ADO.NET File Needs

```csharp
using System.Data;                 // Core — DataSet, DataTable, DataRow
using Microsoft.Data.SqlClient;   // SQL Server — SqlConnection, SqlCommand, etc.
```

These two lines are the **foundation** of every ADO.NET file you write.

---

## 🔷 Namespace 1 — `System.Data` (Core / Always Required)

This is the **heart of ADO.NET**. It contains classes used in **both** Connected and Disconnected architecture, as well as the shared interfaces.

| Class / Interface   | What It Does                                          |
| ------------------- | ----------------------------------------------------- |
| `DataSet`         | In-memory mini-database (holds multiple tables)       |
| `DataTable`       | Single table in memory (rows + columns)               |
| `DataRow`         | One record inside a DataTable                         |
| `DataColumn`      | Defines one column (name, type, constraints)          |
| `DataRelation`    | Parent-child relationship between two DataTables      |
| `CommandType`     | Enum:`Text`, `StoredProcedure`, `TableDirect`   |
| `ConnectionState` | Enum:`Open`, `Closed`, `Connecting`, `Broken` |
| `IDbConnection`   | Interface — base for all connection classes          |
| `IDbCommand`      | Interface — base for all command classes             |
| `IDataReader`     | Interface — base for all reader classes              |

---

## 🔷 Namespace 2 — `Microsoft.Data.SqlClient` ✅ (Modern — Always Use This)

This is the **SQL Server-specific namespace**. It contains the concrete classes that actually communicate with SQL Server.

| Class                 | What It Does                                            |
| --------------------- | ------------------------------------------------------- |
| `SqlConnection`     | Opens and manages a connection to SQL Server            |
| `SqlCommand`        | Sends SQL queries or stored procedures to the DB        |
| `SqlDataReader`     | Reads query results row-by-row (connected mode)         |
| `SqlDataAdapter`    | Bridge: fetches data into memory and saves changes back |
| `SqlParameter`      | Safely passes values into SQL to prevent injection      |
| `SqlTransaction`    | Manages transactions (commit / rollback)                |
| `SqlCommandBuilder` | Auto-generates INSERT/UPDATE/DELETE commands            |
| `SqlException`      | Exception thrown when SQL Server returns an error       |
| `SqlBulkCopy`       | Efficiently inserts thousands of rows at once           |

---

## 🔷 Legacy Namespace — `System.Data.SqlClient` ❌ (Old — Avoid)

```csharp
using System.Data.SqlClient;   // ❌ OLD — do not use for new projects
```

|                   | `Microsoft.Data.SqlClient` ✅ | `System.Data.SqlClient` ❌ |
| ----------------- | ------------------------------- | ---------------------------- |
| Maintained        | Yes — actively updated         | No — legacy                 |
| .NET Core support | Full                            | Limited                      |
| Azure AD auth     | Supported                       | Not supported                |
| New projects      | **Use this**              | Avoid                        |

---

## 🔷 Other Database Providers (Reference Only)

```csharp
using MySql.Data.MySqlClient;              // MySQL
using Npgsql;                              // PostgreSQL
using Oracle.ManagedDataAccess.Client;    // Oracle
```

> All providers follow the same ADO.NET pattern — only the class prefix changes (`MySqlConnection`, `NpgsqlConnection`, etc.).

---

## 🔷 Class Hierarchy — How Everything Relates

```
System.Data (Provider-agnostic core)
├── DataSet
├── DataTable
│     └── DataRow
│     └── DataColumn
├── IDbConnection      ← interface implemented by SqlConnection
├── IDbCommand         ← interface implemented by SqlCommand
├── IDataReader        ← interface implemented by SqlDataReader
└── CommandType (enum)

Microsoft.Data.SqlClient (SQL Server concrete classes)
├── SqlConnection      implements IDbConnection
├── SqlCommand         implements IDbCommand
├── SqlDataReader      implements IDataReader
├── SqlDataAdapter
├── SqlParameter
├── SqlTransaction
├── SqlCommandBuilder
└── SqlException
```

---

## 🔷 Installing the NuGet Package

`Microsoft.Data.SqlClient` must be installed in your ASP.NET Core MVC project:

```bash
# Package Manager Console
Install-Package Microsoft.Data.SqlClient

# .NET CLI
dotnet add package Microsoft.Data.SqlClient
```

---

## 🔷 Reading Connection String in ASP.NET Core MVC

**`appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EmployeeDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Repository class:**

```csharp
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class EmployeeRepository {
    private readonly string _cs;

    public EmployeeRepository(IConfiguration config) {
        _cs = config.GetConnectionString("DefaultConnection");
    }

    public void SomeMethod() {
        using (SqlConnection con = new SqlConnection(_cs)) {
            con.Open();
            // ... ADO.NET work here
        }
    }
}
```

---

## 🔷 Important Notes

- Namespaces **do not create connections** — they only give you access to classes
- The connection is created using a **Connection String** passed to `SqlConnection`
- The `using` keyword (for resource management) ensures connections are closed automatically

---

## 🔷 One-Line Summary

> **`System.Data` gives you the core in-memory data classes. `Microsoft.Data.SqlClient` gives you the SQL Server-specific classes. You need both in every ADO.NET file.**

---

## ⭐ Interview Quick-Fire

| Question                            | Answer                                              |
| ----------------------------------- | --------------------------------------------------- |
| Core ADO.NET namespace?             | `System.Data`                                     |
| SQL Server namespace (modern)?      | `Microsoft.Data.SqlClient`                        |
| Old SQL Server namespace?           | `System.Data.SqlClient` — avoid for new projects |
| Class for connecting?               | `SqlConnection`                                   |
| Class for executing SQL?            | `SqlCommand`                                      |
| Class for reading rows?             | `SqlDataReader`                                   |
| Class for in-memory table?          | `DataTable`                                       |
| Class for in-memory database?       | `DataSet`                                         |
| Class for safe parameters?          | `SqlParameter`                                    |
| Class for auto-generating commands? | `SqlCommandBuilder`                               |
