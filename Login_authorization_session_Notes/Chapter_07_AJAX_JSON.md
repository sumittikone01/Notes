# Chapter 7 — AJAX + JSON in ASP.NET Core MVC
### Making Your Login System Work Without Full Page Reloads

---

## Prerequisites

| What You Need | Your File / Chapter |
|---|---|
| Razor Views, forms, ViewBag | `ASP.NET / 02 & 03 Passing Data notes` |
| Controller actions, IActionResult | `ASP.NET / 01.IntroToASP_NET.md` |
| Chapter 5 — Login controller complete | ✓ |
| Chapter 6 — Session complete | ✓ |
| Basic JavaScript (variables, functions) | General knowledge |

---

## 1. What is AJAX and Why You Need It

Right now your login form does a **full page reload** on every submit.
User fills form → clicks Login → entire page refreshes → new page loads.

That works. But it feels old. And for things like the admin "Unlock User" button,
a full page reload just to change one value is unnecessary and slow.

**AJAX** (Asynchronous JavaScript and XML — ignore the XML part, it's JSON now)
lets your JavaScript send an HTTP request to the server **in the background**,
get a response, and update just part of the page — all without any reload.

```
WITHOUT AJAX (traditional form):
User clicks "Unlock" button
    → Entire page reloads
    → Server runs action
    → New page comes back
    → User scrolls back to where they were
    → Jarring, slow experience

WITH AJAX:
User clicks "Unlock" button
    → JavaScript sends request in background
    → Server runs action
    → Server returns small JSON: { "success": true }
    → JavaScript updates just that row in the grid
    → Smooth, instant, no reload
```

**In your login system you will use AJAX for:**
- Unlock User button in the admin Kendo grid
- Any form action where you want inline error messages
- Loading grid data (Kendo handles this with AJAX internally)

---

## 2. How AJAX Works — The Mental Model

```
Browser                                    Server
   │                                          │
   │  JavaScript sends HTTP request           │
   │  (in background, page stays open)        │
   │ ──── POST /Admin/UnlockUser ──────────>  │
   │       body: { userId: "sumit" }          │
   │                                          │  Controller action runs
   │                                          │  ADO.NET → sp_UnlockUser
   │                                          │  Returns JSON
   │ <──── { "success": true, ──────────────  │
   │         "message": "User unlocked" }     │
   │                                          │
   │  JavaScript receives JSON                │
   │  Updates the page (no reload)            │
```

The controller action looks **almost identical** to a normal action.
The only difference: instead of `return View()`, you `return Json(...)`.

---

## 3. Returning JSON from a Controller

```csharp
// Normal action — returns a View (HTML page)
public IActionResult Index()
{
    return View();
}

// AJAX action — returns JSON data
public IActionResult UnlockUser(string userId)
{
    // Do the work...
    _userRepo.UnlockUser(userId);

    // Return JSON instead of a View
    return Json(new { success = true, message = "User unlocked successfully." });
}
```

`return Json(...)` takes any C# object, serializes it to JSON automatically,
and sends it back with `Content-Type: application/json`.

The browser's JavaScript receives it as a JavaScript object.

---

## 4. Two Ways to Send AJAX — fetch() and jQuery $.ajax()

### Way 1 — fetch() — Modern JavaScript (no library needed)

```javascript
fetch('/Admin/UnlockUser', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({ userId: 'sumit' })
})
.then(response => response.json())   // parse the JSON response
.then(data => {
    if (data.success) {
        console.log(data.message);   // "User unlocked successfully."
    }
})
.catch(error => {
    console.error('Request failed:', error);
});
```

### Way 2 — jQuery $.ajax() — Kendo-friendly (you'll use this more)

Kendo UI already includes jQuery. Since you're using Kendo, use `$.ajax()`.

```javascript
$.ajax({
    url:         '/Admin/UnlockUser',
    type:        'POST',
    contentType: 'application/json',
    data:        JSON.stringify({ userId: 'sumit' }),
    success: function(data) {
        if (data.success) {
            alert(data.message);   // or update the grid row
        }
    },
    error: function(xhr, status, error) {
        alert('Something went wrong: ' + error);
    }
});
```

**Both do the exact same thing.** Use `$.ajax()` since Kendo includes jQuery anyway.

---

## 5. The Anti-Forgery Token Problem with AJAX

Every POST in your MVC app needs the anti-forgery token (Chapter 2).
Normal forms include it with `@Html.AntiForgeryToken()`.

With AJAX — you are not submitting a form. So you need to send the token manually.

**The fix — two steps:**

**Step 1:** Put the token in your layout or page (it generates a hidden field):

```html
@* In your _Layout.cshtml or the specific view *@
@Html.AntiForgeryToken()

@* OR — generate it as a meta tag that JavaScript can read: *@
<meta name="csrf-token" content="@Antiforgery.GetAndStoreTokens(Context).RequestToken" />
```

**Step 2:** Include the token in every AJAX POST request:

```javascript
// Read the token from the hidden field generated by @Html.AntiForgeryToken()
var token = $('input[name="__RequestVerificationToken"]').val();

$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: {
        userId:                     'sumit',
        __RequestVerificationToken: token    // ← include the token
    },
    success: function(data) { ... }
});
```

**Simpler approach — send all form data including token:**

```javascript
// If you have a form with @Html.AntiForgeryToken() in it:
var formData = $('#myForm').serialize();   // includes all fields + the token

$.ajax({
    url:  '/Admin/SomeAction',
    type: 'POST',
    data: formData,    // token is already in here
    success: function(data) { ... }
});
```

---

## 6. Controller Action for AJAX — Full Pattern

This is the complete pattern for any AJAX-called action in your admin panel.

```csharp
// Controllers/AdminController.cs

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserRepository _userRepo;

    public AdminController(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // ── AJAX: Unlock a user ──────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UnlockUser(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Json(new { success = false, message = "User ID is required." });

            _userRepo.UnlockUser(userId);

            return Json(new { success = true, message = $"User '{userId}' has been unlocked." });
        }
        catch (Exception ex)
        {
            // Never send exception details to the browser in production
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }

    // ── AJAX: Get all users (for Kendo Grid) ─────────────────────────────
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userRepo.GetAllUsers();
        return Json(users);   // Kendo Grid reads this
    }
}
```

---

## 7. Handling the JSON Response in JavaScript

```javascript
$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: {
        userId:                     userId,
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    },
    success: function(data) {
        //  data is already a JavaScript object — no JSON.parse needed
        //  jQuery does it automatically

        if (data.success) {
            // Update the UI — show success message
            $('#statusMessage')
                .text(data.message)
                .removeClass('error')
                .addClass('success')
                .show();

            // If using Kendo Grid, refresh it
            $('#userGrid').data('kendoGrid').dataSource.read();

        } else {
            // Show error message from server
            $('#statusMessage')
                .text(data.message)
                .removeClass('success')
                .addClass('error')
                .show();
        }
    },
    error: function(xhr) {
        // xhr.status gives you the HTTP status code (401, 403, 500...)
        if (xhr.status === 401) {
            // Session expired — redirect to login
            window.location.href = '/Account/Login';
        } else {
            $('#statusMessage').text('Request failed. Please try again.').show();
        }
    }
});
```

---

## 8. Sending Data TO the Server — Three Ways

Depending on what data you're sending, you use different approaches.

### Sending form field values

```javascript
// HTML:
// <input id="userId" type="text" value="sumit" />

var userId = $('#userId').val();

$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: {
        userId: userId,
        __RequestVerificationToken: token
    },
    // No contentType needed — defaults to form-encoded
    success: function(data) { ... }
});

// Controller receives:
[HttpPost]
public IActionResult UnlockUser(string userId)  // ← matches the data key name
```

### Sending a whole object as JSON

```javascript
var userData = { userId: 'sumit', reason: 'Admin request' };

$.ajax({
    url:         '/Admin/UnlockUser',
    type:        'POST',
    contentType: 'application/json',   // ← must set this for JSON body
    data:        JSON.stringify(userData),
    success: function(data) { ... }
});

// Controller:
[HttpPost]
public IActionResult UnlockUser([FromBody] UnlockRequest request)
//                               ↑ [FromBody] required when sending JSON body
```

### Sending a simple value in the URL (for GET requests)

```javascript
$.ajax({
    url:     '/Admin/GetUser?userId=sumit',   // query string
    type:    'GET',
    success: function(data) { ... }
});

// Or:
$.get('/Admin/GetUser', { userId: 'sumit' }, function(data) { ... });
// ↑ Shorthand for GET requests

// Controller:
[HttpGet]
public IActionResult GetUser(string userId)   // auto-bound from query string
```

---

## 9. The 401 Problem — Session Expired During AJAX

A user leaves the app open for a long time. Session expires. Auth cookie also
expires. They click "Unlock User" — the AJAX call goes out.

The server returns **401 Unauthorized**.

But because it's AJAX, the browser doesn't redirect — it just quietly returns
the 401 to your JavaScript. The user sees nothing and wonders why it didn't work.

**The fix — check for 401 in your error handler and redirect:**

```javascript
// Put this ONCE at the top of your main JS file or _Layout
$(document).ajaxError(function(event, xhr) {
    if (xhr.status === 401) {
        // Session expired — send them to login
        window.location.href = '/Account/Login';
    }
    if (xhr.status === 403) {
        alert('You do not have permission for this action.');
    }
});
```

`$(document).ajaxError` catches ALL failed AJAX calls across the whole page.
One place to handle it, not in every individual `$.ajax` call.

---

## 10. Putting It All Together — Unlock User Button

This is a full working example of AJAX in your admin panel.

**View (Admin/UserList.cshtml):**

```html
@Html.AntiForgeryToken()

<div id="statusMessage" style="display:none;"></div>

<table id="userTable">
    <thead>
        <tr>
            <th>User ID</th>
            <th>Failed Attempts</th>
            <th>Status</th>
            <th>Action</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var user in Model)
        {
            <tr id="row-@user.UserId">
                <td>@user.UserId</td>
                <td>@user.FailedAttempts</td>
                <td id="status-@user.UserId">
                    @(user.IsLocked ? "🔒 Locked" : "✅ Active")
                </td>
                <td>
                    @if (user.IsLocked)
                    {
                        <button class="btn-unlock"
                                data-userid="@user.UserId"
                                onclick="unlockUser('@user.UserId')">
                            Unlock
                        </button>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>

@section Scripts {
<script>
    var token = $('input[name="__RequestVerificationToken"]').val();

    function unlockUser(userId) {
        if (!confirm('Unlock user ' + userId + '?')) return;

        $.ajax({
            url:  '@Url.Action("UnlockUser", "Admin")',
            type: 'POST',
            data: {
                userId:                     userId,
                __RequestVerificationToken: token
            },
            success: function(data) {
                if (data.success) {
                    // Update the status cell — no page reload
                    $('#status-' + userId).text('✅ Active');

                    // Hide the unlock button for this row
                    $(event.target).hide();

                    // Show success message
                    $('#statusMessage')
                        .text(data.message)
                        .css('color', 'green')
                        .show()
                        .delay(3000)
                        .fadeOut();  // message disappears after 3 seconds
                } else {
                    alert('Error: ' + data.message);
                }
            },
            error: function(xhr) {
                if (xhr.status === 401) window.location.href = '/Account/Login';
                else alert('Request failed.');
            }
        });
    }
</script>
}
```

**Controller:**

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly UserRepository _userRepo;
    public AdminController(UserRepository userRepo) { _userRepo = userRepo; }

    // ── Regular page load ────────────────────────────────────────────────
    public IActionResult UserList()
    {
        var users = _userRepo.GetAllUsers();
        return View(users);   // passes List<UserRecord> to the view
    }

    // ── AJAX endpoint ────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UnlockUser(string userId)
    {
        try
        {
            _userRepo.UnlockUser(userId);
            return Json(new { success = true, message = $"'{userId}' unlocked." });
        }
        catch
        {
            return Json(new { success = false, message = "Error unlocking user." });
        }
    }
}
```

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **AJAX** | HTTP request sent by JavaScript in the background — page does not reload |
| **return Json(...)** | Controller returns JSON instead of a View — the AJAX response |
| **$.ajax()** | jQuery method for making AJAX calls — use this since Kendo includes jQuery |
| **success callback** | Function that runs when server returns 200 with JSON |
| **error callback** | Function that runs when server returns 4xx/5xx |
| **Anti-forgery token** | Must be included in every AJAX POST — read it from the hidden input |
| **[FromBody]** | Required on controller parameter when sending JSON body with contentType: 'application/json' |
| **401 in AJAX** | Must handle manually — redirect to login in error handler |
| **$(document).ajaxError** | Global error handler — one place for 401/403 instead of every call |
| **dataSource.read()** | Refreshes a Kendo Grid after an AJAX action — Chapter 8 |

---

## What's Next — Chapter 8

**Kendo UI Grid Integration**

With AJAX working, Chapter 8 connects a Kendo Grid to your secured
controller endpoints — showing all users, their lock status, and wiring
the Unlock button directly into the grid as a custom command column.
Kendo handles AJAX data loading internally, so you will see how
your Chapter 7 knowledge and Kendo's built-in DataSource connect.

---

*Chapter 7 of 8 — Sumit's Login System — ASP.NET Core MVC*
