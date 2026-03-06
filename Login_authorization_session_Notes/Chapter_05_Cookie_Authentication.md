# Chapter 5 — Cookie Authentication
### SignInAsync, Claims, [Authorize], and Logout — The Login System Complete

---

## Prerequisites

| What You Need | Your File / Chapter |
|---|---|
| Middleware and Program.cs | `API In ASP.NET / Complete asp net core api Notes.md` |
| Chapter 2 — Pipeline, [Authorize], LoginPath | ✓ |
| Chapter 3 — UserRepository, sp_ValidateLogin | ✓ |
| Chapter 4 — PasswordHelper.Verify() | ✓ |

After this chapter your login system works end to end.
User visits protected page → redirect to login → enters credentials →
DB check → password verify → cookie created → back to the page they wanted.

---

## 1. What Cookie Authentication Does — Recap in One Diagram

```
LOGIN SUCCESS
     │
     ▼
You call SignInAsync(principal)
     │
     ▼
ASP.NET Core encrypts your Claims into a cookie string
     │
     ▼
Server sends:  Set-Cookie: .AspNetCore.Cookies=CfDJ8Kx...
     │
     ▼
Browser stores the cookie silently

═══════════════════════════════════════════════

EVERY FUTURE REQUEST
     │
     ▼
Browser sends: Cookie: .AspNetCore.Cookies=CfDJ8Kx...
     │
     ▼
UseAuthentication() middleware decrypts the cookie
     │
     ▼
Sets HttpContext.User = ClaimsPrincipal (Sumit, Admin role)
     │
     ▼
[Authorize] sees a valid user → allows through
     │
     ▼
Your controller action runs
```

You only write two things: `SignInAsync()` at login and `[Authorize]`
on controllers. The framework handles everything in between.

---

## 2. Setting Up Cookie Auth in Program.cs

You saw this in Chapter 2. Here it is complete and final.

```csharp
// Program.cs — Services section

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath          = "/Account/Login";
        options.AccessDeniedPath   = "/Account/AccessDenied";
        options.ExpireTimeSpan     = TimeSpan.FromHours(8);
        options.SlidingExpiration  = true;    // resets the timer on each request
        options.Cookie.HttpOnly    = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        //  ↑ Use Always in production (requires HTTPS)
        //    SameAsRequest is fine for local development (http)
    });

builder.Services.AddScoped<UserRepository>();
```

```csharp
// Program.cs — Pipeline section (order must be exactly this)

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();           // ← before authentication
app.UseAuthentication();    // ← reads cookie, sets HttpContext.User
app.UseAuthorization();     // ← checks [Authorize]
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
```

**Write this once. Then forget it. Everything else just works.**

---

## 3. Claims — What You Put in the Cookie

A **Claim** is a piece of information about the user that you want
available on every request without hitting the database.

```csharp
// Think of Claims as fields on an ID card inside the cookie:
//
// ┌───────────────────────────────────┐
// │  Name:       Sumit Sharma         │
// │  NameId:     sumit                │
// │  Role:       Admin                │
// └───────────────────────────────────┘
//
// Encrypted → sent to browser → decrypted on every request → available as HttpContext.User
```

**What to put in Claims vs Session:**

| Store in Claims (Cookie) | Store in Session |
|---|---|
| Anything needed by `[Authorize]` — especially **Role** | User's display name |
| UserId (to identify who is logged in) | Anything you read frequently on UI |
| Anything needed by middleware | Data that changes during the session |

Keep Claims small — they are encrypted and sent on every request.
Don't put large data in Claims. Use Session for that (Chapter 6).

---

## 4. The Complete Login Action — All Chapters Combined

