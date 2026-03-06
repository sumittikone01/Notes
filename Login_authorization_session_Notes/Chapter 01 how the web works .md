# Chapter 1 — How the Web Works for Authentication

### The Foundation You Need Before Writing a Single Login Line

---

## Prerequisites — What You Already Have

You already have these notes. Make sure you are comfortable with them
before this chapter — because auth builds directly on top of them.

| What You Need to Know                                   | Your Notes File                                          |
| ------------------------------------------------------- | -------------------------------------------------------- |
| ASP.NET MVC basics — Controller, Action, View, Routing | `ASP.NET / 01.IntroToASP_NET.md`                       |
| Sending data from Controller to View and back           | `ASP.NET / 02.Passing data from Controller to View.md` |
| Sending data from View to Controller (forms)            | `ASP.NET / 03.Passing Data From View To Controller.md` |
| SqlConnection, SqlCommand basics                        | `ADO.NET / 06SqlConnection.md` + `07SqlCommand.md`   |
| SqlParameters — preventing SQL injection               | `ADO.NET / 09SqlParameters.md`                         |
| CRUD with ADO.NET (INSERT, UPDATE, SELECT)              | `ADO.NET / 14.CRUDOperationsInDA.md`                   |
| Middleware and Dependency Injection in ASP.NET          | `API In ASP.NET / Complete asp net core api Notes.md`  |

**High-level CS knowledge is enough to start.**
You already know what a cookie is, what a session roughly does,
what login means — from general CS knowledge. Good. This chapter
makes that vague knowledge concrete, code-level, and ready to build with.
No deep theory. Just what you actually need.

---

## The One Problem That Drives Everything

Here is the single sentence that explains why authentication, cookies,
and sessions were invented:

> **HTTP has no memory. Every request arrives as a complete stranger.**

You know HTTP. Browser sends request, server sends response. Simple.

What you may not have stopped to think about is this:

```
You open the browser. You go to your company's app. You log in.
You click "My Dashboard".

How does the server know that the "My Dashboard" request
is coming from YOU — the person who just logged in —
and not from some random anonymous person on the internet?
```

The answer is **not** that HTTP remembers you.
HTTP already forgot you the moment it sent back the login page.

That is the problem. Cookie and Session are the solution.

---

## 1. HTTP is Stateless — What That Actually Means

**Stateless** = the server remembers nothing between requests.

Each request is handled, response is sent, connection is closed.
That's it. The server has zero memory of you.

```
Browser                                 Server
  │                                        │
  │──── GET /Login ───────────────────>    │
  │                                        │  "Here's the login page"
  │<─── 200 OK (login page HTML) ──────    │  *server forgets you*
  │                                        │
  │──── POST /Login {user, password} ─>    │
  │                                        │  "Login OK, go to dashboard"
  │<─── 302 Redirect to /Dashboard ────    │  *server forgets you again*
  │                                        │
  │──── GET /Dashboard ───────────────>    │
  │                                        │  "...who are you? I don't know you."
```

That last request — the server genuinely does not know who you are.
It just sees an HTTP request. Anonymous. No identity attached.

**This is not a bug. This is how HTTP was designed.**
The fix was built on top of it — using Cookies.

---

## 2. Cookie — The Browser's ID Card

**What it is:**

> A cookie is a small piece of text the server puts in your browser,
> and the browser automatically sends it back on every future request.

You do not write JavaScript to send cookies.
You do not think about it. The browser just does it.

```
─────────────────── AFTER SUCCESSFUL LOGIN ───────────────────

Server says in the HTTP response header:
┌─────────────────────────────────────────────────────────┐
│  Set-Cookie: .auth=CfDJ8Kx92mAbc...;  HttpOnly; Secure │
└─────────────────────────────────────────────────────────┘
                 ↑
         This is the auth cookie.
         It is encrypted — nobody can read what's inside.

Browser receives this. Stores it silently.

─────────────────── EVERY FUTURE REQUEST ─────────────────────

Browser automatically adds to every request:
┌─────────────────────────────────────────────────────────┐
│  Cookie: .auth=CfDJ8Kx92mAbc...                        │
└─────────────────────────────────────────────────────────┘
                 ↑
         Server reads this → "I know this person. It's Sumit."
```

### Real-World Analogy

Think of a cookie like the **stamp on your hand** at a club entrance.

- Bouncer checks your ID once when you enter (login)
- Bouncer stamps your hand (sets cookie)
- Every time you go in and out, you just show your hand (cookie is sent automatically)
- No need to show ID again (no need to login again)
- Stamp wears off at end of night (cookie expires on logout or timeout)

---

## 3. Session — Storing Your Data on the Server

A cookie by itself only proves you are logged in.
But your app needs to know your **name, role, user ID** on every page.

You could store all that inside the cookie — but cookies are tiny (4KB max),
and putting sensitive data in the browser is risky.

**Better approach: Session.**

**What it is:**

