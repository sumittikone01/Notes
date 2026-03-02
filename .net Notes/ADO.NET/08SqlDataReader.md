Here are your **clean, professional, one-read revision notes** for:

# 🔷 What is SqlDataReader?

* `SqlDataReader` is used to read data returned from a SQL query.
* It works only with `SELECT` statements.
* It reads data  **row by row** .
* It is part of:

```csharp
using Microsoft.Data.SqlClient;
```

---

# 🔷 Key Characteristics

* It is **Connected Architecture** (connection must remain open).
* It is **Forward Only** (cannot go back to previous row).
* It is **Read Only** (cannot modify data).
* It is  **Fast and memory efficient** .
* It retrieves data directly from database.

---

# Example

```csharp
using Microsoft.Data.SqlClient;
using System;

SqlConnection conn = new SqlConnection("your_connection_string");
conn.Open();

SqlCommand cmd = new SqlCommand("SELECT * FROM Students", conn);

SqlDataReader reader = cmd.ExecuteReader();

while (reader.Read())
{
    Console.WriteLine(reader["Name"]);
}

reader.Close();
conn.Close();
```

---

# 🔷 Common Properties (Short Explanation)

| Property                      | Meaning                            |
| ----------------------------- | ---------------------------------- |
| **FieldCount**          | Number of columns in current row   |
| **HasRows**             | Returns true if data exists        |
| **IsClosed**            | Checks if reader is closed         |
| **RecordsAffected**     | Rows affected (mainly for DML)     |
| **Depth**               | Depth of nesting (rarely used)     |
| **Item[index or name]** | Gets column value by index or name |

---

# 🔷Common Methods (Explained One by One)

---

## 1️⃣ Read()

* **Read() method** is used to  **move the SqlDataReader to the next row of data** .
* It Returns `true` if more rows exist & also  **loads that row into memory so you can access column values** of that row .

```csharp
while (reader.Read())
{
    Console.WriteLine(reader["Name"]);
}
```

✔ Most important method.

---

## 2️⃣ Close()

* It Closes the SqlDataReader.
* Always close after use.

```csharp
reader.Close();
```

✔ Important to free connection.

---

## 3️⃣ GetValue(int i)

* Gets value of column by index.
* Returns object.

```csharp
var value = reader.GetValue(0);
```

---

## 4️⃣ GetName(int i)

* Returns column name by index.

```csharp
string columnName = reader.GetName(0);
```

---

## 5️⃣ GetOrdinal(string name)

* Returns column index by column name.
* Useful for performance.

```csharp
int index = reader.GetOrdinal("Name");
```

---

## 6️⃣ IsDBNull(int i)

* Checks if column value is NULL.
* Prevents runtime errors.

```csharp
if (!reader.IsDBNull(0))
{
    Console.WriteLine(reader.GetString(0));
}
```

✔ Always check for NULL in real projects.

---

## 7️⃣ GetString(int i)

* Gets column value as string.

```csharp
string name = reader.GetString(1);
```

---

## 8️⃣ GetInt32(int i)

* Gets column value as integer.

```csharp
int id = reader.GetInt32(0);
```

---

## 9️⃣ NextResult()

* Moves to next result set.
* Used when multiple SELECT queries are executed together.

```csharp
reader.NextResult();
```

✔ Used in stored procedures returning multiple tables.

---

# 🔷 Important Interview Points

* SqlDataReader is faster than DataSet.
* Works only in connected mode.
* Cannot jump to previous row.
* Connection must remain open while reading.
* Always use inside `using` block for safety.

---

# 🔷  Best Practice Version (Professional Way)

```csharp
using (SqlConnection conn = new SqlConnection(connectionString))
{
    conn.Open();

    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Students", conn))
    {
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(1));
            }
        }
    }
}
```

✔ Automatically closes everything.

---

# 🧠 Final One-Line Understanding

> **SqlDataReader reads database data row by row in connected, fast, forward-only mode.**
