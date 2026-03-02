
# 01 — Introduction to ADO.NET

---

## 🔷 What is ADO.NET?

* **ADO.NET** (ActiveX Data Objects .NET) is Microsoft's **built-in data access technology** for .NET applications.
* It gives your C# application a complete set of classes to **connect to a database, read data, insert records, update values, and delete rows** — all using plain SQL.

> 💡 Think of ADO.NET as the **translator** sitting between your C# code and SQL Server. Your code speaks C# & the database speaks SQL — ADO.NET bridges the two.

---

## 🔷 Key Points

- Developed by **Microsoft** as part of the .NET Framework
- Used to **connect applications with databases**
- Supports full **CRUD** — Create, Read, Update, Delete
- Works with **multiple databases**: SQL Server, Oracle, MySQL, PostgreSQL, MS Access, XML
- Core classes are in the **`System.Data`** namespace
- SQL Server classes are in **`Microsoft.Data.SqlClient`**

---

## 🔷 Where ADO.NET Fits in ASP.NET Core MVC

```
┌─────────────────────────────────────────────────┐
│             ASP.NET Core MVC App                │
│                                                 │
│   Browser  →  Controller  →  Repository         │
│                                  ↓              │
│                            ADO.NET Layer        │
│                    (SqlConnection, SqlCommand)   │
│                                  ↓              │
│                          SQL Server Database    │
└─────────────────────────────────────────────────┘
```

In a real MVC project, ADO.NET code lives in a **Data Access** or **Repository** class — never directly in a Controller or View.

* If DAL exists, ADO.NET code goes inside DAL.
* If DAL does not exist, Repository itself acts as DAL and contains ADO.NET code.
* Both designs are correct.

***What is Repository***

* **Repository is an abstraction layer that provides clean methods for data access using business objects.**
* It hides database details from upper layers.

---

## 🔷 ADO.NET Architecture — Two Models

ADO.NET has **two ways** to work with data:

```
┌──────────────────────────────────────────────────────────────┐
│                    ADO.NET Architecture                       │
│                                                              │
│   ┌──────────────────────┐    ┌──────────────────────────┐   │
│   │  CONNECTED MODEL     │    │   DISCONNECTED MODEL     │   │
│   │                      │    │                          │   │
│   │  SqlConnection       │    │  SqlDataAdapter          │   │
│   │  SqlCommand          │    │  DataSet                 │   │
│   │  SqlDataReader       │    │  DataTable               │   │
│   │                      │    │  DataRow                 │   │
│   │  Connection OPEN     │    │  Connection CLOSES       │   │
│   │  while reading data  │    │  after data is fetched   │   │
│   └──────────────────────┘    └──────────────────────────┘   │
└──────────────────────────────────────────────────────────────┘
```

---

## 🔷 Model 1 — Connected Architecture (Quick Summary)

- In  **Connected Architecture** a **continuous connection**  is maintained to db while performing database operations.
- Data is read **row by row** using `SqlDataReader`
- Best for: **live data, quick operations, login checks**

```
Open Connection → Execute Query → Read Row by Row → Close Connection
```

### Commonly Used Objects

* `Connection`
* `Command`
* `DataReader`

### Commonly Used Methods

* `Open()` – Opens the database connection
* `Close()` – Closes the connection
* `ExecuteReader()` – Reads data in forward-only mode
* `ExecuteNonQuery()` – Executes INSERT, UPDATE, DELETE
* `Read()` – Reads records one by one from DataReader

### Example Use Case

* Login authentication
* Reading live data

---

## 🔷 Model 2 — Disconnected Architecture (Quick Summary)

- In  **Disconnected Architecture** , data is **fetched once** from the database and stored in  **memory**, and the database connection is  **closed immediately** .
- Data lives in memory (`DataTable` / `DataSet`) — you edit it offline
- Best for: **reports, data grids (Kendo), scalable web apps**

```
Open → Fetch into Memory → Close → Edit Offline → Reopen → Save → Close
```

### Commonly Used Objects

* `DataSet`
* `DataTable`
* `DataAdapter`

### Commonly Used Methods

