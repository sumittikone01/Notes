# AJAX — Chapter 1
### How AJAX Works — The Engine Under Everything

---

## Prerequisites

| What You Need |
|---|
| JSON Complete Notes — what JSON is, stringify, parse |
| Basic JavaScript — variables, functions, callbacks |
| HTTP basics — request/response (Chapter 1 of login series) |

---

## 1. What is AJAX — The One-Paragraph Definition

**AJAX** stands for **A**synchronous **J**avaScript **A**nd **X**ML.
Ignore the XML part — it's JSON now. The important word is **Asynchronous**.

**Simple definition:**
> AJAX is a technique that lets JavaScript send an HTTP request to a server
> in the background — while the page stays open and usable —
> and do something with the response when it comes back.

Before AJAX existed, every interaction with the server required a **full page reload**.
AJAX changed the web from page-flipping documents into smooth, app-like experiences.

```
WITHOUT AJAX — traditional web (1990s–early 2000s)
───────────────────────────────────────────────────
User clicks "Unlock User" button
  → Entire page disappears
  → White screen while loading
  → New page appears from scratch
  → User lost their scroll position
  → Every click = full page load
  → Feels like flipping physical pages

WITH AJAX — modern web
───────────────────────────────────────────────────
User clicks "Unlock User" button
  → Nothing visible happens for a split second
  → JavaScript sent a request in the background
  → Server processed it
  → Just that one row in the grid updated
  → No reload. No flicker. Instant.
  → Feels like a desktop application
```

---

## 2. Synchronous vs Asynchronous — The Core Concept

This is the most important concept in AJAX. Understand this deeply.

### Synchronous — Do One Thing, Wait, Then Next

```
Synchronous = blocking. One task must FINISH before the next one STARTS.

You at a restaurant (synchronous style):
  1. You order food
  2. You SIT AND STARE at the kitchen. Do nothing. Just wait.
  3. Food arrives.
  4. Only NOW can you talk to your friend, drink water, or do anything else.

This is how traditional page loading works.
The browser freezes while waiting for the server.
```

```javascript
// Synchronous example (hypothetical — not real AJAX)
var result = sendRequestAndWait('/api/users');   // BLOCKS HERE
// ↑ The entire browser freezes until this returns
// No scrolling, no clicking, nothing works
doSomethingWith(result);   // only runs after the above fully completes
```

### Asynchronous — Start a Task, Continue, Handle Result Later

```
Asynchronous = non-blocking. Start a task, keep doing other things,
               handle the result when it's ready (via callback).

You at a restaurant (asynchronous style):
  1. You order food
  2. The waiter takes your order to the kitchen
  3. While waiting, you talk to your friend, drink water, look at your phone
  4. When the food is ready, the waiter brings it to you
  5. You handle it (eat) when it arrives

This is how AJAX works.
JavaScript sends the request, immediately continues executing,
and handles the response when it comes back via a callback function.
```

```javascript
// Asynchronous — how AJAX actually works
sendAjaxRequest('/api/users', function(result) {
    // This callback runs WHEN the response comes back
    // Could be 50ms or 2000ms later — JavaScript doesn't freeze waiting
    doSomethingWith(result);
});

// These lines run IMMEDIATELY — don't wait for the request above
console.log('Request sent — continuing...');
updateProgressBar();
letUserKeepScrolling();
```

---

## 3. The Browser Event Loop — Why Async is Possible

To truly understand AJAX, you need to know one thing about how JavaScript runs.

JavaScript is **single-threaded**. It can only do one thing at a time.
So how can it send a network request AND keep the page responsive?

The answer is the **Event Loop**.

