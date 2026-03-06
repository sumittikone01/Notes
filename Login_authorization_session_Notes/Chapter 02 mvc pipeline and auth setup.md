# Chapter 2 — ASP.NET Core MVC Request Pipeline

### Where Authentication Lives and How Every Request Flows

---

## Prerequisites

| What You Need                                     | Your File                                               |
| ------------------------------------------------- | ------------------------------------------------------- |
| ASP.NET MVC basics — Controller, Action, Routing | `ASP.NET / 01.IntroToASP_NET.md`                      |
| Middleware and DI basics                          | `API In ASP.NET / Complete asp net core api Notes.md` |
| Chapter 1 of this series                          | `Chapter_01_How_The_Web_Works.md`                     |

From Chapter 1 you know: HTTP is stateless, cookies carry identity,
`[Authorize]` protects pages. Now you need to understand **where** all
of that fits inside the actual request flow — before you can wire it up.

---

## The Core Question This Chapter Answers

You write `[Authorize]` on a controller. A user without a cookie requests
that page. Who catches that? When? How does the redirect to `/Login` happen?

The answer is: **Middleware**. And it runs before your controller even starts.

---

## 1. The Request Pipeline — The Real Picture

Every single HTTP request in ASP.NET Core goes through a **pipeline** —
a chain of middleware components that each get a chance to inspect,
modify, or stop the request before it reaches your controller.

```
HTTP Request from Browser
         │
         ▼
┌─────────────────────────────┐
│   Exception Handler         │  ← catches any crash, returns clean error
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   HTTPS Redirection         │  ← forces http → https
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   Static Files              │  ← serves CSS, JS, images (stops here if found)
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   Session                   │  ← loads the session from the store
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   Authentication ◄──────────┼── reads the auth cookie, identifies the user
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   Authorization ◄───────────┼── checks [Authorize] — allowed or redirect?
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   Routing                   │  ← matches URL to controller/action
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   YOUR CONTROLLER ACTION    │  ← your code finally runs here
└──────────────┬──────────────┘
               │
               ▼
         HTTP Response
         goes back UP
         through the same
         pipeline
```

**Key point:** Your controller is at the bottom. By the time your code runs,
authentication has already identified the user and authorization has already
decided they are allowed in. You don't check cookies yourself. Ever.

---

## 2. Program.cs — Where You Configure the Pipeline

You already know Program.cs from your API notes. Here is what it looks like
specifically for a login system — annotated line by line.

```csharp
var builder = WebApplication.CreateBuilder(args);

// ── SECTION 1: Register services (what the app can use) ──────────────

builder.Services.AddControllersWithViews();     // MVC controllers + Razor views

builder.Services.AddSession(options =>          // session service
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session expires after 30 min idle
    options.Cookie.HttpOnly = true;             // session cookie: JS cannot read it
    options.Cookie.IsEssential = true;          // works even if user declined cookies
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // redirect here if not logged in
        options.AccessDeniedPath = "/Account/AccessDenied"; // redirect if wrong role
        options.ExpireTimeSpan = TimeSpan.FromHours(8);     // cookie lasts 8 hours
        options.Cookie.HttpOnly = true;         // JS cannot steal the auth cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
    });

var app = builder.Build();

// ── SECTION 2: Configure the pipeline (order matters!) ───────────────

app.UseExceptionHandler("/Home/Error");  // catches unhandled exceptions
app.UseHttpsRedirection();               // http → https
app.UseStaticFiles();                    // wwwroot files (CSS, JS, images)

app.UseRouting();                        // ← routing must come BEFORE auth

app.UseSession();                        // ← session BEFORE authentication
app.UseAuthentication();                 // ← reads the cookie, sets User identity
app.UseAuthorization();                  // ← checks [Authorize] attributes

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### Why Order Matters

```
UseSession()        must come BEFORE UseAuthentication()
                    (session must be loaded before auth can use it)

