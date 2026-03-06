# JSON — Complete Notes
### JavaScript Object Notation — The Language of the Web

---

## Prerequisites

| What You Need |
|---|
| Basic JavaScript (variables, objects, arrays) |
| C# classes and properties |
| ASP.NET Core Controller basics |

---

## 1. What is JSON

**JSON** is a text format for representing structured data.
That's it. It is just text — but text written in a very specific structure
that both humans and computers can easily read and understand.

**Simple definition:**
> JSON is a way to write data as text so it can be sent between a browser
> and a server, stored in a file, or shared between any two systems.

```
Without JSON — how would you send an object over HTTP?
─────────────────────────────────────────────────────
HTTP is text-based. You cannot literally send a C# object
or a JavaScript object over the wire. They live in memory.

You need to convert the object to TEXT first.
JSON is that text format. Universal. Simple. Readable.

C# Object                    JSON Text (sent over wire)
───────────────              ──────────────────────────
user.UserId   = "sumit"      {
user.Role     = "Admin"        "userId": "sumit",
user.IsLocked = false          "role": "Admin",
                               "isLocked": false
                             }
```

**Where you will use JSON every single day:**
- Kendo Grid DataSource reads JSON from your controller
- AJAX calls send and receive JSON
- `return Json(...)` in your controller
- `appsettings.json` is a JSON file
- API responses are JSON

---

## 2. JSON Syntax — The 6 Rules

```
RULE 1 — Data is in key:value pairs
──────────────────────────────────────────
"userId": "sumit"
  ↑          ↑
 key       value
Both key AND string values must use double quotes. Never single quotes.


RULE 2 — Pairs are separated by commas
──────────────────────────────────────────
{
    "userId": "sumit",    ← comma
    "role": "Admin",      ← comma
    "isLocked": false     ← NO comma on last item
}


RULE 3 — Objects use curly braces {}
──────────────────────────────────────────
{ "name": "Sumit", "age": 25 }


RULE 4 — Arrays use square brackets []
──────────────────────────────────────────
["Admin", "User", "Manager"]

[
    { "userId": "sumit" },
    { "userId": "rahul" }
]


RULE 5 — No comments allowed
──────────────────────────────────────────
// This is NOT valid JSON  ← will break the parser
{ "name": "Sumit" }        ← valid


RULE 6 — No trailing commas
──────────────────────────────────────────
{
    "name": "Sumit",
    "age": 25,          ← ❌ trailing comma — invalid JSON in strict parsers
}
```

---

## 3. JSON Data Types — All Six

JSON supports exactly 6 data types. No more.

```json
{
    "name":        "Sumit Sharma",   // String  — always double quotes
    "age":         25,               // Number  — integer or decimal, no quotes
    "salary":      45000.50,         // Number  — decimals work too
    "isActive":    true,             // Boolean — lowercase true or false, no quotes
    "middleName":  null,             // Null    — lowercase null, no quotes
    "address": {                     // Object  — nested {}
        "city":  "Mumbai",
        "pin":   "400001"
    },
    "skills": ["C#", "SQL", "AJAX"]  // Array   — []
}
```

| JSON Type | Example | C# Equivalent | JavaScript Equivalent |
|---|---|---|---|
| String | `"sumit"` | `string` | `string` |
| Number | `25` or `45.5` | `int`, `double`, `decimal` | `number` |
| Boolean | `true` / `false` | `bool` | `boolean` |
| Null | `null` | `null` | `null` |
| Object | `{ "key": "val" }` | `class` instance | `object` |
| Array | `[1, 2, 3]` | `List<T>`, array | `array` |

---

## 4. JSON vs XML — Why JSON Won

Before JSON, XML was the standard for data exchange.
You may still see XML in older enterprise systems.

```xml
<!-- Same data in XML — verbose, hard to read -->
<user>
    <userId>sumit</userId>
    <role>Admin</role>
    <isLocked>false</isLocked>
    <skills>
        <skill>C#</skill>
        <skill>SQL</skill>
    </skills>
</user>
```

```json
// Same data in JSON — clean, half the size
{
    "userId": "sumit",
    "role": "Admin",
    "isLocked": false,
    "skills": ["C#", "SQL"]
}
```

| | JSON | XML |
|---|---|---|
| **Readability** | Very easy | Verbose, noisy |
| **File size** | Smaller | Larger (opening + closing tags) |
| **Parsing speed** | Faster | Slower |
| **Supports arrays** | Yes, natively | No native array |
| **Comments** | Not allowed | Allowed |
| **Used with** | REST APIs, AJAX, Kendo | SOAP APIs, old enterprise |
| **Learning curve** | Very low | Higher |

**JSON is the standard for modern web APIs and AJAX. You will use JSON, not XML.**

---

## 5. JSON in JavaScript

### JSON.stringify() — Object → Text

