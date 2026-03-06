# AJAX — Chapter 2
### fetch() + Promises + async/await — The Modern Way

---

## Prerequisites

| What You Need |
|---|
| AJAX Chapter 1 — callbacks, async concept, Event Loop |
| JSON Complete Notes — JSON.parse(), JSON.stringify() |

---

## 1. Why fetch() — What Problem It Solves

Chapter 1 showed XMLHttpRequest — powerful but ugly.
jQuery's `$.ajax()` is cleaner but requires jQuery to be loaded.

**`fetch()` is built directly into every modern browser.**
No library. No jQuery. No setup. Just write it.

But more importantly — `fetch()` introduced a new concept: **Promises**.
Promises are not just a fetch thing. They are the foundation of all modern
async JavaScript. Once you understand Promises, you understand:

- `fetch()`
- `async/await`
- Kendo's async operations
- Any modern JavaScript API

```
Callback style (Chapter 1 / jQuery):          Promise style (fetch / modern):
──────────────────────────────────            ──────────────────────────────
$.ajax({                                       fetch('/api/users')
    url: '/api/users',                           .then(r  => r.json())
    success: function(data) {                    .then(data => {
        process(data);                               process(data);
    },                                           })
    error: function(xhr) {                       .catch(error => {
        handleError(xhr);                            handleError(error);
    }                                            });
});

Both do the same thing. Promise style is more readable
and chains multiple operations cleanly.
```

---

## 2. What is a Promise

**Simple definition:**
> A Promise is an object that represents the eventual result of an
> async operation. It is a placeholder that says:
> "I don't have the value yet, but I promise to give it to you when ready."

A Promise is always in one of three states:

```
┌──────────────────────────────────────────────────────────────┐
│                                                              │
│   PENDING ──── operation in progress ────> FULFILLED ✅     │
│      │                                     (resolved with   │
│      │                                      the value)      │
│      │                                                       │
│      └────────── something went wrong ───> REJECTED ❌      │
│                                             (rejected with  │
│                                              an error)      │
│                                                              │
└──────────────────────────────────────────────────────────────┘

Once a Promise is FULFILLED or REJECTED, it never changes state.
A fulfilled Promise stays fulfilled forever.
```

**Real world analogy:**

> You order a pizza online. The app gives you a tracking page.
>
> - **Pending** — pizza is being prepared. You have the tracking page (the Promise object) but no pizza yet.
> - **Fulfilled** — pizza delivered. Tracking page shows "Delivered". You have your pizza (the value).
> - **Rejected** — delivery failed. Tracking page shows "Failed". You get an error (the reason).
>
> The tracking page IS the Promise. The pizza IS the resolved value.

---

## 3. Creating and Using a Promise

```javascript
// Creating a Promise — inside it you do your async work
var myPromise = new Promise(function(resolve, reject) {
    //                                ↑         ↑
    //                          call when     call when
    //                          success       failure

    // Simulate async work (like a network request)
    setTimeout(function() {
        var success = true;

        if (success) {
            resolve("Here is your data!");   // Promise fulfilled with this value
        } else {
            reject("Something went wrong.");  // Promise rejected with this reason
        }
    }, 1000);
});

// Using (consuming) a Promise
myPromise
    .then(function(value) {
        // Runs when Promise is FULFILLED
        // 'value' is what was passed to resolve()
        console.log(value);   // "Here is your data!"
    })
    .catch(function(error) {
        // Runs when Promise is REJECTED
        // 'error' is what was passed to reject()
        console.log(error);   // "Something went wrong."
    })
    .finally(function() {
        // Runs ALWAYS — after .then() or .catch()
        // Same as 'complete' in $.ajax()
        hideLoadingSpinner();
    });
```

### The Three Methods

| Method | When it runs | Equivalent in $.ajax() |
|---|---|---|
| `.then(value => ...)` | Promise fulfilled (success) | `success: function(data)` |
| `.catch(error => ...)` | Promise rejected (failure) | `error: function(xhr)` |
| `.finally(() => ...)` | Always — after either | `complete: function()` |

---

## 4. Promise Chaining — The Real Power

The real reason Promises are better than callbacks:
**you can chain them** in a flat, readable sequence.

