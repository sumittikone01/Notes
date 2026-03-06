# Chapter 4 — Password Hashing and Security
### Why You Never Store Plain Text and How to Do It Right

---

## Prerequisites

| What You Need | Your File |
|---|---|
| C# classes and static methods | `C# Notes / Oops In C#.md` |
| Chapter 3 — Users table with PasswordHash column | ✓ |

This chapter is short but critical. It directly fills in the
`/* Chapter 4 fills this in */` placeholder from Chapter 3's login controller.

---

## 1. Why Plain Text Passwords Are Catastrophic

Imagine your Users table looks like this:

```
UserId    | PasswordHash
──────────┼─────────────────
sumit     | abc123
rahul     | mypassword
admin     | admin@123
```

Now imagine your database gets hacked (SQL injection, insider, backup leak —
it happens to real companies). The attacker has **every user's password
in plain text**. Instantly. No work needed.

Worse: most people reuse passwords. So now the attacker also has their
Gmail, their banking, their everything.

**This is not a theoretical risk. It is why companies lose crores of
rupees and face lawsuits.** Never store plain text passwords.

---

## 2. What Hashing Is

**Hashing** is a one-way transformation. You put a password in,
you get a fixed-length scrambled string out. You can never reverse it.

```
Plain text                  Hash (SHA256)
──────────────────────────────────────────────────────────
"abc123"       ──hash──>   "ungWv48Bz+pBQUDeXa4iI7ADYaOWF..."
"abc124"       ──hash──>   "CrAEH+u69X9mH3WM6qxGXQxFj/zM..."
"abc123"       ──hash──>   "ungWv48Bz+pBQUDeXa4iI7ADYaOWF..."  (always same)
```

Three properties that make hashing work for passwords:

| Property | Meaning |
|---|---|
| **Deterministic** | Same input always gives same output — needed for login comparison |
| **One-way** | Cannot reverse the hash to get the original password |
| **Avalanche effect** | Tiny change in input = completely different hash |

**How login works with hashing:**

```
REGISTRATION (storing password):
User sets password: "abc123"
You hash it:        "ungWv48B..."
You store the hash in DB — NEVER the plain text

LOGIN (checking password):
User enters: "abc123"
You hash it: "ungWv48B..."
You compare: stored hash == entered hash?   YES → login ok
```

The database only ever holds hashes.
Even YOU as a developer cannot see the user's actual password.

---

## 3. SHA256 — Basic Hashing in C#

SHA256 is a well-known hashing algorithm built into .NET.
No NuGet package needed.

```csharp
// Helpers/PasswordHelper.cs
using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    // Hashes a plain text password → returns Base64 hash string
    public static string Hash(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

    // Compares entered password with stored hash
    public static bool Verify(string enteredPassword, string storedHash)
    {
        string hashOfEntered = Hash(enteredPassword);
        return hashOfEntered == storedHash;
    }
}
```

Usage in the login controller:

```csharp
// Registering a new user
string hash = PasswordHelper.Hash("abc123");
// store hash in DB via sp_CreateUser

// Login verification
bool isValid = PasswordHelper.Verify(model.Password, user.PasswordHash);
```

**SHA256 is fast. For passwords, that is actually a problem.**
Read section 4 to understand why — and what to use instead.

---

## 4. The Problem with SHA256 for Passwords

SHA256 is designed to be **extremely fast** — millions of hashes per second
on modern hardware. For files, checksums, and data integrity, that's great.
For passwords, it's a weakness.

**Why fast is bad for passwords:**

```
Attacker has your database. They have the hashes.
They don't reverse the hash — they BRUTE FORCE it.

They take a list of common passwords:
  "password123" → hash → compare with your hashes
  "abc123"      → hash → compare with your hashes
  "admin@123"   → hash → compare with your hashes
  ... 10 billion attempts per second with a GPU

With SHA256: cracking a common password takes seconds.
With BCrypt: cracking the same password takes YEARS.
```

The solution is a **slow** hashing algorithm — one designed specifically
for passwords that intentionally takes time to compute.

---

## 5. BCrypt — The Right Tool for Passwords

**BCrypt** is a password hashing algorithm that is:
- Deliberately slow (you control how slow with a "work factor")
- Automatically salted (see section 6)
- Widely used in production systems worldwide

Install it via NuGet:

```
Tools → NuGet Package Manager → Manage NuGet Packages
Search: BCrypt.Net-Next
Install it
```

Or via terminal:
```
dotnet add package BCrypt.Net-Next
```

---

## 6. What is a Salt

Even with hashing, there is another attack: **Rainbow Tables**.

A rainbow table is a pre-computed list of millions of common passwords
and their hashes. Attacker just looks up your hash → instant plain text.

**A salt fixes this.** A salt is a random string added to the password
before hashing. Different for every user.

