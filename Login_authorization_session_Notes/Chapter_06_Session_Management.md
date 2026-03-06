# Chapter 6 — Session Management
### Storing and Reading User Data Across Requests

---

## Prerequisites

| What You Need | Your File / Chapter |
|---|---|
| Chapter 1 — Session concept (coat check analogy) | ✓ |
| Chapter 2 — Program.cs setup with AddSession | ✓ |
| Chapter 5 — Login works, SignInAsync done | ✓ |
| Middleware notes — UseSession position in pipeline | ✓ |

After Chapter 5 your login works and creates an auth cookie.
Chapter 6 adds the session layer on top — storing the user's name,
role, and ID so every page can access them without touching Claims or
making a database call.

---

## 1. Why You Need Session If You Already Have Claims

A fair question. You put Name and Role in Claims (Chapter 5).
You can read them anywhere with `User.Identity.Name`.
So why do you also need Session?

**Claims are for authentication middleware — small, fixed data.**
**Session is for your application logic — flexible, changeable data.**

| | Claims (in Cookie) | Session |
|---|---|---|
| **Set when** | At login only — fixed for the session | Anytime, can change |
| **Read by** | `[Authorize]`, middleware, `User.*` | Your controller code |
| **Stored** | Encrypted inside the auth cookie | On the server |
| **Survives** | Until cookie expires | Until timeout or logout |
| **Best for** | Role, UserId — things middleware needs | Name, preferences, temporary data |
| **Size limit** | Small (cookie = ~4KB total) | Much larger (server memory) |

**In your login system you'll use both:**
- Claims → `[Authorize(Roles="Admin")]` uses Role from Claims
- Session → "Welcome, Sumit" on the dashboard reads name from Session

---

## 2. Setup in Program.cs — You Already Have This

From Chapter 2/5, this is already in your Program.cs. Here's what each line means.

```csharp
// Services section
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    //  ↑ Session clears itself 30 minutes after the last request
    //    Reset timer happens automatically on every request

    options.Cookie.HttpOnly    = true;
    //  ↑ The session ID cookie cannot be read by JavaScript
    //    Same reason as auth cookie — protects against XSS

    options.Cookie.IsEssential = true;
    //  ↑ Session cookie works even if user hasn't accepted cookies
    //    Required for login to work regardless of cookie consent banners
});

// Pipeline section (must be before UseAuthentication)
app.UseSession();
```

That's it for setup. You wrote this once in Chapter 2. It's done.

---

## 3. The Four Operations — Set, Get, Remove, Clear

Session works like a dictionary. Key → Value.

```csharp
// ── SET — store a value ──────────────────────────────────────────────

HttpContext.Session.SetString("UserName", "Sumit Sharma");
HttpContext.Session.SetString("UserId",   "sumit");
HttpContext.Session.SetString("Role",     "Admin");
HttpContext.Session.SetInt32("LoginCount", 5);


// ── GET — read a value ───────────────────────────────────────────────

string name  = HttpContext.Session.GetString("UserName") ?? "";
string userId = HttpContext.Session.GetString("UserId")  ?? "";
string role   = HttpContext.Session.GetString("Role")    ?? "";
int    count  = HttpContext.Session.GetInt32("LoginCount") ?? 0;
//                                                           ↑
//                               Returns null if key doesn't exist
//                               ?? "" gives you empty string instead of null


// ── REMOVE — delete one key ──────────────────────────────────────────

HttpContext.Session.Remove("TempMessage");


// ── CLEAR — wipe all session data ────────────────────────────────────

HttpContext.Session.Clear();   // call this on LOGOUT
```

---

## 4. Only Two Types Built In — Handling Complex Objects

ASP.NET Core session only stores `string` and `int` natively.
For anything else (a whole object), you serialize to JSON.

```csharp
// The problem:
// You want to store a UserInfo object in session
public class UserInfo
{
    public string UserId   { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Role     { get; set; } = "";
}

// HttpContext.Session.Set(key, UserInfo) ← won't compile — no direct object method
```

