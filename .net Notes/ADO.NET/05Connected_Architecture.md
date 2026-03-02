
# 03 — Connected Architecture in ADO.NET

---

## 🔷 What is Connected Architecture?

**Connected Architecture** is a model where your application **keeps the database connection open** for the entire duration of a data operation — reading, inserting, updating, or deleting.

> 💡 Think of it like a **live phone call** — you stay on the line while you exchange information. The moment you hang up (close the connection), the conversation is over.

---

## 🔷 How It Works — Step by Step

```
┌──────────────────────────────────────────────────────────┐
│              CONNECTED ARCHITECTURE FLOW                 │
│                                                          │
│  Step 1 ──→  Open Connection      SqlConnection.Open()   │
│      ↓                                                   │
│  Step 2 ──→  Write SQL Query      string query = "..."   │
│      ↓                                                   │
│  Step 3 ──→  Create Command       new SqlCommand(q, con) │
│      ↓                                                   │
│  Step 4 ──→  Execute Command      .ExecuteReader()       │
│      ↓                            .ExecuteNonQuery()     │
│      ↓                            .ExecuteScalar()       │
│  Step 5 ──→  Process Results      reader.Read() loop     │
│      ↓                                                   │
│  Step 6 ──→  Close Connection     automatic (using block)│
└──────────────────────────────────────────────────────────┘
```

> ⚠️ The connection is **open the entire time** from Step 1 to Step 6.

---

## 🔷 Key Classes in Connected Architecture

```
┌───────────────────────────────────────────────────────┐
│               CONNECTED ARCHITECTURE                  │
│                                                       │
│  SqlConnection  ── Opens / closes the DB connection   │
│       ↓                                               │
│  SqlCommand     ── Holds and sends your SQL query     │
│       ↓                                               │
│  SqlDataReader  ── Reads result rows one by one       │
│                                                       │
│  SqlTransaction ── Groups multiple commands (optional)│
└───────────────────────────────────────────────────────┘
```

| Class              | Role                                             |
| ------------------ | ------------------------------------------------ |
| `SqlConnection`  | Opens the door to the database                   |
| `SqlCommand`     | Sends your SQL instruction through that door     |
| `SqlDataReader`  | Reads the rows coming back — forward-only, fast |
| `SqlTransaction` | Makes multiple operations all-or-nothing         |

---

## 🔷 Key Characteristics

| Characteristic | Detail                                                              |
| -------------- | ------------------------------------------------------------------- |
| Connection     | Stays**open** throughout the operation                        |
| Data Access    | **Real-time** — live from the database                       |
| Reading        | `SqlDataReader` is **forward-only** and **read-only** |
| Speed          | ⚡ Fast — no in-memory copying                                     |
| Memory         | 🟢 Low — one row in memory at a time                               |
| Scalability    | 🔴 Lower — connection held open                                    |

---

## 🔷 Execution Methods at a Glance

| Method                | Returns                   | Use For                       |
| --------------------- | ------------------------- | ----------------------------- |
| `ExecuteReader()`   | `SqlDataReader`         | SELECT — multiple rows       |
| `ExecuteNonQuery()` | `int` (rows affected)   | INSERT, UPDATE, DELETE        |
| `ExecuteScalar()`   | `object` (single value) | COUNT, MAX, SUM, single field |

---

## 🔷 When to Use Connected Architecture

✅ **Use it when:**

- You need **live, real-time data** (login, live dashboards)
- You're doing a **single quick operation** (insert one row, count rows)
- You're reading a **large result set** and don't want it all in memory
- You need **fast sequential reading** of many rows

❌ **Avoid it when:**

- You need to **edit data offline** → use Disconnected
- You're building a **high-traffic scalable web app** → connections held too long
- You need to **pass data across layers** → DataReader needs open connection

---

## 🔷 Complete Code Example — Connected Architecture

```csharp
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ConnectedDemo {
    class Program {
        static string cs = @"Server=.\SQLEXPRESS; Database=EmployeeDB; Trusted_Connection=True; TrustServerCertificate=True;";

        static void Main(string[] args) {
            ShowAllEmployees();
            GetEmployeeCount();
            GetEmployeeById(1);
        }

        // ── READ multiple rows — ExecuteReader + SqlDataReader ─────────
        static void ShowAllEmployees() {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                string query = "SELECT Id, Name, Role, Salary FROM Employee";

                using (SqlCommand cmd = new SqlCommand(query, con)) {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        Console.WriteLine($"\n{"ID",-5} {"Name",-20} {"Role",-20} {"Salary",-10}");
                        Console.WriteLine(new string('-', 60));

                        while (reader.Read()) {
                            Console.WriteLine(
                                $"{reader["Id"],-5} " +
                                $"{reader["Name"],-20} " +
                                $"{reader["Role"],-20} " +
                                $"{reader["Salary"],-10}");
                        }
                    }
                }
            }
        }

        // ── READ single value — ExecuteScalar ──────────────────────────
        static void GetEmployeeCount() {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employee", con)) {
                    int count = (int)cmd.ExecuteScalar();
                    Console.WriteLine($"\nTotal Employees: {count}");
                }
            }
        }

        // ── READ one row by ID — with parameter ────────────────────────
        static void GetEmployeeById(int id) {
            using (SqlConnection con = new SqlConnection(cs)) {
                con.Open();
                string query = "SELECT Id, Name, Role, Salary FROM Employee WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, con)) {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            Console.WriteLine($"\nFound: {reader["Name"]} — {reader["Role"]}");
                        } else {
                            Console.WriteLine("\nEmployee not found.");
                        }
                    }
                }
            }
        }
    }
}
```

---

## 🔷 Connection State Best Practice

```csharp
if (con.State == ConnectionState.Closed) {
    con.Open();
}
```

---

## 🔷 Note on `GO` and `@` in SQL

- **`GO`** = "Send this batch of SQL to the server now and start a fresh batch."
  - It's a SQL Server Management Studio (SSMS) command — **not valid in C# strings**.
- **`@`** before a path string = "Treat backslashes as literal characters."
  ```csharp
  // These are identical:
  string cs1 = "Server=.\\SQLEXPRESS";   // escaped backslash
  string cs2 = @"Server=.\SQLEXPRESS";   // verbatim string — easier to read
  ```

---

## 🔷 Connected vs Disconnected — Visual Comparison

```
CONNECTED                          DISCONNECTED
───────────────────────────        ───────────────────────────
Open Connection                    Open Connection
      ↓                                  ↓
Execute Query                      Fetch → Fill DataTable
      ↓                                  ↓
Read Row by Row      vs.           Close Connection ← (key!)
      ↓                                  ↓
Process Each Row                   Edit Data in Memory
      ↓                                  ↓
Close Connection                   Reopen → Save → Close
```

---

## 🔷 One-Line Summary

> **Connected Architecture keeps a live connection open while reading data row-by-row — it's fast and memory-efficient but less scalable. Best for real-time, single operations.**

---

## ⭐ Interview Quick-Fire

| Question                              | Answer                                       |
| ------------------------------------- | -------------------------------------------- |
| Main reading class in connected mode? | `SqlDataReader`                            |
| Is SqlDataReader forward-only?        | ✅ Yes                                       |
| Is SqlDataReader read-only?           | ✅ Yes                                       |
| How to advance to next row?           | `reader.Read()`                            |
| Return type of ExecuteReader?         | `SqlDataReader`                            |
| Return type of ExecuteNonQuery?       | `int` — rows affected                     |
| Return type of ExecuteScalar?         | `object` — single value                   |
| Why less scalable?                    | Connection stays open, blocking DB resources |