```
Without salt:
  "abc123"              → SHA256 → "ungWv48B..."    ← same hash every time

With salt (BCrypt does this automatically):
  "abc123" + "$2a$12$Kx9..." → BCrypt → "$2a$12$Kx9...hashed"
  "abc123" + "$2a$12$Yz7..." → BCrypt → "$2a$12$Yz7...different_hash"
```

The salt is stored inside the BCrypt hash string itself — you don't
manage it separately. BCrypt.Verify() extracts the salt automatically
during comparison. You just call two methods and forget the rest.

---

## 7. The Final PasswordHelper with BCrypt

Replace the SHA256 version with this:

```csharp
// Helpers/PasswordHelper.cs
using BCrypt.Net;

public static class PasswordHelper
{
    // work factor 12 = good balance of security vs speed (~250ms per hash)
    // Higher = slower = harder to brute force. 12 is the standard recommendation.
    private const int WorkFactor = 12;

    // Hash a plain text password — call this when CREATING or CHANGING a password
    public static string Hash(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
    }

    // Verify entered password against stored hash — call this at LOGIN
    public static bool Verify(string enteredPassword, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
    }
}
```

That is literally the entire class. Two methods.

---

## 8. BCrypt Hash Format

When you hash "abc123" with BCrypt, the result looks like:

```
$2a$12$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy
 │   │  │                            │
 │   │  └── The Salt (22 chars)      └── The Hash (31 chars)
 │   └── Work factor (12)
 └── BCrypt version
```

The entire string goes into the `PasswordHash` column in your DB.
`NVARCHAR(255)` is more than enough to hold it.

---

## 9. Complete Usage — Registration and Login

### When an admin creates a new user (registration):

```csharp
// In AdminController or a setup script:
string plainPassword = "Welcome@123";    // given to the user
string hash          = PasswordHelper.Hash(plainPassword);

// Store hash via stored procedure sp_CreateUser
_userRepo.CreateUser("sumit", hash);
```

### At login — filling in Chapter 3's placeholder:

```csharp
[HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    UserRecord? user = _userRepo.GetUserById(model.UserId);

    if (user == null)
    {
        ModelState.AddModelError("", "Invalid user ID or password.");
        return View(model);
    }

    if (user.IsLocked)
    {
        ModelState.AddModelError("", "Account is locked. Contact your administrator.");
        return View(model);
    }

    // ── Chapter 4: Password verification ─────────────────────────────
    bool passwordCorrect = PasswordHelper.Verify(model.Password, user.PasswordHash);
    //                                    ↑ entered    ↑ from database
    // ─────────────────────────────────────────────────────────────────

    if (!passwordCorrect)
    {
        _userRepo.IncrementFailedAttempts(model.UserId);
        var updated = _userRepo.GetUserById(model.UserId);
        if (updated?.IsLocked == true)
            ModelState.AddModelError("", "Too many failed attempts. Account is now locked.");
        else
            ModelState.AddModelError("", "Invalid user ID or password.");
        return View(model);
    }

    _userRepo.ResetFailedAttempts(model.UserId);

    // ── Chapter 5: Sign in (create auth cookie) ── coming next
    // ── Chapter 6: Set session                  ── coming after

    return RedirectToAction("Index", "Home");
}
```

---

## 10. Security Rules — Remember These Always

| Rule | Why |
|---|---|
| Never store plain text passwords | One DB leak = all passwords exposed |
| Never log passwords | Server logs can be read by attackers |
| Always use BCrypt (not SHA256/MD5) for passwords | Fast algorithms are easily brute-forced |
| Never compare hashes with `==` directly | Use `BCrypt.Verify()` — it's timing-safe |
| Never reveal which field was wrong | Don't say "wrong password" — say "invalid credentials" (don't help attackers) |
| Always use SqlParameters | Prevents SQL injection in your login query |

The last point about error messages: if your login says "User not found" vs
"Wrong password" — an attacker learns which usernames exist. Always use
a generic message like "Invalid credentials" for both cases.

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **Hashing** | One-way scrambling — cannot be reversed |
| **SHA256** | Fast hash — good for files, bad for passwords |
| **BCrypt** | Slow hash with auto-salt — correct choice for passwords |
| **Salt** | Random string added before hashing — defeats rainbow table attacks |
| **Work factor 12** | How slow BCrypt is — 12 is the standard recommendation |
| **PasswordHelper.Hash()** | Call when creating/changing a password |
| **PasswordHelper.Verify()** | Call at login — compares entered vs stored |
| **Generic error message** | Never say "wrong password" — say "invalid credentials" |

---

## What's Next — Chapter 5

**Cookie Authentication — Completing the Login**

You now have the database layer (Chapter 3) and the password tool (Chapter 4).
Chapter 5 puts them together and completes the login — writing
`SignInAsync()`, building the Claims, protecting controllers with `[Authorize]`,
and handling logout. After Chapter 5 your login system will actually work end to end.

---

*Chapter 4 of 8 — Sumit's Login System — ASP.NET Core MVC*