```javascript
// Without chaining (callback hell — nested and hard to read)
getUser(userId, function(user) {
    getOrders(user.id, function(orders) {
        getOrderDetails(orders[0].id, function(details) {
            displayDetails(details);   // 4 levels deep — "callback hell"
        });
    });
});


// With Promise chaining (flat and readable)
getUser(userId)
    .then(user    => getOrders(user.id))
    .then(orders  => getOrderDetails(orders[0].id))
    .then(details => displayDetails(details))
    .catch(error  => handleError(error));   // one catch handles ALL errors above
```

**Key rule of chaining:**
> If a `.then()` returns a value, the next `.then()` receives that value.
> If a `.then()` returns a Promise, the chain waits for that Promise to resolve.

```javascript
fetch('/api/users')                        // returns a Promise<Response>
    .then(response => response.json())     // .json() also returns a Promise<data>
    .then(data => {                        // data is the actual parsed object
        console.log(data);
    })
    .catch(error => {
        console.error(error);
    });
```

---

## 5. The fetch() API — Complete Reference

`fetch()` returns a Promise that resolves to a `Response` object.

```
fetch(url, options)
  ↓
Promise<Response>      ← resolves when headers arrive (not body yet)
  ↓
response.json()
  ↓
Promise<data>          ← resolves when body is fully parsed
  ↓
your data object
```

### Basic GET Request

```javascript
fetch('/Admin/GetUsers')
    .then(function(response) {
        // response is a Response object — not the data yet!
        // Need to parse the JSON body
        return response.json();     // returns another Promise
    })
    .then(function(data) {
        // data is now your actual JavaScript object/array
        console.log(data);
        renderGrid(data);
    })
    .catch(function(error) {
        console.error('Fetch failed:', error);
    });
```

### The Response Object — What You Get First

```javascript
fetch('/Admin/GetUsers')
    .then(function(response) {
        // Response has these important properties:
        console.log(response.status);       // 200, 401, 403, 500
        console.log(response.ok);           // true if status is 200-299
        console.log(response.statusText);   // "OK", "Not Found", etc.
        console.log(response.headers.get('Content-Type'));

        // You MUST call one of these to read the body:
        // response.json()     → parses JSON → returns Promise
        // response.text()     → raw text   → returns Promise
        // response.blob()     → binary data → returns Promise (for files)

        return response.json();
    });
```

### CRITICAL — fetch() Does NOT Reject on 4xx/5xx

This is the most common mistake with `fetch()`:

```javascript
// ❌ WRONG ASSUMPTION — .catch() does NOT run for 401, 404, 500
fetch('/Admin/GetUsers')
    .then(data => console.log(data))
    .catch(error => console.log('Error!'));   // this only runs for NETWORK failures

// fetch() only rejects if the request never got through:
//   - no internet connection
//   - DNS failure
//   - server completely unreachable
//
// If the server responds with 401, 403, 500 — fetch() RESOLVES (not rejects)
// You must check response.ok yourself!
```

```javascript
// ✅ CORRECT — check response.ok for HTTP errors
fetch('/Admin/GetUsers')
    .then(function(response) {
        if (!response.ok) {
            // Manually reject for HTTP error responses
            throw new Error('HTTP Error: ' + response.status);
        }
        return response.json();
    })
    .then(function(data) {
        renderGrid(data);
    })
    .catch(function(error) {
        // Now catches both network failures AND HTTP errors
        console.error(error.message);

        if (error.message.includes('401')) {
            window.location.href = '/Account/Login';
        }
    });
```

---

## 6. POST Request with fetch()

```javascript
// Sending a POST with JSON body
fetch('/Admin/UnlockUser', {
    method:  'POST',
    headers: {
        'Content-Type':              'application/json',
        'RequestVerificationToken':  document.querySelector('input[name="__RequestVerificationToken"]').value
    },
    body: JSON.stringify({ userId: 'sumit' })   // must stringify the object
})
.then(function(response) {
    if (!response.ok) throw new Error('HTTP ' + response.status);
    return response.json();
})
.then(function(data) {
    if (data.success) {
        showMessage(data.message, 'success');
        refreshGrid();
    } else {
        showMessage(data.message, 'error');
    }
})
.catch(function(error) {
    if (error.message.includes('401')) window.location.href = '/Account/Login';
    else showMessage('Request failed. Please try again.', 'error');
});
```