```javascript
// Convert a JavaScript object TO a JSON string (to send over wire)

var user = {
    userId:   "sumit",
    role:     "Admin",
    isLocked: false
};

var jsonText = JSON.stringify(user);
console.log(jsonText);
// Output: '{"userId":"sumit","role":"Admin","isLocked":false}'
// ↑ Now this is a STRING — can be sent in an HTTP request body


// Pretty print (for debugging — adds indentation)
var prettyJson = JSON.stringify(user, null, 2);
console.log(prettyJson);
// Output:
// {
//   "userId": "sumit",
//   "role": "Admin",
//   "isLocked": false
// }
```

### JSON.parse() — Text → Object

```javascript
// Convert a JSON string BACK to a JavaScript object (received from server)

var jsonText = '{"userId":"sumit","role":"Admin","isLocked":false}';

var user = JSON.parse(jsonText);
console.log(user.userId);    // "sumit"
console.log(user.role);      // "Admin"
console.log(user.isLocked);  // false
```

### The Two Functions — When to Use Which

```
Data you want to SEND (object → string):   JSON.stringify()
Data you RECEIVED    (string → object):    JSON.parse()

Browser                                Server
  │                                       │
  │  JSON.stringify(myObject)             │
  │ ──── sends JSON string ──────────>    │
  │                                       │  parses it back to C# object
  │ <──── receives JSON string ──────     │
  │  JSON.parse(responseText)             │
  │  use as JavaScript object             │
```

### Safe Parsing — try/catch

```javascript
// If the JSON text is malformed, JSON.parse throws an error
// Always wrap in try/catch when parsing data you don't control

function safeParseJson(jsonText) {
    try {
        return JSON.parse(jsonText);
    } catch (e) {
        console.error('Invalid JSON:', e.message);
        return null;
    }
}

var data = safeParseJson(responseText);
if (data !== null) {
    // safe to use data
}
```

---

## 6. JSON in C# — System.Text.Json

.NET has two JSON libraries. You need to know both.

| Library | Namespace | When to Use |
|---|---|---|
| `System.Text.Json` | Built into .NET — no NuGet needed | **Default in ASP.NET Core 6+** |
| `Newtonsoft.Json` | NuGet: `Newtonsoft.Json` | Older projects, more features |

### System.Text.Json (Modern — Use This)

```csharp
using System.Text.Json;

// ── Serialize: C# object → JSON string ──────────────────────────────
var user = new UserRecord { UserId = "sumit", Role = "Admin", IsLocked = false };

string jsonText = JsonSerializer.Serialize(user);
// Output: {"userId":"sumit","role":"Admin","isLocked":false}

// Pretty print
string prettyJson = JsonSerializer.Serialize(user, new JsonSerializerOptions
{
    WriteIndented = true
});


// ── Deserialize: JSON string → C# object ─────────────────────────────
string json    = "{\"userId\":\"sumit\",\"role\":\"Admin\"}";
UserRecord? u  = JsonSerializer.Deserialize<UserRecord>(json);
Console.WriteLine(u?.UserId);   // "sumit"


// ── Serialize a List → JSON array ────────────────────────────────────
var users = new List<UserRecord>
{
    new() { UserId = "sumit", Role = "Admin" },
    new() { UserId = "rahul", Role = "User"  }
};

string jsonArray = JsonSerializer.Serialize(users);
// Output: [{"userId":"sumit","role":"Admin"},{"userId":"rahul","role":"User"}]


// ── Deserialize JSON array → List ─────────────────────────────────────
List<UserRecord>? list = JsonSerializer.Deserialize<List<UserRecord>>(jsonArray);
```

---

## 7. JSON Property Naming — The Casing Problem

C# uses **PascalCase** (`UserId`, `IsLocked`).
JSON (and JavaScript) conventionally uses **camelCase** (`userId`, `isLocked`).

By default, `System.Text.Json` keeps PascalCase.
This can cause mismatches with JavaScript code expecting camelCase.

```csharp
// Fix: configure camelCase globally in Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // Now: UserId → userId, IsLocked → isLocked, FailedAttempts → failedAttempts
    });
```

```csharp
// OR — fix per-property with [JsonPropertyName]
public class UserRecord
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = "";

    [JsonPropertyName("isLocked")]
    public bool IsLocked { get; set; }
}
```

**After adding camelCase globally**, your Kendo Grid field names must match:

```javascript
// C# property: UserId → JSON: "userId"
schema: {
    model: {
        fields: {
            userId:         { type: 'string' },   // ← camelCase
            failedAttempts: { type: 'number' },
            isLocked:       { type: 'boolean' }
        }
    }
}
```

---

## 8. JSON in ASP.NET Core MVC — Daily Use

### return Json() — Send JSON from Controller

