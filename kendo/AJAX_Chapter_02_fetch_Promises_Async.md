# AJAX — Chapter 2
### fetch() + Promises + async/await — The Modern Way

---

## Prerequisites

| What You Need |
|---|
| AJAX Chapter 1 — callbacks, async concept, event loop |
| JSON Complete Notes — JSON.parse, JSON.stringify |
| Basic JavaScript — functions, arrow functions |

---

## 1. Why fetch() Exists — The Callback Problem

In Chapter 1 you saw the callback pattern.
Callbacks work, but they create a serious problem when you have
multiple async operations that depend on each other.

```javascript
// Real scenario: Get user → then get their orders → then get order details
// With callbacks — "Callback Hell" (Pyramid of Doom)

getUser('sumit', function(user) {
    // Got user...
    getOrders(user.userId, function(orders) {
        // Got orders...
        getOrderDetails(orders[0].id, function(details) {
            // Got details...
            getProductInfo(details.productId, function(product) {
                // Got product...
                // This keeps going deeper and deeper
                // Hard to read, hard to debug, hard to maintain
                console.log(product.name);
            });
        });
    });
});

// This is called "Callback Hell" or the "Pyramid of Doom"
// Each level is indented more. Error handling is a nightmare.
```

**Promises** were invented to solve this.
**fetch()** uses Promises.
**async/await** makes Promises look like synchronous code.

All three build on each other. This chapter covers all three in order.

---

## 2. What is a Promise

**Simple definition:**
> A Promise is an object that represents the eventual result
> of an asynchronous operation. It is a placeholder that says:
> "I don't have the value yet, but I promise to give it to you
> when it's ready — or tell you if it failed."

A Promise is always in one of three states:

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│   PENDING ──── operation running ──── FULFILLED (success)  │
│      │                                  (has a value)      │
│      │                                                      │
│      └─────────────────────────────── REJECTED (failure)   │
│                                         (has a reason)     │
│                                                             │
└─────────────────────────────────────────────────────────────┘

States:
PENDING    → Request sent, waiting for response
FULFILLED  → Response arrived, all good (status 2xx)
REJECTED   → Something went wrong (network error, 4xx, 5xx)

Once a Promise moves from PENDING → FULFILLED or REJECTED,
it NEVER changes state again. It is permanent.
```

### Real World Analogy

> A Promise is like an **order receipt at a restaurant**.
> When you order, the cashier gives you a receipt (the Promise object).
> You walk away with the receipt — you don't stand there waiting.
> When your order is ready (FULFILLED), they call your number.
> If they run out of ingredients (REJECTED), they come tell you.
> The receipt itself is the placeholder — not the food.

---

## 3. Creating and Using a Promise

```javascript
// Creating a Promise — you usually don't create them manually
// fetch() and other APIs create them for you
// But knowing the structure helps you understand what's happening

var myPromise = new Promise(function(resolve, reject) {
    // Do async work here
    var success = true;   // simulate success/failure

    if (success) {
        resolve("Here is the data!");   // fulfilled with this value
    } else {
        reject("Something went wrong"); // rejected with this reason
    }
});

// Using a Promise — .then() and .catch()
myPromise
    .then(function(result) {
        console.log(result);   // "Here is the data!"
        // .then() runs when the Promise is FULFILLED
    })
    .catch(function(error) {
        console.log(error);    // "Something went wrong"
        // .catch() runs when the Promise is REJECTED
    })
    .finally(function() {
        console.log("Always runs — like complete() in $.ajax");
        // .finally() runs regardless of fulfilled or rejected
    });
```

### .then() .catch() .finally() — The Three Handlers

| Method | Runs when | Equivalent in $.ajax |
|---|---|---|
| `.then(fn)` | Promise fulfilled (success) | `success: function(data){}` |
| `.catch(fn)` | Promise rejected (error) | `error: function(xhr){}` |
| `.finally(fn)` | Always, after then or catch | `complete: function(){}` |

---

## 4. Promise Chaining — Solving Callback Hell

The real power of Promises is **chaining** — `.then()` returns a new Promise,
so you can chain operations one after another, flat and readable.

```javascript
// Callback Hell (Chapter 1 pattern):
getUser('sumit', function(user) {
    getOrders(user.userId, function(orders) {
        getDetails(orders[0].id, function(details) {
            console.log(details);
        });
    });
});

