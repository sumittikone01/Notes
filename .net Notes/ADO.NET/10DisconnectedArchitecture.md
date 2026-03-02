
# 04 — Disconnected Architecture in ADO.NET

---

## 🔷 What is Disconnected Architecture?

**Disconnected Architecture** is a model where your application **fetches data from the database, immediately closes the connection**, stores the data in memory, and works with it offline. Only when saving changes does it reconnect.

> 💡 Think of it like a **WhatsApp message** — you send your message, the other person goes offline, reads it, replies, and sends back. Neither of you stays on an open call the whole time.

---

## 🔷 How It Works — Step by Step

```
┌──────────────────────────────────────────────────────────────┐
│             DISCONNECTED ARCHITECTURE FLOW                    │
│                                                              │
│  Step 1 ──→  Open Connection     SqlDataAdapter.Fill()       │
│      ↓        (auto-managed)                                 │
│  Step 2 ──→  Fetch Data          da.Fill(dt)                 │
│      ↓                                                       │
│  Step 3 ──→  Close Connection    (automatic after Fill)      │
│      ↓                                                       │
│  Step 4 ──→  Work Offline        Edit rows in DataTable      │
│      ↓        (no DB needed)     Add / Modify / Delete rows  │
│  Step 5 ──→  Reopen Connection   da.Update(dt)               │
│      ↓        (auto-managed)                                 │
│  Step 6 ──→  Save Changes        Synced to SQL Server        │
│      ↓                                                       │
│  Step 7 ──→  Close Connection    (automatic after Update)    │
└──────────────────────────────────────────────────────────────┘
```

> ✅ Notice: `SqlDataAdapter` **opens and closes the connection automatically** during `Fill()` and `Update()`. You don't call `con.Open()` manually.

---

## 🔷 Key Classes in Disconnected Architecture

```
┌──────────────────────────────────────────────────────┐
│            DISCONNECTED ARCHITECTURE                 │
│                                                      │
│  SqlDataAdapter   ── The "smart bridge"              │
│       ↓              Fetches data, saves changes     │
│  DataSet          ── In-memory mini-database         │
│       ↓              Holds multiple DataTables       │
│  DataTable        ── Single table in memory          │
│       ↓              Rows + Columns                  │
│  DataRow          ── One record inside DataTable     │
│                                                      │
│  SqlCommandBuilder ── Auto-generates CRUD SQL cmds   │
└──────────────────────────────────────────────────────┘
```

| Class                 | Role                                             |
| --------------------- | ------------------------------------------------ |
| `SqlDataAdapter`    | Fetches into memory AND saves changes back to DB |
| `DataSet`           | In-memory container for multiple tables          |
| `DataTable`         | Single table stored in RAM                       |
| `DataRow`           | One row of a DataTable                           |
| `SqlCommandBuilder` | Auto-generates INSERT/UPDATE/DELETE commands     |

---

## 🔷 Key Characteristics

| Characteristic | Detail                                                     |
| -------------- | ---------------------------------------------------------- |
| Connection     | **Closes** right after fetching data                 |
| Data Access    | **Snapshot** — not live                             |
| Navigation     | **Full** — forward and backward                     |
| Mode           | **Read + Write** — add, edit, delete rows in memory |
| Memory         | 🔴 Higher — full result set in RAM                        |
| Scalability    | ✅ Higher — connection not held open                      |

---

## 🔷 Row State Tracking — How the Adapter Knows What to Save

The `DataTable` **automatically tracks the state of every row**:

| RowState      | When It Happens                | Action on `da.Update()` |
| ------------- | ------------------------------ | ------------------------- |
| `Unchanged` | Just loaded from DB            | Nothing — skip           |
| `Added`     | `dt.Rows.Add(newRow)` called | Runs**INSERT**      |
| `Modified`  | `row["Col"] = value` changed | Runs**UPDATE**      |
| `Deleted`   | `row.Delete()` called        | Runs**DELETE**      |

```csharp
DataRow row = dt.Rows[0];
Console.WriteLine(row.RowState);  // Unchanged

row["Salary"] = 99000;
Console.WriteLine(row.RowState);  // Modified
```

---

## 🔷 When to Use Disconnected Architecture

✅ **Use it when:**

- Building **Kendo Grid / data grid** features
- Working with **reports or dashboards**
- **High-traffic web apps** — connections freed instantly
- Passing data **across application layers** (DataTable is portable)
- You need to **edit multiple rows** before saving