UseAuthentication() must come BEFORE UseAuthorization()
                    (must know WHO you are before checking IF you're allowed)

UseRouting()        must come BEFORE UseAuthorization()
                    (must know WHICH endpoint before checking its [Authorize])
```

This is one of the most common bugs beginners make — wrong order in Program.cs.
Bookmark this. If auth is not working, check your order first.

---

## 3. What UseAuthentication Actually Does

When `UseAuthentication()` runs on every request, here is what happens:

```
Request arrives with Cookie: .auth=CfDJ8Kx92m...
         │
         ▼
UseAuthentication reads the cookie
         │
         ▼
Decrypts it (it's encrypted by ASP.NET Core automatically)
         │
         ▼
Finds the Claims inside:
   - UserId = "sumit"
   - Role   = "Admin"
         │
         ▼
Sets HttpContext.User to a ClaimsPrincipal representing Sumit
         │
         ▼
Now EVERY piece of code downstream knows who the user is
via: HttpContext.User.Identity.Name
     HttpContext.User.IsInRole("Admin")
```

If there is **no cookie** (or it's expired/invalid):

```
Request arrives with no cookie
         │
         ▼
UseAuthentication: no cookie found
         │
         ▼
HttpContext.User = anonymous (not authenticated)
         │
         ▼
UseAuthorization sees [Authorize] on the controller
         │
         ▼
Automatically redirects to /Account/Login   (the LoginPath you configured)
```

**You never write this redirect logic yourself.**
You just configure `LoginPath` once in Program.cs and `[Authorize]` everywhere else.

---

## 4. Claims — What Gets Stored in the Cookie

A **Claim** is a key-value pair of information about the user that gets
encrypted into the auth cookie.

```
Think of Claims like the fields on your ID card:
┌──────────────────────────────────┐
│  Name:   Sumit Sharma            │  ← Claim: Name = "Sumit Sharma"
│  Role:   Admin                   │  ← Claim: Role = "Admin"
│  UserId: sumit                   │  ← Claim: NameIdentifier = "sumit"
└──────────────────────────────────┘
```

When you create the auth cookie (after login), you build a list of Claims:

```csharp
// After successful login — building the claims
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name,           "Sumit Sharma"),  // display name
    new Claim(ClaimTypes.NameIdentifier, "sumit"),         // user ID
    new Claim(ClaimTypes.Role,           "Admin"),         // role
};

var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
var principal = new ClaimsPrincipal(identity);

// This creates the encrypted cookie and sends it to the browser
await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    principal);
```

After that, anywhere in your app you can read these claims:

```csharp
// In any controller action, after user is logged in:
string name   = User.Identity!.Name;                          // "Sumit Sharma"
string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // "sumit"
bool isAdmin  = User.IsInRole("Admin");                       // true
```

**`User` is ASP.NET Core's shortcut for `HttpContext.User`.**
It is available in every controller without any extra code.

---

## 5. [Authorize] Attribute — All the Ways You Use It

```csharp
// Protect entire controller — all actions require login
[Authorize]
public class DashboardController : Controller { }


// Protect entire controller — AND require specific role
[Authorize(Roles = "Admin")]
public class AdminController : Controller { }


// Protect a single action
public class ProductController : Controller
{
    [Authorize]                          // login required for this action only
    public IActionResult Create() { }

    public IActionResult List() { }     // this action is public (no attribute)
}


// Allow anonymous access — overrides [Authorize] on the controller
[Authorize]
public class AccountController : Controller
{
    [AllowAnonymous]                     // login page itself must NOT require login
    public IActionResult Login() { }

    public IActionResult Logout() { }   // this one requires login (inherits [Authorize])
}


// Multiple roles — user needs at least one of these
[Authorize(Roles = "Admin,Manager")]
public IActionResult Reports() { }
```

---

## 6. The Login Controller Structure

This is the full shape of your `AccountController`.
Not all the code yet — just the structure so you can see how it fits.

```csharp
public class AccountController : Controller
{
    private readonly IConfiguration _config;       // to read connection string
    // (your service/repository class will go here)

    public AccountController(IConfiguration config)
    {
        _config = config;
    }

    // ── Shows the login page ─────────────────────────────────────────────
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        // If already logged in, skip to dashboard
        if (User.Identity!.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // ── Handles login form submission ────────────────────────────────────
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // 1. Call ADO.NET → sp_ValidateLogin  (Chapter 3)
        // 2. Hash password and compare          (Chapter 4)
        // 3. SignInAsync if valid               (Chapter 5)
        // 4. Set session                        (Chapter 6)

        return RedirectToAction("Index", "Home");
    }