// Promise Chain — flat, readable, same logic:
getUser('sumit')
    .then(function(user) {
        return getOrders(user.userId);   // return next Promise
    })
    .then(function(orders) {
        return getDetails(orders[0].id); // return next Promise
    })
    .then(function(details) {
        console.log(details);            // final result
    })
    .catch(function(error) {
        // ONE catch handles errors from ANY step above
        console.error('Something failed:', error);
    });
```

**Key rule for chaining:** Return the next Promise from inside `.then()`.
If you don't return it, the chain doesn't wait for it.

```javascript
// ❌ Wrong — not returning the next Promise
.then(function(user) {
    getOrders(user.userId);   // missing return — chain doesn't wait!
})
.then(function(orders) {
    // orders is undefined — previous .then didn't return anything
})

// ✅ Correct — return the next Promise
.then(function(user) {
    return getOrders(user.userId);   // ← return it
})
.then(function(orders) {
    // orders has actual data now
})
```

---

## 5. fetch() — The Modern AJAX API

`fetch()` is built into modern browsers. No jQuery needed.
It takes a URL, returns a Promise.

### Simplest possible fetch:

```javascript
fetch('/Admin/GetUsers')
    .then(function(response) {
        // response is the HTTP response object
        // It is NOT the data yet — you need to parse it
        return response.json();   // parse JSON body — also returns a Promise
    })
    .then(function(data) {
        // Now data is your actual JavaScript object/array
        console.log(data);
    })
    .catch(function(error) {
        console.error('Request failed:', error);
    });
```

### Why Two .then() Calls?

This confuses everyone at first. Here's why:

```
fetch() returns a Promise<Response>
            │
            ▼
.then(response => ...)
The response object has:
  response.status       → 200, 404, 500
  response.ok           → true if status 200-299
  response.headers      → HTTP headers
  response.json()       → reads the body as JSON → returns Promise<data>
  response.text()       → reads the body as text → returns Promise<string>
            │
            │  response.json() returns ANOTHER Promise
            ▼
.then(data => ...)
Now data is your actual parsed object — { userId: "sumit", ... }
```

```javascript
fetch('/Admin/GetUsers')
    .then(function(response) {
        //  response = HTTP response envelope
        //  { status: 200, ok: true, headers: {...}, body: [unread stream] }
        return response.json();
        //  .json() reads the body stream and parses it
        //  returns a new Promise
    })
    .then(function(data) {
        //  data = [ {userId:"sumit",...}, {userId:"rahul",...} ]
        //  This is your actual data
        renderGrid(data);
    });
```

---

## 6. fetch() — GET and POST

### GET Request

```javascript
// Basic GET — simplest form
fetch('/Admin/GetUsers')
    .then(r => r.json())
    .then(data => console.log(data))
    .catch(err => console.error(err));


// GET with query parameters
fetch('/Admin/GetUser?userId=sumit')
    .then(r => r.json())
    .then(data => console.log(data));


// GET with options object
fetch('/Admin/GetUsers', {
    method:  'GET',                          // default — can omit for GET
    headers: {
        'Accept': 'application/json'         // tell server you want JSON back
    }
})
.then(r => r.json())
.then(data => console.log(data));
```

### POST Request

```javascript
// POST with JSON body
fetch('/Admin/UnlockUser', {
    method:  'POST',
    headers: {
        'Content-Type':              'application/json',
        'RequestVerificationToken':  document.querySelector('input[name="__RequestVerificationToken"]').value
    },
    body: JSON.stringify({ userId: 'sumit' })   // must stringify the object
})
.then(r => r.json())
.then(data => {
    if (data.success) {
        console.log(data.message);
    }
})
.catch(err => console.error(err));


// POST with form-encoded data (no [FromBody] needed in controller)
var formData = new URLSearchParams();
formData.append('userId', 'sumit');
formData.append('__RequestVerificationToken', tokenValue);