**Solution: JSON serialization**

```csharp
using System.Text.Json;

// Store object as JSON string
var info = new UserInfo { UserId = "sumit", UserName = "Sumit Sharma", Role = "Admin" };
string json = JsonSerializer.Serialize(info);
HttpContext.Session.SetString("UserInfo", json);


// Read back and deserialize
string json    = HttpContext.Session.GetString("UserInfo") ?? "{}";
UserInfo? info = JsonSerializer.Deserialize<UserInfo>(json);
string name    = info?.UserName ?? "";
```

**For your login system, storing them individually as strings is simpler:**

```csharp
// At login — easier to read separately
HttpContext.Session.SetString("UserId",   user.UserId);
HttpContext.Session.SetString("UserName", user.UserId);   // or a full name column if you add one
HttpContext.Session.SetString("Role",     user.Role ?? "User");
```

---

## 5. Where to Set Session — In the Login Action

Going back to the login controller from Chapter 5.
The `// Chapter 6 placeholder` is now filled in.

```csharp
[HttpPost("Login")]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
{
    // ... (all Chapter 3, 4, 5 code above this)

    // ── Chapter 5: Sign in ────────────────────────────────────────────
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId),
        new Claim(ClaimTypes.Name,           user.UserId),
        new Claim(ClaimTypes.Role,           user.Role ?? "User"),
    };
    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme)));

    // ── Chapter 6: Set Session ────────────────────────────────────────
    HttpContext.Session.SetString("UserId",   user.UserId);
    HttpContext.Session.SetString("UserName", user.UserId);
    HttpContext.Session.SetString("Role",     user.Role ?? "User");
    // ─────────────────────────────────────────────────────────────────

    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        return Redirect(returnUrl);

    return RedirectToAction("Index", "Home");
}
```

---

## 6. Reading Session in Controllers

```csharp
[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Read from session — available because UseSession runs on every request
        string userId   = HttpContext.Session.GetString("UserId")   ?? "";
        string userName = HttpContext.Session.GetString("UserName") ?? "";
        string role     = HttpContext.Session.GetString("Role")     ?? "";

        // Pass to view via ViewBag (you know this from your passing-data notes)
        ViewBag.UserName = userName;
        ViewBag.Role     = role;

        return View();
    }
}
```

---

## 7. Reading Session in Razor Views

```html
@* Direct read from session in a Razor view *@
@{
    string userName = Context.Session.GetString("UserName") ?? "Guest";
    string role     = Context.Session.GetString("Role")     ?? "";
}

<p>Welcome, @userName</p>

@if (role == "Admin")
{
    <a asp-controller="Admin" asp-action="Index">Admin Panel</a>
}
```

**`Context`** is the Razor view's shortcut for `HttpContext`.
Same object, different name in views.

Or use ViewBag from the controller (simpler):

```html
<p>Welcome, @ViewBag.UserName</p>
```

---

## 8. Checking Session is Active — Guard Against Expired Sessions

Session can expire (after the `IdleTimeout` you configured — 30 minutes).
When it does, `GetString()` returns `null`.

A good pattern: create a helper method to check if session is valid.

```csharp
// Base class for all controllers that need auth ─────────────────────
// Put this in Controllers/BaseController.cs
public class BaseController : Controller
{
    protected string SessionUserId   => HttpContext.Session.GetString("UserId")   ?? "";
    protected string SessionUserName => HttpContext.Session.GetString("UserName") ?? "";
    protected string SessionRole     => HttpContext.Session.GetString("Role")     ?? "";

    protected bool IsSessionValid => !string.IsNullOrEmpty(SessionUserId);

    // Call this at the start of any action where session data is critical
    protected IActionResult? RequireSession()
    {
        if (!IsSessionValid)
        {
            // Session expired — force re-login even if cookie is still valid
            return RedirectToAction("Login", "Account");
        }
        return null;
    }
}

// Your controllers inherit from BaseController instead of Controller:
[Authorize]
public class HomeController : BaseController
{
    public IActionResult Index()
    {
        var redirect = RequireSession();
        if (redirect != null) return redirect;

        ViewBag.UserName = SessionUserName;
        return View();
    }
}
```

