# Chapter 8 — Kendo UI Grid Integration
### Displaying Secured Data in a Professional Grid

---

## Prerequisites

| What You Need | Your File / Chapter |
|---|---|
| Chapter 7 — AJAX + JSON working | ✓ |
| Chapter 5 — [Authorize] on controllers | ✓ |
| AdminController returning JSON | ✓ |
| jQuery included in the project | ✓ (Kendo includes it) |

---

## 1. What is Kendo UI

**Kendo UI** is a professional UI component library by Telerik/Progress.
It gives you ready-made, polished UI elements — grids, dropdowns, date pickers,
charts — that would take weeks to build from scratch.

Your company uses Kendo. For your login/admin system you need:

- **Kendo Grid** — the data table that shows all users
- **Kendo Grid DataSource** — the built-in AJAX connector that calls your controller
- **Custom Commands** — the "Unlock" button inside the grid

Everything you learned in Chapter 7 about AJAX and JSON is exactly what
Kendo uses internally. Kendo Grid just wraps it in a clean API.

---

## 2. Including Kendo in Your Project

Your company likely has Kendo already set up. But for reference —
there are two ways it gets included:

### Way 1 — CDN (for learning/testing)

```html
@* In _Layout.cshtml <head> *@
<link rel="stylesheet" href="https://kendo.cdn.telerik.com/2023.3.1114/styles/kendo.default-v2.min.css" />

@* Before closing </body> — jQuery must come FIRST *@
<script src="https://code.jquery.com/jquery-3.7.0.min.js"></script>
<script src="https://kendo.cdn.telerik.com/2023.3.1114/js/kendo.all.min.js"></script>
```

### Way 2 — NuGet / Local files (company standard)

```html
@* Kendo files in wwwroot — your company's setup *@
<link rel="stylesheet" href="~/kendo/styles/kendo.default-v2.min.css" />
<script src="~/kendo/js/jquery.min.js"></script>
<script src="~/kendo/js/kendo.all.min.js"></script>
```

**Important:** jQuery must always load before Kendo. Kendo depends on jQuery.

---

## 3. How Kendo Grid Works — The Mental Model

```
KENDO GRID
┌──────────────────────────────────────────────────────────┐
│  Grid (Visual Table)                                     │
│  ┌────────────────────────────────────────────────────┐  │
│  │  DataSource (the data engine)                      │  │
│  │  ┌──────────────────────────────────────────────┐  │  │
│  │  │  transport: {                                 │  │  │
│  │  │      read: { url: '/Admin/GetUsers' }         │  │  │ ← calls your controller
│  │  │  }                                            │  │  │
│  │  └──────────────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────────────┘  │
│                                                          │
│  columns: [UserId, FailedAttempts, IsLocked, Actions]    │
└──────────────────────────────────────────────────────────┘
```

You tell Kendo:
- **Where to get data** → your controller URL
- **What columns to show** → column definitions
- **What the data model looks like** → field names matching your JSON

Kendo does the AJAX call, gets the JSON, and renders the table. You don't
write a single `$.ajax()` call for data loading — Kendo handles it.

---

## 4. The Controller Endpoint — What Kendo Expects

Kendo Grid's DataSource makes a GET request to your URL and expects
**a JSON array** back.

```csharp
// Controllers/AdminController.cs

[Authorize(Roles = "Admin")]
public class AdminController : BaseController
{
    private readonly UserRepository _userRepo;

    public AdminController(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // ── Page that contains the grid ──────────────────────────────────────
    public IActionResult UserList()
    {
        return View();    // empty view — grid loads data via AJAX
    }

    // ── Kendo Grid calls this to get data ────────────────────────────────
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userRepo.GetAllUsers();

        // Return in Kendo's expected format
        return Json(users);

        // If using Kendo MVC Wrappers, return:
        // return Json(users.ToDataSourceResult(request));
        // But for basic Kendo JS, plain Json(list) works fine
    }

    // ── AJAX: Unlock user (from Chapter 7) ──────────────────────────────
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

The JSON that `GetUsers()` returns will look like:

```json
[
    { "userId": "admin", "failedAttempts": 0, "isLocked": false, "role": "Admin" },
    { "userId": "sumit", "failedAttempts": 2, "isLocked": false, "role": "User" },
    { "userId": "rahul", "failedAttempts": 3, "isLocked": true,  "role": "User" }
]
```

---

## 5. Basic Kendo Grid — Step by Step

This is the minimum to get a Kendo Grid on your page showing user data.

**View (Admin/UserList.cshtml):**

```html
@{
    ViewData["Title"] = "User Management";
}