    // ── Logout ───────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
```

---

## 7. LoginViewModel — The Form Data

You already know ViewModels from your passing-data notes.
The login form needs a model just like any other form.

```csharp
public class LoginViewModel
{
    [Required(ErrorMessage = "User ID is required")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]      // tells Razor to render as <input type="password">
    public string Password { get; set; } = string.Empty;
}
```

And the Razor login View:

```html
@model LoginViewModel

<form asp-action="Login" asp-controller="Account" method="post">

    @Html.AntiForgeryToken()  <!-- CSRF protection — include on every POST form -->

    <div>
        <label asp-for="UserId">User ID</label>
        <input asp-for="UserId" />
        <span asp-validation-for="UserId"></span>
    </div>

    <div>
        <label asp-for="Password">Password</label>
        <input asp-for="Password" />
        <span asp-validation-for="Password"></span>
    </div>

    <button type="submit">Login</button>

</form>
```

This is exactly the same View-to-Controller data passing pattern
from your `03.Passing Data From View To Controller.md` notes.
Nothing new here — just a different form.

---

## 8. Reading User Info After Login

After login, on any protected page, you can access the logged-in user's data:

```csharp
// In any controller — from the auth cookie claims
string name   = User.Identity!.Name;
bool   isAuth = User.Identity.IsAuthenticated;
bool   isAdmin = User.IsInRole("Admin");

// In any controller — from session (Chapter 6)
string userId = HttpContext.Session.GetString("UserId") ?? "";
string role   = HttpContext.Session.GetString("Role")   ?? "";

// In Razor views — @User is available directly
@if (User.Identity!.IsAuthenticated)
{
    <p>Welcome, @User.Identity.Name</p>
}

@if (User.IsInRole("Admin"))
{
    <a href="/Admin">Admin Panel</a>
}
```

---

## 9. Anti-Forgery Token — CSRF Protection

You will see `@Html.AntiForgeryToken()` on every POST form.
Know why it exists.

**CSRF (Cross-Site Request Forgery):** An attacker tricks your browser into
making a request to your app without you knowing —
e.g., a malicious website that silently submits a form to your app.

Since your browser would send the auth cookie automatically,
the server would think it's a legitimate request from you.

**The fix:** ASP.NET Core generates a hidden random token in the form.
The server checks this token on every POST.
The attacker cannot know this token, so their fake request is rejected.

```csharp
// On any POST action that should be CSRF-protected:
[ValidateAntiForgeryToken]
[HttpPost]
public async Task<IActionResult> Login(LoginViewModel model) { }
```

**Add `[ValidateAntiForgeryToken]` to every POST action in your login system.**
Add `@Html.AntiForgeryToken()` or `asp-antiforgery="true"` to every POST form.

---

## Chapter Summary

| Concept                       | One-Line Meaning                                                      |
| ----------------------------- | --------------------------------------------------------------------- |
| **Middleware pipeline** | Every request passes through a chain before reaching your controller  |
| **Order in Program.cs** | Session → Authentication → Authorization. Wrong order = broken auth |
| **UseAuthentication**   | Reads the cookie, sets `HttpContext.User` on every request          |
| **UseAuthorization**    | Checks `[Authorize]` attributes, redirects if not allowed           |
| **Claims**              | Key-value pairs about the user, encrypted inside the auth cookie      |
| **SignInAsync**         | Creates the encrypted auth cookie from your claims                    |
| **[Authorize]**         | Protects a controller/action — no cookie = redirect to login         |
| **[AllowAnonymous]**    | Overrides [Authorize] — used on Login action itself                  |
| **Anti-Forgery Token**  | Prevents fake POST requests from other websites                       |
| **LoginPath**           | Where middleware sends unauthenticated users (set once in Program.cs) |

---

## What's Next — Chapter 3

**ADO.NET + Stored Procedures for the Login System**

Now you understand the pipeline and where your LoginController fits in.
Chapter 3 writes the actual database code — the stored procedures and
ADO.NET calls that make `sp_ValidateLogin`, `sp_LockUser`, and
`sp_ResetAttempts` work. This is your existing ADO.NET knowledge
applied directly to the login use case.

---

*Chapter 2 of 8 — Sumit's Login System — ASP.NET Core MVC*
