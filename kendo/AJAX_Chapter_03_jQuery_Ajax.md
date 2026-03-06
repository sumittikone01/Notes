# AJAX — Chapter 3
### jQuery $.ajax() — Your Daily Tool, Mastered Completely

---

## Prerequisites

| What You Need |
|---|
| AJAX Chapter 1 — callbacks, async, GET vs POST |
| AJAX Chapter 2 — Promises, fetch(), response.ok |
| JSON Complete Notes — stringify, parse, [FromBody] |
| jQuery basics — `$()`, `.val()`, `.text()`, `.hide()`, `.show()` |

---

## 1. Why $.ajax() is Your Primary Tool

You learned `fetch()` in Chapter 2. `fetch()` is cleaner and more modern.
But in a **Kendo UI + ASP.NET MVC** project, `$.ajax()` is the right choice
because:

```
1. Kendo UI already loads jQuery — no extra library needed
2. Kendo DataSource uses jQuery AJAX internally
3. $.ajax() has ONE error callback for ALL failures (401, 403, 500)
   fetch() requires a manual response.ok check for each request
4. All Kendo examples and company code use $.ajax() — consistency
5. $(document).ajaxError handles all AJAX errors globally — one place
```

**Know fetch() and Promises (Chapter 2) to understand the concepts.**
**Use $.ajax() for your actual daily work.**

---

## 2. The Full $.ajax() Options Object — Every Parameter

```javascript
$.ajax({
    // ── Required ─────────────────────────────────────────────────────
    url:         '/Admin/GetUsers',   // where to send the request

    // ── Method ───────────────────────────────────────────────────────
    type:        'GET',               // 'GET', 'POST', 'PUT', 'DELETE'
    // method:   'GET',               // 'method' is an alias for 'type'

    // ── Data to send ─────────────────────────────────────────────────
    data:        { userId: 'sumit' }, // object — auto-serialized to form-encoded
    // For GET:  appended to URL as ?userId=sumit
    // For POST: sent in request body

    // ── Response type ─────────────────────────────────────────────────
    dataType:    'json',              // tells jQuery: parse response as JSON
                                      // auto-parses — no JSON.parse needed
    // 'json', 'text', 'html', 'xml', 'script'

    // ── Request body type ─────────────────────────────────────────────
    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
    // default ↑ — use for normal form data (no [FromBody] on controller)
    // 'application/json' — use when sending JSON.stringify(obj) with [FromBody]

    // ── Callbacks ─────────────────────────────────────────────────────
    success: function(data, textStatus, jqXHR) {
        // data       = parsed response (object if dataType: 'json')
        // textStatus = "success", "notmodified", "nocontent"
        // jqXHR      = the XHR object (has .status, .responseText, .responseJSON)
    },

    error: function(jqXHR, textStatus, errorThrown) {
        // jqXHR.status    = 401, 403, 404, 500
        // jqXHR.responseText  = raw response body
        // jqXHR.responseJSON  = parsed JSON body (if server returned JSON error)
        // textStatus      = "error", "timeout", "abort", "parsererror"
        // errorThrown     = "Unauthorized", "Forbidden", "Not Found"
    },

    complete: function(jqXHR, textStatus) {
        // Always runs — after success or error
        // textStatus = "success", "error", "timeout", "abort"
        $('#loadBtn').prop('disabled', false);   // re-enable button
        $('#spinner').hide();
    },

    // ── Timing ───────────────────────────────────────────────────────
    timeout: 30000,         // 30 seconds — abort if no response by then
                            // triggers error callback with textStatus: "timeout"

    // ── Before send ───────────────────────────────────────────────────
    beforeSend: function(jqXHR, settings) {
        // Runs right before request is sent
        // Use to: add headers, show spinner, validate data
        $('#spinner').show();
        jqXHR.setRequestHeader('X-Custom-Header', 'value');
        // Return false here to CANCEL the request
    },

    // ── Caching ───────────────────────────────────────────────────────
    cache:       false,     // default true for GET, false for POST
                            // false adds ?_=timestamp to URL to prevent caching

    // ── Async ─────────────────────────────────────────────────────────
    async:       true,      // default — always keep true
                            // false makes it synchronous — freezes browser — NEVER use
});
```

---

## 3. The Three Callbacks — Deep Dive

### success — What You Get Back