@Html.AntiForgeryToken()

<h2>User Management</h2>

<div id="statusMessage" style="display:none; margin-bottom:10px;"></div>

@* The grid renders inside this div *@
<div id="userGrid"></div>

@section Scripts {
<script>
$(document).ready(function() {

    // ── Initialize Kendo Grid ────────────────────────────────────────
    $('#userGrid').kendoGrid({

        // ── DataSource: where Kendo gets data ────────────────────────
        dataSource: {
            transport: {
                read: {
                    url:      '@Url.Action("GetUsers", "Admin")',  // your controller
                    type:     'GET',
                    dataType: 'json'
                }
            },
            schema: {
                model: {
                    fields: {
                        userId:         { type: 'string' },
                        failedAttempts: { type: 'number' },
                        isLocked:       { type: 'boolean' },
                        role:           { type: 'string' }
                    }
                }
            },
            pageSize: 10    // how many rows per page
        },

        // ── Grid features ────────────────────────────────────────────
        pageable:  true,   // show page numbers at the bottom
        sortable:  true,   // click column headers to sort
        filterable: true,  // show filter row
        height:    500,    // grid height in pixels

        // ── Column definitions ───────────────────────────────────────
        columns: [
            {
                field: 'userId',
                title: 'User ID',
                width: 150
            },
            {
                field: 'failedAttempts',
                title: 'Failed Attempts',
                width: 150
            },
            {
                field: 'isLocked',
                title: 'Status',
                width: 120,
                template: function(dataItem) {
                    // Custom display — shows icon based on value
                    return dataItem.isLocked
                        ? '<span style="color:red;">🔒 Locked</span>'
                        : '<span style="color:green;">✅ Active</span>';
                }
            },
            {
                field: 'role',
                title: 'Role',
                width: 100
            },
            {
                // Action column — custom buttons
                title:    'Actions',
                width:    120,
                template: function(dataItem) {
                    if (dataItem.isLocked) {
                        return '<button class="k-button k-button-solid-primary" ' +
                               'onclick="unlockUser(\'' + dataItem.userId + '\')">' +
                               'Unlock</button>';
                    }
                    return '<span style="color:#999;">No action</span>';
                }
            }
        ]
    });

});

// ── Unlock user function (from Chapter 7) ───────────────────────────
var token = $('input[name="__RequestVerificationToken"]').val();

function unlockUser(userId) {
    if (!confirm('Unlock user: ' + userId + '?')) return;

    $.ajax({
        url:  '@Url.Action("UnlockUser", "Admin")',
        type: 'POST',
        data: {
            userId:                     userId,
            __RequestVerificationToken: token
        },
        success: function(data) {
            if (data.success) {
                // Refresh the grid — Kendo re-calls GetUsers automatically
                $('#userGrid').data('kendoGrid').dataSource.read();

                // Show success message
                $('#statusMessage')
                    .text(data.message)
                    .css({ 'color': 'green', 'display': 'block' });

                setTimeout(function() {
                    $('#statusMessage').fadeOut();
                }, 3000);

            } else {
                alert('Error: ' + data.message);
            }
        },
        error: function(xhr) {
            if (xhr.status === 401) window.location.href = '/Account/Login';
            else alert('Request failed. Please try again.');
        }
    });
}
</script>
}
```

---

## 6. Key Kendo Grid Concepts — Reference

### DataSource — The Data Engine

```javascript
dataSource: {
    transport: {
        read: { url: '/Admin/GetUsers', type: 'GET' }
        // For full CRUD you also add: create, update, destroy
    },
    schema: {
        model: {
            id: 'userId',          // the primary key field
            fields: {
                userId:         { type: 'string', editable: false },
                failedAttempts: { type: 'number' },
                isLocked:       { type: 'boolean' }
            }
        }
    },
    pageSize: 10,
    serverPaging:  false,   // true = server handles paging (better for large data)
    serverSorting: false,   // true = server handles sorting
}
```

### Column Templates — Custom Cell Content

```javascript
// Static template string (simple)
{ field: 'isLocked', template: '<strong>#= isLocked ? "Locked" : "Active" #</strong>' }
//                                        ↑ Kendo template syntax: #= value #