> A session is a temporary storage box created on the server for each
> logged-in user. The browser only carries the box's key (a session ID cookie).
> All actual data stays on the server.

```
SERVER MEMORY (Session Store)
┌──────────────────────────────────────────────────────┐
│                                                      │
│  Key: "abc123"                                       │
│  ┌────────────────────────────────┐                  │
│  │  UserId   = "sumit"            │  ← Sumit's data  │
│  │  UserName = "Sumit Sharma"     │                  │
│  │  Role     = "Admin"            │                  │
│  └────────────────────────────────┘                  │
│                                                      │
│  Key: "xyz789"                                       │
│  ┌────────────────────────────────┐                  │
│  │  UserId   = "rahul"            │  ← Rahul's data  │
│  │  UserName = "Rahul Verma"      │                  │
│  │  Role     = "User"             │                  │
│  └────────────────────────────────┘                  │
│                                                      │
└──────────────────────────────────────────────────────┘

SUMIT'S BROWSER
┌──────────────────────────────────┐
│  Cookie: .Session = "abc123"     │  ← just the key, not the data
└──────────────────────────────────┘
```

The browser carries only the **key** (session ID).
The **data** lives safely on the server.

### Real-World Analogy

Think of session like a **coat check at a restaurant**.

- You arrive and hand over your coat (your user data after login)
- They give you a small numbered ticket (the session cookie)
- You carry only the ticket number — not the coat
- When you need your coat back, show the ticket → they find your coat
- When you leave (logout), they clear out your hook (session cleared)

---

## 4. Cookie vs Session — How They Work Together

In your project you will use **both**. They do different jobs.

|                             | Auth Cookie                    | Session                             |
| --------------------------- | ------------------------------ | ----------------------------------- |
| **Job**               | Proves you are logged in       | Stores your data for easy access    |
| **Data lives**        | Encrypted inside the cookie    | On the server                       |
| **Browser stores**    | The encrypted proof            | Just the session ID key             |
| **Expires**           | Set by you (e.g. 1 day)        | Clears after inactivity (~20 min)   |
| **In your code**      | `HttpContext.SignInAsync()`  | `HttpContext.Session.SetString()` |
| **Cleared on logout** | `HttpContext.SignOutAsync()` | `HttpContext.Session.Clear()`     |

**They work as a pair:**

- Auth cookie → Is this person logged in? Yes/No.
- Session → If yes, who are they and what do I show them?

---

## 5. Authentication vs Authorization

These two words are everywhere in login systems.
They look similar. They mean completely different things.

### Authentication = Proving WHO you are

```
Sumit enters: userId = "sumit", password = "abc123"

Server checks DB with ADO.NET stored procedure:
  → Does user "sumit" exist?        YES
  → Does the password hash match?   YES
  → Is the account locked?          NO

Result: "Identity confirmed. You are Sumit."
        Auth cookie is created.
        You are now AUTHENTICATED.
```

### Authorization = Checking WHAT you can access

```
Sumit (authenticated) tries to open /Admin/UserList

Server: "Sumit is logged in ✓"
Server: "But /Admin/UserList needs Role = 'Admin'"
Server: "Sumit's Role = 'User'"

Result: 403 Forbidden.
        Sumit is authenticated but NOT AUTHORIZED for this page.
```

### The Flow

```
Request arrives at a protected page
           │
           ▼
   Is there an auth cookie?
           │
    NO ────┴──── Redirect to /Login      ← Authentication failed
           │
          YES
           │
           ▼
   Does the user have the required Role?
           │
    NO ────┴──── Show 403 Forbidden      ← Authorization failed
           │
          YES
           │
           ▼
   Show the page ✓
```

**Memory shortcut:**

> **Authen**tication = **Authen**tic ID card. Are you really who you say you are?
> **Author**ization = **Author**ity. Do you have the authority to be here?

---

## 6. The [Authorize] Attribute — Your Bouncer

In ASP.NET Core MVC, you protect a page with one attribute:

```csharp
[Authorize]
public class DashboardController : Controller
{
    // Only logged-in users can reach any action here
}

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // Only users with Role = "Admin" can reach this
}
```

**That's it.** You write `[Authorize]` and ASP.NET Core handles the rest.
No cookie check code. No redirect code. The middleware does it for you.

Where does this magic happen? In **Program.cs** — the middleware pipeline.
That is Chapter 2.

---

## 7. The Complete Login Flow in Your App

This is exactly what you will build. Every step maps to code.