```javascript
success: function(data, textStatus, jqXHR) {

    // When dataType: 'json' — data is already a JS object
    console.log(data.userId);      // "sumit"
    console.log(data.isLocked);    // false

    // When dataType: 'json' and server returns an array
    data.forEach(function(user) {
        console.log(user.userId);
    });

    // Checking the server's own success/failure flag
    // (this is the { success: true/false } pattern from your controllers)
    if (data.success) {
        showSuccessMessage(data.message);
        refreshGrid();
    } else {
        showErrorMessage(data.message);
    }
}
```

### error — Reading What Went Wrong

```javascript
error: function(jqXHR, textStatus, errorThrown) {

    // The three parameters explained:
    // jqXHR.status      → HTTP status code: 0, 400, 401, 403, 404, 500
    // textStatus        → string: "error", "timeout", "abort", "parsererror"
    // errorThrown       → HTTP status text: "Unauthorized", "Not Found", etc.

    console.log('Status Code:',  jqXHR.status);
    console.log('Status Text:',  errorThrown);
    console.log('Response Body:', jqXHR.responseText);

    // When server returns JSON in the error body (e.g. { message: "reason" })
    if (jqXHR.responseJSON) {
        console.log(jqXHR.responseJSON.message);
    }

    // Handle specific codes:
    switch (jqXHR.status) {
        case 0:
            showError('No internet connection or server unreachable.');
            break;
        case 400:
            showError('Invalid request: ' + (jqXHR.responseJSON?.message || 'Bad input'));
            break;
        case 401:
            window.location.href = '/Account/Login';
            break;
        case 403:
            showError('You do not have permission for this action.');
            break;
        case 404:
            showError('The requested resource was not found.');
            break;
        case 408:
        case 'timeout':  // jqXHR.status is 0 for timeout, use textStatus
            showError('Request timed out. Please try again.');
            break;
        case 500:
            showError('Server error. Please contact your administrator.');
            break;
        default:
            showError('Something went wrong. Please try again.');
    }
}
```

### complete — Cleanup That Always Runs

```javascript
// Pattern: disable button before request, re-enable after
// This ensures the button is always re-enabled even if request fails

$('#unlockBtn').click(function() {
    $(this).prop('disabled', true);   // disable before request

    $.ajax({
        url:  '/Admin/UnlockUser',
        type: 'POST',
        data: { userId: userId, __RequestVerificationToken: token },

        success: function(data) {
            showSuccess(data.message);
            refreshGrid();
        },
        error: function(xhr) {
            showError('Request failed.');
        },
        complete: function() {
            $('#unlockBtn').prop('disabled', false);  // ALWAYS re-enable
            $('#spinner').hide();                      // ALWAYS hide spinner
        }
    });
});
```

---

## 4. Sending Data — All the Ways

### Sending Form Field Values (Most Common)

```javascript
// HTML:
// <input id="userId" value="sumit" />
// <input id="reason" value="Admin request" />

$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: {
        userId:                     $('#userId').val(),
        reason:                     $('#reason').val(),
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    },
    dataType: 'json',
    success:  function(data) { ... }
});

// Controller — no [FromBody] needed:
[HttpPost]
public IActionResult UnlockUser(string userId, string reason) { ... }
```

### Sending an Entire Form

```javascript
// Serialize entire form — includes ALL inputs + anti-forgery token
// Perfect when you have many fields

$.ajax({
    url:      $('#myForm').attr('action'),   // use form's own action URL
    type:     'POST',
    data:     $('#myForm').serialize(),      // all fields as form-encoded string
    dataType: 'json',
    success:  function(data) { ... }
});

// Controller receives each field as a parameter, or as a model:
[HttpPost]
public IActionResult Save(UserViewModel model) { ... }
```

### Sending JSON Body (with [FromBody])

```javascript
// Use when your controller parameter has [FromBody]
// Must set contentType: 'application/json'
// Must JSON.stringify the data

var payload = {
    userId:    'sumit',
    newRole:   'Admin',
    changedBy: 'admin'
};

$.ajax({
    url:         '/Admin/UpdateUser',
    type:        'POST',
    contentType: 'application/json',           // ← required for [FromBody]
    data:        JSON.stringify(payload),       // ← must stringify
    dataType:    'json',
    success:     function(data) { ... }
});

// Controller:
[HttpPost]
public IActionResult UpdateUser([FromBody] UpdateUserRequest request) { ... }
```

### Sending URL Parameters (GET)