// JavaScript function template (complex logic)
{ field: 'isLocked', template: function(item) {
    return item.isLocked
        ? '<span class="badge-red">Locked</span>'
        : '<span class="badge-green">Active</span>';
}}
```

### Refreshing the Grid After an Action

```javascript
// After UnlockUser AJAX call succeeds:
var grid = $('#userGrid').data('kendoGrid');
grid.dataSource.read();   // re-fetches data from GetUsers endpoint
```

### Getting Selected Row Data

```javascript
var grid     = $('#userGrid').data('kendoGrid');
var selected = grid.select();               // selected <tr> element
var dataItem = grid.dataItem(selected);     // the data object for that row
console.log(dataItem.userId);              // "sumit"
```

---

## 7. Sending the Anti-Forgery Token with Kendo DataSource

If Kendo's DataSource needs to make POST requests (create/update/delete),
the anti-forgery token must be included. The cleanest way — configure it once
as a global jQuery AJAX setup:

```javascript
// Put this in your _Layout.cshtml Scripts section or a site-wide JS file
// It adds the token to EVERY $.ajax call automatically

var token = $('input[name="__RequestVerificationToken"]').val();

$.ajaxSetup({
    beforeSend: function(xhr) {
        xhr.setRequestHeader('RequestVerificationToken', token);
    }
});
```

Then in your controller, tell ASP.NET Core to look for the token in the header:

```csharp
// Program.cs — services section
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";  // match the header name above
});
```

Now every Kendo DataSource POST automatically carries the token.
No changes needed in individual controller actions.

---

## 8. Handling Auth in Kendo — When Session Expires

Kendo Grid's DataSource makes AJAX calls internally.
If the session expires and Kendo calls `GetUsers()`, the server
returns 401 and Kendo shows an empty grid with an error bar —
confusing for the user.

**The fix — handle it in Kendo's error event:**

```javascript
dataSource: {
    transport: {
        read: { url: '@Url.Action("GetUsers", "Admin")', type: 'GET' }
    },
    error: function(e) {
        if (e.xhr.status === 401) {
            // Session expired — redirect to login
            window.location.href = '/Account/Login';
        } else {
            alert('Failed to load data. Please refresh the page.');
        }
    }
}
```

---

## 9. The Complete Picture — Everything Connected

This is the full data flow from database to Kendo Grid and back.

```
LOADING DATA
─────────────────────────────────────────────────────────────

Kendo Grid initializes on page load
         │
         │  DataSource.read()
         ▼
GET /Admin/GetUsers
         │
         ▼  [Authorize(Roles="Admin")] passes (auth cookie valid)
AdminController.GetUsers()
         │
         ▼
UserRepository.GetAllUsers()
         │
         ▼
ADO.NET → sp_GetAllUsers → SQL Server
         │
         ▼
List<UserRecord> → return Json(users)
         │
         ▼  JSON array arrives
Kendo DataSource parses it
         │
         ▼
Grid renders rows — each UserRecord = one row
         │
         ▼
isLocked column template shows 🔒 or ✅
Locked rows show Unlock button


UNLOCKING A USER
─────────────────────────────────────────────────────────────

Admin clicks "Unlock" for user "rahul"
         │
         ▼
unlockUser('rahul') function called
         │
         ▼
$.ajax POST /Admin/UnlockUser { userId: 'rahul', token: '...' }
         │
         ▼  [Authorize(Roles="Admin")] + [ValidateAntiForgeryToken] pass
