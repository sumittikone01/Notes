# Middleware in ASP.NET Core
### What It Is, How It Works, and Why Auth Depends on It

---

## Prerequisites

| What You Need | Your File |
|---|---|
| ASP.NET MVC basics | `ASP.NET / 01.IntroToASP_NET.md` |
| Program.cs structure | `API In ASP.NET / Complete asp net core api Notes.md` |
| Chapter 2 of login series — pipeline overview | `Chapter_02_MVC_Pipeline_And_Auth_Setup.md` |

---

## Why You Must Understand This

Every time you write in Program.cs:

```csharp
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
```

You are adding **middleware**. If these three lines are in the wrong order,
your entire login system breaks silently — no error, just auth not working.

You need to know what middleware is, why order matters, and how to write
a simple custom one — because you will eventually need to.

---

## 1. What is Middleware

**Simple definition:**
> Middleware is a function that sits in the path of every HTTP request.
> It can look at the request, do something, then decide to either
> pass it to the next function or stop it right there.

Think of middleware like a **series of checkpoints** at an airport:

```
You (HTTP Request) arrive at the airport
         │
         ▼
┌──────────────────┐
│  CHECK-IN DESK   │  ← verifies your booking, prints boarding pass
└────────┬─────────┘
         │  pass through
         ▼
┌──────────────────┐
│  SECURITY SCAN   │  ← checks for dangerous items, can STOP you here
└────────┬─────────┘
         │  pass through
         ▼
┌──────────────────┐
│  PASSPORT CHECK  │  ← verifies identity, can STOP you here
└────────┬─────────┘
         │  pass through
         ▼
┌──────────────────┐
│  BOARDING GATE   │  ← your actual destination (controller action)
└──────────────────┘
         │
         ▼
You board the plane (response goes back)
         │
         ▼  (response travels BACK through the same checkpoints)
┌──────────────────┐
│  PASSPORT CHECK  │  ← can add stamps on the way back
└────────┬─────────┘
         ▼
┌──────────────────┐
│  SECURITY SCAN   │  ← can log what's leaving
└────────┬─────────┘
         ▼
┌──────────────────┐
│  CHECK-IN DESK   │  ← final processing
└──────────────────┘
         │
         ▼
Response arrives at browser
```

**Key facts:**
- Requests go **in** through the pipeline
- Responses come back **out** through the same pipeline (in reverse)
- Each middleware can run code **before** passing to the next (request side)
- Each middleware can run code **after** the next one finishes (response side)
- Any middleware can **short-circuit** — stop the pipeline and return immediately

---

## 2. The Request-Response Flow — Visual

```
HTTP Request
     │
     ▼
  Middleware 1  (e.g. Exception Handler)
     │  runs before code
     │──────────────────────────────────────>
     │                                       │
     │                             Middleware 2  (e.g. HTTPS Redirect)
     │                                       │──────────────────>
     │                                       │                  │
     │                                       │         Middleware 3  (e.g. Auth)
     │                                       │                  │──────────>
     │                                       │                  │          │
     │                                       │                  │   Controller Action
     │                                       │                  │      runs here
     │                                       │                  │          │
     │                                       │                  │<──────────
     │                                       │                  │  runs after code
     │                                       │<──────────────────
     │                                       │  runs after code
     │<──────────────────────────────────────
     │  runs after code
     ▼
HTTP Response sent to browser
```

Each middleware wraps the rest of the pipeline.
This is how exception handling works — it wraps everything,
so if anything inside throws an exception, it can catch it.

---

## 3. The next() Function — The Key Concept

Every middleware receives a `next` function. Calling `next()` means
"pass the request to the next middleware in the chain."
Not calling `next()` means "stop here and return a response immediately."

```csharp
// The shape of every middleware — conceptually
public async Task InvokeAsync(HttpContext context, RequestDelegate next)
{
    // Code here runs BEFORE the rest of the pipeline (on the request going IN)

    await next(context);   // ← pass to the next middleware

    // Code here runs AFTER the rest of the pipeline (on the response coming OUT)
}
```

**If you don't call `await next(context)`, the pipeline stops.**
The request never reaches the controller. The middleware returns its own response.

This is exactly how `UseAuthorization` works for unauthenticated requests:
- It checks for the auth cookie
- No cookie found → it does NOT call next()
- Instead it returns a 302 redirect to /Account/Login
- Your controller never even runs

---

## 4. Built-in Middleware — What Each One Does

These are the ones you register in Program.cs. Now you know what each actually does.