fetch('/Admin/UnlockUser', {
    method:  'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body:    formData
})
.then(r => r.json())
.then(data => console.log(data));
```

---

## 7. fetch() Error Handling — The Important Gotcha

**This is the most common mistake with fetch().**

```javascript
// ❌ THIS IS WRONG — fetch() does NOT reject on 4xx/5xx
fetch('/Admin/GetUsers')
    .then(r => r.json())
    .then(data => console.log(data))
    .catch(err => {
        // This ONLY runs on network failure (no internet, server unreachable)
        // It does NOT run for 401, 403, 404, 500
        // So a 401 response silently goes to .then() — confusing!
    });
```

```javascript
// ✅ CORRECT — always check response.ok before parsing

fetch('/Admin/GetUsers')
    .then(function(response) {
        // response.ok is true for status 200-299
        if (!response.ok) {
            // Manually throw an error for 4xx/5xx
            throw new Error('HTTP error: ' + response.status);
        }
        return response.json();
    })
    .then(function(data) {
        renderGrid(data);
    })
    .catch(function(error) {
        // Now catches BOTH network failures AND 4xx/5xx
        if (error.message.includes('401')) {
            window.location.href = '/Account/Login';
        } else {
            console.error('Request failed:', error.message);
        }
    });
```

### fetch() vs $.ajax() — How Errors Work

| Situation | fetch() behaviour | $.ajax() behaviour |
|---|---|---|
| Network failure | `.catch()` runs | `error` callback runs |
| 401 Unauthorized | **`.then()` runs** (gotcha!) | `error` callback runs |
| 403 Forbidden | **`.then()` runs** (gotcha!) | `error` callback runs |
| 500 Server Error | **`.then()` runs** (gotcha!) | `error` callback runs |
| 200 OK | `.then()` runs | `success` callback runs |

**This is why jQuery's `$.ajax()` is easier for error handling in MVC projects.**
`$.ajax()` treats 4xx/5xx as errors automatically. You don't need the `response.ok` check.

---

## 8. async/await — Promises That Look Synchronous

`async/await` is syntactic sugar over Promises.
It makes async code look like regular synchronous code.
Same thing underneath — just easier to read and write.

### The Two Keywords

```javascript
// async — marks a function as async (it will return a Promise)
async function loadUsers() {
    // ...
}

// await — pauses INSIDE the async function until the Promise resolves
// Can ONLY be used inside an async function
async function loadUsers() {
    var response = await fetch('/Admin/GetUsers');
    var data     = await response.json();
    return data;
}
```

### Side-by-Side Comparison

```javascript
// Same logic — Promise chain vs async/await

// PROMISE CHAIN VERSION:
function loadAndRenderUsers() {
    fetch('/Admin/GetUsers')
        .then(function(response) {
            if (!response.ok) throw new Error(response.status);
            return response.json();
        })
        .then(function(data) {
            renderGrid(data);
        })
        .catch(function(error) {
            showError(error.message);
        });
}

// ASYNC/AWAIT VERSION — same logic, reads like synchronous code:
async function loadAndRenderUsers() {
    try {
        var response = await fetch('/Admin/GetUsers');
        if (!response.ok) throw new Error(response.status);
        var data = await response.json();
        renderGrid(data);
    } catch (error) {
        showError(error.message);
    }
}
```

**`await` pauses execution inside the function** — but not in the browser.
The browser is still responsive. Other JavaScript can still run.
It is just a cleaner way to write "wait for this Promise to resolve, then continue."

---

## 9. async/await — Real Examples for Your Project

```javascript
// ── Loading Kendo Grid data ──────────────────────────────────────────
async function loadUsers() {
    try {
        var response = await fetch('/Admin/GetUsers');

        if (!response.ok) {
            if (response.status === 401) {
                window.location.href = '/Account/Login';
                return;
            }
            throw new Error('Failed to load users: ' + response.status);
        }

        var users = await response.json();
        return users;

    } catch (error) {
        showErrorMessage('Could not load users. Please refresh.');
        return [];
    }
}


