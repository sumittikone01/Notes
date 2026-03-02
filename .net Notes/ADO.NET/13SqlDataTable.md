🔷 **DataTable Class in ADO.NET**

* A **DataTable** is an in-memory, tabular data structure that represents a single table from a relational database.
* It consists of **Rows and Columns** and allows you to store and manipulate data without maintaining a continuous connection to the database.
* It is a core part of the **`System.Data`** namespace and is heavily used in  **Disconnected Architecture** .
* While a `DataSet` can hold multiple tables, a `DataTable` is just  **one single table** .

---

📌 **Why DataTable Exists (Problem it Solves)**

* **Structured Storage:** It provides a way to store data in a strictly organized rows-and-columns format while offline.
* **Local Manipulation:** You can sort, filter, search, and edit data locally in RAM instead of repeatedly querying the SQL server.
* **Schema Definition:** It allows you to define column names, data types, and constraints (like Primary Keys) manually without needing a database.

---

📌 **Important Characteristics of DataTable**

* **Tabular Format:** It stores data specifically in a grid of rows and columns.
* **Schema & Constraints:** You can define rules like  **Primary Keys** ,  **Unique constraints** , and **Foreign Keys** directly on the table.
* **Serialization:** It can be easily converted to **XML or JSON** formats for sharing data between different systems.
* **Disconnected Use:** It is widely used to manipulate data retrieved from a database while the connection is closed.
* **Rich Methods:** It provides built-in functions for complex tasks like filtering, sorting, and merging data.

---

📌 **Commonly Used Properties**

* **`Columns`** : Gets the collection of `DataColumn` objects that define the table's structure (schema).
* **`Rows`** : Gets the collection of `DataRow` objects containing the actual data records.
* **`TableName`** : Gets or sets a specific name for the table (useful when used inside a `DataSet`).
* **`Constraints`** : Holds the collection of rules like `PrimaryKey` or `ForeignKey`.
* **`PrimaryKey`** : An array of `DataColumn` objects that uniquely identify each row.

---

📌 **Commonly Used Methods**

* **`NewRow()`** : Creates a new, blank `DataRow` that follows the table's specific structure.
* **`Rows.Add()`** : Takes a new row and adds it to the table's data collection.
* **`Clear()`** : Removes all data rows from the table while keeping the structure intact.
* **`Copy()`** : Creates an exact duplicate of the table, including its structure and all data.
* **`AcceptChanges()`** : Commits all pending changes (Inserts/Updates) and marks the rows as "unchanged".
* **`RejectChanges()`** : Rolls back all modifications made since the last time changes were accepted.

---

📌 **Basic Example (Creating and Adding Data)**

**C#**

```
using System;
using System.Data;

namespace DataTableDemo {
    class Program {
        static void Main() {
            // 1. Initialize the Table
            DataTable dt = new DataTable("Employees");

            // 2. Define the Columns (The Schema)
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Role", typeof(string));

            // 3. Set a Primary Key
            dt.PrimaryKey = new DataColumn[] { dt.Columns["ID"] };

            // 4. Add a Row using NewRow()
            DataRow row1 = dt.NewRow();
            row1["ID"] = 1;
            row1["Name"] = "Sumit Tikone";
            row1["Role"] = "Software Developer";
            dt.Rows.Add(row1);

            // 5. Short way to add a row
            dt.Rows.Add(2, "Amit Sharma", "QA Engineer");

            // 6. Display the data
            foreach (DataRow row in dt.Rows) {
                Console.WriteLine($"{row["ID"]}\t{row["Name"]}\t{row["Role"]}");
            }
        }
    }
}
```

---

📌 **Memory Structure Overview**

* **DataTable**
  * **Data Column Collection** (Defines "ID", "Name", etc.)
  * **Data Row Collection** (The actual values)
  * **Constraint Collection** (Primary Key rules)

---

📌 **Important Points to Remember (Interview + Practical)**

* `DataTable` represents **one single table** in memory.
* It belongs to the **System.Data** namespace.
* It allows for **full CRUD** (Create, Read, Update, Delete) operations while offline.
* You can use **`AcceptChanges()`** or **`RejectChanges()`** to manage the state of your data.
* It is the building block of a  **`DataSet`** .

---

📌 **One-Line Summary (Best for Interviews)**

> A **DataTable** is an in-memory representation of a single database table that stores data in a row-and-column format for disconnected use.