```
app.UseExceptionHandler("/Home/Error")
    │
    └── Wraps entire pipeline in try-catch
        If anything inside throws an exception:
          - catches it
          - returns /Home/Error page
          - prevents 500 error from reaching the browser as raw text


app.UseHttpsRedirection()
    │
    └── Checks if request came in over http://
        If yes: returns 301 redirect to https://
        Does not call next() for http requests


app.UseStaticFiles()
    │
    └── Checks if the URL matches a file in wwwroot/
        (e.g. /css/site.css, /js/app.js, /images/logo.png)
        If match found: serves the file directly, does NOT call next()
        If no match: calls next() — passes to routing
        ↑ This is why static files are fast — they never reach a controller


app.UseRouting()
    │
    └── Reads the URL and figures out WHICH controller/action it maps to
        Does not run the controller yet — just selects it
        UseAuthorization needs this to know which endpoint's [Authorize] to check


app.UseSession()
    │
    └── Loads the session from the session store using the session ID cookie
        Makes HttpContext.Session available for the rest of the pipeline
        MUST come before UseAuthentication


app.UseAuthentication()
    │
    └── Reads the auth cookie
        Decrypts it
        Sets HttpContext.User to the authenticated user (or anonymous)
        Does NOT redirect — just sets the identity


app.UseAuthorization()
    │
    └── Looks at the selected endpoint's [Authorize] attribute
        If [Authorize] present AND HttpContext.User is not authenticated:
            → Does NOT call next()
            → Returns 302 redirect to LoginPath
        If [Authorize(Roles="Admin")] AND user is wrong role:
            → Returns 302 redirect to AccessDeniedPath
        Otherwise: calls next() and lets the controller run


app.MapControllerRoute(...)
    │
    └── Not technically middleware — maps URL patterns to controller actions
        This is the last step — actually runs your controller action
```

---

## 5. Why Order Matters — Concrete Examples

**Wrong order → broken auth, no error message:**

```csharp
// ❌ WRONG — UseAuthentication after UseAuthorization
app.UseRouting();
app.UseAuthorization();     // checks HttpContext.User... but it's not set yet!
app.UseAuthentication();    // sets HttpContext.User... too late, auth already ran

// Result: every request looks unauthenticated → every [Authorize] page redirects to login
//         even logged-in users get redirected. No error. Very confusing to debug.
```

```csharp
// ❌ WRONG — Session after Authentication
app.UseAuthentication();    // runs before session is loaded
app.UseSession();           // session loads after auth... auth can't use session

// Result: session data not available when you need it in auth
```

```csharp
// ✅ CORRECT — the only order that works
app.UseRouting();           // 1. Figure out which endpoint
app.UseSession();           // 2. Load the session
app.UseAuthentication();    // 3. Set HttpContext.User from cookie
app.UseAuthorization();     // 4. Check [Authorize] now that User is set
// Controller runs last
```

**Memory trick for the correct order:**

> **R**outing → **S**ession → **A**uth**e**ntication → **A**uth**o**rization
> "**R**eally **S**mart **A**pp **A**rchitecture"

---

## 6. Writing a Custom Middleware

There are two ways. Both do the same thing.

### Way 1 — Inline (quick, for simple logic)

```csharp
// In Program.cs, before MapControllerRoute:
app.Use(async (context, next) =>
{
    // Code runs BEFORE the rest of the pipeline
    Console.WriteLine($"→ {context.Request.Method} {context.Request.Path}");

    await next();   // pass to next middleware

    // Code runs AFTER the rest of the pipeline
    Console.WriteLine($"← {context.Response.StatusCode}");
});
```

### Way 2 — Class-based (cleaner, recommended for real logic)

```csharp
// Middleware/RequestLoggingMiddleware.cs
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;   // _next is the rest of the pipeline
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // BEFORE — request going in
        Console.WriteLine($"→ {context.Request.Method} {context.Request.Path}");

        await _next(context);   // call the next middleware

        // AFTER — response coming out
        Console.WriteLine($"← {context.Response.StatusCode}");
    }
}

// Register it in Program.cs:
app.UseMiddleware<RequestLoggingMiddleware>();
```

---

## 7. A Real Custom Middleware for Your Login System

This middleware blocks all requests from locked accounts —
even if their cookie is still valid (they logged in before being locked).