```
THE BROWSER HAS THREE PARTS:
─────────────────────────────────────────────────────────

┌─────────────────────────┐
│  CALL STACK             │  ← JavaScript runs here, one thing at a time
│                         │
│  Currently running:     │
│  • sendAjaxRequest()    │
│  • updateProgressBar()  │
└─────────────────────────┘

┌─────────────────────────┐
│  WEB APIs               │  ← Browser's background workers (NOT JavaScript)
│  (handled by browser,   │
│   not JS engine)        │
│  • setTimeout           │
│  • fetch / XMLHttpRequest│  ← AJAX request runs here while JS keeps going
│  • DOM events           │
└─────────────────────────┘

┌─────────────────────────┐
│  CALLBACK QUEUE         │  ← Completed async results wait here
│                         │
│  • Response from server │  ← "I'm done, here's the data, call my callback"
└─────────────────────────┘

THE EVENT LOOP watches the Call Stack.
When the Call Stack is EMPTY, it picks the next item from
the Callback Queue and puts it on the Stack to run.
```

### Step-by-Step What Happens in an AJAX Call

```
1. JS runs: $.ajax({ url: '/Admin/GetUsers', success: callback })
            ↓
2. jQuery hands the HTTP request to the BROWSER's networking layer
   (leaves JavaScript entirely)
            ↓
3. JS immediately continues running next lines of code
   Call Stack: continues with other tasks
            ↓
4. Browser sends the HTTP request to the server (in the background)
            ↓
5. Server processes the request... (50ms, 500ms, 2000ms — whatever it takes)
            ↓
6. Response arrives at the browser
   Browser puts your callback(responseData) into the Callback Queue
            ↓
7. Event Loop sees Call Stack is empty
   Moves the callback from Queue → Call Stack
            ↓
8. Your success: function(data) { ... } runs with the response data
```

**The key insight:**
> Steps 3-7 happen while your JavaScript is doing other things.
> The page never freezes. The network request runs in the browser,
> not in JavaScript. JavaScript only handles the result when it's ready.

---

## 4. The XMLHttpRequest — Where It All Started

Before `fetch()` and jQuery, the original way to make AJAX requests
was the `XMLHttpRequest` object. You won't write this directly, but
knowing it helps you understand what jQuery and fetch do underneath.

```javascript
// The original way — still works, still supported
var xhr = new XMLHttpRequest();

// 1. Set up the request (doesn't send yet)
xhr.open('GET', '/Admin/GetUsers', true);
//              ↑ URL             ↑ true = asynchronous

// 2. Define what to do when response arrives
xhr.onreadystatechange = function() {
    //  readyState: 0=unsent, 1=opened, 2=headers received, 3=loading, 4=done
    if (xhr.readyState === 4) {
        if (xhr.status === 200) {
            // Success — parse the JSON text
            var data = JSON.parse(xhr.responseText);
            console.log(data);
        } else {
            console.error('Request failed: ' + xhr.status);
        }
    }
};

// 3. Send the request
xhr.send();

// This line runs IMMEDIATELY — doesn't wait for the request above
console.log('Request sent, continuing...');
```

This is verbose and complex. That's why jQuery's `$.ajax()` and
the modern `fetch()` API were created — to wrap all of this in
something simple and readable.

```
XMLHttpRequest (raw)    →   $.ajax() (jQuery wrapper)   →   fetch() (modern)
   Very verbose                 Simpler, options-based          Promise-based
   Hard to read                 Great for Kendo (jQuery)        Modern JS style
   Still works                  YOUR DAILY TOOL                 Good to know
```

---

## 5. The Callback Pattern — How Results Come Back

In AJAX, you don't `return` the result. You **pass a function** that handles it.
This function is called a **callback**.

```javascript
// ❌ This does NOT work — you can't return async results
function getUsers() {
    var result;
    $.ajax({
        url: '/Admin/GetUsers',
        success: function(data) {
            result = data;    // sets result... but too late!
        }
    });
    return result;    // returns undefined — the request hasn't finished yet!
}

// ✅ This is correct — pass a callback to handle the result
function getUsers(callback) {
    $.ajax({
        url: '/Admin/GetUsers',
        success: function(data) {
            callback(data);   // call the callback WITH the data when it arrives
        }
    });
}

// Usage:
getUsers(function(data) {
    console.log(data);          // runs when data arrives
    populateGrid(data);
});
```