```javascript
// Option 1: data object — jQuery appends to URL automatically
$.ajax({
    url:      '/Admin/GetUser',
    type:     'GET',
    data:     { userId: 'sumit', includeHistory: true },
    dataType: 'json',
    success:  function(data) { ... }
    // Actual URL sent: /Admin/GetUser?userId=sumit&includeHistory=true
});

// Option 2: build URL yourself
$.ajax({
    url:     '/Admin/GetUser?userId=' + encodeURIComponent(userId),
    type:    'GET',
    dataType: 'json',
    success: function(data) { ... }
});
```

---

## 5. Shorthand Methods — For Simple Calls

jQuery provides shorthand methods for the most common patterns.
Use these when you don't need all the options of `$.ajax()`.

```javascript
// ── $.get() — GET request ────────────────────────────────────────────
// $.get(url, data, callback, dataType)

$.get('/Admin/GetUsers', function(data) {
    console.log(data);
});

$.get('/Admin/GetUser', { userId: 'sumit' }, function(data) {
    console.log(data.userId);
});


// ── $.post() — POST request ──────────────────────────────────────────
// $.post(url, data, callback, dataType)

$.post('/Admin/UnlockUser', { userId: 'sumit', __RequestVerificationToken: token },
    function(data) {
        console.log(data.success);
    }
);


// ── $.getJSON() — GET request, expects JSON ───────────────────────────
// Shorthand for $.ajax({ type: 'GET', dataType: 'json' })

$.getJSON('/Admin/GetUsers', function(data) {
    // data is already parsed — no need for JSON.parse
    data.forEach(function(user) {
        console.log(user.userId);
    });
});

$.getJSON('/Admin/GetUser', { userId: 'sumit' }, function(data) {
    console.log(data.role);
});
```

| Shorthand | Full equivalent | Has error callback? |
|---|---|---|
| `$.get(url, fn)` | `$.ajax({ type:'GET', success:fn })` | ❌ (use .fail()) |
| `$.post(url, data, fn)` | `$.ajax({ type:'POST', success:fn })` | ❌ (use .fail()) |
| `$.getJSON(url, fn)` | `$.ajax({ type:'GET', dataType:'json', success:fn })` | ❌ (use .fail()) |
| `$.ajax({...})` | — | ✅ Yes, `error` callback |

**For production code, always use `$.ajax()` so you can handle errors properly.**
Shorthands are fine for quick tests and simple read-only calls.

---

## 6. jqXHR — The Return Value You Shouldn't Ignore

`$.ajax()` returns a **jqXHR** object — a Promise-like object.
You can attach `.done()`, `.fail()`, `.always()` to it — alternative to callbacks.

```javascript
// Style 1 — callbacks inside the options (most common in Kendo projects):
$.ajax({
    url:     '/Admin/GetUsers',
    type:    'GET',
    success: function(data) { ... },
    error:   function(xhr)  { ... }
});


// Style 2 — chained methods on the return value:
var request = $.ajax({ url: '/Admin/GetUsers', type: 'GET' });

request
    .done(function(data) {
        // Same as success
        renderGrid(data);
    })
    .fail(function(xhr, status, error) {
        // Same as error
        handleError(xhr.status);
    })
    .always(function() {
        // Same as complete
        hideSpinner();
    });


// Style 3 — stored and used later (useful for abort):
var activeRequest = $.ajax({ url: '/Admin/GetUsers', type: 'GET' })
    .done(function(data) { renderGrid(data); })
    .fail(function(xhr)  { handleError(xhr.status); });

// Cancel the request if needed:
activeRequest.abort();
```

| Callback option | Chained method | Meaning |
|---|---|---|
| `success` | `.done()` | Runs on success |
| `error` | `.fail()` | Runs on error |
| `complete` | `.always()` | Always runs |

---

## 7. Aborting a Request

Sometimes users click a button multiple times. You want to cancel the previous
request before sending a new one.

```javascript
var currentRequest = null;

$('#searchBtn').click(function() {
    var searchTerm = $('#searchInput').val();

    // Abort any in-progress request from previous click
    if (currentRequest !== null) {
        currentRequest.abort();
    }

    currentRequest = $.ajax({
        url:      '/Admin/SearchUsers',
        type:     'GET',
        data:     { term: searchTerm },
        dataType: 'json',
        success: function(data) {
            renderGrid(data);
            currentRequest = null;    // clear when done
        },
        error: function(xhr, status) {
            if (status !== 'abort') {   // don't show error for intentional abort
                showError('Search failed.');
            }
        }
    });
});
```

---

## 8. Global AJAX Handlers — $(document).ajaxX