```csharp
// Middleware/AccountLockCheckMiddleware.cs
public class AccountLockCheckMiddleware
{
    private readonly RequestDelegate _next;

    public AccountLockCheckMiddleware(RequestDelegate _next)
    {
        this._next = _next;
    }

    public async Task InvokeAsync(HttpContext context, UserRepository userRepo)
    //                                                  ↑ injected by DI — same as constructor injection
    {
        // Only check for authenticated users
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            if (!string.IsNullOrEmpty(userId))
            {
                UserRecord? user = userRepo.GetUserById(userId);

                // If account got locked AFTER they logged in
                if (user?.IsLocked == true)
                {
                    // Sign them out immediately
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Session.Clear();
                    context.Response.Redirect("/Account/Login?reason=locked");
                    return;  // ← does NOT call next() — stops the pipeline
                }
            }
        }

        await _next(context);  // ← all good, continue
    }
}

// In Program.cs — after UseAuthorization:
app.UseMiddleware<AccountLockCheckMiddleware>();
```

This is a real-world use case. An admin locks a user mid-session.
Without this middleware, the user's browser still has a valid cookie
and they can keep browsing until it expires. This middleware
catches that case on every request.

---

## 8. Short-Circuit Middleware — app.Run()

`app.Run()` is a terminal middleware — it never calls `next()`.
It always ends the pipeline and sends a response.

```csharp
// This handles every single request with the same response
// (You would never do this in a real app — just showing the concept)
app.Run(async context =>
{
    await context.Response.WriteAsync("Hello from terminal middleware");
    // No next() call — pipeline ends here
});
```

`app.Run()` at the very end of Program.cs is actually `app.Run()` as in
"start the web server" — different thing, same name. Don't confuse them.

---

## 9. Middleware vs Filter vs Action — What's the Difference

You have `[ValidateAntiForgeryToken]` and `[Authorize]` on controller actions.
Those are **Filters**, not Middleware. They are similar but different.

| | Middleware | Filter |
|---|---|---|
| **Runs for** | Every request, even static files | Only for controller actions |
| **Defined in** | Program.cs (`app.Use...`) | Attribute on controller/action |
| **Access to** | HttpContext only | HttpContext + controller context + action info |
| **When to use** | Cross-cutting concerns for all requests (auth, logging, CORS) | Logic specific to MVC actions (validation, result modification) |
| **Examples** | `UseAuthentication`, `UseSession`, custom logging | `[Authorize]`, `[ValidateAntiForgeryToken]`, `[Produces]` |

**For your login system:**
- Middleware = `UseAuthentication`, `UseSession`, `AccountLockCheck`
- Filters = `[Authorize]`, `[ValidateAntiForgeryToken]`, `[AllowAnonymous]`

---

## 10. HttpContext — The Object Everything Shares

Every middleware and every controller action shares one object: `HttpContext`.

It holds everything about the current request and response.

```csharp
HttpContext.Request.Method          // "GET", "POST"
HttpContext.Request.Path            // "/Account/Login"
HttpContext.Request.Query["id"]     // ?id=5
HttpContext.Request.Cookies["name"] // cookie value
HttpContext.Request.Headers["Authorization"] // header value

HttpContext.Response.StatusCode     // 200, 302, 401
HttpContext.Response.Redirect("/Login") // send redirect

HttpContext.User                    // the logged-in user (ClaimsPrincipal)
HttpContext.User.Identity.Name      // "Sumit"
HttpContext.User.IsInRole("Admin")  // true/false

HttpContext.Session                 // session data store
HttpContext.Session.SetString("key", "value")
HttpContext.Session.GetString("key")

HttpContext.Connection.RemoteIpAddress // user's IP address
```

Middleware receives `HttpContext` directly.
In controllers, you access it via `HttpContext` property (inherited from `Controller`).
In Razor views, you access it via `@Context`.

---

## Summary

| Concept | One-Line Meaning |
|---|---|
| **Middleware** | A function in the pipeline that every request passes through |
| **next()** | Passes the request to the next middleware — if you skip it, pipeline stops |
| **Short-circuit** | Middleware returns a response without calling next() — used by auth for redirects |
| **Order** | Routing → Session → Authentication → Authorization — wrong order breaks auth silently |
| **UseAuthentication** | Reads the cookie, sets HttpContext.User |
| **UseAuthorization** | Checks [Authorize], redirects if not allowed |
| **UseSession** | Loads session data — must be before UseAuthentication |
| **Custom middleware** | `app.Use()` inline or a class with `InvokeAsync(HttpContext, RequestDelegate)` |
| **HttpContext** | The shared object every middleware and controller uses to read request/write response |
| **Middleware vs Filter** | Middleware = all requests; Filter = controller actions only |

---

*Middleware Notes — Sumit's ASP.NET Core MVC Learning Journey*