```csharp
// Simple object
public IActionResult GetUser()
{
    return Json(new { userId = "sumit", role = "Admin", isLocked = false });
}

// A C# class
public IActionResult GetUser()
{
    var user = _userRepo.GetUserById("sumit");
    return Json(user);   // UserRecord class → JSON automatically
}

// A list
public IActionResult GetAllUsers()
{
    var users = _userRepo.GetAllUsers();
    return Json(users);   // List<UserRecord> → JSON array automatically
}

// Success/failure response — pattern you'll use for every AJAX action
public IActionResult UnlockUser(string userId)
{
    _userRepo.UnlockUser(userId);
    return Json(new { success = true, message = "User unlocked." });
}
```

### [FromBody] — Receive JSON in Controller

When JavaScript sends JSON in the request body
(using `contentType: 'application/json'`), you need `[FromBody]`.

```csharp
// JavaScript sends:
// $.ajax({ contentType: 'application/json', data: JSON.stringify({ userId: 'sumit' }) })

// Controller receives:
[HttpPost]
public IActionResult UnlockUser([FromBody] UnlockRequest request)
{
    Console.WriteLine(request.UserId);   // "sumit"
    ...
}

public class UnlockRequest
{
    public string UserId { get; set; } = "";
}
```

### Without [FromBody] — Form-encoded data

When you send data as regular form fields (default jQuery):

```javascript
// jQuery default — sends as form-encoded (no contentType setting needed)
$.ajax({ type: 'POST', data: { userId: 'sumit' } })
```

```csharp
// No [FromBody] needed — MVC binds from form data automatically
[HttpPost]
public IActionResult UnlockUser(string userId)
{
    Console.WriteLine(userId);  // "sumit"
}
```

| Sending method | contentType | Controller attribute |
|---|---|---|
| `data: { key: val }` (default jQuery) | `application/x-www-form-urlencoded` | Nothing (auto-bound) |
| `data: JSON.stringify(obj)` | `application/json` | `[FromBody]` required |

---

## 9. JSON for Session Storage

From Chapter 6 — session only stores strings.
For complex objects, serialize to JSON.

```csharp
// Store object in session
var userInfo = new UserInfo { UserId = "sumit", Role = "Admin" };
string json  = JsonSerializer.Serialize(userInfo);
HttpContext.Session.SetString("UserInfo", json);

// Read back from session
string json      = HttpContext.Session.GetString("UserInfo") ?? "{}";
UserInfo? info   = JsonSerializer.Deserialize<UserInfo>(json);
string userId    = info?.UserId ?? "";
```

---

## 10. Kendo Grid and JSON — The Connection

Kendo Grid's DataSource fetches your controller's JSON and maps it to columns.
The field names in your JSON must exactly match the field names in Kendo's schema.

```
Controller returns:                  Kendo schema must have:
────────────────────                 ─────────────────────────
[                                    schema: {
  {                                    model: {
    "userId": "sumit",      ───────>     fields: {
    "failedAttempts": 2,    ───────>       userId: {},
    "isLocked": false,      ───────>       failedAttempts: {},
    "role": "Admin"         ───────>       isLocked: {},
  }                                        role: {}
]                                        }
                                       }
                                    }
```

If Kendo field name is `userId` but your JSON says `UserId` (capital U),
the column will be empty. Always match the casing exactly.

---

## Quick Reference — Cheat Sheet

```javascript
// JavaScript
JSON.stringify(obj)              // object → JSON string (to send)
JSON.parse(jsonString)           // JSON string → object (received)
JSON.stringify(obj, null, 2)     // pretty print with 2-space indent
```

```csharp
// C# — System.Text.Json
JsonSerializer.Serialize(obj)                  // object → JSON string
JsonSerializer.Deserialize<T>(jsonString)       // JSON string → object
JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true })
```

```csharp
// ASP.NET Core Controller
return Json(anyObject)                     // returns JSON response
[FromBody] MyClass param                   // receive JSON body
.AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
```

---

## Summary

| Concept | One-Line Meaning |
|---|---|
| **JSON** | Text format for structured data — the universal language between browser and server |
| **6 types** | String, Number, Boolean, Null, Object `{}`, Array `[]` |
| **Rules** | Double quotes for keys+strings, no trailing commas, no comments |
| **JSON.stringify** | JavaScript object → JSON text (to send) |
| **JSON.parse** | JSON text → JavaScript object (received) |
| **JsonSerializer.Serialize** | C# object → JSON text |
| **JsonSerializer.Deserialize<T>** | JSON text → C# object |
| **return Json(obj)** | Controller sends JSON response for AJAX calls |
| **[FromBody]** | Required when receiving JSON body (with contentType: application/json) |
| **camelCase config** | Set globally in Program.cs — makes C# PascalCase match JS camelCase |
| **Kendo + JSON** | Kendo field names must exactly match JSON property names |

---

*JSON Complete Notes — Sumit's ASP.NET Core MVC Learning Journey*
