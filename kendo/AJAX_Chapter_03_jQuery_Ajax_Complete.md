# AJAX — Chapter 3
### jQuery $.ajax() — Your Daily Tool, Mastered Completely

---

## Prerequisites

| What You Need |
|---|
| AJAX Chapter 1 — callbacks, sync vs async, Event Loop |
| AJAX Chapter 2 — Promises concept (helpful but not required) |
| JSON Complete Notes — stringify, parse |
| jQuery basics — $(), selectors, .val(), .text() |

---

## 1. Why $.ajax() is Your Primary Tool

You will use `$.ajax()` every single day. Here's why:

- Kendo UI is built on jQuery — jQuery is always loaded on your pages
- `$.ajax()` has a clean, consistent options object
- Error handling is automatic — 4xx/5xx status codes go straight to `error:`
- The anti-forgery token integration is straightforward
- It works the same across all browsers without any extra setup
- Every Kendo DataSource internally uses jQuery AJAX

```
Your Kendo page:
  ┌──────────────────────────────┐
  │  jQuery loaded (by Kendo)    │
  │  → $.ajax() available        │
  │  → Kendo DataSource uses it  │
  │  → Your code uses it too     │
  └──────────────────────────────┘
```

---

## 2. The Full $.ajax() Options Object — Every Setting

```javascript
$.ajax({
    // ── REQUIRED ──────────────────────────────────────────────────────
    url:      '/Admin/GetUsers',    // Where to send the request

    // ── COMMON ────────────────────────────────────────────────────────
    type:        'GET',             // HTTP method: 'GET', 'POST', 'PUT', 'DELETE'
    data:        { userId: 'sumit' },// Data to send (object or string)
    dataType:    'json',            // What you EXPECT back: 'json','html','text','xml'
    contentType: 'application/json',// What you're SENDING: default is form-encoded

    // ── CALLBACKS ─────────────────────────────────────────────────────
    success:  function(data, textStatus, xhr) { },    // 2xx response
    error:    function(xhr, textStatus, error) { },   // 4xx/5xx or network fail
    complete: function(xhr, textStatus) { },          // ALWAYS runs after success/error

    // ── OPTIONAL ──────────────────────────────────────────────────────
    async:       true,          // true (default) = async; false = sync (NEVER use false)
    cache:       true,          // true for GET (default), false for POST
    timeout:     5000,          // milliseconds — triggers error if no response
    headers:     {              // custom HTTP headers
        'RequestVerificationToken': token
    },
    beforeSend:  function(xhr) { },  // runs before request is sent — for setup
    context:     this,          // sets 'this' inside callbacks
});
```

---

## 3. The data: Option — How to Send Data

This is where most beginners make mistakes. How you set `data` changes
what the server receives.

### Sending as Form Data (Default — No contentType needed)

```javascript
// jQuery default — sends as: userId=sumit&action=unlock
$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: {
        userId:                     'sumit',
        __RequestVerificationToken: token
    }
    // No contentType = defaults to 'application/x-www-form-urlencoded'
});

// Controller receives:
[HttpPost]
public IActionResult UnlockUser(string userId)   // bound from form data — no attribute needed
```

### Sending as JSON Body

```javascript
// Use this when sending a complex object
$.ajax({
    url:         '/Admin/UnlockUser',
    type:        'POST',
    contentType: 'application/json',           // MUST set this
    data:        JSON.stringify({ userId: 'sumit' }),  // MUST stringify
    headers:     { 'RequestVerificationToken': token }
});

// Controller receives:
[HttpPost]
public IActionResult UnlockUser([FromBody] UnlockRequest req)  // [FromBody] required
```

### Sending as Query String (GET)

```javascript
// Data with GET goes into URL: /Admin/GetUser?userId=sumit&role=Admin
$.ajax({
    url:      '/Admin/GetUser',
    type:     'GET',
    data:     { userId: 'sumit', role: 'Admin' }
    // jQuery automatically appends these to the URL for GET requests
});

// Controller receives:
[HttpGet]
public IActionResult GetUser(string userId, string role)  // auto-bound from query string
```

### Sending Form Data (Entire Form)

```javascript
// Serialize all form fields including anti-forgery token
var formData = $('#loginForm').serialize();
// Result: "userId=sumit&password=abc123&__RequestVerificationToken=xyz..."

$.ajax({
    url:  '/Account/Login',
    type: 'POST',
    data: formData    // token is already inside — no need to add separately
});
```