❌ **Avoid it when:**

- You need **live real-time data**
- You only need a **single quick value** → use `ExecuteScalar`
- **Memory is constrained** and the table is very large

---

## 🔷 Complete Code Example — Disconnected Architecture

```csharp
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DisconnectedDemo {
    class EmployeeManager {
        static string cs = @"Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; TrustServerCertificate=True;";

        public static void Main(string[] args) {
            FetchAndDisplay();
            UpdateSalaryDisconnected(1, 95000.00m);
            DeleteEmployeeDisconnected(5);
        }

        // ── READ — Fill DataTable and display ─────────────────────────
        public static void FetchAndDisplay() {
            using (SqlConnection con = new SqlConnection(cs)) {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Employee", con);
                DataTable dt = new DataTable();

                da.Fill(dt); // Opens, fetches, CLOSES connection automatically

                Console.WriteLine($"\n{"Id",-5} {"Name",-20} {"Salary",-10}");
                Console.WriteLine(new string('-', 40));

                foreach (DataRow row in dt.Rows) {
                    Console.WriteLine($"{row["Id"],-5} {row["Name"],-20} {row["Salary"],-10}");
                }
            }
        }

        // ── UPDATE — Modify in memory, then sync to DB ─────────────────
        public static void UpdateSalaryDisconnected(int empId, decimal newSalary) {
            using (SqlConnection con = new SqlConnection(cs)) {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Employee", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da); // auto-generates UPDATE SQL

                DataTable dt = new DataTable();
                da.Fill(dt); // Connection opens, fills, CLOSES automatically

                // Work offline — find the row and change it
                foreach (DataRow row in dt.Rows) {
                    if ((int)row["Id"] == empId) {
                        row["Salary"] = newSalary; // RowState becomes Modified
                        Console.WriteLine($"Modified {row["Name"]} salary in memory...");
                        break;
                    }
                }

                da.Update(dt); // Opens, saves Modified rows, CLOSES
                Console.WriteLine("✅ Database updated successfully.");
            }
        }

        // ── DELETE — Mark row deleted, then sync to DB ─────────────────
        public static void DeleteEmployeeDisconnected(int empId) {
            using (SqlConnection con = new SqlConnection(cs)) {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Employee", con);
                SqlCommandBuilder builder = new SqlCommandBuilder(da);

                DataTable dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++) {
                    if ((int)dt.Rows[i]["Id"] == empId) {
                        dt.Rows[i].Delete(); // RowState becomes Deleted
                        Console.WriteLine($"Marked ID {empId} for deletion...");
                        break;
                    }
                }

                da.Update(dt); // Opens, runs DELETE for marked rows, CLOSES
                Console.WriteLine("🗑️ Row removed from database.");
            }
        }
    }
}
```

---

## 🔷 Connected vs Disconnected — Complete Comparison

|                | Connected         | Disconnected                  |
| -------------- | ----------------- | ----------------------------- |
| Connection     | Open throughout   | Closes after fetch            |
| Main class     | `SqlDataReader` | `DataTable` / `DataSet`   |
| Bridge class   | `SqlCommand`    | `SqlDataAdapter`            |
| Can edit data  | ❌ No             | ✅ Yes                        |
| Memory use     | Low               | Higher                        |
| Scalability    | Lower             | Higher                        |
| Real-time data | ✅ Yes            | ❌ Snapshot                   |
| Use for        | Login, live reads | Kendo grids, reports, editing |

---

## 🔷 One-Line Summary

> **Disconnected Architecture fetches a snapshot into DataTable/DataSet via SqlDataAdapter, closes the connection immediately, allows full offline editing, then syncs changes back — making it ideal for scalable web apps and data grids.**

---

## ⭐ Interview Quick-Fire

| Question                               | Answer                   |
| -------------------------------------- | ------------------------ |
| Bridge class in disconnected mode?     | `SqlDataAdapter`       |
| Method to fetch data into memory?      | `da.Fill(dt)`          |
| Method to save changes back to DB?     | `da.Update(dt)`        |
| Does Fill() need manual con.Open()?    | No — adapter handles it |
| Auto-generates SQL commands?           | `SqlCommandBuilder`    |
| How does adapter know what SQL to run? | Via `DataRow.RowState` |
| RowState when you change a value?      | `Modified`             |
| RowState when you call row.Delete()?   | `Deleted`              |