jQuery lets you register handlers that fire for **every** AJAX request on the page.
You set these up once, and they work for all `$.ajax()` calls automatically.

```javascript
// Put this in your main JS file or _Layout.cshtml Scripts section

// ── Runs before EVERY AJAX request ──────────────────────────────────
$(document).ajaxStart(function() {
    $('#globalSpinner').show();   // show a loading indicator for any request
});

// ── Runs after EVERY AJAX request (success or error) ─────────────────
$(document).ajaxStop(function() {
    $('#globalSpinner').hide();   // hide loading indicator when all done
});

// ── Runs when ANY AJAX request succeeds ──────────────────────────────
$(document).ajaxSuccess(function(event, jqXHR, settings) {
    // event.target = the element that triggered it (not always available)
    // settings.url = the URL that was requested
    console.log('Request succeeded:', settings.url);
});

// ── Runs when ANY AJAX request fails ─────────────────────────────────
$(document).ajaxError(function(event, jqXHR, settings, errorThrown) {
    // THIS IS THE MOST IMPORTANT ONE FOR YOUR PROJECT
    // Handles 401 and 403 globally — no need to repeat in every $.ajax call

    if (jqXHR.status === 401) {
        // Session expired — redirect to login
        window.location.href = '/Account/Login';
        return;
    }

    if (jqXHR.status === 403) {
        alert('You do not have permission for this action.');
        return;
    }

    // Log other errors for debugging
    console.error('AJAX Error:', jqXHR.status, settings.url);
});

// ── Runs before EVERY request is sent ────────────────────────────────
$(document).ajaxSend(function(event, jqXHR, settings) {
    console.log('Sending request to:', settings.url);
});
```

### Why $(document).ajaxError is Critical for Your Project

```
WITHOUT global handler:                 WITH global handler:
────────────────────────                ────────────────────────────────
Every $.ajax must handle 401:           Set once, works everywhere:

$.ajax({                                $(document).ajaxError(function(e, xhr) {
    url: '/Admin/GetUsers',                 if (xhr.status === 401)
    error: function(xhr) {                      window.location.href = '/Login';
        if (xhr.status === 401)         });
            window.location = '/Login';
    }                                   $.ajax({
});                                         url: '/Admin/GetUsers',
                                            // no need for 401 handling here
$.ajax({
    url: '/Admin/UnlockUser',           $.ajax({
    error: function(xhr) {                  url: '/Admin/UnlockUser',
        if (xhr.status === 401)             // no need for 401 handling here
            window.location = '/Login';
    }
});
// Repeated in EVERY call             // Clean. DRY. Consistent.
```

---

## 9. $.ajaxSetup — Global Default Settings

Set default options that apply to all `$.ajax()` calls.

```javascript
// Put this early in your page load — in $(document).ready() or _Layout Scripts

$.ajaxSetup({
    // All AJAX calls expect JSON back
    dataType: 'json',

    // All AJAX calls include the anti-forgery token in headers
    beforeSend: function(xhr) {
        xhr.setRequestHeader(
            'RequestVerificationToken',
            $('input[name="__RequestVerificationToken"]').val()
        );
    },

    // Don't cache GET requests (prevents stale data)
    cache: false,

    // Abort if no response in 30 seconds
    timeout: 30000
});

// Now individual calls are simpler — they inherit the defaults:
$.ajax({
    url:  '/Admin/GetUsers',
    type: 'GET',
    success: function(data) { renderGrid(data); }
    // dataType, beforeSend, cache, timeout inherited from ajaxSetup
});
```

---

## 10. Complete Real-World Patterns for Your Project

### Pattern 1 — Load Data on Page Ready

```javascript
$(document).ready(function() {
    loadUserGrid();
});

function loadUserGrid() {
    $('#spinner').show();

    $.ajax({
        url:      '/Admin/GetUsers',
        type:     'GET',
        dataType: 'json',
        success: function(data) {
            $('#userCount').text('Total users: ' + data.length);
            renderGrid(data);
        },
        error: function(xhr) {
            showError('Failed to load users.');
        },
        complete: function() {
            $('#spinner').hide();
        }
    });
}
```

### Pattern 2 — AJAX Form Submit (No Page Reload)