### fetch() Options Object — All Properties

```javascript
fetch(url, {
    method:       'POST',             // 'GET', 'POST', 'PUT', 'DELETE', 'PATCH'
    headers: {
        'Content-Type': 'application/json',
        'Accept':       'application/json',
        'Custom-Header': 'value'
    },
    body:          JSON.stringify(data), // only for POST/PUT/PATCH
    credentials:  'same-origin',         // 'omit' | 'same-origin' | 'include'
    //             ↑ 'same-origin' sends cookies automatically (needed for auth)
    cache:        'no-cache',            // 'default' | 'no-cache' | 'reload'
    mode:         'cors',                // 'cors' | 'no-cors' | 'same-origin'
});
```

**`credentials: 'same-origin'`** is important — it tells fetch to include
cookies (your auth cookie) in the request. Without it, `[Authorize]` will
reject every fetch call as unauthenticated.

---

## 7. async/await — Promises Without the Chain

`async/await` is syntax sugar over Promises. It makes async code look like
synchronous code — much easier to read and write.

### The Two Keywords

```javascript
// async — marks a function as asynchronous
//         it will always return a Promise
async function getUsers() {
    // ...
}

// await — pauses execution INSIDE the async function
//         until the Promise resolves
//         can only be used INSIDE an async function
async function getUsers() {
    var response = await fetch('/Admin/GetUsers');   // pauses here until response arrives
    var data     = await response.json();            // pauses here until body is parsed
    return data;                                     // returns a Promise<data>
}
```

### Side-by-Side Comparison

```javascript
// Same operation — three ways:

// WAY 1: Callbacks (Chapter 1 style)
$.ajax({
    url: '/Admin/GetUsers',
    success: function(data) {
        renderGrid(data);
    },
    error: function(xhr) {
        handleError(xhr.status);
    }
});


// WAY 2: Promises (.then chain)
fetch('/Admin/GetUsers')
    .then(r    => r.json())
    .then(data => renderGrid(data))
    .catch(err => handleError(err));


// WAY 3: async/await (cleanest)
async function loadUsers() {
    try {
        var response = await fetch('/Admin/GetUsers');
        if (!response.ok) throw new Error('HTTP ' + response.status);
        var data = await response.json();
        renderGrid(data);
    } catch (error) {
        handleError(error);
    }
}
loadUsers();
```

### Error Handling with async/await — try/catch

With `async/await`, you use regular `try/catch` instead of `.catch()`.
This is much more familiar — it's the same pattern as C# exception handling.

```javascript
async function unlockUser(userId) {
    try {
        var token    = document.querySelector('input[name="__RequestVerificationToken"]').value;

        var response = await fetch('/Admin/UnlockUser', {
            method:  'POST',
            headers: {
                'Content-Type':             'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({ userId: userId })
        });

        if (!response.ok) {
            if (response.status === 401) {
                window.location.href = '/Account/Login';
                return;
            }
            throw new Error('Server error: ' + response.status);
        }

        var data = await response.json();

        if (data.success) {
            showMessage(data.message, 'success');
            await refreshGrid();    // can await another async function
        } else {
            showMessage(data.message, 'error');
        }

    } catch (error) {
        // Catches: network failure, thrown errors, response.ok failures
        console.error('Unlock failed:', error.message);
        showMessage('Operation failed. Please try again.', 'error');
    } finally {
        // Runs always — same as .finally() on a Promise
        hideLoadingSpinner();
        enableButtons();
    }
}
```

---

## 8. Running Multiple Requests — Promise.all()

Sometimes you need to make several AJAX calls and wait for ALL of them
before doing something with the results.

```javascript
// ❌ Sequential — slow (waits for each one before starting next)
var users    = await fetch('/api/users').then(r => r.json());
var roles    = await fetch('/api/roles').then(r => r.json());
var settings = await fetch('/api/settings').then(r => r.json());
// Total time = time(users) + time(roles) + time(settings) → slow


// ✅ Parallel — fast (all three start at the same time)
var [users, roles, settings] = await Promise.all([
    fetch('/api/users').then(r    => r.json()),
    fetch('/api/roles').then(r    => r.json()),
    fetch('/api/settings').then(r => r.json())
]);
// Total time = longest of the three → much faster
console.log(users, roles, settings);   // all three available here
```

