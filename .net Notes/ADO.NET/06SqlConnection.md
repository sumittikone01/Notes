
# 05 — SqlConnection & Connection Strings

---

## 🔷 What is SqlConnection?

* `SqlConnection` is the class that **opens and manages a connection to SQL Server**.
* It is the first step in any ADO.NET operation — nothing works until you have a connection open.

> 💡 Think of `SqlConnection` as the **door** to your database. You must open it to get in, and you must close it when you're done — or you'll block others from entering.

---

## 🔷 SqlConnection Properties

| Property              | Returns             | Example                                          |
| --------------------- | ------------------- | ------------------------------------------------ |
| `ConnectionString`  | `string`          | The connection string text                       |
| `ConnectionTimeout` | `int`             | Wait time in seconds (default: 15)               |
| `Database`          | `string`          | Current database name:`"EmployeeDB"`           |
| `DataSource`        | `string`          | Server name:`".\SQLEXPRESS"`                   |
| `ServerVersion`     | `string`          | SQL Server version:`"15.00.2000"`              |
| `State`             | `ConnectionState` | `Open`, `Closed`, `Connecting`, `Broken` |

```csharp
con.Open();
Console.WriteLine(con.Database);       // EmployeeDB
Console.WriteLine(con.DataSource);    // .\SQLEXPRESS
Console.WriteLine(con.ServerVersion); // 15.00.2000
Console.WriteLine(con.State);         // Open
```

---

## 🔷 SqlConnection Methods

| Method                   | Purpose                                                        |
| ------------------------ | -------------------------------------------------------------- |
| `Open()`               | Opens the connection                                           |
| `Close()`              | Closes the connection                                          |
| `Dispose()`            | Releases all resources (called by `using`)                   |
| `CreateCommand()`      | Shortcut — creates a new SqlCommand linked to this connection |
| `BeginTransaction()`   | Starts a database transaction                                  |
| `ChangeDatabase(name)` | Switches to a different database on the same server            |

---

## 🔷 One-Line Summary

> **`SqlConnection` is the gateway to SQL Server — always wrap it in a `using` block and store the connection string in `appsettings.json`, never hardcoded in production.**

---

## ⭐ Interview Quick-Fire

| Question                                         | Answer                                                                          |
| ------------------------------------------------ | ------------------------------------------------------------------------------- |
| What is a connection string?                     | Text with server, database, and auth info                                       |
| Best practice for SqlConnection?                 | Wrap in `using` block                                                         |
| What does `using` do?                          | Auto-calls `Close()` + `Dispose()` even on exceptions                       |
| Two auth methods?                                | Windows Auth (`Trusted_Connection`) and SQL Auth (`User Id` + `Password`) |
| Can you mix both auth methods?                   | ❌ No — throws error                                                           |
| Where to store connection string in MVC?         | `appsettings.json` → `ConnectionStrings` section                           |
| What is connection pooling?                      | Reusing existing connections instead of creating new ones                       |
| Default pool size?                               | 100 connections                                                                 |
| Difference: ConnectionTimeout vs CommandTimeout? | Connect wait vs query wait                                                      |