```csharp
// Controllers/AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly UserRepository _userRepo;

    public AccountController(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // ── GET /Account/Login — show the form ──────────────────────────────
    [AllowAnonymous]
    [HttpGet("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        // Already logged in? Skip straight to dashboard
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;   // remember where they were trying to go
        return View();
    }

    // ── POST /Account/Login — handle form submission ─────────────────────
    [AllowAnonymous]
    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        // ── Step 1: Get user from database (Chapter 3) ──────────────────
        UserRecord? user = _userRepo.GetUserById(model.UserId);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials.");   // generic message
            return View(model);
        }

        // ── Step 2: Check account lock (Chapter 3) ───────────────────────
        if (user.IsLocked)
        {
            ModelState.AddModelError("", "Account is locked. Contact your administrator.");
            return View(model);
        }

        // ── Step 3: Verify password (Chapter 4) ──────────────────────────
        bool passwordCorrect = PasswordHelper.Verify(model.Password, user.PasswordHash);

        if (!passwordCorrect)
        {
            _userRepo.IncrementFailedAttempts(model.UserId);

            var refreshed = _userRepo.GetUserById(model.UserId);
            string error  = refreshed?.IsLocked == true
                ? "Too many failed attempts. Account is now locked."
                : "Invalid credentials.";

            ModelState.AddModelError("", error);
            return View(model);
        }

        // ── Step 4: Password correct — reset counter (Chapter 3) ─────────
        _userRepo.ResetFailedAttempts(model.UserId);

        // ── Step 5: Build Claims (this chapter) ──────────────────────────
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),    // "sumit"
            new Claim(ClaimTypes.Name,           user.UserId),    // shown in User.Identity.Name
            new Claim(ClaimTypes.Role,           user.Role ?? "User"), // used by [Authorize(Roles=)]
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // ── Step 6: Create the auth cookie (this chapter) ────────────────
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,                // false = cookie deleted when browser closes
                ExpiresUtc   = DateTime.UtcNow.AddHours(8)
            });

        // ── Step 7: Set Session (Chapter 6 — placeholder for now) ────────
        // HttpContext.Session.SetString("UserId",   user.UserId);
        // HttpContext.Session.SetString("UserName", user.UserId);
        // HttpContext.Session.SetString("Role",     user.Role ?? "User");

        // ── Step 8: Redirect (back to where they came from, or Home) ─────
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    // ── POST /Account/Logout ─────────────────────────────────────────────
    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    // ── GET /Account/AccessDenied — shown when role doesn't match ────────
    [AllowAnonymous]
    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
```

---

## 5. The ReturnUrl — Sending Users Back Where They Wanted to Go

When a user tries to open `/Home/Dashboard` without being logged in,
the middleware redirects them to `/Account/Login?returnUrl=%2FHome%2FDashboard`.

The `returnUrl` query parameter holds where they were going.
After login you send them back there instead of always going to Home.

```csharp
// After login success:
if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
    return Redirect(returnUrl);       // ← goes back to /Home/Dashboard

return RedirectToAction("Index", "Home");  // ← fallback if no returnUrl
```

`Url.IsLocalUrl(returnUrl)` is a security check — it prevents
"open redirect" attacks where an attacker crafts a link like:
`/Login?returnUrl=http://evil.com`

---

## 6. Reading User Info in Controllers and Views

After login, the Claims are available everywhere — no database call needed.

```csharp
// In any Controller action:
string userId   = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
string name     = User.Identity!.Name ?? "";
bool   isAdmin  = User.IsInRole("Admin");
bool   isLoggedIn = User.Identity.IsAuthenticated;
```

```html
@* In any Razor View — @User is available directly *@

@if (User.Identity!.IsAuthenticated)
{
    <span>Welcome, @User.Identity.Name</span>
    <form asp-controller="Account" asp-action="Logout" method="post">
        @Html.AntiForgeryToken()
        <button type="submit">Logout</button>
    </form>
}
else
{
    <a asp-controller="Account" asp-action="Login">Login</a>
}

@if (User.IsInRole("Admin"))
{
    <a asp-controller="Admin" asp-action="Index">Admin Panel</a>
}
```

---

## 7. Protecting Controllers with [Authorize]

```csharp
// Entire controller — all actions require login
[Authorize]
public class HomeController : Controller
{
    public IActionResult Index() { return View(); }
    public IActionResult Profile() { return View(); }
    // Both actions require login
}


// Role-based — only Admins
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult UserList() { return View(); }
    public IActionResult Reports()  { return View(); }
}


// Mix — controller needs login, but one action is open
[Authorize]
public class ProductController : Controller
{
    [AllowAnonymous]                        // product list is public
    public IActionResult List() { return View(); }

    public IActionResult Create() { return View(); }  // login required
    public IActionResult Delete(int id) { ... }       // login required
}
```

**Best practice:** Put `[Authorize]` on the controller level, then use
`[AllowAnonymous]` on specific actions that should be public.
This way you can never accidentally forget to protect a new action
you add later.

---

## 8. The Login View — Complete Razor

