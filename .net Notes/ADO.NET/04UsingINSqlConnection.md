# Why `using SqlConnection con = new SqlConnection(cs);`

and NOT just
`SqlConnection con = new SqlConnection(cs);`

* `using` ensures that the database connection is  **automatically closed and disposed** , even if an exception occurs.

---

## 🔷 The `using` Block — Why It Matters

### ❌ Without `using` — Dangerous

```csharp
SqlConnection con = new SqlConnection(cs);
con.Open();

// If an exception is thrown here...
SomeMethod();

con.Close();    // ← NEVER REACHED if exception thrown above!
con.Dispose();  // ← NEVER REACHED
// Result: connection leak → pool fills up → app crashes
```

### ✅ With `using` — Always Safe

```csharp
using (SqlConnection con = new SqlConnection(cs)) {
    con.Open();

    SomeMethod(); // Even if this throws an exception...

} // ← con.Close() + con.Dispose() ALWAYS called here
```

**What `using` compiles to internally:**

```csharp
SqlConnection con = new SqlConnection(cs);
try {
    con.Open();
    SomeMethod();
}
finally {
    con.Dispose(); // Always runs — even on exception
}
```

---

## 🔷 Two `using` Syntax Styles (Both Are Valid)

```csharp
// Style 1 — Block syntax (traditional, explicit scope)
using (SqlConnection con = new SqlConnection(cs)) {
    con.Open();
    // ... work here
} // con disposed here

// Style 2 — Declaration syntax (C# 8+, disposed at method end)
using SqlConnection con = new SqlConnection(cs);
con.Open();
// ... work here
// con disposed at end of containing method
```

> ⚠️ The `using` for connections is **NOT** the same as `using System.Data;`
>
> - `using System.Data;` = **namespace import**
> - `using (SqlConnection con = ...)` = **resource management**

---



## Important clarification ❗

### ❌ `using` keyword here is NOT:

* `using System;`
* `using Microsoft.Data.SqlClient;`

Those are **namespace imports**

### ✅ This `using` is:

* A **resource management statement**
* Introduced in **C# 8 (using declaration)**

---

## Interview-ready comparison ⭐

| Without `using`    | With `using`    |
| -------------------- | ----------------- |
| Manual cleanup       | Automatic cleanup |
| Error-prone          | Safe              |
| Can leak connections | Prevents leaks    |
| Not recommended      | Best practice     |