```javascript
$('#loginForm').submit(function(e) {
    e.preventDefault();    // ← stop normal form submit (no page reload)

    var $btn = $(this).find('[type="submit"]');
    $btn.prop('disabled', true).text('Logging in...');

    $.ajax({
        url:      $(this).attr('action'),    // use form's own action
        type:     'POST',
        data:     $(this).serialize(),       // all fields + token
        dataType: 'json',
        success: function(data) {
            if (data.success) {
                window.location.href = data.redirectUrl;
            } else {
                $('#errorMsg').text(data.message).show();
            }
        },
        error: function(xhr) {
            $('#errorMsg').text('Login failed. Please try again.').show();
        },
        complete: function() {
            $btn.prop('disabled', false).text('Login');
        }
    });
});
```

### Pattern 3 — Confirm Then AJAX Action

```javascript
function unlockUser(userId) {
    // Always confirm destructive/important actions
    if (!confirm('Unlock user "' + userId + '"? They will be able to log in again.')) {
        return;
    }

    $.ajax({
        url:  '/Admin/UnlockUser',
        type: 'POST',
        data: {
            userId:                     userId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        dataType: 'json',
        success: function(data) {
            if (data.success) {
                // Update just the affected row — no full reload
                $('#status-' + userId).html('<span class="green">✅ Active</span>');
                $('#btn-' + userId).hide();
                showToast(data.message, 'success');
            } else {
                showToast(data.message, 'error');
            }
        },
        error: function(xhr) {
            // 401 handled globally — only handle other errors here
            if (xhr.status !== 401) {
                showToast('Failed to unlock user. Please try again.', 'error');
            }
        }
    });
}
```

### Pattern 4 — Auto-Refresh (Polling)

```javascript
// Refresh user status every 30 seconds
var refreshInterval;

function startAutoRefresh() {
    refreshInterval = setInterval(function() {
        $.ajax({
            url:      '/Admin/GetUsers',
            type:     'GET',
            dataType: 'json',
            success:  function(data) { updateGridSilently(data); }
            // No error callback — global handler covers it
        });
    }, 30000);   // 30,000ms = 30 seconds
}

function stopAutoRefresh() {
    clearInterval(refreshInterval);
}

$(document).ready(function() {
    loadUserGrid();
    startAutoRefresh();
});
```

---

## 11. $.ajax() vs fetch() — Final Comparison

| | $.ajax() | fetch() |
|---|---|---|
| **4xx/5xx errors** | ✅ Goes to `error` callback automatically | ❌ Goes to `.then()` — must check `response.ok` |
| **JSON parsing** | ✅ Auto-parsed with `dataType: 'json'` | Manual — `response.json()` |
| **Global error handler** | ✅ `$(document).ajaxError` | ❌ No built-in equivalent |
| **Abort support** | ✅ `jqXHR.abort()` | ✅ `AbortController` (more complex) |
| **jQuery dependency** | Required | Not needed |
| **Kendo compatibility** | ✅ Same library | ❌ Kendo uses jQuery internally |
| **Code style** | Options object + callbacks | Promises / async/await |
| **IE support** | ✅ Via jQuery | ❌ IE not supported |

**Use `$.ajax()` in your Kendo + MVC project. Always.**

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **$.ajax({})** | The main function — options object with url, type, data, callbacks |
| **dataType: 'json'** | Tells jQuery to auto-parse response as JSON — no JSON.parse needed |
| **contentType: 'application/json'** | Required when sending JSON.stringify data with [FromBody] |
| **success** | Runs on 2xx — receives parsed data as first argument |
| **error** | Runs on 4xx/5xx — jqXHR.status gives the code |
| **complete** | Always runs — use for spinner hide, button re-enable |
| **beforeSend** | Runs before request — add headers, show spinner, cancel if needed |
| **$.get / $.post / $.getJSON** | Shorthands — good for simple calls, no error handling |
| **jqXHR.abort()** | Cancel an in-progress request |
| **$(document).ajaxError** | Global error handler — handle 401/403 once for all requests |
| **$.ajaxSetup** | Set defaults for all $.ajax calls — token, dataType, timeout |
| **serialize()** | Serialize entire form to string — includes anti-forgery token |
| **e.preventDefault()** | Stop default form submit — required for AJAX form handling |

---

## What's Next — Chapter 4

**AJAX with ASP.NET Core MVC — Wiring Both Sides Together**

You now know `$.ajax()` deeply from the JavaScript side.
Chapter 4 covers the **controller side** completely — every way to receive
AJAX data, every way to return responses, anti-forgery deep dive,
the `[FromBody]` vs form binding decision, and handling errors
on both sides cleanly. This is where JavaScript and C# connect.

---

*AJAX Chapter 3 of 5 — Sumit's ASP.NET Core MVC Learning Journey*