### The Callback Sequence — Visualised

```
Your code runs top to bottom:

Line 1:  getUsers(function(data) { console.log(data); })
          ↓
          jQuery sends HTTP request... (browser takes over)
          ↓
Line 2:  console.log('continuing...')    ← runs immediately, not waiting
Line 3:  updateSomethingOnPage()         ← runs immediately
Line 4:  ...                             ← runs immediately

                    ... 200ms later ...

Server responds.
Browser puts the callback in the Queue.
Event Loop picks it up.
→ function(data) { console.log(data); }  RUNS NOW with actual data
```

---

## 6. Three Outcomes of Every AJAX Request

Every AJAX call has exactly three possible outcomes:

```
OUTCOME 1 — SUCCESS
───────────────────
Server received the request.
Server processed it successfully.
Server returned a 2xx status (200, 201, 204).
→ Your success callback runs.

OUTCOME 2 — ERROR
──────────────────
Network failed (no internet, server down).
Server returned an error status (4xx, 5xx).
→ Your error callback runs.

OUTCOME 3 — COMPLETE
─────────────────────
Runs regardless of success or error.
After either success or error callback.
Used for cleanup (hide loading spinner, re-enable button).
→ Your complete callback runs.
```

```javascript
$.ajax({
    url:  '/Admin/GetUsers',
    type: 'GET',

    success: function(data) {
        console.log('Got data:', data);
        renderGrid(data);
    },

    error: function(xhr, status, error) {
        console.error('Request failed.');
        console.log('Status code:', xhr.status);    // 401, 403, 500...
        console.log('Error text:',  xhr.responseText);
    },

    complete: function() {
        // Runs ALWAYS — after success or error
        $('#loadingSpinner').hide();    // hide spinner either way
        $('#loadBtn').prop('disabled', false);  // re-enable button
    }
});
```

---

## 7. Request Types — GET vs POST in AJAX

```
GET — for READING data
────────────────────────────────────────────────────────────
• Data goes in the URL: /Admin/GetUser?userId=sumit
• No request body
• Can be bookmarked, cached
• NEVER use GET for sensitive data (passwords, tokens)
  They appear in the URL, server logs, browser history
• Use for: loading grid data, getting a single record


POST — for WRITING / CHANGING data
────────────────────────────────────────────────────────────
• Data goes in the request body — not visible in URL
• Cannot be bookmarked or cached (each call is unique)
• Required for: login, unlock user, create, update, delete
• Required for: anything with a side effect on the server
```

```javascript
// GET — reading data
$.ajax({
    url:  '/Admin/GetUsers',
    type: 'GET',
    success: function(data) { renderGrid(data); }
});

// Shorthand for simple GETs:
$.get('/Admin/GetUsers', function(data) { renderGrid(data); });


// POST — changing something
$.ajax({
    url:  '/Admin/UnlockUser',
    type: 'POST',
    data: { userId: 'sumit', __RequestVerificationToken: token },
    success: function(data) { ... }
});

// Shorthand for simple POSTs (without anti-forgery token — not for MVC)
$.post('/Admin/SomeAction', { key: 'value' }, function(data) { ... });
```

---

## 8. Reading the Response — Status Codes in AJAX

When an AJAX call returns, you have access to the HTTP status code.
These tell you what happened on the server.