// ── Unlock user via AJAX ─────────────────────────────────────────────
async function unlockUser(userId) {
    var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        var response = await fetch('/Admin/UnlockUser', {
            method:  'POST',
            headers: {
                'Content-Type':             'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({ userId: userId })
        });

        if (!response.ok) throw new Error(response.status);

        var result = await response.json();

        if (result.success) {
            showSuccess(result.message);
            refreshGrid();              // refresh Kendo grid
        } else {
            showError(result.message);
        }

    } catch (error) {
        showError('Request failed. Please try again.');
    }
}
```

---

## 10. Promise.all — Running Multiple Requests Simultaneously

Sometimes you need to load data from multiple endpoints at once.
`Promise.all` runs them in parallel and waits for ALL to finish.

```javascript
// Sequential — slow (each waits for previous to finish)
var users    = await fetch('/Admin/GetUsers').then(r => r.json());
var roles    = await fetch('/Admin/GetRoles').then(r => r.json());
var settings = await fetch('/Admin/GetSettings').then(r => r.json());
// Total time: time1 + time2 + time3

// Parallel with Promise.all — fast (all run at the same time)
var [users, roles, settings] = await Promise.all([
    fetch('/Admin/GetUsers').then(r => r.json()),
    fetch('/Admin/GetRoles').then(r => r.json()),
    fetch('/Admin/GetSettings').then(r => r.json())
]);
// Total time: max(time1, time2, time3)
```

```
Promise.all behaviour:
  All 3 succeed → resolves with array of all 3 results
  Any 1 fails   → immediately rejects with that error
                  (other requests may still be running but results are discarded)
```

---

## 11. fetch() vs $.ajax() — Full Comparison

You need to know both. Here is when to use which.

| | fetch() | $.ajax() |
|---|---|---|
| **Library needed** | None (built into browser) | jQuery required |
| **Returns** | Promise | jqXHR (Promise-like) |
| **Syntax style** | Promises / async/await | Callbacks / options object |
| **Error on 4xx/5xx** | ❌ Does NOT reject — must check response.ok | ✅ Goes to error callback automatically |
| **Anti-forgery token** | Set manually in headers | Set in data or headers |
| **JSON parsing** | Manual — `response.json()` | Automatic with `dataType: 'json'` |
| **Kendo DataSource** | Not used directly | Kendo uses jQuery AJAX internally |
| **Abort request** | `AbortController` | `xhr.abort()` |
| **Progress tracking** | Limited | Limited |
| **Browser support** | IE: No (Edge/Chrome/FF: Yes) | All browsers via jQuery |
| **When to use** | Modern projects, no jQuery, cleaner code | Kendo projects, error handling simplicity |

**For your daily work with Kendo:** use `$.ajax()` — Chapter 3 covers it in full depth.
**For new standalone features without Kendo:** `fetch()` with `async/await` is cleaner.

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **Promise** | Placeholder object for an async result — pending, fulfilled, or rejected |
| **Pending** | Request sent, waiting for response |
| **Fulfilled** | Response arrived successfully |
| **Rejected** | Something went wrong — network or error status |
| **`.then(fn)`** | Runs when Promise is fulfilled — receives the value |
| **`.catch(fn)`** | Runs when Promise is rejected — receives the error |
| **`.finally(fn)`** | Always runs after then or catch — good for cleanup |
| **Promise chaining** | Return next Promise from .then() — flat alternative to callback hell |
| **fetch()** | Built-in browser function — returns a Promise for HTTP requests |
| **response.ok** | True for 2xx status — always check this, fetch doesn't auto-reject 4xx |
| **response.json()** | Reads and parses the response body — returns another Promise |
| **async function** | Marks a function as async — it always returns a Promise |
| **await** | Pauses inside async function until Promise resolves — cleaner than .then() |
| **Promise.all** | Run multiple Promises in parallel — waits for all to finish |

---

## What's Next — Chapter 3

**jQuery $.ajax() — Your Daily Tool**

`$.ajax()` is what you will write every day with Kendo.
Chapter 3 is the deepest chapter — covering every option, parameter,
and pattern of `$.ajax()` with real examples from your project.
Includes: all shorthand methods, global AJAX handlers, request queuing,
aborting requests, and all the patterns you need for production code.

---

*AJAX Chapter 2 of 5 — Sumit's ASP.NET Core MVC Learning Journey*