```html
@model LoginViewModel
@{
    ViewData["Title"] = "Login";
    var returnUrl = ViewData["ReturnUrl"] as string ?? "";
}

<h2>Login</h2>

@* Show validation summary — displays ModelState errors from the controller *@
<div asp-validation-summary="ModelOnly" style="color:red;"></div>

<form asp-action="Login" asp-controller="Account" asp-route-returnUrl="@returnUrl" method="post">

    @Html.AntiForgeryToken()

    <div>
        <label asp-for="UserId">User ID</label>
        <input asp-for="UserId" autocomplete="username" />
        <span asp-validation-for="UserId" style="color:red;"></span>
    </div>

    <div>
        <label asp-for="Password">Password</label>
        <input asp-for="Password" autocomplete="current-password" />
        <span asp-validation-for="Password" style="color:red;"></span>
    </div>

    <button type="submit">Login</button>

</form>
```

`asp-validation-summary="ModelOnly"` shows the `ModelState.AddModelError("", "...")`
messages from the controller — "Invalid credentials", "Account is locked", etc.

---

## 9. Adding Role to Your Users Table

Your current Users table from Chapter 3 does not have a Role column.
Add it:

```sql
ALTER TABLE Users ADD Role NVARCHAR(50) DEFAULT 'User';

-- Give admin role to the admin user:
UPDATE Users SET Role = 'Admin' WHERE UserId = 'sumit';
```

Update `sp_ValidateLogin` to also return Role:

```sql
ALTER PROCEDURE sp_ValidateLogin
    @UserId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, PasswordHash, FailedAttempts, IsLocked, Role
    FROM Users
    WHERE UserId = @UserId;
END
```

Update your `UserRecord` model:

```csharp
public class UserRecord
{
    public string UserId         { get; set; } = string.Empty;
    public string PasswordHash   { get; set; } = string.Empty;
    public int    FailedAttempts { get; set; }
    public bool   IsLocked       { get; set; }
    public string Role           { get; set; } = "User";    // ← add this
}
```

Update the `GetUserById` method in UserRepository to read Role:

```csharp
Role = reader["Role"] == DBNull.Value ? "User" : reader["Role"].ToString()!,
```

---

## 10. The Full Flow — Visualised

```
User opens /Home/Dashboard  (not logged in)
         │
         ▼
[Authorize] + no cookie
         │
         ▼
Redirect → /Account/Login?returnUrl=%2FHome%2FDashboard
         │
         ▼
User enters: sumit / abc123
         │
         ▼
POST /Account/Login
         │
         ├─ GetUserById("sumit")      ← Chapter 3: ADO.NET → sp_ValidateLogin
         ├─ user.IsLocked == false    ← Chapter 3: check
         ├─ PasswordHelper.Verify()  ← Chapter 4: BCrypt compare
         ├─ ResetFailedAttempts()     ← Chapter 3: ADO.NET → sp_Reset
         ├─ Build Claims             ← This chapter: Name, Role
         ├─ SignInAsync()            ← This chapter: create cookie
         └─ Redirect to /Home/Dashboard (returnUrl)
                  │
                  ▼
        User sees Dashboard ✓
        Cookie sent on every
        future request automatically
```

---

## Chapter Summary

| What | How |
|---|---|
| **Setup** | `AddAuthentication().AddCookie()` in Program.cs services |
| **Pipeline** | `UseSession → UseAuthentication → UseAuthorization` in order |
| **Claims** | Key-value pairs encrypted in the cookie — Name, NameIdentifier, Role |
| **SignInAsync** | Creates the auth cookie — call after password is verified |
| **SignOutAsync** | Deletes the cookie — call on logout |
| **[Authorize]** | Put on controller — no valid cookie = redirect to LoginPath |
| **[AllowAnonymous]** | Put on Login action — must not require auth |
| **ReturnUrl** | Sends user back where they came from after login |
| **Url.IsLocalUrl** | Security check — prevents open redirect attacks |
| **User.IsInRole** | Checks role from Claims — no DB query needed |

---

## What's Next — Chapter 6

**Session Management**

Your login system works now. Chapter 6 adds Sessions —
storing the user's name and role on the server so every
controller and view can access them without querying Claims.
It also covers session timeout, storing complex objects,
and clearing session properly on logout.

---

*Chapter 5 of 8 — Sumit's Login System — ASP.NET Core MVC*