AdminController.UnlockUser("rahul")
         │
         ▼
UserRepository.UnlockUser("rahul")
         │
         ▼
ADO.NET → sp_UnlockUser → SQL Server
UPDATE Users SET FailedAttempts=0, IsLocked=0 WHERE UserId='rahul'
         │
         ▼
return Json({ success: true, message: "'rahul' unlocked." })
         │
         ▼
success callback runs
         │
         ├─ grid.dataSource.read() → grid refreshes → rahul's row now shows ✅
         └─ statusMessage shows "rahul unlocked" for 3 seconds then fades
```

---

## 10. Quick Kendo Reference — Properties You'll Use

```javascript
// ── Grid options ─────────────────────────────────────────────────────
height:    500            // grid height in px
pageable:  true           // pagination
sortable:  true           // column sort
filterable: true          // filter row
resizable: true           // drag column borders to resize

// ── Column options ───────────────────────────────────────────────────
field:    'userId'        // JSON property name (must match exactly)
title:    'User ID'       // column header text
width:    150             // column width in px
hidden:   true            // hide the column (data still loads)
template: '...'           // custom cell HTML

// ── DataSource options ───────────────────────────────────────────────
pageSize:       10        // rows per page
serverPaging:   false     // if true, paging handled server-side
serverSorting:  false     // if true, sorting handled server-side
serverFiltering: false    // if true, filtering handled server-side

// ── Methods (called via .data('kendoGrid')) ──────────────────────────
grid.dataSource.read()         // refresh data
grid.dataSource.data()         // get all loaded data items
grid.select()                  // get selected row(s)
grid.dataItem(row)             // get data object for a row
grid.addRow()                  // open new row for entry
grid.removeRow(row)            // remove a row
grid.editRow(row)              // open row for editing
grid.saveChanges()             // save all pending changes
grid.cancelChanges()           // discard pending changes
```

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **Kendo Grid** | A professional data grid that handles AJAX loading, paging, sorting |
| **DataSource** | The data engine inside Kendo — calls your controller via AJAX |
| **transport.read** | URL of your JSON controller action — Kendo calls it automatically |
| **schema.model** | Tells Kendo the field names and types in your JSON |
| **column template** | Custom HTML per cell — used for status icons and action buttons |
| **dataSource.read()** | Refreshes the grid — call after UnlockUser succeeds |
| **$.ajaxSetup** | Global jQuery AJAX config — send anti-forgery token on every call |
| **error event** | Handle 401 in DataSource error — redirect to login on session expiry |
| **grid.dataItem(row)** | Get the data object for a selected/clicked row |
| **kendoGrid('destroy')** | Clean up the grid if you need to re-initialize it |

---

## You're Done — What You Can Now Build

After these 8 chapters, you have everything to build:

```
✅  Login page with ADO.NET + stored procedure validation
✅  Password hashing with BCrypt
✅  Account lockout after 3 failed attempts
✅  Cookie authentication — [Authorize] protecting every page
✅  Session storing user info — "Welcome, Sumit" on every page
✅  Middleware pipeline configured correctly
✅  Admin panel with Kendo Grid showing all users
✅  Unlock User button via AJAX — no page reload
✅  Session expiry handled gracefully
✅  Anti-forgery token on all POST requests
```

**Recommended build order:**
1. Create Users table + all stored procedures, test in SSMS
2. Create UserRepository with ADO.NET methods
3. Set up Program.cs (AddSession, AddAuthentication, pipeline order)
4. Build LoginController + LoginViewModel + Login View
5. Add PasswordHelper (BCrypt)
6. Test login end to end — make sure [Authorize] redirects work
7. Add session set/clear in Login/Logout
8. Build AdminController + UserList View + Kendo Grid
9. Add UnlockUser AJAX endpoint + button in grid
10. Test full flow: login → admin panel → lock detection → unlock → grid refresh

---

*Chapter 8 of 8 — Sumit's Login System — ASP.NET Core MVC*
*All 8 chapters complete.*