```
────────────────────────────────────────────────────
STEP 1 — User tries to open a protected page (not logged in)
────────────────────────────────────────────────────

Browser: GET /Dashboard
Middleware: No auth cookie found
Middleware: Redirect to /Account/Login   ← automatic, you write nothing


────────────────────────────────────────────────────
STEP 2 — User submits the login form
────────────────────────────────────────────────────

Browser: POST /Account/Login
Body: { userId: "sumit", password: "abc123" }

LoginController receives this (exactly like your View→Controller notes)
Calls ADO.NET → stored procedure sp_ValidateLogin

sp_ValidateLogin returns:
  → User found?          YES
  → Password matches?    YES
  → Account locked?      NO

Controller:
  → Call sp_ResetFailedAttempts
  → HttpContext.SignInAsync(...)     ← creates auth cookie
  → HttpContext.Session.SetString("UserId", "sumit")   ← stores in session
  → Redirect to /Dashboard


────────────────────────────────────────────────────
STEP 3 — Wrong password
────────────────────────────────────────────────────

sp_ValidateLogin returns: password wrong
Controller:
  → Call sp_IncrementFailedAttempts
  → If attempts >= 3: call sp_LockUser
  → Return View with error message "Invalid credentials"


────────────────────────────────────────────────────
STEP 4 — User is logged in, visits any page
────────────────────────────────────────────────────

Browser: GET /Dashboard
         Cookie: .auth=CfDJ8...   ← sent automatically

Middleware reads cookie → valid → user is Sumit
[Authorize] passes → controller runs
Controller: var name = HttpContext.Session.GetString("UserName")
View shows: "Welcome, Sumit"


────────────────────────────────────────────────────
STEP 5 — Logout
────────────────────────────────────────────────────

Browser: POST /Account/Logout

LogoutController:
  → HttpContext.SignOutAsync()      ← deletes auth cookie
  → HttpContext.Session.Clear()     ← wipes session data
  → Redirect to /Account/Login
```

---

## 8. HttpOnly and Secure — Two Cookie Settings You Must Use

When the auth cookie is created, you will set these two properties.
Know why they exist.

```
Set-Cookie: .auth=CfDJ8Kx92m...; HttpOnly; Secure; SameSite=Strict
                                    │         │
                                    │         └── Only send over HTTPS
                                    └──────────── JavaScript CANNOT read this
```

**HttpOnly is critical.**

Without it: A script injected into your page by an attacker
           could run `document.cookie` and steal the auth cookie.

With HttpOnly: JavaScript literally cannot access the cookie at all.
              Only the browser and the server can use it.

**You will set this once in Program.cs. After that, forget it.
ASP.NET Core handles it automatically.**

---

## 9. HTTP Status Codes You Will See in Auth Work

| Code                          | Meaning           | When you see it                             |
| ----------------------------- | ----------------- | ------------------------------------------- |
| `200 OK`                    | All good          | Login page loads, dashboard loads           |
| `302 Found`                 | Go here instead   | After login → redirect to dashboard        |
| `401 Unauthorized`          | Not logged in     | No auth cookie, middleware redirects        |
| `403 Forbidden`             | Wrong role        | Has cookie but lacks required role          |
| `500 Internal Server Error` | Something crashed | Exception in controller or stored procedure |

---

## 10. How All Your Previous Notes Connect to This

This is the most important thing to understand.
Auth is **not** a new subject. It is your existing knowledge
with a cookie and session layer on top.

```
Your ADO.NET notes (06, 07, 09, 14)
  └─→ You will write sp_ValidateLogin, sp_LockUser, sp_ResetAttempts
      Using SqlConnection + SqlCommand + SqlParameters
      Exactly the same pattern as your CRUD stored procedures

Your ASP.NET notes (02, 03 — passing data)
  └─→ Login form submits userId + password to LoginController
      Exactly the same as any other form you've built

Your API notes (Middleware, DI)
  └─→ You register auth middleware in Program.cs
      app.UseAuthentication() and app.UseAuthorization()
      Same middleware pipeline you already know
```

Nothing here is completely new.
You are connecting things you already know
with a layer called authentication on top.

---

## Chapter Summary

| Concept                     | One-Line Meaning                                                    |
| --------------------------- | ------------------------------------------------------------------- |
| **HTTP is stateless** | Server forgets you after every response                             |
| **Cookie**            | Hand stamp — browser carries it, server reads it every request     |
| **Session**           | Coat check — server holds your data, browser has the ticket number |
| **Authentication**    | Proving WHO you are — login                                        |
| **Authorization**     | What you are ALLOWED to access — roles                             |
| **[Authorize]**       | One attribute that protects an entire controller                    |
| **SignInAsync**       | Creates the auth cookie                                             |
| **SignOutAsync**      | Deletes the auth cookie                                             |
| **Session.SetString** | Stores user info on the server                                      |
| **HttpOnly**          | JavaScript cannot steal the cookie                                  |

---

## What's Next — Chapter 2

**ASP.NET Core MVC — The Request Pipeline Deep Dive**

You know the basics of MVC from your notes.
Chapter 2 goes into the **Middleware Pipeline** specifically —
where exactly Authentication lives in the pipeline,
what order things run in, and how `[Authorize]` actually
intercepts a request before your controller even runs.
That is where you will set up your login system's backbone.

---

*Chapter 1 of 8 — Sumit's Login System — ASP.NET Core MVC*