### The data Comparison Table

| What you want to send | data: value | contentType: |
|---|---|---|
| Simple key-value pairs | `{ key: 'val' }` | Not needed (default) |
| Whole form with token | `$('#form').serialize()` | Not needed |
| Complex object as JSON | `JSON.stringify(obj)` | `'application/json'` |
| GET query params | `{ key: 'val' }` | Not needed |
| File upload | `new FormData(formEl)` | `false` (don't set it) |

---

## 4. The Callback Parameters — What You Actually Get

### success: function(data, textStatus, xhr)

```javascript
$.ajax({
    url: '/Admin/GetUsers',
    dataType: 'json',
    success: function(data, textStatus, xhr) {
        //         ↑          ↑             ↑
        //    your data    'success'    the XHR object

        // data — the parsed response (because dataType: 'json')
        // If dataType is not set, data might be a string — call JSON.parse(data) yourself
        console.log(data);           // [{ userId: 'sumit', ... }, ...]
        console.log(textStatus);     // "success"
        console.log(xhr.status);     // 200

        renderGrid(data);
    }
});
```

### error: function(xhr, textStatus, errorThrown)

```javascript
$.ajax({
    url: '/Admin/GetUsers',
    error: function(xhr, textStatus, errorThrown) {
        //         ↑         ↑              ↑
        //   XHR object  'error' or    error description
        //               'timeout'

        // xhr.status        — 401, 403, 404, 500
        // xhr.statusText    — "Unauthorized", "Not Found"
        // xhr.responseText  — the raw response body (error details from server)
        // textStatus        — "error", "timeout", "abort", "parseerror"
        // errorThrown       — "Unauthorized", "Internal Server Error"

        console.log('Status:', xhr.status);
        console.log('Response:', xhr.responseText);

        // Try to parse JSON error response from server
        try {
            var serverError = JSON.parse(xhr.responseText);
            showError(serverError.message);
        } catch(e) {
            showError('An error occurred. Status: ' + xhr.status);
        }
    }
});
```

### complete: function(xhr, textStatus)

```javascript
$.ajax({
    url: '/Admin/GetUsers',
    beforeSend: function() {
        showSpinner();
        disableButtons();
    },
    success: function(data) {
        renderGrid(data);
    },
    error: function(xhr) {
        showError(xhr.status);
    },
    complete: function(xhr, textStatus) {
        // textStatus: "success", "error", "timeout", "abort"
        // Runs AFTER success OR error — perfect for cleanup
        hideSpinner();
        enableButtons();
        console.log('Request finished with:', textStatus);
    }
});
```

---

## 5. dataType vs contentType — The Confusion Explained

These two look similar but are completely different.

```
contentType = what YOU are SENDING to the server
dataType    = what you EXPECT to RECEIVE from the server
```

```javascript
$.ajax({
    url:         '/Admin/UnlockUser',
    type:        'POST',
    contentType: 'application/json',    // I am sending JSON
    data:        JSON.stringify(obj),   // (so I must stringify)
    dataType:    'json',                // I expect JSON back
    // jQuery will auto-call JSON.parse on the response
});
```

| | contentType | dataType |
|---|---|---|
| **Means** | Format of what I'm sending | Format of what I expect back |
| **Default** | `application/x-www-form-urlencoded` | Inferred by jQuery from response header |
| **When to set** | Only when sending JSON body | When you want jQuery to auto-parse |
| **Effect on data:** | How the body is formatted | How the response is processed |

**Practical rule:**
- Set `dataType: 'json'` on almost every call — jQuery auto-parses the response for you
- Set `contentType: 'application/json'` only when you're also setting `data: JSON.stringify(...)`

---

## 6. Shorthand Methods — For Common Patterns

jQuery provides shortcuts so you don't write `$.ajax({...})` for simple cases.

### $.get() — Simple GET requests

```javascript
// Full form:
$.ajax({ url: '/Admin/GetUser', type: 'GET', data: { id: 5 }, success: fn, dataType: 'json' });

// Shorthand:
$.get('/Admin/GetUser', { id: 5 }, function(data) {
    console.log(data);
}, 'json');

// Even shorter — no data, just URL + callback:
$.get('/Admin/GetUsers', function(data) {
    renderGrid(data);
});
```

### $.post() — Simple POST requests

```javascript
// Full form:
$.ajax({ url: '/Admin/Unlock', type: 'POST', data: { userId: 'sumit' }, success: fn });

// Shorthand:
$.post('/Admin/Unlock', { userId: 'sumit', __RequestVerificationToken: token },
    function(data) {
        console.log(data);
    });
```

### $.getJSON() — GET + auto-parse JSON

```javascript
// Fetches URL, automatically parses JSON response
$.getJSON('/Admin/GetUsers', function(data) {
    // data is already a parsed object — no JSON.parse needed
    renderGrid(data);
});

// With query parameters:
$.getJSON('/Admin/GetUser', { userId: 'sumit' }, function(data) {
    showUserDetails(data);
});
```

### Shorthand Comparison

| Shorthand | Equivalent $.ajax | Use when |
|---|---|---|
| `$.get(url, data, fn)` | `type:'GET', dataType:'...'` | Simple GET, minimal options |
| `$.post(url, data, fn)` | `type:'POST'` | Simple POST, minimal options |
| `$.getJSON(url, data, fn)` | `type:'GET', dataType:'json'` | GET + want auto-parsed JSON |
| `$.ajax({...})` | (full form) | Any time you need full control |

**Always use `$.ajax()` when:**
- You need the `error:` callback
- You need `complete:` for cleanup
- You need `beforeSend:`
- You need timeout
- You need custom headers (anti-forgery token)

---

## 7. beforeSend — Setup Before Every Request

`beforeSend` runs just before the request is sent. Perfect for:

```javascript
$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',

    beforeSend: function(xhr) {
        // 1. Add anti-forgery token as a header
        xhr.setRequestHeader('RequestVerificationToken',
            $('input[name="__RequestVerificationToken"]').val());

        // 2. Show loading state
        $('#btnUnlock').text('Unlocking...').prop('disabled', true);
        $('#spinner').show();

        // 3. You can cancel the request by returning false
        if (!userId) {
            return false;    // cancels the request
        }
    },

    success:  function(data) { ... },
    complete: function()     { $('#btnUnlock').text('Unlock').prop('disabled', false); $('#spinner').hide(); }
});
```

---

## 8. $.ajaxSetup() — Configure Defaults for ALL Calls

Instead of adding the anti-forgery token to every single `$.ajax()` call,
configure it once globally. This is the cleanest approach.

```javascript
// In your _Layout.cshtml Scripts section — runs once on every page
$(document).ready(function() {

    // Set defaults for ALL future $.ajax() calls on this page
    $.ajaxSetup({
        beforeSend: function(xhr) {
            // Add anti-forgery token to EVERY request automatically
            var token = $('input[name="__RequestVerificationToken"]').val();
            if (token) {
                xhr.setRequestHeader('RequestVerificationToken', token);
            }
        }
    });

    // Handle ALL AJAX errors globally — one place instead of in every call
    $(document).ajaxError(function(event, xhr, settings, error) {
        if (xhr.status === 401) {
            window.location.href = '/Account/Login';
        } else if (xhr.status === 403) {
            showMessage('You do not have permission for this action.', 'error');
        } else if (xhr.status === 500) {
            showMessage('A server error occurred. Please try again.', 'error');
        }
    });

});
```

After setting this up, every `$.ajax()` call you write anywhere on the page
automatically has the anti-forgery token and global error handling.
Your individual calls become much simpler:

```javascript
// Before $.ajaxSetup — repeated in every call
$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: { userId: userId, __RequestVerificationToken: token },
    error: function(xhr) {
        if (xhr.status === 401) window.location.href = '/Account/Login';
        else showError(xhr.status);
    },
    success: function(data) { ... }
});

// After $.ajaxSetup — clean and simple
$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: { userId: userId },   // no token needed — added by beforeSend
    success: function(data) { ... }
    // no error needed — handled globally
});
```

---

## 9. AJAX Events — Global Lifecycle Hooks

jQuery fires global events for every AJAX call. Listen to them for
progress indicators that cover ALL requests on the page.

```javascript
// These run for EVERY $.ajax() call on the page

$(document).ajaxStart(function() {
    // Fires when the FIRST request starts (goes from 0 active → 1 active)
    $('#globalSpinner').show();
});

$(document).ajaxStop(function() {
    // Fires when the LAST request completes (goes from 1 active → 0 active)
    $('#globalSpinner').hide();
});

$(document).ajaxSend(function(event, xhr, settings) {
    // Fires before EVERY request is sent
    console.log('Sending request to:', settings.url);
});

$(document).ajaxComplete(function(event, xhr, settings) {
    // Fires after EVERY request completes (success or error)
    console.log('Request completed:', settings.url, xhr.status);
});

$(document).ajaxError(function(event, xhr, settings, error) {
    // Fires for EVERY failed request
    // Best place for global 401/500 handling
    if (xhr.status === 401) window.location.href = '/Account/Login';
});

$(document).ajaxSuccess(function(event, xhr, settings, data) {
    // Fires for EVERY successful request
});
```

**Practical usage — global loading spinner:**

```html
<!-- In _Layout.cshtml — shows/hides automatically for all AJAX calls -->
<div id="globalSpinner" style="display:none; position:fixed; top:10px; right:10px;">
    Loading...
</div>

<script>
$(document).ajaxStart(function() { $('#globalSpinner').show(); });
$(document).ajaxStop(function()  { $('#globalSpinner').hide(); });
</script>
```

---

## 10. Chaining Requests — Doing One After Another

Sometimes request B needs data from request A's response.

```javascript
// Pattern 1 — Nested callbacks (works but gets messy with more levels)
$.ajax({
    url: '/Admin/GetUser?userId=sumit',
    success: function(user) {
        // Now use user.id to get their orders
        $.ajax({
            url:  '/Admin/GetOrders',
            data: { userId: user.userId },
            success: function(orders) {
                renderOrders(orders);
            }
        });
    }
});


// Pattern 2 — Returning a jqXHR (jQuery Promise-like)
// $.ajax() returns a jqXHR object which has .then() support

$.ajax({ url: '/Admin/GetUser', data: { userId: 'sumit' } })
    .then(function(user) {
        return $.ajax({ url: '/Admin/GetOrders', data: { userId: user.userId } });
    })
    .then(function(orders) {
        return $.ajax({ url: '/Admin/GetOrderDetails', data: { orderId: orders[0].id } });
    })
    .then(function(details) {
        renderDetails(details);
    })
    .fail(function(xhr) {
        // .fail() is jQuery's name for .catch() — handles any error in the chain
        handleError(xhr.status);
    });
```

Note: jQuery uses `.fail()` where standard Promises use `.catch()`.
Same concept, different name.

---

## 11. Aborting a Request

If the user navigates away or clicks cancel, you can abort an in-flight request.

```javascript
var currentRequest = null;

function loadUsers() {
    // If a previous request is still running, abort it
    if (currentRequest !== null) {
        currentRequest.abort();
    }

    currentRequest = $.ajax({
        url:  '/Admin/GetUsers',
        type: 'GET',
        success: function(data) {
            currentRequest = null;   // reset
            renderGrid(data);
        },
        error: function(xhr, textStatus) {
            if (textStatus === 'abort') {
                console.log('Request was aborted — this is expected');
                return;   // don't show error for intentional aborts
            }
            showError(xhr.status);
        }
    });
}
```

**When this matters:**
- Search-as-you-type — user types fast, each keystroke fires a request,
  abort the previous one so old results don't overwrite new ones
- Navigation — user clicks away before data loads

---

## 12. Timeout — Don't Wait Forever

```javascript
$.ajax({
    url:     '/Admin/GetReport',
    type:    'GET',
    timeout: 10000,    // 10 seconds — if no response by then, trigger error

    success: function(data) {
        renderReport(data);
    },

    error: function(xhr, textStatus, error) {
        if (textStatus === 'timeout') {
            showMessage('The server is taking too long. Please try again.', 'warning');
        } else {
            showMessage('Request failed: ' + xhr.status, 'error');
        }
    }
});
```

---

## 13. Complete Real-World Pattern — The Full Template

This is the exact pattern you should use for every `$.ajax()` call in
your ASP.NET Core MVC + Kendo project.

```javascript
// ── Global setup (in _Layout.cshtml — once for all pages) ────────────
$(document).ready(function() {
    $.ajaxSetup({
        beforeSend: function(xhr) {
            var token = $('input[name="__RequestVerificationToken"]').val();
            if (token) xhr.setRequestHeader('RequestVerificationToken', token);
        }
    });

    $(document).ajaxError(function(e, xhr) {
        if (xhr.status === 401) window.location.href = '/Account/Login';
    });

    $(document).ajaxStart(function() { $('#pageSpinner').show(); });
    $(document).ajaxStop(function()  { $('#pageSpinner').hide(); });
});


// ── Individual AJAX call (clean because of global setup above) ────────
function unlockUser(userId) {
    $.ajax({
        url:      '/Admin/UnlockUser',
        type:     'POST',
        data:     { userId: userId },
        dataType: 'json',
        timeout:  8000,

        success: function(data) {
            if (data.success) {
                showToast(data.message, 'success');
                refreshUsersGrid();
            } else {
                showToast(data.message, 'error');
            }
        },

        error: function(xhr, textStatus) {
            if (textStatus === 'timeout') {
                showToast('Request timed out. Please try again.', 'warning');
            }
            // 401 handled globally by ajaxError above
            // Other errors fall through here
        },

        complete: function() {
            $('#btnUnlock').prop('disabled', false).text('Unlock');
        }
    });
}


// ── Loading grid data ─────────────────────────────────────────────────
function loadUsersIntoTable() {
    $.ajax({
        url:      '/Admin/GetUsers',
        type:     'GET',
        dataType: 'json',

        success: function(data) {
            // data is auto-parsed because of dataType: 'json'
            var grid = $('#userGrid').data('kendoGrid');
            grid.dataSource.data(data);   // load into Kendo Grid directly
        },

        error: function(xhr) {
            if (xhr.status !== 401) {   // 401 handled globally
                showToast('Failed to load users.', 'error');
            }
        }
    });
}
```

---

## Quick Reference — $.ajax() Cheat Sheet

```javascript
// Minimal GET
$.ajax({ url: '/endpoint', success: fn });

// Minimal POST with form data + token
$.ajax({ url: '/endpoint', type: 'POST', data: { key: val, __RequestVerificationToken: token }, success: fn });

// POST with JSON body
$.ajax({ url: '/endpoint', type: 'POST', contentType: 'application/json', data: JSON.stringify(obj), dataType: 'json', success: fn });

// GET with query params
$.ajax({ url: '/endpoint', type: 'GET', data: { id: 5 }, dataType: 'json', success: fn });

// Full pattern with error + complete
$.ajax({ url: '...', type: '...', data: {...}, dataType: 'json',
    success:  function(data) { },
    error:    function(xhr, status, err) { },
    complete: function() { }
});

// Shorthands
$.get(url, data, fn);
$.post(url, data, fn);
$.getJSON(url, data, fn);

// jqXHR (for chaining)
$.ajax({...}).then(fn).fail(fn).always(fn);

// Global
$.ajaxSetup({ beforeSend: fn });
$(document).ajaxStart(fn);
$(document).ajaxStop(fn);
$(document).ajaxError(fn);
```

---

## Summary

| Concept | One-Line Meaning |
|---|---|
| **$.ajax({})** | Full-control AJAX call — use this for anything non-trivial |
| **type:** | HTTP method — 'GET' or 'POST' (use type, not method, in jQuery) |
| **data:** | What to send — object for form data, JSON.stringify for JSON body |
| **dataType: 'json'** | Tell jQuery to auto-parse the JSON response |
| **contentType: 'application/json'** | Set this ONLY when data is JSON.stringify'd |
| **success:** | Runs on 2xx — receives parsed data |
| **error:** | Runs on 4xx/5xx or network fail — check xhr.status |
| **complete:** | Always runs — perfect for cleanup (hide spinner, re-enable button) |
| **beforeSend:** | Runs before request — add headers, show loading, cancel if needed |
| **$.ajaxSetup** | Set defaults for all calls — add anti-forgery token once here |
| **$(document).ajaxError** | Global error handler — 401 redirect in one place |
| **$(document).ajaxStart/Stop** | Show/hide global spinner automatically |
| **$.getJSON** | Shorthand for GET + auto-JSON-parse |
| **jqXHR.abort()** | Cancel an in-flight request |
| **timeout:** | Milliseconds before triggering error callback |
| **$.ajax().then().fail()** | Chain dependent requests cleanly |

---

## What's Next — Chapter 4

**AJAX with ASP.NET Core MVC — The Full Integration**

Chapter 4 covers everything specific to ASP.NET Core MVC:
the anti-forgery token in depth (why it exists, all three ways to send it),
`[FromBody]` vs form binding, returning JSON correctly, handling
validation errors as JSON, and the complete controller + JavaScript
patterns your team will use for every feature.

---

*AJAX Chapter 3 of 5 — Sumit's ASP.NET Core MVC Learning Journey*