```javascript
$.ajax({
    url:  '/Admin/GetUsers',
    type: 'GET',

    success: function(data, textStatus, xhr) {
        // textStatus: "success", "notmodified", "nocontent"
        // xhr.status: 200, 201, 204
        console.log('HTTP Status:', xhr.status);
    },

    error: function(xhr, textStatus, errorThrown) {
        // This is where you handle errors
        switch(xhr.status) {
            case 400:
                alert('Bad request — check your input.');
                break;
            case 401:
                // Not logged in — redirect
                window.location.href = '/Account/Login';
                break;
            case 403:
                alert('You do not have permission for this action.');
                break;
            case 404:
                alert('The requested resource was not found.');
                break;
            case 500:
                alert('Server error. Please try again later.');
                break;
            default:
                alert('Something went wrong. Status: ' + xhr.status);
        }
    }
});
```

---

## 9. AJAX Request Structure — Full Picture

```
Every AJAX request has these parts:

┌────────────────────────────────────────────────────────────┐
│  URL          Where to send: '/Admin/GetUsers'              │
│  Method       What to do:   GET, POST, PUT, DELETE          │
│  Headers      Metadata:     Content-Type, Authorization     │
│  Body         Data sent:    { userId: 'sumit' } (POST only) │
│  Callbacks    What happens: success(), error(), complete()  │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│  Every AJAX response has:                                  │
│                                                            │
│  Status Code  What happened:    200, 401, 403, 500         │
│  Headers      Response info:    Content-Type: application/json
│  Body         Response data:    {"success":true,"message":"..."}
└────────────────────────────────────────────────────────────┘
```

---

## 10. The Three Generations of AJAX

It's important to know the history so you understand why there are multiple ways.

```
GENERATION 1 — XMLHttpRequest (XHR)  ~2000-2010
───────────────────────────────────────────────────────────
Raw, verbose, callback-heavy. Still works. You'll see it in old code.
No one writes it from scratch anymore.

var xhr = new XMLHttpRequest();
xhr.open('GET', '/api/data');
xhr.onreadystatechange = function() { if(xhr.readyState===4){...} };
xhr.send();


GENERATION 2 — jQuery $.ajax()  ~2006-present
───────────────────────────────────────────────────────────
jQuery wraps XHR in a clean API. Options object. Named callbacks.
Great browser compatibility. Still the standard in Kendo-based projects.
THIS IS YOUR DAILY TOOL.

$.ajax({ url: '/api/data', type: 'GET', success: function(d){...} });


GENERATION 3 — fetch() + Promises  ~2015-present
───────────────────────────────────────────────────────────
Built into modern browsers. No jQuery needed. Uses Promises (cleaner
than callbacks). Standard in React/Vue/Angular projects.
Important to know. Chapter 2 covers this.

fetch('/api/data')
    .then(r => r.json())
    .then(data => console.log(data));
```

---

## Chapter Summary

| Concept | One-Line Meaning |
|---|---|
| **AJAX** | HTTP request from JavaScript in the background — page doesn't reload |
| **Asynchronous** | Send a request, keep running, handle result via callback when it arrives |
| **Single-threaded** | JavaScript does one thing at a time — async works via Event Loop |
| **Event Loop** | Watches the call stack; when empty, runs the next callback from the queue |
| **XMLHttpRequest** | The original AJAX object — jQuery and fetch wrap this |
| **Callback** | A function you pass in to handle the async result when it's ready |
| **success** | Callback that runs when server returns 2xx |
| **error** | Callback that runs when server returns 4xx/5xx or network fails |
| **complete** | Callback that ALWAYS runs after success or error (cleanup) |
| **GET** | Read data — goes in URL, no body |
| **POST** | Change data — goes in body, not in URL |
| **Never return async results** | You can't return from async — use callbacks to handle results |

---

## What's Next — Chapter 2

**fetch() — The Modern AJAX API**

Chapter 2 covers `fetch()` — the modern, built-in browser API for AJAX.
It uses Promises instead of callbacks, which makes code cleaner and
easier to read. Understanding `fetch()` also teaches you Promises, which
you need to understand `async/await` — used everywhere in modern JavaScript.

---

*AJAX Chapter 1 of 5 — Sumit's ASP.NET Core MVC Learning Journey*