```
Promise.all([p1, p2, p3])
  → Starts all three Promises simultaneously
  → Waits until ALL are fulfilled
  → Returns array of results in same order as input
  → If ANY one rejects → the whole Promise.all rejects immediately
```

---

## 9. async/await in an Event Handler

In your Kendo/MVC pages, you'll use async/await inside click handlers and
Kendo callbacks. Here's the correct way to set that up.

```javascript
// ✅ Correct — async event handler
$('#btnUnlock').on('click', async function() {
    var userId = $(this).data('userid');

    $(this).prop('disabled', true);   // disable button to prevent double click
    showSpinner();

    try {
        var result = await unlockUser(userId);
        refreshGrid();
    } catch(e) {
        showError(e.message);
    } finally {
        $(this).prop('disabled', false);
        hideSpinner();
    }
});


// ✅ Correct — async function called from Kendo template button
async function handleUnlock(userId) {
    try {
        var response = await fetch('/Admin/UnlockUser', {
            method: 'POST',
            // ...
        });
        var data = await response.json();
        if (data.success) {
            $('#userGrid').data('kendoGrid').dataSource.read();
        }
    } catch(e) {
        console.error(e);
    }
}
```

---

## 10. fetch() vs $.ajax() — When to Use Which

You will use both. Know which one to reach for.

| | `fetch()` + async/await | `$.ajax()` |
|---|---|---|
| **Needs jQuery?** | No — built into browser | Yes — jQuery required |
| **Syntax style** | Promises / async/await | Callbacks / options object |
| **Kendo compatibility** | Works fine | Native — Kendo is built on jQuery |
| **Readability** | Very clean with async/await | Good, widely familiar |
| **Error handling** | Must check `response.ok` manually | 4xx/5xx auto-goes to error callback |
| **Browser support** | All modern browsers | All browsers (via jQuery) |
| **Anti-forgery token** | Manual in headers | Manual in data object |
| **Best for** | New code, modern style | Kendo-heavy pages, existing jQuery |
| **In your project** | Either works | Stick to `$.ajax()` in Kendo pages |

**Practical rule for your work:**
> On pages with Kendo Grid — use `$.ajax()` (Chapter 3).
> jQuery is already loaded by Kendo, so you have it for free.
> On pages without Kendo — use `fetch()` with `async/await`.

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **Promise** | Object representing a future value — pending, fulfilled, or rejected |
| **resolve()** | Fulfills a Promise — triggers `.then()` |
| **reject()** | Rejects a Promise — triggers `.catch()` |
| **.then()** | Runs when Promise is fulfilled — receives the resolved value |
| **.catch()** | Runs when Promise is rejected — receives the error |
| **.finally()** | Runs always — after .then or .catch — for cleanup |
| **Promise chaining** | `.then().then().catch()` — flat, readable sequence of async steps |
| **fetch()** | Built-in browser function that returns a Promise for HTTP requests |
| **response.ok** | MUST check this — fetch() does NOT reject on 4xx/5xx HTTP errors |
| **response.json()** | Parses the JSON body — also returns a Promise |
| **async** | Marks a function as async — it always returns a Promise |
| **await** | Pauses inside an async function until a Promise resolves |
| **try/catch with await** | Error handling for async/await — replaces .catch() |
| **Promise.all()** | Run multiple requests in parallel — wait for all to complete |
| **credentials: 'same-origin'** | fetch() option needed to send auth cookies |

---

## What's Next — Chapter 3

**jQuery $.ajax() — Your Daily Tool**

Chapter 3 is the deepest chapter. jQuery's `$.ajax()` is what you'll use
every day — especially in Kendo pages. It covers the full options object,
every shorthand method, how to configure it globally, how to handle
every type of data, how to chain requests, loading states, aborting
requests, and all the real-world patterns you'll actually use at work.

---

*AJAX Chapter 2 of 5 — Sumit's ASP.NET Core MVC Learning Journey*