This is a clean pattern used in real enterprise MVC apps.
The `[Authorize]` handles cookie expiry.
`RequireSession()` handles the edge case where cookie is valid
but session has timed out (can happen with long-running browser tabs).

---

## 9. Logout — Clear Everything

Always clear both the auth cookie AND the session on logout.

```csharp
[HttpPost("Logout")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    // 1. Delete the auth cookie
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // 2. Wipe all session data
    HttpContext.Session.Clear();

    // 3. Redirect to login
    return RedirectToAction("Login", "Account");
}
```

**Why clear both?**
- `SignOutAsync` removes the auth cookie → `[Authorize]` will block access
- `Session.Clear()` removes the session data → no stale data if someone
  shares a computer and the session somehow persists

---

## 10. Session Timeout — What Happens

```
User logs in at 10:00
  Session set: UserId=sumit, UserName=Sumit, Role=Admin

User is active — every request resets the 30-min timer
  10:15 → request → timer resets to 30 min from 10:15
  10:30 → request → timer resets to 30 min from 10:30

User goes to lunch — no requests from 11:00 to 11:35 (35 minutes)

11:35 — User comes back, requests /Home/Dashboard
  Auth cookie: still valid (set to 8 hours in Chapter 5)
  [Authorize]: passes (cookie valid)
  Controller: HttpContext.Session.GetString("UserId") → null
  Session: expired after 30 min of inactivity

Without RequireSession() check:
  Code gets "" for UserId, app behaves weirdly with empty values

With RequireSession() check:
  User is redirected to /Account/Login to set session again
  Clean user experience — looks like a re-login
```

---

## 11. TempData — The One-Request Session

You know ViewBag for Controller→View in the same request.
**TempData** is like a session key that automatically deletes itself
after being read. Perfect for "show a success/error message after redirect."

```csharp
// In controller — set before redirect
TempData["SuccessMessage"] = "User unlocked successfully.";
return RedirectToAction("UserList", "Admin");

// In the view you redirected to — read and it auto-deletes after display
@if (TempData["SuccessMessage"] != null)
{
    <div class="alert alert-success">@TempData["SuccessMessage"]</div>
}

// After this view renders, TempData["SuccessMessage"] is automatically gone
```

TempData uses Session internally. It's available because you have `AddSession()`
already configured.

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **Session** | Server-side storage per user, identified by a session ID cookie |
| **AddSession + UseSession** | Register and enable — done in Program.cs |
| **IdleTimeout** | Session clears after this many minutes of no requests (30 min typical) |
| **SetString / GetString** | The two methods you use most — key-value storage |
| **JSON serialization** | How to store complex objects — serialize to string, deserialize to read |
| **Set at login** | `SetString("UserId", ...)` + `SetString("Role", ...)` right after `SignInAsync` |
| **Clear at logout** | `Session.Clear()` always paired with `SignOutAsync` |
| **BaseController** | Helper base class with session properties — clean pattern for real apps |
| **RequireSession()** | Guards against session expiring while auth cookie is still valid |
| **TempData** | One-request session — auto-deletes after being read, used for redirect messages |

---

## What's Next — Chapter 7

**AJAX + JSON in ASP.NET Core MVC**

Your login system now works completely. Chapter 7 makes it smooth.
Instead of full page reloads, the login form submits via AJAX,
error messages appear inline, and the admin's "Unlock User" button
works without refreshing the page. This is where your app
goes from functional to professional-feeling.

---

*Chapter 6 of 8 — Sumit's Login System — ASP.NET Core MVC*