* `Fill()` – Fills DataSet/DataTable with data
* `Update()` – Updates changes back to database
* `AcceptChanges()` – Commits changes
* `RejectChanges()` – Discards changes
* `Clear()` – Clears data
* `GetChanges()` – Retrieves modified records
* `Merge()` – Merges data from another DataSet

### Example Use Case

* Reporting applications
* Data editing forms

---

## Connected vs Disconnected Architecture (Quick Comparison)

| Connected                | Disconnected                          |
| ------------------------ | ------------------------------------- |
| Continuous DB connection | Connection closed after fetching data |
| Uses DataReader          | Uses DataSet / DataTable              |
| Real-time data           | In-memory data                        |
| Less scalable            | Highly scalable                       |

---

| ADO.NET                                                           | Entity Framework                                    |
| ----------------------------------------------------------------- | --------------------------------------------------- |
| Low-level data access technology                                  | High-level ORM (Object Relational Mapper)           |
| Works directly with databases using SQL                           | Works with databases using C# objects               |
| Requires manual connection handling                               | Automatically manages database connections          |
| Supports connected (DataReader) and disconnected (DataSet) models | Works mainly in a disconnected manner via DbContext |
| SQL queries are written manually                                  | SQL is generated automatically                      |
| More code required                                                | Less code required                                  |
| Faster performance                                                | Slightly slower due to ORM overhead                 |
| Full control over database operations                             | Less control, more abstraction                      |
| Best suited for performance-critical applications                 | Best suited for rapid application development       |

> ✅ **Why learn ADO.NET?** Every senior .NET developer must know it. Real production apps often use it for stored procedures, bulk operations, and fine-tuned queries where EF is too slow or too abstract.

## Summary

* ADO.NET is the backbone of **database connectivity in .NET**
* Supports both **connected and disconnected** models
* Widely used in **real-world .NET applications**
* Essential for **ASP.NET and enterprise applications**

---

## 🔷 Commonly Used In

| Application Type     | Example                              |
| -------------------- | ------------------------------------ |
| Web Applications     | ASP.NET Core MVC, Web API            |
| Desktop Applications | Windows Forms, WPF                   |
| Console Applications | Scripts, batch jobs                  |
| Service Applications | Windows Services, Background Workers |

---

## 🔷 Supported Databases

| Database       | Namespace / Package                 |
| -------------- | ----------------------------------- |
| SQL Server     | `Microsoft.Data.SqlClient`        |
| MySQL          | `MySql.Data.MySqlClient`          |
| PostgreSQL     | `Npgsql`                          |
| Oracle         | `Oracle.ManagedDataAccess.Client` |
| OLE DB sources | `System.Data.OleDb`               |

> 📌 This entire course uses **SQL Server** with `Microsoft.Data.SqlClient`.

---

## 🔷 Connected vs Disconnected — Side by Side

|                | Connected         | Disconnected                |
| -------------- | ----------------- | --------------------------- |
| Connection     | Stays open        | Closes after fetch          |
| Primary class  | `SqlDataReader` | `DataSet` / `DataTable` |
| Bridge class   | `SqlCommand`    | `SqlDataAdapter`          |
| Data editing   | ❌ Read-only      | ✅ Full edit                |
| Memory use     | Low               | Higher                      |
| Scalability    | Lower             | Higher                      |
| Real-time data | ✅ Yes            | ❌ Snapshot                 |
| Best for       | Login, live reads | Kendo grids, reports        |

---

## 🔷 One-Line Summary

> **ADO.NET is the bridge between your C# code and SQL Server — it gives you full control to read, write, update, and delete data using raw SQL, with two models: Connected (live, fast) and Disconnected (offline, scalable).**

---

## ⭐ Interview Quick-Fire

| Question                         | Answer                                      |
| -------------------------------- | ------------------------------------------- |
| Full form of ADO?                | ActiveX Data Objects                        |
| Two architecture models?         | Connected and Disconnected                  |
| Core namespace?                  | `System.Data`                             |
| SQL Server namespace (modern)?   | `Microsoft.Data.SqlClient`                |
| Connected uses which reader?     | `SqlDataReader`                           |
| Disconnected uses which objects? | `DataAdapter`, `DataSet`, `DataTable` |
| ADO.NET faster than EF?          | Yes — no ORM overhead                      |
