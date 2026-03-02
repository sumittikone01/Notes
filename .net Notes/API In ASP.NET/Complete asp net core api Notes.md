# 📘 The Complete ASP.NET Core Web API Guide

---

## 📚 Table of Contents

| Chapter | Topic                                                    |
| ------- | -------------------------------------------------------- |
| 1       | What is a Web API? — The Complete Foundation            |
| 2       | Setting Up Your Project                                  |
| 3       | ApiController — Anatomy & Everything It Does            |
| 4       | HTTP Verbs — GET, POST, PUT, DELETE Explained           |
| 5       | Routing — How URLs Map to Your Code                     |
| 6       | JSON — How Data Travels Between Server and Client       |
| 7       | Model Binding — How Data Gets Into Your Methods         |
| 8       | Validation — Checking Data Before Using It              |
| 9       | ADO.NET in API Controllers — Database Work              |
| 10      | AJAX — Calling Your API from the Browser                |
| 11      | Kendo UI Grid ↔ API — Full CRUD Integration            |
| 12      | Kendo Dropdowns, AutoComplete & Charts                   |
| 13      | Error Handling — Catching and Returning Errors Properly |
| 14      | Filters — Running Code Before/After Every Action        |
| 15      | CORS — Allowing Cross-Origin Requests                   |
| 16      | Authentication & Authorization with JWT                  |
| 17      | Dependency Injection — Clean, Testable Code             |
| 18      | API Versioning                                           |
| 19      | Swagger — Auto-Generated API Documentation              |
| 20      | Best Practices, Patterns & Security                      |
| 21      | Complete Cheat Sheet                                     |

---

---

# ═══════════════════════════════════════════

# CHAPTER 1 — What is a Web API?

# The Complete Foundation

# ═══════════════════════════════════════════

---

## 1.1 What Does "API" Mean?

**API** stands for **Application Programming Interface**.

That sounds complicated. Let's break it word by word:

- **Interface** = A point where two things connect and communicate.
  Think of a power socket in your wall. You plug in any device and it works. You don't need to know how electricity is generated — the socket is the *interface* between the power grid and your device.
- **Application Programming Interface** = A point where two *software applications* connect and communicate.
  One application says "give me some data" → the API says "here is your data."

**Simple definition:**

> An API is a set of rules and endpoints (URLs) that allow one program to talk to another program and exchange data.

---

## 1.2 What is a "Web API" Specifically?

A **Web API** is an API that works over the internet using **HTTP** — the same protocol your browser uses to load websites.

When you type `https://google.com` in your browser, your browser sends an HTTP request to Google's server, and the server sends back an HTTP response (an HTML page).

A Web API works **exactly the same way** — but instead of sending back an HTML page, it sends back **raw data** (usually JSON format) that another program can use.

**Real-world example:**
Think about a weather app on your phone. The app does not have a weather database inside it. Instead, it calls a Weather API:

```
Phone App  →  HTTP Request  →  Weather API Server
           ←  JSON Response ←  (temperature, humidity, forecast...)
```

The app then displays that data beautifully. The data came from the API.

---

## 1.3 Why Do We Need a Web API in ASP.NET MVC?

You already know ASP.NET Core MVC. MVC works like this:

```
Browser → Request → Controller → Returns a VIEW (HTML page) → Browser renders it
```

This works perfectly for traditional websites. But the modern web needs more:

| Situation                                              | Problem with MVC only                  | Solution: Web API                      |
| ------------------------------------------------------ | -------------------------------------- | -------------------------------------- |
| Kendo Grid needs fresh data without reloading the page | MVC returns full HTML page — too slow | API returns only JSON data             |
| You want a mobile app to use your backend              | Mobile apps can't render Razor views   | API returns JSON — any app can use it |
| You want AJAX updates (no full page reload)            | MVC returns HTML pages                 | API returns just the data you need     |
| Another company wants to connect to your system        | They can't use your HTML views         | They call your API endpoint            |
| You have React/Angular frontend                        | They don't use Razor views             | They call your API                     |

**The key insight:**

> MVC returns HTML pages for humans to read in a browser.
> Web API returns JSON data for other programs to process.

---

## 1.4 MVC vs Web API — Side by Side

```
┌──────────────────────────────────────────────────────────────────────┐
│                      ASP.NET Core Application                        │
│                                                                      │
│  ┌────────────────────────┐    ┌─────────────────────────────────┐   │
│  │     MVC Controller     │    │        API Controller           │   │
│  │  (Returns HTML Views)  │    │     (Returns JSON Data)         │   │
│  │                        │    │                                 │   │
│  │  User types URL in     │    │  JavaScript/AJAX/Kendo calls    │   │
│  │  browser:              │    │  a URL:                         │   │
│  │                        │    │                                 │   │
│  │  GET /Products         │    │  GET /api/products              │   │
│  │       ↓                │    │       ↓                         │   │
│  │  Returns HTML page     │    │  Returns JSON:                  │   │
│  │  <html>                │    │  [{"id":1,"name":"Pen"},        │   │
│  │    <table>...          │    │   {"id":2,"name":"Book"}]       │   │
│  │  </html>               │    │                                 │   │
│  │                        │    │  Client program processes it    │   │
│  │  Browser renders it    │    │  and does what it wants with it │   │
│  └────────────────────────┘    └─────────────────────────────────┘   │
│                                                                      │
│  Both can exist in the SAME project at the same time!               │
└──────────────────────────────────────────────────────────────────────┘
```

**In your case (MVC + Kendo + AJAX):**

- Your MVC Controller returns the **initial page** (the HTML with the Kendo Grid on it)
- Your API Controller returns the **data** that the Kendo Grid loads into itself via AJAX

---

## 1.5 How an API Request Works — Step by Step

Let's trace exactly what happens when your Kendo Grid loads data:

```
Step 1: Browser loads the MVC page
        GET /Products/Index
        → Server returns HTML with Kendo Grid (empty)

Step 2: Kendo Grid's JavaScript automatically fires:
        GET /api/products
        (this is an HTTP request, just like a browser request)

Step 3: The request travels to your ASP.NET Core server

Step 4: ASP.NET Core Routing looks at the URL "/api/products"
        → finds the matching ApiController and Action method

Step 5: Your Action method runs
        → Connects to SQL Server via ADO.NET
        → Reads data from Products table
        → Puts data into a C# List<Product>

Step 6: ASP.NET Core automatically converts List<Product>
        to JSON format:
        [{"id":1,"name":"Pen","price":5.0},{"id":2,...}]

Step 7: JSON is sent back as HTTP Response

Step 8: Kendo Grid's JavaScript receives the JSON
        → Parses it
        → Displays rows in the grid

Total time: Usually 50-500 milliseconds
No full page reload happened!
```

---

## 1.6 What is JSON? (Brief Introduction, Full Chapter Later)

**JSON** stands for **JavaScript Object Notation**.

It is a text format for representing data. It's the universal language that Web APIs use to communicate.

Think of JSON as a universal shipping box format. No matter what data you have — products, orders, users — you pack it in JSON format, and any program in any language can unpack it.

```
C# Object:              JSON Text:
──────────             ──────────────────────────
product.Id = 1         {
product.Name = "Pen"     "id": 1,
product.Price = 5.0      "name": "Pen",
product.Active = true    "price": 5.0,
                         "active": true
                       }

List of products → JSON Array:
[
  {"id":1,"name":"Pen","price":5.0},
  {"id":2,"name":"Book","price":12.0},
  {"id":3,"name":"Ruler","price":3.0}
]
```

ASP.NET Core **automatically converts** your C# objects to JSON when you call `return Ok(myObject)`. You don't have to write the JSON yourself.

---

## 1.7 REST — The Standard Your API Should Follow

**REST** stands for **Representational State Transfer**. It is a set of rules/conventions for how Web APIs should be designed.

You don't need to memorize the theory. Just understand the practical rules:

| REST Rule                         | What It Means                                    | Example                                            |
| --------------------------------- | ------------------------------------------------ | -------------------------------------------------- |
| Use nouns in URLs, not verbs      | URLs represent*things*, not *actions*        | `/api/products` ✅ NOT `/api/getProducts` ❌   |
| Use HTTP verbs to describe action | GET=read, POST=create, PUT=update, DELETE=delete | `DELETE /api/products/5` = delete product 5      |
| Use plural nouns                  | Consistency                                      | `/api/products` not `/api/product`             |
| Nest related resources            | Show relationships in URL                        | `/api/customers/3/orders` = orders of customer 3 |
| Return appropriate status codes   | Tell client what happened                        | 200=OK, 404=Not Found, 500=Server Error            |

An API that follows these rules is called a **RESTful API**. This is what you will build.

---

---

# ═══════════════════════════════════════════

# CHAPTER 2 — Setting Up Your Project

# ═══════════════════════════════════════════

---

## 2.1 What is Program.cs and Why Does It Exist?

**Definition:**

> `Program.cs` is the entry point of your ASP.NET Core application. It is the very first file that runs when your application starts. It does two main jobs: (1) Register all the services your app needs, and (2) Configure the middleware pipeline (the chain of steps every HTTP request goes through).

Think of `Program.cs` as the **blueprint and startup checklist** for your entire application.

**Real-world analogy:**
Imagine opening a restaurant. Before you open the doors, you must:

1. *Register/set up* your kitchen equipment, staff, POS system (these are **services**)
2. Set the *order* in which customers are handled — check reservation → seat → take order → serve → bill (this is the **middleware pipeline**)

`Program.cs` does exactly this for your web application.

---

## 2.2 What is a "Service" in ASP.NET Core?

**Definition:**

> A Service is any class or functionality that your application needs to work — like database connections, email senders, business logic repositories, authentication handlers, etc. You *register* services at startup so ASP.NET Core knows about them and can provide them to controllers that need them.

Examples of services you register:

- `AddControllersWithViews()` — tells ASP.NET "I have MVC controllers and Razor views"
- `AddControllers()` — tells ASP.NET "I have API controllers (no views)"
- Your own classes like `IProductRepository` → `ProductRepository`

---

## 2.3 What is Middleware?

**Definition:**

> Middleware is software that sits between the web server receiving an HTTP request and your controller handling it. Each piece of middleware can inspect the request, do something with it, pass it to the next middleware, and then do something with the response on the way back.

**Analogy:**
Think of middleware as a series of **security checkpoints** at an airport:

```
Passenger (HTTP Request) enters airport
         │
    ┌────▼─────────────────┐
    │ Checkpoint 1:         │  UseHttpsRedirection
    │ Is it HTTPS?          │  → Redirect to HTTPS if not
    └────┬─────────────────┘
         │
    ┌────▼─────────────────┐
    │ Checkpoint 2:         │  UseStaticFiles
    │ Is it a static file? │  → Return CSS/JS/images directly
    └────┬─────────────────┘
         │
    ┌────▼─────────────────┐
    │ Checkpoint 3:         │  UseRouting
    │ Which controller?     │  → Match URL to controller/action
    └────┬─────────────────┘
         │
    ┌────▼─────────────────┐
    │ Checkpoint 4:         │  UseAuthentication
    │ Who is this person?   │  → Read JWT token, set User identity
    └────┬─────────────────┘
         │
    ┌────▼─────────────────┐
    │ Checkpoint 5:         │  UseAuthorization
    │ Are they allowed?     │  → Check if user has permission
    └────┬─────────────────┘
         │
    ┌────▼─────────────────┐
    │ YOUR CONTROLLER       │  The actual action method runs here
    │ Action Method Runs    │
    └────┬─────────────────┘
         │
    Response travels BACK through all middleware
    (each can modify the response on the way back)
         │
         ▼
    Response sent to client
```

**Order matters!** Authentication must come before Authorization. Routing must come before endpoints. If you put them in the wrong order, things break.

---

## 2.4 The Complete Program.cs — Every Line Explained

```csharp
// Program.cs

// ─────────────────────────────────────────────────────────────
// PART 1: Create a Builder
// WebApplication.CreateBuilder sets up the basic infrastructure:
// - Reads appsettings.json
// - Sets up logging
// - Sets up configuration system
// - Prepares dependency injection container
// ─────────────────────────────────────────────────────────────
var builder = WebApplication.CreateBuilder(args);


// ─────────────────────────────────────────────────────────────
// PART 2: Register Services
// This is where you tell ASP.NET Core what your app needs.
// Think of this as filling your toolbox before starting work.
// ─────────────────────────────────────────────────────────────

// AddControllersWithViews() = Support for BOTH:
//   - MVC Controllers (that return Razor Views/HTML)
//   - API Controllers (that return JSON)
// This is the best choice for a hybrid MVC + API app like yours.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // By default, C# property names are PascalCase: ProductName
        // JSON convention is camelCase: productName
        // This setting converts automatically: ProductName → productName
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        // If a property is null, don't include it in JSON at all.
        // Saves bandwidth and keeps JSON clean.
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


// ─────────────────────────────────────────────────────────────
// PART 3: Build the App
// After registering all services, we "build" the application.
// This finalizes the service container.
// ─────────────────────────────────────────────────────────────
var app = builder.Build();


// ─────────────────────────────────────────────────────────────
// PART 4: Configure Middleware Pipeline
// Order is critical here. Think of it as the order of checkpoints.
// ─────────────────────────────────────────────────────────────

// Only show detailed error pages in Development.
// In Production, show a friendly error page (not stack traces).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Tells browsers to always use HTTPS
}

// Redirect all HTTP requests to HTTPS automatically
app.UseHttpsRedirection();

// Allow serving static files from wwwroot folder:
// CSS, JavaScript, images, fonts, Kendo UI files, etc.
app.UseStaticFiles();

// Enable the routing system — figure out which controller handles this URL
app.UseRouting();

// Enable authentication — read the JWT token and identify the user
// MUST come before UseAuthorization
app.UseAuthentication();

// Enable authorization — check if identified user has permission
app.UseAuthorization();


// ─────────────────────────────────────────────────────────────
// PART 5: Map Routes
// Tell ASP.NET Core how to map URLs to Controllers/Actions
// ─────────────────────────────────────────────────────────────

// This maps conventional MVC routes:
// URL pattern: /ControllerName/ActionName/id
// Default: /Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// This maps API controllers that use [Route] attributes on them.
// API controllers define their own routes via [Route("api/...")] attribute.
app.MapControllers();


// Start the web server and begin listening for requests
app.Run();
```

---

## 2.5 appsettings.json — Configuration File

**Definition:**

> `appsettings.json` is a configuration file where you store application settings — database connection strings, API keys, feature toggles, email server settings, etc. It is read at startup by ASP.NET Core and made available throughout your application.

**Why not just hardcode these values in code?**
Because:

1. You have different values for Development vs Production (different databases)
2. Sensitive values (passwords, keys) should not be in source code
3. Changing config should not require recompiling the app

```json
{
  "ConnectionStrings": {
    // This is the connection string to your SQL Server database.
    // Integrated Security=True means use Windows authentication (no password needed)
    // TrustServerCertificate=True is needed for local development with self-signed certs
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Integrated Security=True;TrustServerCertificate=True"
  },

  "Jwt": {
    // Secret key used to sign JWT tokens. Keep this private. Minimum 32 characters.
    "Key": "YourSuperSecretKeyMustBe32CharactersOrLonger!",
    // Who issues the token (your app)
    "Issuer": "https://myapp.com",
    // Who the token is for (your app's clients)
    "Audience": "https://myapp.com"
  },

  "Logging": {
    "LogLevel": {
      // Controls how much logging output you see
      // Trace → Debug → Information → Warning → Error → Critical
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning" // Less noise from framework itself
    }
  },

  // true = Development mode (detailed errors, more logging)
  // false = Production mode
  "AllowedHosts": "*"
}
```

**Reading values from appsettings.json in code:**

```csharp
// In a Controller or class, inject IConfiguration:
public class ProductsController : ControllerBase
{
    private readonly string _connectionString;

    public ProductsController(IConfiguration configuration)
    {
        // GetConnectionString("DefaultConnection") reads from ConnectionStrings section
        _connectionString = configuration.GetConnectionString("DefaultConnection");

        // Reading other values:
        var jwtKey = configuration["Jwt:Key"];           // Reads Jwt → Key
        var issuer = configuration["Jwt:Issuer"];         // Reads Jwt → Issuer
        var logLevel = configuration["Logging:LogLevel:Default"]; // Nested path
    }
}
```

---

## 2.6 Project Folder Structure for Hybrid MVC + API App

```
MyApplication/
│
├── Controllers/                        ← All controllers go here
│   ├── HomeController.cs               ← MVC: returns Views (HTML)
│   ├── ProductsController.cs           ← MVC: returns Views (HTML)
│   └── API/                            ← Sub-folder for API controllers (good practice)
│       ├── ProductsApiController.cs    ← API: returns JSON
│       ├── CategoriesApiController.cs  ← API: returns JSON
│       └── AuthController.cs           ← API: login/logout
│
├── Models/                             ← C# classes that represent your data
│   ├── Product.cs                      ← Product entity
│   ├── Category.cs                     ← Category entity
│   └── DTOs/                           ← Data Transfer Objects (safe input/output shapes)
│       ├── CreateProductRequest.cs
│       └── ProductDto.cs
│
├── Repositories/                       ← ADO.NET database access code
│   ├── IProductRepository.cs           ← Interface (the contract)
│   └── ProductRepository.cs           ← Implementation (the actual ADO.NET code)
│
├── Views/                              ← Razor views (HTML templates)
│   ├── Home/
│   │   └── Index.cshtml
│   └── Products/
│       └── Index.cshtml               ← This view contains the Kendo Grid
│
├── wwwroot/                            ← Static files (served directly to browser)
│   ├── css/
│   ├── js/
│   │   └── site.js
│   └── lib/
│       └── kendo/                     ← Kendo UI files go here
│
├── Program.cs                          ← App startup, service registration, middleware
└── appsettings.json                    ← Configuration values
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 3 — ApiController

# Anatomy & Everything It Does

# ═══════════════════════════════════════════

---

## 3.1 What is a Controller?

**Definition:**

> A Controller is a C# class that receives HTTP requests and returns HTTP responses. It is the traffic director of your application — it decides what data to fetch, what logic to run, and what to send back to the client.

Every URL your API exposes corresponds to a specific **method** (called an *Action*) inside a Controller class.

**Analogy:**
A Controller is like a **restaurant waiter**. The waiter (controller) receives your order (HTTP request), goes to the kitchen (database/business logic), and brings back your food (HTTP response/JSON).

---

## 3.2 What is `ControllerBase` vs `Controller`?

ASP.NET Core gives you two base classes to inherit from:

**`ControllerBase`** — The base class for API controllers.

- Contains all the helper methods for returning HTTP responses: `Ok()`, `NotFound()`, `BadRequest()`, `Created()`, etc.
- Does NOT have anything related to Views, ViewBag, ViewData, or Razor.
- Use this for all API controllers that return JSON.

**`Controller`** — The base class for MVC controllers (inherits from `ControllerBase`).

- Has everything `ControllerBase` has.
- ALSO has: `View()`, `PartialView()`, `ViewBag`, `ViewData`, `TempData`, `RedirectToAction()`, etc.
- Use this for MVC controllers that return HTML Views.

```csharp
// ✅ For API (returns JSON):
public class ProductsApiController : ControllerBase
{
    // Has: Ok(), NotFound(), BadRequest(), Created(), etc.
    // Does NOT have: View(), ViewBag, etc.
}

// ✅ For MVC (returns HTML Views):
public class ProductsController : Controller
{
    // Has everything ControllerBase has
    // PLUS: View(), PartialView(), ViewBag, ViewData, etc.
}
```

**Rule of thumb:**

> If your controller returns JSON → inherit `ControllerBase`
> If your controller returns Views (HTML) → inherit `Controller`

---

## 3.3 The `[ApiController]` Attribute — What It Does

**Definition:**

> `[ApiController]` is an attribute you put on your API controller class. It activates several automatic behaviors that make API development easier and more consistent.

Without it, you have to do a lot of manual work. With it, ASP.NET Core handles the common tasks automatically.

Here is exactly what `[ApiController]` does for you:

### Feature 1: Automatic Model Validation

**Without `[ApiController]`:**

```csharp
[HttpPost]
public IActionResult Create([FromBody] Product product)
{
    // You MUST manually check if the incoming data is valid
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);  // You have to write this every time
    }
    // ... rest of code
}
```

**With `[ApiController]`:**

```csharp
[HttpPost]
public IActionResult Create([FromBody] Product product)
{
    // If data is invalid, ASP.NET Core AUTOMATICALLY returns 400 Bad Request
    // You never even reach this line if the model is invalid
    // No manual check needed!
    // ... rest of code
}
```

### Feature 2: Automatic `[FromBody]` Inference for POST/PUT

**Without `[ApiController]`:**

```csharp
// You MUST write [FromBody] explicitly or ASP.NET won't read the JSON body
[HttpPost]
public IActionResult Create([FromBody] Product product) { }
```

**With `[ApiController]`:**

```csharp
// [ApiController] knows: complex type + POST/PUT = comes from body
// [FromBody] is inferred automatically
[HttpPost]
public IActionResult Create(Product product) { }  // Still works!
```

### Feature 3: Structured Error Responses (ProblemDetails)

**Without `[ApiController]`:**
Validation errors might return raw, inconsistent formats.

**With `[ApiController]`:**
Validation errors return a standardized JSON format called **ProblemDetails** (RFC 7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Name is required"],
    "Price": ["Price must be between 0.01 and 99999.99"]
  }
}
```

This is a consistent, predictable format that your AJAX error handlers can always rely on.

### Feature 4: Attribute Routing Required

`[ApiController]` requires you to use `[Route]` attribute. You cannot use conventional routing (`{controller}/{action}`) with it. This is actually a good thing — it makes your API routes explicit and predictable.

---

## 3.4 The `[Route]` Attribute

**Definition:**

> The `[Route]` attribute on a controller class defines the **base URL prefix** for all actions in that controller.

```csharp
[ApiController]
[Route("api/[controller]")]   // ← [controller] is a placeholder
```

`[controller]` is automatically replaced with the controller's name (minus the word "Controller"):

| Controller Class Name    | `[controller]` becomes | Full Route         |
| ------------------------ | ------------------------ | ------------------ |
| `ProductsController`   | `products`             | `api/products`   |
| `CategoriesController` | `categories`           | `api/categories` |
| `OrdersController`     | `orders`               | `api/orders`     |
| `CustomersController`  | `customers`            | `api/customers`  |

**Best practice:** Name your API controllers without "Api" in the name, so the route stays clean:

```csharp
// Good: Results in route "api/products"
[Route("api/[controller]")]
public class ProductsController : ControllerBase { }

// Avoid: Results in route "api/productsapi" — ugly
[Route("api/[controller]")]
public class ProductsApiController : ControllerBase { }

// OR: Hardcode the route (most explicit — no surprises)
[Route("api/products")]
public class ProductsApiController : ControllerBase { }
```

---

## 3.5 Complete ApiController — Full Anatomy

```csharp
using Microsoft.AspNetCore.Mvc;  // Namespace for ApiController, Route, HttpGet, etc.
using System.Data;
using System.Data.SqlClient;

namespace MyApp.Controllers.API
{
    //─────────────────────────────────────────────────────────────────
    // [ApiController] — Activates all the automatic API behaviors
    //                   described in section 3.3
    // [Route("api/[controller]")] — Base URL for all actions in this class
    //                   [controller] = "Products" (from class name)
    //                   So every URL here starts with: /api/products
    //─────────────────────────────────────────────────────────────────
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    //                                ↑ ControllerBase = base class for API controllers
    //                                  (not Controller, which is for MVC/Views)
    {
        //─────────────────────────────────────────────────────────────
        // Private field to store the database connection string
        // readonly = can only be set in constructor, never changed after
        //─────────────────────────────────────────────────────────────
        private readonly string _connectionString;

        //─────────────────────────────────────────────────────────────
        // Constructor — runs when this controller is created
        //
        // IConfiguration is automatically injected by ASP.NET Core.
        // It gives access to appsettings.json values.
        //
        // Dependency Injection (DI) provides IConfiguration automatically.
        // You don't create it manually with "new". DI handles it.
        // (Full DI chapter coming up later)
        //─────────────────────────────────────────────────────────────
        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //─────────────────────────────────────────────────────────────
        // ACTION METHOD — A public method that handles a specific HTTP request
        //
        // [HttpGet] tells ASP.NET: this handles GET requests
        // Full URL: GET /api/products
        //
        // IActionResult = return type that can be any HTTP response
        //   (200 OK, 404 Not Found, 400 Bad Request, etc.)
        //─────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult GetAll()
        {
            // ... ADO.NET code to get data from DB
            // return Ok(data);  ← returns 200 OK with JSON body
        }

        // More actions below...
    }
}
```

---

## 3.6 What Return Types Can an Action Return?

### Option 1: `IActionResult` — Most Common and Flexible

**Definition:**

> `IActionResult` is an interface that represents any possible HTTP response. It allows your action to return different types of responses (200, 404, 400, etc.) from the same method.

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var product = GetFromDatabase(id);

    if (product == null)
        return NotFound();    // ← This is an IActionResult (404)

    return Ok(product);       // ← This is also an IActionResult (200 + JSON)
    // Both work because both implement IActionResult
}
```

### Option 2: `ActionResult<T>` — Typed + Flexible

**Definition:**

> `ActionResult<T>` is like `IActionResult` but it also tells Swagger/tooling what type of data you return on success. `T` is your data type.

```csharp
[HttpGet("{id}")]
public ActionResult<Product> GetById(int id)
{
    var product = GetFromDatabase(id);

    if (product == null)
        return NotFound();   // ← Standard error response

    return product;          // ← Implicitly becomes Ok(product)
    // ASP.NET Core automatically wraps it in 200 OK
}
```

### Option 3: Direct Type — Simple but Limited

```csharp
[HttpGet]
public List<Product> GetAll()
{
    return GetAllFromDatabase();   // Always returns 200 OK with JSON list
    // Can't return 404 or errors this way — always succeeds
}
```

**Which to use?**

> Use `IActionResult` for most cases. Use `ActionResult<T>` when you want Swagger to know the response type. Use direct type only for very simple read-only endpoints.

---

## 3.7 Helper Methods — Built Into ControllerBase

These methods create HTTP responses. Every one of them returns an `IActionResult`.

```csharp
// ── Success Responses (2xx) ────────────────────────────────────────

// 200 OK — Request succeeded, returning data
return Ok(data);
return Ok(new { id = 1, name = "Pen" });
return Ok(listOfProducts);

// 201 Created — New resource was created
// Use after POST that creates something
// Includes a Location header pointing to the new resource
return Created("/api/products/5", newProduct);
return CreatedAtAction(nameof(GetById), new { id = newId }, newProduct);
// ↑ CreatedAtAction is better — it generates the Location URL automatically
//   nameof(GetById) = the action method that can GET this new resource
//   new { id = newId } = the route values for that action
//   newProduct = the created data to return in body

// 204 No Content — Success, but nothing to return in body
// Common for PUT (update) and DELETE
return NoContent();

// ── Client Error Responses (4xx) ──────────────────────────────────

// 400 Bad Request — Client sent invalid data
return BadRequest();
return BadRequest("Name is required");
return BadRequest(new { message = "Validation failed", field = "Name" });
return BadRequest(ModelState); // Returns all validation errors

// 401 Unauthorized — Client is not authenticated (not logged in)
return Unauthorized();
return Unauthorized(new { message = "Please log in" });

// 403 Forbidden — Client is authenticated but not allowed
// (Logged in, but doesn't have permission)
return Forbid();

// 404 Not Found — The requested resource doesn't exist
return NotFound();
return NotFound(new { message = "Product with ID 5 not found" });

// 409 Conflict — Request conflicts with current state
// (e.g., trying to create a duplicate record)
return Conflict();
return Conflict(new { message = "Email already exists" });

// ── Server Error Responses (5xx) ──────────────────────────────────

// 500 Internal Server Error — Something went wrong on the server
return StatusCode(500, "An internal error occurred");
return StatusCode(500, new { message = "Database error" });

// Custom status code:
return StatusCode(418, "I'm a teapot"); // Any status code you want
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 4 — HTTP Verbs

# GET, POST, PUT, DELETE Explained

# ═══════════════════════════════════════════

---

## 4.1 What are HTTP Verbs?

**Definition:**

> HTTP Verbs (also called HTTP Methods) are words in an HTTP request that tell the server WHAT you want to do. They are part of the HTTP protocol standard.

When your browser loads a page, it uses GET. When you submit a login form, it uses POST. Web APIs use all four main verbs to represent CRUD operations.

**CRUD** = **C**reate, **R**ead, **U**pdate, **D**elete — the four basic operations on data.

| HTTP Verb | CRUD Operation   | When to Use            | Sends Data? | Idempotent? |
| --------- | ---------------- | ---------------------- | ----------- | ----------- |
| GET       | Read             | Fetch data             | No body     | Yes         |
| POST      | Create           | Add new record         | Yes (body)  | No          |
| PUT       | Update (Full)    | Replace entire record  | Yes (body)  | Yes         |
| PATCH     | Update (Partial) | Change specific fields | Yes (body)  | Sometimes   |
| DELETE    | Delete           | Remove a record        | No body     | Yes         |

**What is Idempotent?**

> An operation is idempotent if doing it multiple times gives the same result as doing it once.
>
> - GET: Fetching the same data 10 times = same result ✅
> - DELETE: Deleting the same record 10 times = still deleted ✅
> - POST: Submitting a form 10 times = 10 new records created ❌ Not idempotent

---

## 4.2 How HTTP Verbs Map to Attributes in C#

```csharp
[HttpGet]     → Handles GET requests   → Read data
[HttpPost]    → Handles POST requests  → Create new record
[HttpPut]     → Handles PUT requests   → Full update
[HttpPatch]   → Handles PATCH requests → Partial update
[HttpDelete]  → Handles DELETE requests → Delete record
```

---

## 4.3 The Complete CRUD API — Every Verb With Full Explanation

Let's build a complete Products API. We'll explain every line:

```csharp
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly string _conn;

    public ProductsController(IConfiguration config)
        => _conn = config.GetConnectionString("DefaultConnection");


    // ═══════════════════════════════════════════════════════════
    // GET /api/products
    // Purpose: Get ALL products from the database
    // Called by: Kendo Grid on load, AJAX to populate a table
    // Returns: 200 OK with JSON array of all products
    // ═══════════════════════════════════════════════════════════
    [HttpGet]
    public IActionResult GetAll()
    {
        var products = new List<Product>();

        // Open connection to SQL Server
        using var con = new SqlConnection(_conn);
        // Create the SQL command
        using var cmd = new SqlCommand("SELECT Id, Name, Price, Category FROM Products ORDER BY Name", con);

        con.Open();  // Actually open the connection
        using var reader = cmd.ExecuteReader();  // Execute SELECT, get a reader

        // Loop through each row the database returns
        while (reader.Read())
        {
            // Map database row to C# Product object
            products.Add(new Product
            {
                Id       = (int)reader["Id"],
                Name     = reader["Name"].ToString(),
                Price    = (decimal)reader["Price"],
                Category = reader["Category"].ToString()
            });
        }

        // Return 200 OK. ASP.NET Core automatically converts List<Product>
        // to JSON array and puts it in the response body.
        return Ok(products);
        // Client receives: [{"id":1,"name":"Pen",...},{"id":2,...}]
    }


    // ═══════════════════════════════════════════════════════════
    // GET /api/products/5
    // Purpose: Get ONE product by its ID
    // Called by: AJAX when user clicks "View Details" or when
    //            editing a specific product
    // {id:int} — route parameter constraint: must be an integer
    // Returns: 200 OK with one product, OR 404 if not found
    // ═══════════════════════════════════════════════════════════
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    // ↑ int id is automatically populated from the URL:
    //   GET /api/products/5  →  id = 5
    {
        Product product = null;  // Start as null, fill if found

        using var con = new SqlConnection(_conn);
        // Note: @Id is a SQL parameter — NEVER concatenate user input into SQL!
        // Concatenating = SQL Injection vulnerability
        using var cmd = new SqlCommand(
            "SELECT Id, Name, Price, Category FROM Products WHERE Id = @Id", con);

        // Add the parameter value safely
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        using var reader = cmd.ExecuteReader();

        // If a row was found, Read() returns true; if no row, returns false
        if (reader.Read())
        {
            product = new Product
            {
                Id       = (int)reader["Id"],
                Name     = reader["Name"].ToString(),
                Price    = (decimal)reader["Price"],
                Category = reader["Category"].ToString()
            };
        }

        // If no product found, return 404 Not Found with a message
        if (product == null)
            return NotFound(new { message = $"Product with ID {id} was not found." });

        // Product found → return 200 OK with the product as JSON
        return Ok(product);
    }


    // ═══════════════════════════════════════════════════════════
    // POST /api/products
    // Purpose: Create a NEW product
    // Called by: AJAX form submission, Kendo Grid "Add" action
    // Client sends JSON in request body:
    //   { "name": "Pen", "price": 5.0, "category": "Stationery" }
    // Returns: 201 Created with the new product (including its new ID)
    // ═══════════════════════════════════════════════════════════
    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    // ↑ [FromBody] — tells ASP.NET Core to read the JSON body
    //   and deserialize it into the Product object.
    //   With [ApiController], [FromBody] is inferred, but being explicit is fine.
    {
        // At this point, [ApiController] has already validated the model.
        // If Product has [Required] Name and it wasn't sent, we never reach here.
        // ASP.NET Core automatically returned 400 Bad Request.

        using var con = new SqlConnection(_conn);
        // INSERT returns nothing, but we need the new ID.
        // SCOPE_IDENTITY() returns the ID that was just inserted.
        using var cmd = new SqlCommand(
            @"INSERT INTO Products (Name, Price, Category)
              VALUES (@Name, @Price, @Category);
              SELECT SCOPE_IDENTITY();", con);

        cmd.Parameters.AddWithValue("@Name",     product.Name);
        cmd.Parameters.AddWithValue("@Price",    product.Price);
        cmd.Parameters.AddWithValue("@Category", product.Category);

        con.Open();
        // ExecuteScalar() — use when your SQL returns a single value
        int newId = Convert.ToInt32(cmd.ExecuteScalar());

        // Set the new ID on the product object
        product.Id = newId;

        // Return 201 Created.
        // - First arg: the action that can GET this new resource (for Location header)
        // - Second arg: the route values for that action (to build the URL)
        // - Third arg: the created object to return in body
        // This sets the response header: Location: /api/products/5
        return CreatedAtAction(nameof(GetById), new { id = newId }, product);
    }


    // ═══════════════════════════════════════════════════════════
    // PUT /api/products/5
    // Purpose: FULLY update an existing product
    // "Full update" means: the client sends the COMPLETE object.
    //   Whatever fields are not sent are treated as empty/null.
    // Client sends:
    //   { "id": 5, "name": "Blue Pen", "price": 6.0, "category": "Stationery" }
    // Returns: 204 No Content (success, nothing to return)
    // ═══════════════════════════════════════════════════════════
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Product product)
    // ↑ id comes from the URL: PUT /api/products/5  → id = 5
    // ↑ product comes from JSON body
    {
        // Safety check: the ID in the URL must match the ID in the body
        // Prevents accidentally updating the wrong record
        if (id != product.Id)
            return BadRequest(new { message = "ID in URL does not match ID in body" });

        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            @"UPDATE Products
              SET Name = @Name, Price = @Price, Category = @Category
              WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Id",       id);
        cmd.Parameters.AddWithValue("@Name",     product.Name);
        cmd.Parameters.AddWithValue("@Price",    product.Price);
        cmd.Parameters.AddWithValue("@Category", product.Category);

        con.Open();
        // ExecuteNonQuery() — use for INSERT/UPDATE/DELETE
        // Returns the number of rows affected
        int rowsAffected = cmd.ExecuteNonQuery();

        // If 0 rows were affected, the ID didn't exist
        if (rowsAffected == 0)
            return NotFound(new { message = $"Product {id} not found, nothing was updated" });

        // 204 No Content — success but we return nothing in the body
        // This is the REST convention for successful PUT/DELETE
        return NoContent();
    }


    // ═══════════════════════════════════════════════════════════
    // DELETE /api/products/5
    // Purpose: Delete a product by ID
    // Called by: Kendo Grid delete button, AJAX delete action
    // No request body needed — ID is in the URL
    // Returns: 204 No Content (success), or 404 if not found
    // ═══════════════════════════════════════════════════════════
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand("DELETE FROM Products WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        con.Open();
        int rowsAffected = cmd.ExecuteNonQuery();

        if (rowsAffected == 0)
            return NotFound(new { message = $"Product {id} not found, nothing was deleted" });

        return NoContent();  // 204 — Deleted successfully
    }
}
```

---

## 4.4 HTTP Status Codes — The Complete Guide

**Definition:**

> HTTP Status Codes are three-digit numbers that the server includes in every response to tell the client what happened. The first digit indicates the category.

```
1xx — Informational  (rare, usually handled by framework)
2xx — Success        (your request worked)
3xx — Redirect       (the resource moved, go here instead)
4xx — Client Error   (you sent something wrong)
5xx — Server Error   (something broke on our side)
```

**The ones you MUST know:**

```
200 OK
    → Everything worked, here is your data.
    → Use for: GET requests that return data

201 Created
    → A new resource was created successfully.
    → Use for: POST requests that create records
    → Usually includes Location header and the created object

204 No Content
    → Success, but nothing to return.
    → Use for: PUT (update) and DELETE (delete)
    → Response body is empty

400 Bad Request
    → The client sent invalid/malformed data.
    → Use for: Failed validation, missing required fields,
               mismatched IDs, invalid formats

401 Unauthorized
    → The client is not authenticated (not logged in).
    → Use for: API requires login but no token was provided
    → Client should: go to login page

403 Forbidden
    → The client IS authenticated but doesn't have permission.
    → Use for: Regular user trying to access admin endpoint
    → Different from 401: user is logged in, but not allowed

404 Not Found
    → The requested resource doesn't exist.
    → Use for: GET/PUT/DELETE on an ID that doesn't exist in DB

409 Conflict
    → The request conflicts with current data state.
    → Use for: Trying to create a record with a duplicate unique field
               (e.g., email already exists)

422 Unprocessable Entity
    → Data format is valid but semantically wrong.
    → Use for: Data is valid JSON/structure but business rules fail
               (e.g., end date before start date)

500 Internal Server Error
    → Something crashed on the server side.
    → Use for: Unexpected exceptions, database errors
    → Client cannot fix this; developer must investigate
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 5 — Routing

# How URLs Map to Your Code

# ═══════════════════════════════════════════

---

## 5.1 What is Routing?

**Definition:**

> Routing is the process by which ASP.NET Core looks at the incoming URL of an HTTP request and decides which Controller class and which Action method should handle it.

Think of routing as a **post office sorting system**:

- A letter arrives (HTTP request) with an address (URL: `/api/products/5`)
- The sorting system (router) reads the address
- It finds the correct delivery route (ProductsController, GetById action)
- The letter is delivered to that exact method

Without routing, ASP.NET Core wouldn't know which of your many controller methods should handle which URL.

---

## 5.2 How Routing Works — Step by Step

```
Request arrives: GET /api/products/5

Step 1: UseRouting() middleware activates
        → Scans all registered controllers and their [Route] attributes

Step 2: Finds match:
        [Route("api/products")]   ← matches "api/products"
          [HttpGet("{id}")]       ← matches the "5" part

Step 3: Extracts route values:
        {id} = "5"
        Converts to int (because parameter is int id)

Step 4: Creates the controller instance (ProductsController)
        Calls the matched action method (GetById(5))

Step 5: Returns the action's result
```

---

## 5.3 Types of Routes in API Controllers

### Type 1: No Extra Segment — Matches the Controller Base Route

```csharp
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet]       // Handles: GET /api/products
    [HttpPost]      // Handles: POST /api/products
}
```

### Type 2: Fixed Extra Segment

```csharp
[HttpGet("featured")]     // Handles: GET /api/products/featured
[HttpGet("on-sale")]      // Handles: GET /api/products/on-sale
[HttpGet("top-sellers")]  // Handles: GET /api/products/top-sellers
```

### Type 3: Route Parameter (Variable Segment)

```csharp
// {id} is a placeholder — captures whatever is in that position of the URL
[HttpGet("{id}")]              // GET /api/products/5      → id = "5"
[HttpGet("{id}")]              // GET /api/products/99     → id = "99"
[HttpGet("{id}")]              // GET /api/products/abc    → id = "abc"

[HttpGet("{id:int}")]          // GET /api/products/5      → id = 5 (int)
                               // GET /api/products/abc    → 404 (constraint fails)

[HttpGet("{name:alpha}")]      // GET /api/products/Pen    → name = "Pen"
                               // GET /api/products/123    → 404 (must be letters only)
```

### Type 4: Multiple Route Parameters

```csharp
// GET /api/products/category/Electronics/page/2
[HttpGet("category/{category}/page/{page:int}")]
public IActionResult GetByCategory(string category, int page) { }

// GET /api/orders/5/items/3
[HttpGet("{orderId}/items/{itemId}")]
public IActionResult GetOrderItem(int orderId, int itemId) { }
```

### Type 5: Override Base Route (Leading `/`)

```csharp
[Route("api/products")]   // Base route
public class ProductsController : ControllerBase
{
    // Normal: appends to base route
    [HttpGet("featured")]       // → GET /api/products/featured

    // Override: leading / makes it absolute (ignores base route)
    [HttpGet("/api/v2/products/special")]  // → GET /api/v2/products/special
}
```

---

## 5.4 Route Constraints — Enforcing URL Formats

**Definition:**

> Route constraints are rules you add to route parameters that restrict what values are acceptable. If the URL doesn't match the constraint, the route doesn't match (404 is returned).

**Why use them?**

- Protect against bad data before it even reaches your action
- Make routes self-documenting (`:int` tells you this expects a number)
- Prevent ambiguous routes (two routes that could match the same URL)

```csharp
// ── Type Constraints ──────────────────────────────────────────
{id:int}        // Must be a whole number:   /5, /99, /1000
{id:long}       // Must be a long integer:   /123456789012345
{id:decimal}    // Must be decimal:          /5.5, /10.00
{id:double}     // Must be double
{id:bool}       // Must be true or false:    /true, /false
{id:guid}       // Must be a GUID:           /3fa85f64-5717-4562-b3fc-2c963f66afa6

// ── Value Constraints ─────────────────────────────────────────
{id:min(1)}        // Minimum value of 1 (no zero or negative IDs)
{id:max(1000)}     // Maximum value of 1000
{id:range(1,100)}  // Between 1 and 100 inclusive

// ── String Constraints ────────────────────────────────────────
{name:alpha}            // Letters only (a-z, A-Z)
{name:length(5)}        // Exactly 5 characters
{name:minlength(2)}     // At least 2 characters
{name:maxlength(50)}    // At most 50 characters
{name:regex(^\\d{3}$)}  // Exactly 3 digits (regex pattern)

// ── Required vs Optional ──────────────────────────────────────
{id}            // Required — must be present in URL
{id?}           // Optional — can be omitted
{id:int?}       // Optional integer

// ── Combining Constraints ─────────────────────────────────────
{id:int:min(1)}                 // Integer AND minimum 1
{name:alpha:minlength(2):maxlength(50)}  // Alpha, between 2 and 50 chars
```

**Practical examples:**

```csharp
[HttpGet("{id:int:min(1)}")]     // GET /api/products/5   ✅
                                  // GET /api/products/0   ❌ (min is 1)
                                  // GET /api/products/-1  ❌ (min is 1)
                                  // GET /api/products/abc ❌ (not int)

[HttpGet("{code:alpha:length(3)}")]  // GET /api/products/PEN  ✅
                                     // GET /api/products/AB   ❌ (not length 3)
                                     // GET /api/products/123  ❌ (not alpha)
```

---

## 5.5 Query String Parameters — Reading URL Parameters

**Definition:**

> A query string is the part of a URL after the `?` character. It consists of key=value pairs separated by `&`. Query strings are used to pass optional filters, search terms, pagination info, and other parameters.

```
URL: /api/products?category=Electronics&maxPrice=500&page=2&pageSize=10
                  ↑
              Query String starts here

Breakdown:
  category = "Electronics"
  maxPrice = 500
  page     = 2
  pageSize = 10
```

**Reading query string in action methods:**

```csharp
// GET /api/products?category=Electronics&minPrice=100&page=1&pageSize=10
[HttpGet]
public IActionResult GetFiltered(
    string category  = null,    // If not in URL, defaults to null
    decimal? minPrice = null,   // If not in URL, defaults to null (nullable)
    decimal? maxPrice = null,
    int page     = 1,           // Default to page 1 if not specified
    int pageSize = 10)          // Default to 10 items per page

// ↑ ASP.NET Core automatically reads these from the query string
// No attribute needed — ASP.NET Core is smart enough to recognize:
//   - Simple types (int, string, bool, decimal) without [FromRoute] = query string
//   - Complex types (classes) without [FromRoute] = body (for POST/PUT)
{
    // Build dynamic query based on what was provided
    var sql = "SELECT * FROM Products WHERE 1=1";  // "WHERE 1=1" is a trick to easily append conditions

    var parameters = new List<SqlParameter>();

    if (!string.IsNullOrEmpty(category))
    {
        sql += " AND Category = @Category";
        parameters.Add(new SqlParameter("@Category", category));
    }

    if (minPrice.HasValue)
    {
        sql += " AND Price >= @MinPrice";
        parameters.Add(new SqlParameter("@MinPrice", minPrice.Value));
    }

    if (maxPrice.HasValue)
    {
        sql += " AND Price <= @MaxPrice";
        parameters.Add(new SqlParameter("@MaxPrice", maxPrice.Value));
    }

    sql += " ORDER BY Name";
    sql += $" OFFSET {(page-1)*pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

    // Execute and return...
}
```

**Explicitly marking query string parameters (optional but clear):**

```csharp
// [FromQuery] makes it explicit — this comes from query string
[HttpGet]
public IActionResult Search(
    [FromQuery] string term,
    [FromQuery] string sort = "name",
    [FromQuery] string order = "asc")
{ }
```

---

## 5.6 Route vs Query String — When to Use Which

| Use Route Parameter                   | Use Query String                        |
| ------------------------------------- | --------------------------------------- |
| Identifying a specific resource       | Filtering results                       |
| Required, always present              | Optional                                |
| Short, simple values                  | Multiple optional values                |
| /api/products/**5**             | /api/products?**category=Tech&page=2**  |
| /api/categories/**Electronics** | /api/products?**search=pen&sort=price** |
| Looks like a "path" to something      | Looks like "options" for something      |

```csharp
// ✅ Good use of route parameter — identifies a specific product
GET /api/products/5          → [HttpGet("{id}")]

// ✅ Good use of query string — filters a list
GET /api/products?category=Electronics&maxPrice=500

// ❌ Bad — don't put filters in route
GET /api/products/Electronics/500   // Confusing, what is 500?

// ❌ Bad — don't put ID in query string
GET /api/products?id=5             // Use /api/products/5 instead
```

---

## 5.7 Route Ambiguity — When Two Routes Match

**Problem:** What if two action methods could match the same URL?

```csharp
[HttpGet("{id}")]      // Could match /api/products/5
[HttpGet("{name}")]    // Could also match /api/products/5 ← CONFLICT!
```

ASP.NET Core throws an `AmbiguousMatchException`. Fix it with constraints:

```csharp
[HttpGet("{id:int}")]         // Only matches if it's an integer: /api/products/5
[HttpGet("{name:alpha}")]     // Only matches if it's letters: /api/products/Pen
// Now /api/products/5   → goes to first method (it's an int)
// Now /api/products/Pen → goes to second method (it's alpha)
// No conflict!
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 6 — JSON

# How Data Travels Between Server and Client

# ═══════════════════════════════════════════

---

## 6.1 What is JSON and Why Does Every API Use It?

**Definition:**

> JSON (JavaScript Object Notation) is a lightweight, human-readable text format for representing structured data. It has become the universal standard for data exchange between web APIs and clients because every programming language, browser, and framework can read and write it.

**Why JSON and not something else?**
Before JSON, APIs used XML (much more verbose). JSON won because:

1. Easy for humans to read
2. Very compact (less data to transfer)
3. JavaScript can parse it natively (no extra libraries needed)
4. Every programming language has JSON libraries

**JSON Data Types:**

```json
{
  "stringValue":  "Hello World",              // Text — always in double quotes
  "intValue":     42,                          // Whole number — no quotes
  "decimalValue": 19.99,                       // Decimal number
  "boolValue":    true,                        // Boolean: true or false (lowercase)
  "nullValue":    null,                        // Null/nothing
  "arrayValue":   [1, 2, 3, "a", "b"],        // Array — square brackets, comma-separated
  "objectValue":  { "x": 1, "y": 2 },         // Nested object — curly braces
  "dateValue":    "2024-01-15T10:30:00"        // Date — JSON has no date type, use ISO string
}
```

**JSON Object vs JSON Array:**

```json
// A single object (represents one Product):
{
  "id": 1,
  "name": "Pen",
  "price": 5.0,
  "category": "Stationery",
  "inStock": true
}

// An array of objects (represents multiple Products):
[
  { "id": 1, "name": "Pen",   "price": 5.0,  "category": "Stationery" },
  { "id": 2, "name": "Book",  "price": 12.0, "category": "Education" },
  { "id": 3, "name": "Chair", "price": 150.0,"category": "Furniture" }
]
```

---

## 6.2 How ASP.NET Core Converts C# ↔ JSON (Serialization)

**Serialization:** Converting C# objects → JSON text (when returning from API)
**Deserialization:** Converting JSON text → C# objects (when receiving in POST/PUT body)

ASP.NET Core does this **automatically**. You just write C# code.

```
SERIALIZATION (outgoing — returning data):
C# List<Product>  →  [HttpGet] returns Ok(list)  →  JSON Array sent to client

DESERIALIZATION (incoming — receiving data):
JSON body from client  →  [HttpPost] [FromBody] Product p  →  C# Product object
```

**Naming Convention — PascalCase vs camelCase:**

```
C# Convention (PascalCase):   JSON Convention (camelCase):
ProductName                → productName
UnitPrice                  → unitPrice
IsAvailable                → isAvailable
CategoryId                 → categoryId
```

Configure this in `Program.cs` once:

```csharp
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Automatically converts PascalCase C# → camelCase JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

---

## 6.3 Controlling JSON Output with Attributes

**On your C# model class, you can use attributes to control how properties appear in JSON:**

```csharp
using System.Text.Json.Serialization;

public class Product
{
    // Normal property — serialized as-is (or camelCase if configured)
    public int Id { get; set; }

    // ── [JsonPropertyName] — Override the JSON field name ──────────
    // C# property: Name
    // JSON field: "product_name" (uses underscore instead)
    [JsonPropertyName("product_name")]
    public string Name { get; set; }

    public decimal Price { get; set; }

    // ── [JsonIgnore] — Never include this in JSON ──────────────────
    // Useful for sensitive data: passwords, internal codes, etc.
    [JsonIgnore]
    public string InternalCode { get; set; }

    // ── [JsonIgnore(Condition)] — Conditionally exclude ────────────
    // WhenWritingNull: only include in JSON if not null
    // Saves bandwidth — don't send null fields
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    // ── [JsonIgnore(WhenWritingDefault)] — Skip if default value ───
    // Skip if it's 0 for int, false for bool, null for reference types
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int StockCount { get; set; }

    // ── [JsonInclude] — Include even private/internal setters ──────
    [JsonInclude]
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    // ── [JsonConverter] — Use custom conversion logic ───────────────
    // e.g., serialize an enum as its string name instead of its number
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductStatus Status { get; set; }
}

public enum ProductStatus
{
    Active,     // Without converter: 0  | With converter: "Active"
    Inactive,   // Without converter: 1  | With converter: "Inactive"
    Discontinued// Without converter: 2  | With converter: "Discontinued"
}
```

---

## 6.4 Designing Your JSON Response Shape

Sometimes you need to return JSON that doesn't exactly match any single C# class. You can use:

### Anonymous Objects (Quick, No Class Needed)

```csharp
[HttpGet("summary")]
public IActionResult GetSummary()
{
    // Return a custom shape without creating a class
    return Ok(new
    {
        total    = 150,
        active   = 120,
        inactive = 30,
        lastUpdated = DateTime.Now,
        categories = new[] { "Electronics", "Stationery", "Furniture" }
    });
}

// JSON Result:
// {
//   "total": 150,
//   "active": 120,
//   "inactive": 30,
//   "lastUpdated": "2024-01-15T10:30:00",
//   "categories": ["Electronics", "Stationery", "Furniture"]
// }
```

### Standardized API Response Wrapper

A very common pattern is to wrap ALL responses in a consistent structure so client-side code always knows what to expect:

```csharp
// ApiResponse.cs — Create this class once, use it everywhere
public class ApiResponse<T>
{
    // Was the request successful?
    public bool Success { get; set; }

    // Human-readable message
    public string Message { get; set; }

    // The actual data (T can be Product, List<Product>, int, anything)
    public T Data { get; set; }

    // Total records (for paginated responses)
    public int? TotalCount { get; set; }

    // Any errors (validation errors, etc.)
    public object Errors { get; set; }

    // Factory methods for convenience
    public static ApiResponse<T> OK(T data, string message = "Success", int? total = null)
        => new() { Success = true, Message = message, Data = data, TotalCount = total };

    public static ApiResponse<T> Fail(string message, object errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

// Usage in controllers:
[HttpGet]
public IActionResult GetAll()
{
    var products = _repo.GetAll();
    return Ok(ApiResponse<List<Product>>.OK(products, "Products retrieved", products.Count));
}

// JSON result:
// {
//   "success": true,
//   "message": "Products retrieved",
//   "data": [{"id":1,"name":"Pen",...}, ...],
//   "totalCount": 25,
//   "errors": null
// }

[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var product = _repo.GetById(id);
    if (product == null)
        return NotFound(ApiResponse<Product>.Fail($"Product {id} not found"));
    return Ok(ApiResponse<Product>.OK(product));
}
```

---

## 6.5 Handling Dates in JSON

Dates are tricky in JSON because JSON has no built-in date type. ASP.NET Core uses ISO 8601 format by default.

```csharp
// C# DateTime: DateTime.Now = January 15, 2024 at 10:30 AM
// JSON output: "2024-01-15T10:30:00"  (ISO 8601 format)

// Reading a date from JSON body — works automatically:
public class Order
{
    public DateTime OrderDate { get; set; }   // Client sends: "2024-01-15T10:30:00"
}

// Handling timezone-aware dates:
public class Event
{
    public DateTimeOffset StartTime { get; set; }  // "2024-01-15T10:30:00+05:30"
    // DateTimeOffset is better than DateTime — it includes timezone offset
}
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 7 — Model Binding

# How Data Gets Into Your Methods

# ═══════════════════════════════════════════

---

## 7.1 What is Model Binding?

**Definition:**

> Model Binding is the process by which ASP.NET Core automatically reads data from an incoming HTTP request (from the URL, query string, headers, or body) and maps it to the parameters of your action method.

Without model binding, you'd have to manually read the raw HTTP request every time — checking headers, parsing query strings, deserializing JSON. Model binding does all of this automatically.

**Where can data come from in an HTTP request?**

```
HTTP Request
│
├── URL Route Segment:  /api/products/5/category/Electronics
│                       ↑                ↑
│                       This "5" and "Electronics" are route values
│
├── Query String:       ?page=1&pageSize=10&sort=name
│                       ↑
│                       Key=value pairs after the ?
│
├── Request Body:       { "name": "Pen", "price": 5.0 }
│                       ↑
│                       JSON (or form data) in the body of POST/PUT
│
├── HTTP Headers:       Authorization: Bearer eyJ...
│                       X-Api-Key: my-secret-key
│                       ↑
│                       Metadata about the request
│
└── Form Data:          name=Pen&price=5.0 (from HTML forms)
```

---

## 7.2 The Binding Source Attributes

```csharp
// ── [FromRoute] — Data comes from the URL route segment ───────────
// URL: GET /api/products/5
[HttpGet("{id}")]
public IActionResult GetById([FromRoute] int id)
//                           ↑ Reads from {id} in the route
// id = 5

// ── [FromQuery] — Data comes from the query string ─────────────────
// URL: GET /api/products?category=Electronics&page=2
[HttpGet]
public IActionResult GetAll([FromQuery] string category, [FromQuery] int page = 1)
//                          ↑ Reads from ?category=...&page=...

// ── [FromBody] — Data comes from the JSON body ─────────────────────
// Used for POST/PUT where client sends JSON
// Body: { "name": "Pen", "price": 5.0 }
[HttpPost]
public IActionResult Create([FromBody] Product product)
//                          ↑ Deserializes JSON body into Product object

// ── [FromHeader] — Data comes from HTTP headers ─────────────────────
// Header: X-User-Language: en-US
[HttpGet]
public IActionResult GetLocalized([FromHeader(Name = "X-User-Language")] string language)
//                                ↑ Reads from the named header

// ── [FromForm] — Data comes from HTML form submission ───────────────
// Content-Type: multipart/form-data or application/x-www-form-urlencoded
[HttpPost("upload")]
public IActionResult Upload([FromForm] string description, [FromForm] IFormFile file)

// ── Combining multiple sources in one action ────────────────────────
[HttpPut("{id}")]
public IActionResult Update(
    [FromRoute]  int id,               // From URL: /api/products/5
    [FromBody]   Product product,      // From JSON body
    [FromQuery]  bool notify = false,  // From ?notify=true
    [FromHeader(Name = "X-Reason")] string reason = null)  // From header
{ }
```

---

## 7.3 Automatic Inference (With `[ApiController]`)

`[ApiController]` is smart enough to infer the binding source in most cases, so you don't always have to write the attribute explicitly:

| Parameter Type                                                 | Where ASP.NET Core Looks |
| -------------------------------------------------------------- | ------------------------ |
| Simple types (int, string, bool, decimal...) in route `{id}` | `[FromRoute]`          |
| Simple types NOT in route                                      | `[FromQuery]`          |
| Complex types (your C# classes) in POST/PUT                    | `[FromBody]`           |
| `IFormFile`                                                  | `[FromForm]`           |

```csharp
// These two are identical:

// Explicit:
[HttpGet("{id}")]
public IActionResult GetById([FromRoute] int id, [FromQuery] string format) { }

// Implicit (same behavior with [ApiController]):
[HttpGet("{id}")]
public IActionResult GetById(int id, string format) { }
// ASP.NET Core figures out: id → route, format → query string
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 8 — Validation

# Checking Data Before Using It

# ═══════════════════════════════════════════

---

## 8.1 Why Validation is Critical

**Definition:**

> Validation is the process of checking that incoming data from clients meets the rules and constraints your application requires before processing it.

**Why validate?**

1. **Security:** Never trust client input. A malicious user can send anything.
2. **Data integrity:** You don't want empty names or negative prices in your database.
3. **User experience:** Return helpful error messages instead of cryptic database errors.
4. **Crash prevention:** An unexpected null value can crash your code.

**The Golden Rule of Web Development:**

> Never trust data coming from the client. Always validate on the server side, even if you validate on the client side too.

---

## 8.2 Data Annotations — Validation Rules on Your Models

**Definition:**

> Data Annotations are attributes you put on C# class properties to define validation rules. When `[ApiController]` processes a request, it automatically validates the model against these rules.

```csharp
using System.ComponentModel.DataAnnotations;

public class Product
{
    // Id doesn't need validation — it's set by DB or not required on create
    public int Id { get; set; }

    // ── [Required] — Field must be present and not empty/null ────────
    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; }

    // ── [Required] + [StringLength] — Required AND length limits ─────
    [Required(ErrorMessage = "Category is required")]
    [StringLength(
        maximumLength: 50,
        MinimumLength: 2,
        ErrorMessage = "Category must be between 2 and 50 characters")]
    public string Category { get; set; }

    // ── [Range] — Number must be within a range ────────────────────
    [Required]
    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99,999.99")]
    public decimal Price { get; set; }

    // ── [Range] for integers ──────────────────────────────────────
    [Range(0, int.MaxValue, ErrorMessage = "Stock count cannot be negative")]
    public int StockCount { get; set; }

    // ── [EmailAddress] — Must be a valid email format ─────────────
    [EmailAddress(ErrorMessage = "Please provide a valid email address")]
    public string ContactEmail { get; set; }

    // ── [Phone] — Must be a valid phone number format ─────────────
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; }

    // ── [Url] — Must be a valid URL format ────────────────────────
    [Url(ErrorMessage = "Invalid website URL")]
    public string Website { get; set; }

    // ── [MaxLength] / [MinLength] — For strings and arrays ─────────
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    // ── [RegularExpression] — Must match a pattern ─────────────────
    // Pattern: Exactly 3 letters, a dash, then 4 digits (e.g. PEN-1234)
    [RegularExpression(@"^[A-Z]{3}-\d{4}$",
        ErrorMessage = "Product code must be format: ABC-1234")]
    public string ProductCode { get; set; }

    // ── [Compare] — Must match another field's value ───────────────
    // Used for "confirm password" scenarios
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    // ── Multiple attributes on one property ───────────────────────
    [Required]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9\s]+$",
        ErrorMessage = "Name can only contain letters, numbers, and spaces")]
    public string ValidatedName { get; set; }
}
```

---

## 8.3 What Happens When Validation Fails

With `[ApiController]`:

1. Client sends `POST /api/products` with body: `{ "price": -5 }` (no name, invalid price)
2. ASP.NET Core creates a `Product` object from the JSON body
3. It checks the validation attributes
4. Name: fails `[Required]`; Price: fails `[Range(0.01, 99999.99)]`
5. **Automatically returns 400 Bad Request** — your action method is never called
6. Response body:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-abc123-def456-00",
  "errors": {
    "Name": [
      "Product name is required"
    ],
    "Price": [
      "Price must be between 0.01 and 99,999.99"
    ]
  }
}
```

Your AJAX error handler receives this structured response and can display field-specific error messages.

---

## 8.4 Custom Validation Attribute

When built-in attributes don't cover your business rule, create a custom one:

```csharp
// CustomValidationAttributes.cs

// Example: A date field that must be in the future
public class FutureDateAttribute : ValidationAttribute
{
    // This method is called by ASP.NET Core during validation
    // value = the actual value of the property being validated
    // context = information about the model and property
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        if (value == null)
            return ValidationResult.Success;  // Let [Required] handle nulls

        if (value is DateTime date)
        {
            if (date > DateTime.Today)
                return ValidationResult.Success;  // Valid!
            else
                return new ValidationResult("Date must be in the future");  // Invalid!
        }

        return new ValidationResult("Invalid date");
    }
}

// Example: Validate that end date is after start date
// This validates across multiple properties
public class DateRangeAttribute : ValidationAttribute
{
    private readonly string _startDateProperty;

    public DateRangeAttribute(string startDatePropertyName)
    {
        _startDateProperty = startDatePropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        // Get the value of the start date property from the same object
        var startDateProp = context.ObjectType.GetProperty(_startDateProperty);
        var startDate = (DateTime)startDateProp.GetValue(context.ObjectInstance);
        var endDate = (DateTime)value;

        if (endDate > startDate)
            return ValidationResult.Success;

        return new ValidationResult($"End date must be after start date");
    }
}

// Usage:
public class Event
{
    [Required]
    [FutureDate]   // Uses our custom attribute
    public DateTime StartDate { get; set; }

    [Required]
    [DateRange("StartDate")]  // Must be after StartDate
    public DateTime EndDate { get; set; }
}
```

---

## 8.5 Validation Inside Action (For Business Rules)

Some rules can't be expressed as attributes — they involve database checks or complex logic:

```csharp
[HttpPost]
public IActionResult Create([FromBody] CreateProductRequest req)
{
    // [ApiController] already handled data annotation validation.
    // Now handle BUSINESS RULE validation:

    // Business Rule 1: Product code must be unique in DB
    if (ProductCodeExistsInDb(req.ProductCode))
    {
        // Add custom error to ModelState
        ModelState.AddModelError("ProductCode", "This product code is already taken");
        return BadRequest(ModelState);
    }

    // Business Rule 2: Category must exist in Categories table
    if (!CategoryExistsInDb(req.CategoryId))
    {
        ModelState.AddModelError("CategoryId", "Selected category does not exist");
        return BadRequest(ModelState);
    }

    // Business Rule 3: Price cannot exceed the category's maximum allowed price
    var categoryMaxPrice = GetCategoryMaxPrice(req.CategoryId);
    if (req.Price > categoryMaxPrice)
    {
        return BadRequest(new {
            message = $"Price cannot exceed {categoryMaxPrice:C} for this category"
        });
    }

    // All validation passed — proceed with creation
    // ...
}
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 9 — ADO.NET in API Controllers

# Working with SQL Server Database

# ═══════════════════════════════════════════

---

## 9.1 What is ADO.NET?

**Definition:**

> ADO.NET (ActiveX Data Objects .NET) is Microsoft's low-level data access library for connecting to databases, executing SQL commands, and reading results in .NET applications. It is the foundation that all higher-level ORMs (like Entity Framework) are built on.

**Why use ADO.NET instead of Entity Framework?**

- Full control over your SQL queries — you write exactly the SQL you want
- Better performance for complex queries
- Works naturally with stored procedures
- No "magic" — you know exactly what SQL is being executed
- Required when working with legacy databases or complex schemas

**ADO.NET Key Objects:**

| Object             | What It Is                     | What It Does                                      |
| ------------------ | ------------------------------ | ------------------------------------------------- |
| `SqlConnection`  | The connection to SQL Server   | Opens/closes the connection channel               |
| `SqlCommand`     | A SQL statement to execute     | Holds your SELECT/INSERT/UPDATE/DELETE or SP name |
| `SqlDataReader`  | Reads query results row by row | Fast, forward-only reader                         |
| `SqlDataAdapter` | Fills a DataTable from a query | Used with DataTable/DataSet                       |
| `SqlParameter`   | A named parameter in SQL       | Safely passes values to SQL (prevents injection)  |
| `DataTable`      | In-memory table                | Stores query results in a grid-like structure     |
| `DataSet`        | Collection of DataTables       | Multiple tables in memory                         |

---

## 9.2 The Essential ADO.NET Pattern

Every database operation follows the same structure:

```csharp
// The ADO.NET Pattern Template:
using var connection = new SqlConnection(connectionString);  // 1. Create connection
using var command = new SqlCommand(sql, connection);         // 2. Create command with SQL
command.Parameters.AddWithValue("@Param", value);           // 3. Add parameters (if any)
connection.Open();                                           // 4. Open the connection
// 5. Execute (one of three ways depending on what you need):
var reader = command.ExecuteReader();   // For SELECT — returns rows
var scalar = command.ExecuteScalar();   // For SELECT returning one value (COUNT, SCOPE_IDENTITY)
var rows    = command.ExecuteNonQuery();// For INSERT/UPDATE/DELETE — returns rows affected
```

**`using` keyword:**

> `using var con = new SqlConnection(...)` automatically closes and disposes the connection when the code block exits — even if an exception occurs. This is critical to prevent connection leaks. Always use `using` with SqlConnection and SqlCommand.

---

## 9.3 READ — SELECT Data from Database

```csharp
[HttpGet]
public IActionResult GetAll()
{
    var products = new List<Product>();  // Will hold our results

    // "using" ensures the connection is closed automatically when done
    using var connection = new SqlConnection(_connectionString);

    // Write your SQL. Always use @Parameters — NEVER string concatenation!
    using var command = new SqlCommand(
        "SELECT Id, Name, Price, Category, IsActive FROM Products ORDER BY Name",
        connection);

    connection.Open();  // Open the connection to SQL Server

    // ExecuteReader() runs the SELECT and returns a reader
    using var reader = command.ExecuteReader();

    // reader.Read() advances to the next row and returns false when no more rows
    while (reader.Read())
    {
        products.Add(new Product
        {
            // reader["ColumnName"] reads the value of that column in current row
            // You must cast it to the right C# type

            // Cast to int:
            Id = (int)reader["Id"],

            // .ToString() handles string columns:
            Name     = reader["Name"].ToString(),
            Category = reader["Category"].ToString(),

            // Cast to decimal:
            Price = (decimal)reader["Price"],

            // Cast to bool:
            IsActive = (bool)reader["IsActive"],

            // Safe reading for columns that might be NULL in database:
            Description = reader["Description"] == DBNull.Value
                          ? null
                          : reader["Description"].ToString()
        });
    }

    return Ok(products);
    // ASP.NET Core converts List<Product> → JSON array automatically
}
```

---

## 9.4 READ ONE — SELECT Single Record

```csharp
[HttpGet("{id:int}")]
public IActionResult GetById(int id)
{
    Product product = null;

    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand(
        "SELECT Id, Name, Price, Category FROM Products WHERE Id = @Id",
        connection);

    // AddWithValue(paramName, value) — safely adds the parameter
    // @Id in SQL will be replaced by the value of `id`
    // This prevents SQL Injection — never do: "WHERE Id = " + id
    command.Parameters.AddWithValue("@Id", id);

    connection.Open();
    using var reader = command.ExecuteReader();

    // For a single record, we only call Read() once
    // If the record exists, Read() returns true; if not, returns false
    if (reader.Read())
    {
        product = new Product
        {
            Id       = (int)reader["Id"],
            Name     = reader["Name"].ToString(),
            Price    = (decimal)reader["Price"],
            Category = reader["Category"].ToString()
        };
    }

    if (product == null)
        return NotFound(new { message = $"Product {id} not found" });

    return Ok(product);
}
```

---

## 9.5 INSERT — Create a New Record

```csharp
[HttpPost]
public IActionResult Create([FromBody] Product product)
{
    using var connection = new SqlConnection(_connectionString);

    // The SQL inserts the row, then immediately returns the new auto-generated ID
    // SCOPE_IDENTITY() = the ID value that was just inserted
    using var command = new SqlCommand(@"
        INSERT INTO Products (Name, Price, Category, IsActive, CreatedDate)
        VALUES (@Name, @Price, @Category, @IsActive, @CreatedDate);
        SELECT SCOPE_IDENTITY();", connection);

    // Add all parameters — never concatenate user input into SQL!
    command.Parameters.AddWithValue("@Name",        product.Name);
    command.Parameters.AddWithValue("@Price",       product.Price);
    command.Parameters.AddWithValue("@Category",    product.Category);
    command.Parameters.AddWithValue("@IsActive",    product.IsActive);
    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);  // Server sets the date

    connection.Open();

    // ExecuteScalar() returns the first column of the first row of the result
    // Our SQL returns SCOPE_IDENTITY() = the new ID
    var result = command.ExecuteScalar();
    int newId = Convert.ToInt32(result);

    product.Id = newId;  // Update our object with the new ID

    // Return 201 Created with the new product (including its ID)
    // CreatedAtAction also sets the Location header to /api/products/{newId}
    return CreatedAtAction(nameof(GetById), new { id = newId }, product);
}
```

---

## 9.6 UPDATE — Modify an Existing Record

```csharp
[HttpPut("{id:int}")]
public IActionResult Update(int id, [FromBody] Product product)
{
    if (id != product.Id)
        return BadRequest(new { message = "ID in URL must match ID in body" });

    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand(@"
        UPDATE Products
        SET Name        = @Name,
            Price       = @Price,
            Category    = @Category,
            IsActive    = @IsActive,
            UpdatedDate = @UpdatedDate
        WHERE Id = @Id", connection);

    command.Parameters.AddWithValue("@Id",          id);
    command.Parameters.AddWithValue("@Name",        product.Name);
    command.Parameters.AddWithValue("@Price",       product.Price);
    command.Parameters.AddWithValue("@Category",    product.Category);
    command.Parameters.AddWithValue("@IsActive",    product.IsActive);
    command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

    connection.Open();

    // ExecuteNonQuery() returns the number of rows affected
    // If 0 rows affected, the ID didn't exist
    int rowsAffected = command.ExecuteNonQuery();

    if (rowsAffected == 0)
        return NotFound(new { message = $"Product {id} not found" });

    return NoContent();  // 204 — success, no body
}
```

---

## 9.7 DELETE — Remove a Record

```csharp
[HttpDelete("{id:int}")]
public IActionResult Delete(int id)
{
    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand(
        "DELETE FROM Products WHERE Id = @Id", connection);

    command.Parameters.AddWithValue("@Id", id);

    connection.Open();
    int rowsAffected = command.ExecuteNonQuery();

    if (rowsAffected == 0)
        return NotFound(new { message = $"Product {id} not found" });

    return NoContent();  // 204 — deleted successfully
}
```

---

## 9.8 Using Stored Procedures

**Definition:**

> A Stored Procedure is a named, pre-compiled SQL code block stored in the SQL Server database. You call it by name and pass parameters. They are great for complex queries, security (restrict direct table access), and reusability.

```sql
-- In SQL Server Management Studio:
CREATE PROCEDURE GetProductsByCategory
    @Category  NVARCHAR(50),
    @MinPrice  DECIMAL(18,2) = 0,     -- Default value: 0
    @MaxPrice  DECIMAL(18,2) = 99999  -- Default value: 99999
AS
BEGIN
    SELECT Id, Name, Price, Category
    FROM Products
    WHERE Category = @Category
      AND Price BETWEEN @MinPrice AND @MaxPrice
    ORDER BY Price;
END
```

```csharp
[HttpGet("by-category/{category}")]
public IActionResult GetByCategory(string category, decimal minPrice = 0, decimal maxPrice = 99999)
{
    var list = new List<Product>();

    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand("GetProductsByCategory", connection);

    // CRITICAL: Tell ADO.NET this is a stored procedure, not a SQL string
    command.CommandType = CommandType.StoredProcedure;

    // Add parameters matching the SP's parameter names
    command.Parameters.AddWithValue("@Category", category);
    command.Parameters.AddWithValue("@MinPrice",  minPrice);
    command.Parameters.AddWithValue("@MaxPrice",  maxPrice);

    connection.Open();
    using var reader = command.ExecuteReader();
    while (reader.Read())
        list.Add(MapProduct(reader));  // See helper method below

    return Ok(list);
}

// Helper method — map a data reader row to a Product object
// Centralizes the mapping so you don't repeat it in every action
private Product MapProduct(SqlDataReader reader)
{
    return new Product
    {
        Id       = (int)reader["Id"],
        Name     = reader["Name"].ToString(),
        Price    = (decimal)reader["Price"],
        Category = reader["Category"].ToString()
    };
}
```

---

## 9.9 Stored Procedure with OUTPUT Parameter

An OUTPUT parameter lets a stored procedure return a value without using a SELECT:

```sql
-- SP that creates a product and returns the new ID via OUTPUT param
CREATE PROCEDURE CreateProduct
    @Name        NVARCHAR(100),
    @Price       DECIMAL(18,2),
    @Category    NVARCHAR(50),
    @NewId       INT OUTPUT      -- OUTPUT parameter
AS
BEGIN
    INSERT INTO Products (Name, Price, Category) VALUES (@Name, @Price, @Category);
    SET @NewId = SCOPE_IDENTITY();  -- Set the OUTPUT parameter
END
```

```csharp
using var connection = new SqlConnection(_connectionString);
using var command = new SqlCommand("CreateProduct", connection);
command.CommandType = CommandType.StoredProcedure;

// Regular input parameters
command.Parameters.AddWithValue("@Name",     product.Name);
command.Parameters.AddWithValue("@Price",    product.Price);
command.Parameters.AddWithValue("@Category", product.Category);

// Output parameter — must specify direction
var outputParam = new SqlParameter("@NewId", SqlDbType.Int);
outputParam.Direction = ParameterDirection.Output;  // ← This is the key part
command.Parameters.Add(outputParam);

connection.Open();
command.ExecuteNonQuery();  // Execute (OUTPUT param is set by SP)

// Read the OUTPUT parameter value after execution
int newId = (int)outputParam.Value;
```

---

## 9.10 Transactions — Ensuring Multiple Operations Succeed or Fail Together

**Definition:**

> A database transaction is a group of SQL operations that must ALL succeed or ALL be rolled back (undone). If any one operation fails, the entire group is undone — the database returns to its original state.

**The classic example: Bank Transfer**

```
Transfer $500 from Account A to Account B:
  Step 1: Deduct $500 from Account A
  Step 2: Add $500 to Account B

Without transaction:
  Step 1 succeeds ✅
  Step 2 fails ❌ (power outage, network error)
  → Account A lost $500, Account B has nothing. Money disappeared!

With transaction:
  Step 1 succeeds ✅
  Step 2 fails ❌
  → ROLLBACK: Step 1 is undone. Account A still has its $500.
  → Database is back to original state. No money lost.
```

```csharp
[HttpPost("transfer-stock")]
public IActionResult TransferStock([FromBody] StockTransferRequest req)
{
    using var connection = new SqlConnection(_connectionString);
    connection.Open();  // Open BEFORE starting transaction

    // Begin a transaction on this connection
    using var transaction = connection.BeginTransaction();

    try
    {
        // ── Operation 1: Deduct from source product ──────────────
        using var deductCmd = new SqlCommand(
            "UPDATE Products SET Stock = Stock - @Amount WHERE Id = @SourceId AND Stock >= @Amount",
            connection, transaction);  // ← Pass the transaction to the command!
        deductCmd.Parameters.AddWithValue("@Amount",   req.Amount);
        deductCmd.Parameters.AddWithValue("@SourceId", req.SourceProductId);
        int deductRows = deductCmd.ExecuteNonQuery();

        // Business rule: if no rows affected, source has insufficient stock
        if (deductRows == 0)
        {
            transaction.Rollback();  // Undo everything
            return BadRequest(new { message = "Insufficient stock or product not found" });
        }

        // ── Operation 2: Add to destination product ───────────────
        using var addCmd = new SqlCommand(
            "UPDATE Products SET Stock = Stock + @Amount WHERE Id = @DestId",
            connection, transaction);  // ← Same transaction!
        addCmd.Parameters.AddWithValue("@Amount", req.Amount);
        addCmd.Parameters.AddWithValue("@DestId", req.DestinationProductId);
        int addRows = addCmd.ExecuteNonQuery();

        if (addRows == 0)
        {
            transaction.Rollback();  // Undo the deduction too
            return NotFound(new { message = "Destination product not found" });
        }

        // ── Log the transfer ──────────────────────────────────────
        using var logCmd = new SqlCommand(
            "INSERT INTO StockTransferLog (FromId, ToId, Amount, Date) VALUES (@F, @T, @A, @D)",
            connection, transaction);
        logCmd.Parameters.AddWithValue("@F", req.SourceProductId);
        logCmd.Parameters.AddWithValue("@T", req.DestinationProductId);
        logCmd.Parameters.AddWithValue("@A", req.Amount);
        logCmd.Parameters.AddWithValue("@D", DateTime.Now);
        logCmd.ExecuteNonQuery();

        // All operations succeeded → COMMIT (make permanent)
        transaction.Commit();

        return Ok(new { message = "Stock transferred successfully" });
    }
    catch (Exception ex)
    {
        // Something crashed → ROLLBACK (undo all changes)
        transaction.Rollback();
        return StatusCode(500, new { message = "Transfer failed", error = ex.Message });
    }
}
```

---

## 9.11 Pagination with ADO.NET

**Definition:**

> Pagination is the practice of returning data in small chunks (pages) instead of all at once. Without pagination, a table with 100,000 rows would return all 100,000 rows on every request — very slow!

**SQL Server pagination uses `OFFSET` and `FETCH NEXT`:**

```sql
-- Get page 3, showing 10 items per page
-- Page 1: rows 1-10 (offset 0)
-- Page 2: rows 11-20 (offset 10)
-- Page 3: rows 21-30 (offset 20)  ← OFFSET = (page-1) * pageSize

SELECT Id, Name, Price FROM Products
ORDER BY Name
OFFSET 20 ROWS          -- Skip the first 20 rows
FETCH NEXT 10 ROWS ONLY -- Return only 10 rows
```

```csharp
[HttpGet]
public IActionResult GetPaged(int page = 1, int pageSize = 10)
{
    // Validation: ensure page and pageSize make sense
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;  // Max 100 per page

    int offset = (page - 1) * pageSize;  // How many rows to skip

    var list = new List<Product>();
    int totalCount = 0;

    using var connection = new SqlConnection(_connectionString);
    connection.Open();

    // ── Step 1: Get total count (for Kendo Grid pagination info) ──
    using (var countCmd = new SqlCommand("SELECT COUNT(*) FROM Products", connection))
    {
        totalCount = (int)countCmd.ExecuteScalar();
    }

    // ── Step 2: Get just the data for this page ───────────────────
    using var dataCmd = new SqlCommand(@"
        SELECT Id, Name, Price, Category
        FROM Products
        ORDER BY Name              -- Must ORDER BY to use OFFSET/FETCH
        OFFSET @Offset ROWS        -- Skip these many rows
        FETCH NEXT @PageSize ROWS ONLY  -- Return this many rows", connection);

    dataCmd.Parameters.AddWithValue("@Offset",   offset);
    dataCmd.Parameters.AddWithValue("@PageSize", pageSize);

    using var reader = dataCmd.ExecuteReader();
    while (reader.Read())
        list.Add(MapProduct(reader));

    // Return data plus metadata that Kendo Grid needs
    return Ok(new
    {
        data       = list,
        total      = totalCount,
        page       = page,
        pageSize   = pageSize,
        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
    });
}
```

---

## 9.12 The Repository Pattern — Clean, Organized Data Access

**Definition:**

> The Repository Pattern is a design pattern that separates database access code from controller logic. Instead of putting ADO.NET code directly in controllers, you put it in a separate "Repository" class. Controllers call the repository, and the repository talks to the database.

**Why use it?**

```
WITHOUT Repository Pattern:
  ProductsController has 200 lines of ADO.NET code
  OrdersController duplicates some of the same code
  Hard to test (can't swap database in tests)
  Hard to change (if you switch DB, edit every controller)

WITH Repository Pattern:
  ProductsController: 30 lines — calls _productRepo.GetAll()
  ProductRepository: 200 lines — all ADO.NET code is here
  Easy to test (inject a fake repository)
  Easy to change (swap only the repository implementation)
```

```csharp
// Step 1: Define the Interface (the contract — what methods it has)
// IProductRepository.cs

public interface IProductRepository
{
    List<Product> GetAll();
    List<Product> GetPaged(int page, int pageSize, out int totalCount);
    Product GetById(int id);
    int Create(Product product);
    bool Update(Product product);
    bool Delete(int id);
}

// ─────────────────────────────────────────────────────────────────────────

// Step 2: Implement the Interface (all ADO.NET code goes here)
// ProductRepository.cs

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public List<Product> GetAll()
    {
        var list = new List<Product>();
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT * FROM Products ORDER BY Name", con);
        con.Open();
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read()) list.Add(MapProduct(rdr));
        return list;
    }

    public Product GetById(int id)
    {
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();
        using var rdr = cmd.ExecuteReader();
        return rdr.Read() ? MapProduct(rdr) : null;
    }

    public int Create(Product p)
    {
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            "INSERT INTO Products(Name,Price,Category) VALUES(@N,@P,@C); SELECT SCOPE_IDENTITY();", con);
        cmd.Parameters.AddWithValue("@N", p.Name);
        cmd.Parameters.AddWithValue("@P", p.Price);
        cmd.Parameters.AddWithValue("@C", p.Category);
        con.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public bool Update(Product p)
    {
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(
            "UPDATE Products SET Name=@N,Price=@P,Category=@C WHERE Id=@Id", con);
        cmd.Parameters.AddWithValue("@Id", p.Id);
        cmd.Parameters.AddWithValue("@N",  p.Name);
        cmd.Parameters.AddWithValue("@P",  p.Price);
        cmd.Parameters.AddWithValue("@C",  p.Category);
        con.Open();
        return cmd.ExecuteNonQuery() > 0;  // True if at least one row was updated
    }

    public bool Delete(int id)
    {
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("DELETE FROM Products WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    // Private helper — centralizes the mapping from reader to object
    private Product MapProduct(SqlDataReader rdr)
    {
        return new Product
        {
            Id       = (int)rdr["Id"],
            Name     = rdr["Name"].ToString(),
            Price    = (decimal)rdr["Price"],
            Category = rdr["Category"].ToString()
        };
    }
}

// ─────────────────────────────────────────────────────────────────────────

// Step 3: Register in Program.cs
// "When someone needs IProductRepository, give them ProductRepository"
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// ─────────────────────────────────────────────────────────────────────────

// Step 4: Use in Controller — clean, simple, no ADO.NET visible!
// ProductsController.cs

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;

    public ProductsController(IProductRepository repo)
    {
        _repo = repo;  // Injected by DI — see Chapter 17
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = _repo.GetAll();    // Clean! No SQL here
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var product = _repo.GetById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        int newId = _repo.Create(product);
        product.Id = newId;
        return CreatedAtAction(nameof(GetById), new { id = newId }, product);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Product product)
    {
        if (id != product.Id) return BadRequest("ID mismatch");
        bool updated = _repo.Update(product);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        bool deleted = _repo.Delete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
```

---

## 9.13 Async ADO.NET — Non-Blocking Database Calls

**Definition:**

> Async/await is a programming pattern that allows your code to wait for slow operations (like database queries) without blocking the thread. This means your server can handle many more simultaneous requests.

**Why does this matter?**

- A web server has a limited number of threads
- Each thread waiting for a database response is "blocked" — it can't do anything else
- With async, while waiting for the database, the thread is released to handle other requests
- Result: your server can handle 10x more simultaneous users

```csharp
// Sync version (blocks thread while waiting for DB):
public IActionResult GetAll()
{
    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand("SELECT * FROM Products", con);
    con.Open();                      // Thread is blocked here waiting for connection
    using var rdr = cmd.ExecuteReader();  // Thread is blocked here waiting for query
    while (rdr.Read()) { /* map */ }
    return Ok(list);
}

// Async version (thread is released while waiting):
public async Task<IActionResult> GetAll()
//     ↑ async keyword        ↑ Return type wraps in Task
{
    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand("SELECT * FROM Products", con);
    await con.OpenAsync();                    // Thread released while connecting
    using var rdr = await cmd.ExecuteReaderAsync(); // Thread released while querying
    while (await rdr.ReadAsync())            // Thread released between rows
    {
        list.Add(MapProduct(rdr));
    }
    return Ok(list);
}
```

**Async equivalents for every ADO.NET method:**

| Sync                          | Async                                    |
| ----------------------------- | ---------------------------------------- |
| `connection.Open()`         | `await connection.OpenAsync()`         |
| `command.ExecuteReader()`   | `await command.ExecuteReaderAsync()`   |
| `command.ExecuteScalar()`   | `await command.ExecuteScalarAsync()`   |
| `command.ExecuteNonQuery()` | `await command.ExecuteNonQueryAsync()` |
| `reader.Read()`             | `await reader.ReadAsync()`             |

---

---

# ═══════════════════════════════════════════

# CHAPTER 10 — AJAX

# Calling Your API from the Browser

# ═══════════════════════════════════════════

---

## 10.1 What is AJAX?

**Definition:**

> AJAX (Asynchronous JavaScript and XML) is a technique that allows a web page to communicate with a server and update parts of itself **without reloading the entire page**. Despite the name including "XML", modern AJAX almost exclusively uses JSON.

**Before AJAX:**
Every time you needed fresh data, the whole page had to reload. Click a button → entire page refreshes → user sees white screen briefly → all their scroll position and state is lost.

**After AJAX:**
Click a button → JavaScript sends a background HTTP request → receives JSON → updates just that part of the page. User stays on the page, no white screen, instant feel.

**Real-world examples of AJAX:**

- Kendo Grid refreshing its data without reloading the page
- A search box showing suggestions as you type
- Saving a form without navigating away
- A dropdown reloading its options based on another dropdown's selection

---

## 10.2 jQuery AJAX — The Foundation

jQuery's `$.ajax()` is the most common way to make HTTP requests in a traditional MVC + Kendo app:

```javascript
// The full $.ajax() structure:
$.ajax({
    url:         '/api/products',      // Which URL to call
    type:        'GET',                // HTTP method: GET, POST, PUT, DELETE
    contentType: 'application/json',   // Format of data you're SENDING (needed for POST/PUT)
    dataType:    'json',               // Format you expect BACK (optional — jQuery detects it)
    data:        { ... },              // Data to send (query string for GET, body for POST)
    headers:     { ... },              // Extra HTTP headers to include
    success: function(response) {      // Runs if request succeeds (2xx status)
        console.log(response);
    },
    error: function(xhr, status, err) { // Runs if request fails (4xx or 5xx)
        console.error(xhr.responseText);
    },
    complete: function() {             // Runs regardless of success or failure
        // Hide loading spinner here
    }
});
```

---

## 10.3 AJAX GET — Fetching Data

```javascript
// ── Simple GET — Load all products ────────────────────────────────
$.ajax({
    url:     '/api/products',
    type:    'GET',
    success: function(products) {
        // products is a JavaScript array of objects
        // [ {id:1, name:"Pen", price:5}, {id:2, name:"Book", price:12} ]

        // Clear and rebuild a table:
        var html = '';
        $.each(products, function(index, product) {
            html += '<tr>';
            html += '<td>' + product.id + '</td>';
            html += '<td>' + product.name + '</td>';
            html += '<td>$' + product.price.toFixed(2) + '</td>';
            html += '<td><button onclick="deleteProduct(' + product.id + ')">Delete</button></td>';
            html += '</tr>';
        });
        $('#productsTableBody').html(html);
    },
    error: function(xhr) {
        // xhr.status = HTTP status code (404, 500, etc.)
        // xhr.responseText = the response body as text
        // xhr.responseJSON = the response body parsed as JSON (if it was JSON)
        alert('Error loading products: ' + xhr.status);
    }
});

// ── GET with query string ──────────────────────────────────────────
// This will call: /api/products?category=Electronics&maxPrice=500
$.ajax({
    url:  '/api/products',
    type: 'GET',
    data: {
        category: 'Electronics',   // → ?category=Electronics
        maxPrice:  500,            // → &maxPrice=500
        page:      1,              // → &page=1
        pageSize:  10              // → &pageSize=10
    },
    success: function(result) {
        console.log('Total:', result.total);
        console.log('Data:', result.data);
    }
});

// ── GET single item ────────────────────────────────────────────────
function loadProductDetails(productId) {
    $.ajax({
        url:  '/api/products/' + productId,    // /api/products/5
        type: 'GET',
        success: function(product) {
            // Populate a form or detail view:
            $('#txtName').val(product.name);
            $('#txtPrice').val(product.price);
            $('#ddlCategory').val(product.category);
        },
        error: function(xhr) {
            if (xhr.status === 404)
                alert('Product not found');
        }
    });
}
```

---

## 10.4 AJAX POST — Creating Data

```javascript
// ── POST — Create a new product ────────────────────────────────────
function createProduct() {
    // Collect data from your form fields
    var productData = {
        name:     $('#txtName').val().trim(),
        price:    parseFloat($('#txtPrice').val()),
        category: $('#ddlCategory').val(),
        isActive: $('#chkActive').is(':checked')   // true or false
    };

    // Basic client-side check (server validates too, but give quick feedback)
    if (!productData.name) {
        alert('Name is required');
        return;  // Stop here
    }

    $.ajax({
        url:         '/api/products',
        type:        'POST',

        // ← contentType tells the server the body is JSON
        // WITHOUT THIS: the server gets null in [FromBody] parameter!
        contentType: 'application/json',

        // ← JSON.stringify converts JS object → JSON string
        // WITHOUT THIS: you'd send [object Object] instead of real JSON!
        data:        JSON.stringify(productData),

        success: function(createdProduct) {
            // createdProduct = the object returned from the API (201 Created)
            // It includes the new ID assigned by the database
            alert('Created successfully! New ID: ' + createdProduct.id);

            // Clear the form
            $('#txtName').val('');
            $('#txtPrice').val('');

            // Refresh the Kendo Grid or table to show the new record
            refreshGrid();
        },
        error: function(xhr) {
            if (xhr.status === 400) {
                // Validation errors — structured ProblemDetails format
                var problemDetails = xhr.responseJSON;
                if (problemDetails.errors) {
                    // Build error message from all validation errors
                    var messages = [];
                    $.each(problemDetails.errors, function(field, errors) {
                        $.each(errors, function(i, msg) {
                            messages.push(field + ': ' + msg);
                        });
                    });
                    alert('Validation errors:\n' + messages.join('\n'));
                } else {
                    alert(problemDetails.message || 'Bad request');
                }
            } else if (xhr.status === 409) {
                alert('Conflict: ' + xhr.responseJSON.message);
            } else {
                alert('Error: ' + xhr.status + ' - ' + xhr.responseText);
            }
        }
    });
}
```

---

## 10.5 AJAX PUT — Updating Data

```javascript
function updateProduct(productId) {
    var productData = {
        id:       productId,              // Must include ID in body for PUT
        name:     $('#txtName').val(),
        price:    parseFloat($('#txtPrice').val()),
        category: $('#ddlCategory').val()
    };

    $.ajax({
        url:         '/api/products/' + productId,   // ID in URL too
        type:        'PUT',
        contentType: 'application/json',
        data:        JSON.stringify(productData),

        success: function() {
            // PUT returns 204 No Content — no response body
            // success callback still fires (no error)
            alert('Updated successfully');
            refreshGrid();
        },
        error: function(xhr) {
            if (xhr.status === 404)
                alert('Product not found');
            else if (xhr.status === 400)
                alert('Validation error: ' + xhr.responseText);
            else
                alert('Update failed: ' + xhr.status);
        }
    });
}
```

---

## 10.6 AJAX DELETE

```javascript
function deleteProduct(productId) {
    // Always confirm before destructive operations
    if (!confirm('Are you sure you want to delete this product?'))
        return;

    $.ajax({
        url:  '/api/products/' + productId,
        type: 'DELETE',
        // No body needed — ID is in the URL

        success: function() {
            // 204 No Content returned
            // Remove the row from the DOM without refreshing everything
            $('#product-row-' + productId).fadeOut(400, function() {
                $(this).remove();
            });
            // OR: refresh the Kendo Grid
            // grid.dataSource.read();
        },
        error: function(xhr) {
            if (xhr.status === 404)
                alert('Product already deleted or not found');
            else
                alert('Delete failed: ' + xhr.status);
        }
    });
}
```

---

## 10.7 Setting Up Global AJAX Defaults

Instead of repeating headers/settings in every AJAX call, set them once globally:

```javascript
// In your site.js or in $(document).ready():

// ── Set JSON as default content type for all AJAX calls ────────────
$.ajaxSetup({
    contentType: 'application/json',

    // Automatically add JWT token to all requests
    beforeSend: function(xhr) {
        var token = localStorage.getItem('jwtToken');
        if (token) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        }
    }
});

// ── Global error handler for all AJAX calls ────────────────────────
$(document).ajaxError(function(event, xhr, settings, error) {
    console.error('AJAX Error:', settings.url, xhr.status, error);

    if (xhr.status === 401) {
        // Token expired or invalid → redirect to login
        alert('Your session has expired. Please log in again.');
        window.location.href = '/Account/Login';
    } else if (xhr.status === 403) {
        alert('You do not have permission to perform this action.');
    } else if (xhr.status === 500) {
        alert('A server error occurred. Please try again or contact support.');
    }
});

// ── Show/hide loading indicator for all AJAX calls ──────────────────
$(document).ajaxStart(function() {
    $('#loadingOverlay').show();  // Show spinner
});

$(document).ajaxStop(function() {
    $('#loadingOverlay').hide();  // Hide spinner
});
```

---

## 10.8 Complete AJAX Modal Form Example

This is a very common pattern in MVC + API apps:

```html
<!-- In your View (Products/Index.cshtml) -->

<!-- Button to open modal -->
<button id="btnAddProduct" class="btn btn-primary">+ Add Product</button>

<!-- Modal Form (hidden by default) -->
<div id="productModal" style="display:none; position:fixed; top:50%; left:50%;
     transform:translate(-50%,-50%); background:white; padding:30px;
     border:1px solid #ccc; z-index:1000; min-width:400px;">

    <h3 id="modalTitle">Add Product</h3>

    <div style="margin-bottom:10px;">
        <input type="hidden" id="hidProductId" value="0" />
        <label>Name: <span style="color:red">*</span></label><br />
        <input type="text" id="txtName" style="width:100%" /><br />
        <span id="errName" style="color:red; display:none"></span>
    </div>

    <div style="margin-bottom:10px;">
        <label>Price: <span style="color:red">*</span></label><br />
        <input type="number" id="txtPrice" step="0.01" style="width:100%" /><br />
        <span id="errPrice" style="color:red; display:none"></span>
    </div>

    <div style="margin-bottom:10px;">
        <label>Category:</label><br />
        <select id="ddlCategory" style="width:100%">
            <option value="">-- Select --</option>
            <option value="Electronics">Electronics</option>
            <option value="Stationery">Stationery</option>
            <option value="Furniture">Furniture</option>
        </select>
    </div>

    <div>
        <button id="btnSave" onclick="saveProduct()">Save</button>
        <button onclick="closeModal()">Cancel</button>
    </div>
</div>
<div id="modalOverlay" style="display:none; position:fixed; top:0; left:0;
     width:100%; height:100%; background:rgba(0,0,0,0.5); z-index:999;"></div>

<script>
    // ── Open modal for CREATE ──────────────────────────────────────
    $('#btnAddProduct').click(function() {
        clearModal();
        $('#modalTitle').text('Add Product');
        $('#hidProductId').val('0');   // 0 = new product
        openModal();
    });

    // ── Open modal for EDIT ────────────────────────────────────────
    function editProduct(productId) {
        clearModal();
        $('#modalTitle').text('Edit Product');
        $('#hidProductId').val(productId);

        // Load current data from API
        $.ajax({
            url: '/api/products/' + productId,
            type: 'GET',
            success: function(product) {
                $('#txtName').val(product.name);
                $('#txtPrice').val(product.price);
                $('#ddlCategory').val(product.category);
                openModal();
            },
            error: function() {
                alert('Could not load product data');
            }
        });
    }

    // ── Save (Create or Update) ────────────────────────────────────
    function saveProduct() {
        clearErrors();

        var productId = parseInt($('#hidProductId').val());
        var productData = {
            id:       productId,
            name:     $('#txtName').val().trim(),
            price:    parseFloat($('#txtPrice').val()) || 0,
            category: $('#ddlCategory').val()
        };

        var isNew = productId === 0;
        var url   = isNew ? '/api/products' : '/api/products/' + productId;
        var method = isNew ? 'POST' : 'PUT';

        $.ajax({
            url:         url,
            type:        method,
            contentType: 'application/json',
            data:        JSON.stringify(productData),
            success: function() {
                closeModal();
                grid.dataSource.read();  // Refresh Kendo Grid
                showToast(isNew ? 'Product created!' : 'Product updated!');
            },
            error: function(xhr) {
                if (xhr.status === 400 && xhr.responseJSON?.errors) {
                    // Show field-specific validation errors
                    var errors = xhr.responseJSON.errors;
                    if (errors.Name)  { $('#errName').text(errors.Name[0]).show(); }
                    if (errors.Price) { $('#errPrice').text(errors.Price[0]).show(); }
                } else {
                    alert('Save failed: ' + (xhr.responseJSON?.message || xhr.responseText));
                }
            }
        });
    }

    function openModal()  { $('#productModal, #modalOverlay').show(); }
    function closeModal() { $('#productModal, #modalOverlay').hide(); }
    function clearModal() { $('#txtName,#txtPrice').val(''); $('#ddlCategory').val(''); clearErrors(); }
    function clearErrors() { $('[id^="err"]').hide().text(''); }

    function showToast(message) {
        // Simple toast notification
        $('<div>')
            .text(message)
            .css({position:'fixed', bottom:'20px', right:'20px',
                  background:'green', color:'white', padding:'15px',
                  borderRadius:'5px', zIndex:9999})
            .appendTo('body')
            .delay(3000)
            .fadeOut(function() { $(this).remove(); });
    }
</script>
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 11 — Kendo UI Grid ↔ API

# Full CRUD Integration

# ═══════════════════════════════════════════

---

## 11.1 What is Kendo UI DataSource?

**Definition:**

> Kendo UI DataSource is a JavaScript component that acts as a bridge between your Kendo widgets (Grid, DropDownList, Chart, etc.) and your API. It handles HTTP requests automatically — you just configure it with your API endpoints, and Kendo takes care of calling GET/POST/PUT/DELETE at the right times.

**What DataSource does for you:**

- On grid load: sends GET request, receives JSON, displays rows
- When user clicks "Add" and saves: sends POST request with new data
- When user edits a row and saves: sends PUT request with updated data
- When user clicks delete: sends DELETE request
- Handles server-side paging: sends skip/take parameters, reads total count
- Handles errors from API and triggers grid error events

---

## 11.2 Your API Must Return This Format for Kendo Grid

When using server paging, Kendo Grid expects:

```json
{
  "data": [
    { "id": 1, "name": "Pen",  "price": 5.0,  "category": "Stationery" },
    { "id": 2, "name": "Book", "price": 12.0, "category": "Education" }
  ],
  "total": 150
}
```

- `data` → the array of records for the current page
- `total` → the total number of records in the database (for pagination controls)

**API that returns this format:**

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    public ProductsController(IProductRepository repo) => _repo = repo;

    [HttpGet]
    public IActionResult GetForKendo(int skip = 0, int take = 10)
    // Kendo sends: ?skip=0&take=10 (first page)
    //              ?skip=10&take=10 (second page)
    //              ?skip=20&take=10 (third page)
    {
        int totalCount = 0;
        var data = _repo.GetPaged(skip, take, out totalCount);

        return Ok(new
        {
            data  = data,        // Current page of records
            total = totalCount   // Total records in DB (for pagination)
        });
    }
}
```

---

## 11.3 Complete Kendo Grid with Full CRUD — Explained in Detail

```html
<!-- Products/Index.cshtml -->

<div id="productsGrid"></div>

<script>
$(function() {

    // ══════════════════════════════════════════════════════════════
    // STEP 1: Configure the DataSource
    // This tells Kendo how to talk to your API
    // ══════════════════════════════════════════════════════════════
    var dataSource = new kendo.data.DataSource({

        // serverPaging: true = Kendo sends skip/take to API and the API pages the data
        // serverPaging: false = Kendo loads ALL data and pages it in the browser
        // Use serverPaging: true for large datasets (better performance)
        serverPaging: true,

        // transport = how to communicate with the server
        transport: {

            // READ — Called when grid loads or refreshes
            // GET /api/products?skip=0&take=10
            read: {
                url:      '/api/products',
                type:     'GET',
                dataType: 'json'
            },

            // CREATE — Called when user saves a new row
            // POST /api/products
            // Body: { "name": "Pen", "price": 5, "category": "Stationery" }
            create: {
                url:         '/api/products',
                type:        'POST',
                contentType: 'application/json',
                dataType:    'json'
            },

            // UPDATE — Called when user edits and saves an existing row
            // PUT /api/products/5
            // Body: { "id": 5, "name": "Blue Pen", "price": 6, "category": "Stationery" }
            update: {
                // url can be a function — lets you use the item's id dynamically
                url: function(item) {
                    return '/api/products/' + item.id;
                },
                type:        'PUT',
                contentType: 'application/json',
                dataType:    'json'
            },

            // DESTROY — Called when user clicks delete
            // DELETE /api/products/5
            destroy: {
                url: function(item) {
                    return '/api/products/' + item.id;
                },
                type: 'DELETE'
            },

            // parameterMap — transforms data before sending to server
            // operation: "read", "create", "update", or "destroy"
            parameterMap: function(data, operation) {
                if (operation === 'read') {
                    // For READ, send as query string (Kendo's default for GET)
                    return data;
                }
                // For CREATE/UPDATE/DESTROY, send as JSON string in body
                return JSON.stringify(data);
            }
        },

        // schema — tells Kendo how to interpret the JSON response
        schema: {
            // Which property in the JSON holds the array of records?
            // Our API returns: { "data": [...], "total": 150 }
            // So: data = "data", total = "total"
            data:  'data',
            total: 'total',

            // model — describes the shape of each record
            model: {
                id: 'id',    // Which field is the primary key?

                fields: {
                    id:       { type: 'number', editable: false },  // Auto-generated, not editable
                    name:     { type: 'string',  validation: { required: true } },
                    price:    { type: 'number',  validation: { required: true, min: 0.01 } },
                    category: { type: 'string',  validation: { required: true } },
                    isActive: { type: 'boolean' }
                }
            }
        },

        pageSize: 10,

        // Error handler — called if any transport operation fails
        error: function(e) {
            console.error('DataSource error:', e);
            // If editing, cancel any pending changes to reset the grid
            this.cancelChanges();

            if (e.xhr) {
                var status = e.xhr.status;
                var response = e.xhr.responseJSON;
                if (status === 400 && response?.errors) {
                    var msgs = [];
                    $.each(response.errors, function(k, v) { msgs.push(k + ': ' + v.join(', ')); });
                    alert('Validation errors:\n' + msgs.join('\n'));
                } else {
                    alert('Error ' + status + ': ' + (response?.message || 'Request failed'));
                }
            }
        }
    });

    // ══════════════════════════════════════════════════════════════
    // STEP 2: Create the Kendo Grid
    // ══════════════════════════════════════════════════════════════
    var grid = $('#productsGrid').kendoGrid({
        dataSource: dataSource,

        // Pagination controls
        pageable: {
            refresh:   true,         // Show "Refresh" button
            pageSizes: [5, 10, 25, 50],  // Page size options
            buttonCount: 5           // Max page buttons shown
        },

        sortable:  true,  // Click column headers to sort
        filterable: true, // Show filter row below headers

        // Toolbar: what buttons appear at the top of the grid
        // "create" = Add New button, "save"/"cancel" = for batch editing
        toolbar: ['create'],

        // How rows are edited:
        // "inline"  = row becomes editable in-place with Save/Cancel buttons
        // "popup"   = a popup form appears
        // "incell"  = click a cell to edit it directly
        editable: 'popup',  // Popup is usually most user-friendly

        columns: [
            {
                field: 'id',
                title: 'ID',
                width: 70,
                // filterable: false means no filter for this column
                filterable: false
            },
            {
                field: 'name',
                title: 'Product Name',
                width: 200
            },
            {
                field: 'price',
                title: 'Price',
                width: 120,
                // format applies a display format — doesn't change the stored value
                // {0:c2} = currency with 2 decimal places
                format: '{0:c2}',
                // filterable with custom operator
                filterable: { operators: { number: { gte: '>=', lte: '<=' } } }
            },
            {
                field: 'category',
                title: 'Category',
                width: 150,
                // Custom cell editor — a DropDownList instead of a plain textbox
                editor: categoryDropDownEditor
            },
            {
                field: 'isActive',
                title: 'Active',
                width: 80,
                // template: custom HTML for displaying the value
                template: '#= isActive ? "<span style=\\'color:green\\'>Yes</span>" : "<span style=\\'color:red\\'>No</span>" #'
            },
            {
                // Command column: edit and delete buttons per row
                command: ['edit', 'destroy'],
                title: 'Actions',
                width: 180
            }
        ]
    }).data('kendoGrid');  // .data() gives you the grid instance for later use

    // ── Custom editor for Category column ──────────────────────────
    // container = the DOM element inside the edit cell/popup
    // options.field = the field name (e.g., "category")
    function categoryDropDownEditor(container, options) {
        // Create a Kendo DropDownList inside the edit container
        $('<input required name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                // Hardcoded list — could also load from another API
                dataSource: ['Electronics', 'Stationery', 'Furniture', 'Food', 'Clothing'],
                optionLabel: '-- Select Category --'
            });
    }

});
</script>
```

---

## 11.4 Refreshing the Grid Programmatically

```javascript
// Get reference to the grid widget
var grid = $('#productsGrid').data('kendoGrid');

// Reload data from server:
grid.dataSource.read();

// Go to first page and reload:
grid.dataSource.page(1);
grid.dataSource.read();

// Add a new row (opens edit form):
grid.addRow();

// Get the currently selected row's data:
var dataItem = grid.dataItem(grid.select());
console.log('Selected ID:', dataItem.id, 'Name:', dataItem.name);

// Select a specific row:
var row = grid.tbody.find("tr[data-uid='" + someUid + "']");
grid.select(row);
```

---

## 11.5 Kendo Grid with Server-Side Filtering

Kendo can send filter parameters to your API. Here's the pattern:

```javascript
transport: {
    read: {
        url:  '/api/products',
        type: 'GET',
        data: function() {
            // Add your own custom parameters alongside Kendo's skip/take
            return {
                searchTerm: $('#txtSearch').val(),    // Custom search input
                categoryId: $('#ddlFilter').val()     // Custom filter
            };
            // Kendo automatically adds: skip=0&take=10
            // Your additions add: &searchTerm=pen&categoryId=3
        }
    }
}
```

```csharp
[HttpGet]
public IActionResult GetFiltered(
    int skip = 0, int take = 10,
    string searchTerm = null,
    int? categoryId = null)
{
    // Build SQL with dynamic WHERE clause based on provided params
    var sql = "SELECT * FROM Products WHERE 1=1";
    var parameters = new List<SqlParameter>();

    if (!string.IsNullOrEmpty(searchTerm))
    {
        sql += " AND Name LIKE @Search";
        parameters.Add(new SqlParameter("@Search", $"%{searchTerm}%"));
    }
    if (categoryId.HasValue)
    {
        sql += " AND CategoryId = @CatId";
        parameters.Add(new SqlParameter("@CatId", categoryId.Value));
    }

    // Count total (with same filters)
    var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)");
    // ... execute countSql to get totalCount

    // Add paging
    sql += " ORDER BY Id OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
    parameters.Add(new SqlParameter("@Skip", skip));
    parameters.Add(new SqlParameter("@Take", take));

    // ... execute and return { data, total }
}
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 12 — Kendo Dropdowns, AutoComplete & Charts

# ═══════════════════════════════════════════

---

## 12.1 Kendo DropDownList from API

**Definition:**

> A Kendo DropDownList is an enhanced `<select>` element that can load its options from an API, support searching, and use separate display/value fields.

```html
<input id="categoryDropDown" />

<script>
$('#categoryDropDown').kendoDropDownList({

    // DataSource configured to call your API
    dataSource: {
        transport: {
            read: {
                url:      '/api/categories',
                type:     'GET',
                dataType: 'json'
            }
        }
    },

    // dataTextField = which property from JSON to DISPLAY to the user
    dataTextField: 'name',

    // dataValueField = which property from JSON to use as the VALUE
    // (what gets submitted, what you store in DB)
    dataValueField: 'id',

    // Show a placeholder option when nothing is selected
    optionLabel: '-- Select Category --',

    // Event: fires when user selects a different option
    change: function() {
        var selectedId   = this.value();  // The value (id)
        var selectedText = this.text();   // The display text (name)
        console.log('Selected:', selectedId, selectedText);

        // Common use: reload something else based on this selection
        loadSubCategories(selectedId);
    }
});
</script>
```

**API for categories:**

```csharp
[HttpGet]
[Route("api/categories")]
public IActionResult GetCategories()
{
    var list = new List<object>();
    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand("SELECT Id, Name FROM Categories ORDER BY Name", con);
    con.Open();
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
        list.Add(new { id = (int)rdr["Id"], name = rdr["Name"].ToString() });
    return Ok(list);
}
```

---

## 12.2 Cascading Dropdowns (Parent → Child)

A common pattern: selecting a Country loads Cities for that country. Selecting a Category loads its Sub-Categories.

```html
<label>Category:</label>
<input id="ddlCategory" />

<label>Sub-Category:</label>
<input id="ddlSubCategory" />

<script>
// ── Parent: Category ──────────────────────────────────────────────
var ddlCategory = $('#ddlCategory').kendoDropDownList({
    dataSource: {
        transport: { read: { url: '/api/categories', dataType: 'json' } }
    },
    dataTextField:  'name',
    dataValueField: 'id',
    optionLabel: 'Select Category...',

    change: function() {
        var selectedCategoryId = this.value();

        if (selectedCategoryId) {
            // Tell sub-category dropdown to reload WITH the selected category ID
            ddlSubCategory.dataSource.read({ categoryId: selectedCategoryId });
        } else {
            // No category selected — clear sub-categories
            ddlSubCategory.dataSource.data([]);
        }
        ddlSubCategory.value('');  // Reset sub-category selection
    }
}).data('kendoDropDownList');

// ── Child: Sub-Category ───────────────────────────────────────────
var ddlSubCategory = $('#ddlSubCategory').kendoDropDownList({
    autoBind: false,  // DON'T load data on page load — wait for parent selection

    dataSource: {
        transport: {
            read: {
                url:  '/api/subcategories',
                type: 'GET',
                dataType: 'json'
                // data: { categoryId: ... } is passed by ddlCategory's change event
            }
        }
    },
    dataTextField:  'name',
    dataValueField: 'id',
    optionLabel: 'Select Sub-Category...'
}).data('kendoDropDownList');
</script>
```

```csharp
[HttpGet]
[Route("api/subcategories")]
public IActionResult GetSubCategories(int categoryId)
{
    var list = new List<object>();
    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand(
        "SELECT Id, Name FROM SubCategories WHERE CategoryId = @CatId ORDER BY Name", con);
    cmd.Parameters.AddWithValue("@CatId", categoryId);
    con.Open();
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
        list.Add(new { id = (int)rdr["Id"], name = rdr["Name"].ToString() });
    return Ok(list);
}
```

---

## 12.3 Kendo AutoComplete (Search-as-You-Type)

**Definition:**

> An AutoComplete shows suggestions below an input field as the user types, loading suggestions from your API based on what was typed.

```html
<label>Search Product:</label>
<input id="productSearch" placeholder="Type to search..." style="width:300px" />
<input type="hidden" id="hidSelectedProductId" />

<script>
$('#productSearch').kendoAutoComplete({

    minLength: 2,   // Don't start searching until at least 2 characters typed
                    // (Prevents API calls on every single keystroke)

    dataSource: {
        transport: {
            read: {
                url:      '/api/products/search',
                type:     'GET',
                dataType: 'json',
                // The 'filter' parameter is automatically sent by AutoComplete
                // It's the text the user has typed so far
                data: function() {
                    return { term: $('#productSearch').val() };
                }
            }
        }
    },

    // Which property to display in the suggestions list
    dataTextField: 'name',

    // Fires when user selects a suggestion from the dropdown
    select: function(e) {
        var selectedProduct = e.dataItem;  // The full object from API
        // Store the selected product's ID in a hidden field
        $('#hidSelectedProductId').val(selectedProduct.id);
        console.log('Selected product ID:', selectedProduct.id, 'Name:', selectedProduct.name);
    }
});
</script>
```

```csharp
[HttpGet("search")]
public IActionResult Search(string term)
{
    if (string.IsNullOrEmpty(term) || term.Length < 2)
        return Ok(new List<object>());  // Return empty for very short terms

    var list = new List<object>();
    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand(
        "SELECT TOP 10 Id, Name FROM Products WHERE Name LIKE @Term ORDER BY Name", con);
    cmd.Parameters.AddWithValue("@Term", $"%{term}%");
    con.Open();
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
        list.Add(new { id = (int)rdr["Id"], name = rdr["Name"].ToString() });
    return Ok(list);
}
```

---

## 12.4 Kendo Chart from API

```html
<div id="salesChart" style="height:400px"></div>

<script>
// First, fetch data from API, then build the chart
$.ajax({
    url:  '/api/reports/monthly-sales',
    type: 'GET',
    success: function(data) {
        // data = [ {month:"Jan", sales:15000}, {month:"Feb", sales:22000}, ... ]

        $('#salesChart').kendoChart({
            title: { text: 'Monthly Sales - ' + new Date().getFullYear() },

            legend: { position: 'bottom' },

            series: [
                {
                    type: 'column',               // Bar chart (column = vertical bars)
                    data: data.map(d => d.sales), // Extract just the values array
                    name: 'Sales',
                    color: '#3498db'
                }
            ],

            categoryAxis: {
                categories: data.map(d => d.month),  // X-axis labels
                title: { text: 'Month' }
            },

            valueAxis: {
                labels: { format: '${0:N0}' },       // Y-axis: dollar format
                title: { text: 'Sales Amount' }
            },

            tooltip: {
                visible: true,
                format:  '${0:N0}'   // Show dollar format in hover tooltip
            }
        });
    }
});
</script>
```

```csharp
[HttpGet("monthly-sales")]
[Route("api/reports/monthly-sales")]
public IActionResult GetMonthlySales()
{
    var result = new List<object>();
    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand(@"
        SELECT
            FORMAT(OrderDate, 'MMM') AS Month,
            MONTH(OrderDate)         AS MonthNum,
            SUM(TotalAmount)         AS Sales
        FROM Orders
        WHERE YEAR(OrderDate) = YEAR(GETDATE())
        GROUP BY MONTH(OrderDate), FORMAT(OrderDate, 'MMM')
        ORDER BY MONTH(OrderDate)", con);  // ORDER BY number to ensure Jan,Feb,Mar not alphabetical

    con.Open();
    using var rdr = cmd.ExecuteReader();
    while (rdr.Read())
        result.Add(new
        {
            month = rdr["Month"].ToString(),
            sales = (decimal)rdr["Sales"]
        });

    return Ok(result);
}
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 13 — Error Handling

# Catching and Returning Errors Properly

# ═══════════════════════════════════════════

---

## 13.1 Why Error Handling in APIs is Different from MVC

In a traditional MVC app, an unhandled exception shows a yellow error page in the browser (in dev) or a friendly error HTML page in production.

In a Web API, when an exception occurs:

- The response might become an HTML error page (your AJAX/Kendo receives HTML when expecting JSON)
- The AJAX call fails with a confusing error
- The Kendo Grid might show "undefined" or stop working
- Sensitive stack trace information might be exposed

**Goals of API error handling:**

1. Always return JSON — never HTML — even when an error occurs
2. Return the right HTTP status code
3. Include a helpful message for the client (not sensitive details)
4. Log the full error details on the server (for your debugging)

---

## 13.2 Option 1: Try-Catch in Each Action (Simple but Repetitive)

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    try
    {
        var product = _repo.GetById(id);
        if (product == null) return NotFound(new { message = $"Product {id} not found" });
        return Ok(product);
    }
    catch (SqlException sqlEx)
    {
        // SQL Server specific errors — log them
        Console.WriteLine($"[Database Error] SqlException #{sqlEx.Number}: {sqlEx.Message}");
        return StatusCode(500, new { message = "A database error occurred. Please try again." });
        // Note: Don't return sqlEx.Message to clients — it may contain sensitive DB info
    }
    catch (Exception ex)
    {
        // Unexpected errors
        Console.WriteLine($"[Error] {ex}");
        return StatusCode(500, new { message = "An unexpected error occurred." });
    }
}
```

**Problem:** You'd need to copy this try-catch into every action method. That's repetitive.

---

## 13.3 Option 2: Global Exception Middleware (Best Practice)

**Definition:**

> Exception handling middleware is a piece of middleware that wraps the entire request pipeline in a try-catch. Any unhandled exception anywhere in your app is caught here, logged, and turned into a proper JSON error response.

```csharp
// GlobalExceptionMiddleware.cs

public class GlobalExceptionMiddleware
{
    // RequestDelegate = the next middleware in the pipeline
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    // This method is called for EVERY HTTP request
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next middleware (and ultimately your controller action)
            await _next(context);
        }
        catch (Exception ex)
        {
            // Any unhandled exception anywhere in the pipeline ends up here
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Always return JSON (even for errors)
        context.Response.ContentType = "application/json";

        // Determine the HTTP status code based on exception type
        int statusCode;
        string message;

        if (ex is ArgumentNullException || ex is ArgumentException)
        {
            statusCode = 400;
            message = ex.Message;
        }
        else if (ex is UnauthorizedAccessException)
        {
            statusCode = 401;
            message = "You are not authorized to perform this action";
        }
        else if (ex is KeyNotFoundException)
        {
            statusCode = 404;
            message = ex.Message;
        }
        else if (ex is InvalidOperationException)
        {
            statusCode = 409;
            message = ex.Message;
        }
        else if (ex is SqlException sqlEx)
        {
            statusCode = 500;
            // SQL Error 2627 = unique constraint violation (duplicate)
            // SQL Error 547 = foreign key violation
            message = sqlEx.Number switch
            {
                2627 => "A record with this value already exists",
                547  => "Cannot delete — this record is referenced by other data",
                _    => "A database error occurred"
            };
        }
        else
        {
            statusCode = 500;
            message = "An unexpected error occurred. Please try again.";
        }

        context.Response.StatusCode = statusCode;

        // The error response we send to the client
        var errorResponse = new
        {
            success    = false,
            status     = statusCode,
            message    = message,
            // Only include stack trace in Development — never in Production!
            stackTrace = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                         == "Development" ? ex.ToString() : null,
            timestamp  = DateTime.UtcNow
        };

        var json = System.Text.Json.JsonSerializer.Serialize(errorResponse,
            new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(json);
    }
}
```

**Register in Program.cs — MUST be the FIRST middleware:**

```csharp
var app = builder.Build();

// This MUST come first — it wraps everything else
app.UseMiddleware<GlobalExceptionMiddleware>();

// Then all other middleware...
app.UseHttpsRedirection();
app.UseStaticFiles();
// ...
```

---

## 13.4 Custom Business Exception Classes

Instead of throwing generic exceptions, create specific ones that carry meaning:

```csharp
// Exceptions/AppExceptions.cs

// Thrown when a requested resource doesn't exist
public class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with ID '{key}' was not found.") { }
}

// Thrown when a business rule is violated
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

// Thrown when there's a duplicate
public class DuplicateException : Exception
{
    public DuplicateException(string fieldName, object value)
        : base($"A record with {fieldName} = '{value}' already exists.") { }
}

// Usage in repository:
public Product GetById(int id)
{
    var product = FetchFromDb(id);
    if (product == null)
        throw new NotFoundException("Product", id);  // Clear, descriptive
    return product;
}

public int Create(Product product)
{
    if (ProductCodeExistsInDb(product.ProductCode))
        throw new DuplicateException("ProductCode", product.ProductCode);
    // ...
}

// In GlobalExceptionMiddleware, handle these:
else if (ex is NotFoundException)    { statusCode = 404; message = ex.Message; }
else if (ex is DuplicateException)   { statusCode = 409; message = ex.Message; }
else if (ex is BusinessRuleException){ statusCode = 422; message = ex.Message; }
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 14 — Filters

# Running Code Before/After Every Action

# ═══════════════════════════════════════════

---

## 14.1 What Are Filters?

**Definition:**

> Filters are components in ASP.NET Core's request processing pipeline that run code at specific stages around your action method — before it runs, after it runs, when an exception occurs, or around the result. They let you apply cross-cutting concerns (things that apply to many actions) in one place instead of repeating code in every action.

**Cross-cutting concerns examples:**

- Logging every API call (who called what, when)
- Checking if a custom API key is valid
- Recording execution time of each action
- Transforming the response before it's sent

---

## 14.2 The Filter Pipeline — Order of Execution

```
HTTP Request arrives
        │
   ┌────▼──────────────────────────────────┐
   │ [Authorization Filter]                │  ← Are they allowed at all?
   │ OnAuthorization()                     │     Runs first, can short-circuit
   └────┬──────────────────────────────────┘
        │
   ┌────▼──────────────────────────────────┐
   │ [Action Filter]                       │  ← Before action
   │ OnActionExecuting()                   │     Can inspect/modify request
   └────┬──────────────────────────────────┘
        │
   ┌────▼──────────────────────────────────┐
   │ YOUR ACTION METHOD RUNS HERE          │
   │ GetById(5)                            │
   └────┬──────────────────────────────────┘
        │
   ┌────▼──────────────────────────────────┐
   │ [Action Filter]                       │  ← After action
   │ OnActionExecuted()                    │     Can inspect/modify response
   └────┬──────────────────────────────────┘
        │                    ← If exception: [Exception Filter] handles it
   ┌────▼──────────────────────────────────┐
   │ [Result Filter]                       │  ← Around result execution
   │ OnResultExecuting/Executed()          │
   └────┬──────────────────────────────────┘
        │
   Response sent to client
```

---

## 14.3 Action Filter — Log Every API Call

```csharp
// Filters/ApiLoggingFilter.cs

// IActionFilter has two methods:
// OnActionExecuting — runs BEFORE the action
// OnActionExecuted  — runs AFTER the action
public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;
    private Stopwatch _timer;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    // Called BEFORE the action method runs
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _timer = Stopwatch.StartNew();  // Start measuring time

        _logger.LogInformation(
            "[API] {Method} {Path} | User: {User} | IP: {IP}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            context.HttpContext.User.Identity?.Name ?? "Anonymous",
            context.HttpContext.Connection.RemoteIpAddress);
    }

    // Called AFTER the action method runs
    public void OnActionExecuted(ActionExecutedContext context)
    {
        _timer.Stop();

        // Get the HTTP status code from the result
        int statusCode = (context.Result as ObjectResult)?.StatusCode
                      ?? (context.Result as StatusCodeResult)?.StatusCode
                      ?? 200;

        _logger.LogInformation(
            "[API] Completed {Path} | Status: {Status} | Duration: {Ms}ms",
            context.HttpContext.Request.Path,
            statusCode,
            _timer.ElapsedMilliseconds);

        // Flag slow requests
        if (_timer.ElapsedMilliseconds > 1000)
        {
            _logger.LogWarning("[SLOW API] {Path} took {Ms}ms",
                context.HttpContext.Request.Path, _timer.ElapsedMilliseconds);
        }
    }
}

// Register GLOBALLY in Program.cs — applies to ALL controllers and actions:
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ApiLoggingFilter>();
});
// Also register the filter itself so DI can create it:
builder.Services.AddScoped<ApiLoggingFilter>();
```

---

## 14.4 Custom Attribute Filter — API Key Validation

```csharp
// Filters/RequireApiKeyAttribute.cs

// By inheriting Attribute AND IActionFilter, this can be used as [RequireApiKey]
public class RequireApiKeyAttribute : Attribute, IActionFilter
{
    private const string API_KEY_HEADER = "X-Api-Key";
    private const string VALID_API_KEY  = "my-super-secret-api-key-123";
    // In real apps, get this from IConfiguration/appsettings.json

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if the header exists
        if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER, out var providedKey))
        {
            // Header not present at all
            context.Result = new UnauthorizedObjectResult(new
            {
                success = false,
                message = $"API Key is required. Send header: {API_KEY_HEADER}: your-key"
            });
            return;  // Short-circuit: action method will NOT run
        }

        // Check if the key is correct
        if (providedKey != VALID_API_KEY)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                success = false,
                message = "Invalid API Key"
            });
            return;  // Short-circuit
        }

        // Key is valid — let the action run
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
    // Nothing to do after the action
}

// Usage — apply to any controller or action:

[RequireApiKey]   // ← Applied to entire controller — all actions require API key
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase { }

// OR on a specific action only:
[HttpGet("sensitive-data")]
[RequireApiKey]    // ← Only this action requires the key
public IActionResult GetSensitiveData() { }
```

---

## 14.5 Exception Filter — Catch Unhandled Exceptions

```csharp
// Filters/GlobalExceptionFilter.cs

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        => _logger = logger;

    // OnException is called when an unhandled exception escapes an action method
    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception;
        _logger.LogError(ex, "Unhandled exception in {Action}",
            context.ActionDescriptor.DisplayName);

        // Set the response
        context.Result = new ObjectResult(new
        {
            success = false,
            message = "An error occurred processing your request",
            error   = ex.Message  // Be careful: may contain sensitive info in production
        })
        { StatusCode = 500 };

        // Mark as handled so ASP.NET Core doesn't also try to handle it
        context.ExceptionHandled = true;
    }
}

// Register globally:
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
builder.Services.AddScoped<GlobalExceptionFilter>();
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 15 — CORS

# Allowing Cross-Origin Requests

# ═══════════════════════════════════════════

---

## 15.1 What is CORS and Why Does It Exist?

**Definition:**

> CORS (Cross-Origin Resource Sharing) is a browser security mechanism that prevents JavaScript running on one domain from making HTTP requests to a different domain — unless the server explicitly allows it.

**What is an "origin"?**
An origin is the combination of: **protocol + domain + port**

```
https://myapp.com          ← one origin
https://api.myapp.com      ← different origin (different subdomain)
http://myapp.com           ← different origin (different protocol)
https://myapp.com:3000     ← different origin (different port)
```

**The CORS Problem:**

```
SCENARIO:
  Your MVC site:   https://www.mycompany.com      (served by Server A)
  Your API:        https://api.mycompany.com      (served by Server B)

  JavaScript on www.mycompany.com tries to call api.mycompany.com

  BROWSER says: "Different origin! I'm going to block this!"
  → AJAX fails with: "Access to fetch at 'api...' from origin 'www...' has been blocked by CORS policy"

WHY? Browser security. Without CORS, a malicious website could make API calls
     using your logged-in user's session cookies without their knowledge.
```

**The Solution:**
The API server adds response headers telling the browser:

> "I explicitly allow requests from https://www.mycompany.com"

The browser sees these headers and allows the request.

**Important note:** CORS is enforced by the **browser**, not the server. Direct server-to-server calls are not affected by CORS.

---

## 15.2 Configuring CORS in Program.cs

```csharp
// Program.cs

builder.Services.AddCors(options =>
{
    // ── Policy for Production: specific, secure ──────────────────
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy
            // Only allow requests from these exact origins:
            .WithOrigins(
                "https://www.mycompany.com",
                "https://app.mycompany.com",
                "https://admin.mycompany.com")

            // Allow any HTTP method (GET, POST, PUT, DELETE, OPTIONS)
            .AllowAnyMethod()

            // Allow any request header (Content-Type, Authorization, etc.)
            .AllowAnyHeader()

            // Allow cookies and Authorization headers to be sent
            // Needed if you use cookie-based auth or send JWT in header
            .AllowCredentials();
    });

    // ── Policy for Development: open, permissive ─────────────────
    // WARNING: Never use this in production — allows ANY website to call your API!
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()    // Any domain can call the API
            .AllowAnyMethod()
            .AllowAnyHeader();
            // Note: Can't use AllowCredentials with AllowAnyOrigin
    });
});

var app = builder.Build();

// Apply CORS middleware BEFORE routing
// The policy name must match what you defined above
if (app.Environment.IsDevelopment())
    app.UseCors("DevelopmentPolicy");
else
    app.UseCors("ProductionPolicy");

app.UseRouting();
// rest of middleware...
```

---

## 15.3 CORS on Specific Endpoints

```csharp
// Apply to entire controller (overrides global policy):
[EnableCors("ProductionPolicy")]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase { }

// Apply to specific action:
[HttpGet("public-data")]
[EnableCors("DevelopmentPolicy")]  // This action is more open
public IActionResult GetPublicData() { }

// Disable CORS for specific action (even if global policy allows it):
[HttpGet("internal-only")]
[DisableCors]
public IActionResult GetInternalData() { }
```

---

## 15.4 CORS with Preflight Requests

For non-simple requests (POST with JSON, requests with custom headers), browsers first send an **OPTIONS** request (called a "preflight") to ask:

> "Hey server, am I allowed to make this cross-origin request?"

Your server must respond to OPTIONS requests with the CORS headers. ASP.NET Core's CORS middleware handles this automatically when configured correctly.

```
Browser wants to POST JSON to api.mycompany.com from www.mycompany.com:

Step 1: Browser sends:
        OPTIONS /api/products HTTP/1.1
        Origin: https://www.mycompany.com
        Access-Control-Request-Method: POST
        Access-Control-Request-Headers: Content-Type

Step 2: Server responds (CORS middleware handles this):
        HTTP/1.1 204 No Content
        Access-Control-Allow-Origin: https://www.mycompany.com
        Access-Control-Allow-Methods: GET, POST, PUT, DELETE
        Access-Control-Allow-Headers: Content-Type, Authorization

Step 3: Browser allows the actual POST request to proceed

The CORS middleware in ASP.NET Core automatically responds to OPTIONS requests.
You don't need to write any code for this.
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 16 — Authentication & Authorization

# Securing Your API with JWT

# ═══════════════════════════════════════════

---

## 16.1 Authentication vs Authorization — Key Difference

These two words are often confused. They mean different things:

**Authentication** = *Who are you?*

> The process of verifying someone's identity. Are you really who you claim to be?
> Example: Entering your username and password → system verifies it → "Yes, you are John Smith"

**Authorization** = *What are you allowed to do?*

> The process of checking if an authenticated user has permission for a specific action.
> Example: John Smith is authenticated, but is he an Admin? Can he delete products?

```
                    ┌─────────────────┐
Request arrives →   │ Authentication  │  →  "You are John Smith" (or 401 if not)
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │  Authorization  │  →  "John is Admin — allowed" (or 403 if not)
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │  Your Action    │  →  Runs and returns data
                    └─────────────────┘
```

---

## 16.2 What is JWT?

**Definition:**

> JWT (JSON Web Token, pronounced "jot") is a compact, self-contained token format for securely transmitting information between parties. It is the most common authentication method for REST APIs.

**How JWT solves the stateless problem:**

HTTP is stateless — every request is independent. The server doesn't remember the previous request. Traditional websites solve this with sessions (server stores a session, client stores a cookie with the session ID). APIs use JWT instead:

1. Client logs in with credentials
2. Server verifies credentials and generates a JWT token
3. Server sends the token to the client
4. Client stores the token (localStorage, sessionStorage, or memory)
5. Client includes the token in every future request (in the `Authorization` header)
6. Server validates the token on every request — no database lookup needed!

**JWT Structure:**
A JWT looks like this:

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huIiwicm9sZSI6IkFkbWluIiwiZXhwIjoxNjk5OTk5OTk5fQ.abc123signature
```

It has three parts separated by dots:

```
Header.Payload.Signature

Part 1 — Header (base64 encoded):
{
  "alg": "HS256",   ← Algorithm used to sign
  "typ": "JWT"
}

Part 2 — Payload/Claims (base64 encoded):
{
  "sub":        "john",              ← Subject (username)
  "name":       "John Smith",
  "role":       "Admin",             ← User's role
  "department": "Sales",             ← Custom claim
  "exp":        1699999999,          ← Expiry timestamp
  "jti":        "unique-token-id"    ← Token ID (for invalidation)
}

Part 3 — Signature:
HMACSHA256(Base64(header) + "." + Base64(payload), secretKey)
← This signature proves the token hasn't been tampered with
```

The server can verify the signature using the secret key. If anyone modifies the payload, the signature becomes invalid.

---

## 16.3 Setting Up JWT — Step by Step

**Step 1: Install the package:**

```
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**Step 2: Configure in Program.cs:**

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Configure JWT Authentication Service
builder.Services.AddAuthentication(options =>
{
    // Set the default scheme — when [Authorize] is used,
    // use JWT Bearer token authentication
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // These parameters are validated on every incoming request that has a token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the "iss" (issuer) claim matches your app
        ValidateIssuer   = true,
        ValidIssuer      = builder.Configuration["Jwt:Issuer"],

        // Validate the "aud" (audience) claim matches expected audience
        ValidateAudience = true,
        ValidAudience    = builder.Configuration["Jwt:Audience"],

        // Validate that the token hasn't expired
        ValidateLifetime = true,
        ClockSkew        = TimeSpan.Zero,  // No grace period after expiry

        // Validate the signature using our secret key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    // Optional: customize what happens on 401 error
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            if (ctx.Exception is SecurityTokenExpiredException)
                ctx.Response.Headers["Token-Expired"] = "true";
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// In middleware pipeline (ORDER MATTERS):
app.UseAuthentication();   // ← MUST come before UseAuthorization
app.UseAuthorization();
```

**Step 3: Add to appsettings.json:**

```json
"Jwt": {
    "Key":      "ThisIsMyVerySecretKeyForJwtTokensMustBe32CharsOrMore!",
    "Issuer":   "https://myapp.com",
    "Audience": "https://myapp.com",
    "ExpiryHours": 8
}
```

---

## 16.4 Login Endpoint — Generates JWT Token

```csharp
// Controllers/API/AuthController.cs

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly string _conn;

    public AuthController(IConfiguration config)
    {
        _config = config;
        _conn   = config.GetConnectionString("DefaultConnection");
    }

    // POST /api/auth/login
    // Body: { "username": "john", "password": "mypassword" }
    [HttpPost("login")]
    [AllowAnonymous]  // This endpoint must be accessible without being logged in!
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Step 1: Verify the user exists and password is correct
        var user = VerifyCredentials(request.Username, request.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        // Step 2: Generate a JWT token for the verified user
        var token   = GenerateJwtToken(user);
        var expiry  = DateTime.UtcNow.AddHours(
            _config.GetValue<int>("Jwt:ExpiryHours", 8));

        // Step 3: Return the token to the client
        return Ok(new
        {
            token     = token,
            expiresAt = expiry,
            user      = new { user.Id, user.Username, user.Role, user.FullName }
        });
    }

    private UserInfo VerifyCredentials(string username, string password)
    {
        // In a real app, passwords are HASHED — never stored plain text!
        // Use BCrypt.Net: BCrypt.Verify(password, storedHash)
        using var con = new SqlConnection(_conn);
        using var cmd = new SqlCommand(
            @"SELECT Id, Username, Role, FullName, PasswordHash
              FROM Users
              WHERE Username = @Username AND IsActive = 1", con);
        cmd.Parameters.AddWithValue("@Username", username);
        con.Open();
        using var rdr = cmd.ExecuteReader();
        if (!rdr.Read()) return null;

        var storedHash = rdr["PasswordHash"].ToString();

        // Verify password against hash:
        // BCrypt.Net.BCrypt.Verify(password, storedHash)
        // For simplicity here, just compare (replace with BCrypt in real app):
        if (password != storedHash) return null;  // ← Replace with BCrypt

        return new UserInfo
        {
            Id       = (int)rdr["Id"],
            Username = rdr["Username"].ToString(),
            Role     = rdr["Role"].ToString(),
            FullName = rdr["FullName"].ToString()
        };
    }

    private string GenerateJwtToken(UserInfo user)
    {
        // Claims are the pieces of information encoded inside the token
        // The server can read these on every request without a DB lookup
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   // User ID
            new Claim(ClaimTypes.Name,            user.Username),        // Username
            new Claim(ClaimTypes.Role,            user.Role),            // Role (Admin, User, etc.)
            new Claim("fullName",                 user.FullName),        // Custom claim
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // Unique token ID
        };

        // The secret key — must match what's in TokenValidationParameters
        var key   = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        // The signing algorithm
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Build the token
        var token = new JwtSecurityToken(
            issuer:            _config["Jwt:Issuer"],
            audience:          _config["Jwt:Audience"],
            claims:            claims,
            expires:           DateTime.UtcNow.AddHours(_config.GetValue<int>("Jwt:ExpiryHours", 8)),
            signingCredentials: creds);

        // Serialize to the string format: header.payload.signature
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTOs:
public class LoginRequest
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}

public class UserInfo
{
    public int    Id       { get; set; }
    public string Username { get; set; }
    public string Role     { get; set; }
    public string FullName { get; set; }
}
```

---

## 16.5 Protecting API Endpoints

```csharp
// ── Protect entire controller — any authenticated user ─────────────
[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    // All actions here require a valid JWT token

    // ── Allow anonymous access to specific action (even on protected controller) ──
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll()
    {
        // Anyone can see products (no login needed)
        return Ok(_repo.GetAll());
    }

    // ── Require a specific role ────────────────────────────────────
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create([FromBody] Product product)
    {
        // Only users with role "Admin" can create products
        return Ok(_repo.Create(product));
    }

    // ── Require multiple roles (either one) ───────────────────────
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Product product)
    {
        // Admin OR Manager can update
    }

    // ── Read claims from the token inside an action ────────────────
    [Authorize]
    [HttpGet("my-products")]
    public IActionResult GetMyProducts()
    {
        // User is guaranteed to be authenticated here (due to [Authorize])
        // Read information from the JWT token's claims:

        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier);  // "123"
        var username = User.Identity.Name;                               // "john"
        var role     = User.FindFirstValue(ClaimTypes.Role);             // "Admin"
        var fullName = User.FindFirstValue("fullName");                  // "John Smith"

        // Use these to filter data for the current user:
        var products = _repo.GetByCreatedUserId(int.Parse(userId));
        return Ok(products);
    }
}
```

---

## 16.6 AJAX Login and Using JWT Token

```javascript
// ── Login ──────────────────────────────────────────────────────────
function login() {
    $.ajax({
        url:         '/api/auth/login',
        type:        'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            username: $('#txtUsername').val(),
            password: $('#txtPassword').val()
        }),
        success: function(response) {
            // Store the token (in memory or localStorage)
            // For production, consider using httpOnly cookies instead
            localStorage.setItem('jwtToken',   response.token);
            localStorage.setItem('jwtExpiry',  response.expiresAt);
            localStorage.setItem('userName',   response.user.username);
            localStorage.setItem('userRole',   response.user.role);

            // Set global AJAX header for all future requests
            $.ajaxSetup({
                headers: {
                    'Authorization': 'Bearer ' + response.token
                }
            });

            // Redirect to main page
            window.location.href = '/Home/Dashboard';
        },
        error: function(xhr) {
            if (xhr.status === 401) {
                $('#loginError').text('Invalid username or password').show();
            }
        }
    });
}

// ── Set JWT header for all AJAX calls (call on page load) ─────────
function setupAjaxAuth() {
    var token = localStorage.getItem('jwtToken');
    if (token) {
        $.ajaxSetup({
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });
    }
}

// ── Check if token is expired before making calls ─────────────────
function isTokenExpired() {
    var expiry = localStorage.getItem('jwtExpiry');
    if (!expiry) return true;
    return new Date() >= new Date(expiry);
}

// ── Make authenticated API call ────────────────────────────────────
function loadProducts() {
    if (isTokenExpired()) {
        // Redirect to login if token expired
        window.location.href = '/Account/Login';
        return;
    }

    $.ajax({
        url:  '/api/products',
        type: 'GET',
        // Authorization header is set globally by setupAjaxAuth()
        success: function(data) { /* populate grid */ },
        error: function(xhr) {
            if (xhr.status === 401) {
                localStorage.clear();
                window.location.href = '/Account/Login';
            }
        }
    });
}

// ── Logout ────────────────────────────────────────────────────────
function logout() {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('jwtExpiry');
    $.ajaxSetup({ headers: {} });  // Remove the auth header
    window.location.href = '/Account/Login';
}

// ── On page load ──────────────────────────────────────────────────
$(document).ready(function() {
    setupAjaxAuth();  // Set JWT header for all AJAX calls
    loadProducts();   // Load initial data
});
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 17 — Dependency Injection

# Clean, Testable, Maintainable Code

# ═══════════════════════════════════════════

---

## 17.1 What is Dependency Injection?

**Definition:**

> Dependency Injection (DI) is a design pattern where a class receives its dependencies (objects it needs to work) from the outside, rather than creating them itself. ASP.NET Core has a built-in DI container that creates and manages these objects for you.

**The word "dependency":**

> A dependency is any object that a class needs to function. A `ProductsController` depends on `IProductRepository` to get data. `IProductRepository` is a dependency.

**Without DI (bad — tightly coupled):**

```csharp
public class ProductsController : ControllerBase
{
    // Controller creates its own dependency
    private readonly ProductRepository _repo = new ProductRepository();
    // Problems:
    // 1. Hard to test — can't swap with a fake/mock repository
    // 2. Tightly coupled — change the constructor of ProductRepository = change here too
    // 3. Can't share one instance across requests (lifetime management is your problem)
}
```

**With DI (good — loosely coupled):**

```csharp
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;

    // ASP.NET Core's DI container provides the dependency
    public ProductsController(IProductRepository repo)
    {
        _repo = repo;
        // Benefits:
        // 1. Easy to test — inject a mock IProductRepository
        // 2. Loosely coupled — change the implementation, controller stays the same
        // 3. DI manages lifetime — you don't worry about when to create/dispose
    }
}
```

---

## 17.2 How DI Works in ASP.NET Core

```
REGISTRATION (in Program.cs):
"When someone needs IProductRepository, create a ProductRepository"
builder.Services.AddScoped<IProductRepository, ProductRepository>();

                    │
                    ▼

REQUEST ARRIVES:
ProductsController needs to be created
ASP.NET Core's DI container looks at its constructor:
  public ProductsController(IProductRepository repo)
"Oh, it needs IProductRepository. I have a registration for that.
 I'll create a ProductRepository and pass it in."

                    │
                    ▼

CONTROLLER CREATED with dependency injected:
  new ProductsController(new ProductRepository())
  ↑ DI container does this automatically
```

---

## 17.3 Service Lifetimes — Three Options

**Definition:**

> Service Lifetime controls how long a registered service instance lives and how many instances are created.

### Transient — New Instance Every Time

```csharp
builder.Services.AddTransient<IEmailService, EmailService>();

// Every time IEmailService is requested (even within the same request),
// a brand new EmailService instance is created.

// Use for: lightweight services that don't hold state
//          e.g., email senders, validators, formatters
```

### Scoped — One Instance Per HTTP Request

```csharp
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// One instance is created at the beginning of an HTTP request.
// The SAME instance is used for the entire request (even if injected in multiple places).
// Instance is disposed when the request ends.

// Use for: database repositories, unit of work, services that should share
//          state within one request
//          This is the most common lifetime for your repositories
```

### Singleton — One Instance for the Entire Application

```csharp
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<IAppSettings, AppSettings>();

// One instance is created on first use and reused for every request forever.
// Never disposed until the application shuts down.

// Use for: configuration readers, in-memory caches, shared state
//          Be careful: must be thread-safe! Multiple requests use it simultaneously.
```

**Lifetime Decision Chart:**

```
Does this service hold state that should be SHARED across requests? → Singleton
Does this service hold state that should be SHARED within ONE request? → Scoped
Should every usage get a fresh, independent instance? → Transient
Is it a database access class? → Scoped (almost always)
```

---

## 17.4 Registering Services in Program.cs

```csharp
// ── Your own classes ──────────────────────────────────────────────
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// ── When you have multiple implementations ────────────────────────
// For testing, you might register a fake implementation:
// builder.Services.AddScoped<IProductRepository, FakeProductRepository>();

// ── Register a concrete class (no interface) ─────────────────────
// Not recommended (couples you to the implementation) but sometimes necessary:
builder.Services.AddScoped<ProductRepository>();

// ── Register using a factory function ────────────────────────────
// Useful when construction needs logic:
builder.Services.AddScoped<IProductRepository>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<ProductRepository>>();
    return new ProductRepository(config, logger);
});

// ── Register connection string as injectable ──────────────────────
builder.Services.AddSingleton(
    builder.Configuration.GetConnectionString("DefaultConnection"));
// Now you can inject string directly:
// public ProductRepository(string connectionString) { }
```

---

## 17.5 Injecting Multiple Services

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    // All these are injected by DI:
    private readonly IOrderRepository    _orderRepo;
    private readonly IProductRepository  _productRepo;
    private readonly IEmailService       _emailService;
    private readonly ICacheService       _cache;
    private readonly ILogger<OrdersController> _logger;

    // DI container provides all of these automatically
    public OrdersController(
        IOrderRepository orderRepo,
        IProductRepository productRepo,
        IEmailService emailService,
        ICacheService cache,
        ILogger<OrdersController> logger)
    {
        _orderRepo    = orderRepo;
        _productRepo  = productRepo;
        _emailService = emailService;
        _cache        = cache;
        _logger       = logger;
    }

    [HttpPost]
    public IActionResult PlaceOrder([FromBody] OrderRequest req)
    {
        _logger.LogInformation("Placing order for user {UserId}", req.UserId);

        // Check product availability
        var product = _productRepo.GetById(req.ProductId);
        if (product == null || product.StockCount < req.Quantity)
            return BadRequest(new { message = "Insufficient stock" });

        // Create the order
        var orderId = _orderRepo.Create(req);

        // Send confirmation email
        _emailService.SendOrderConfirmation(req.UserEmail, orderId, product.Name);

        // Invalidate cached dashboard data
        _cache.Remove("dashboard-stats");

        return CreatedAtAction(nameof(GetById), new { id = orderId }, new { orderId });
    }
}
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 18 — API Versioning

# Evolving Your API Without Breaking Clients

# ═══════════════════════════════════════════

---

## 18.1 Why Version Your API?

**The problem:**
Once your API is in use (by Kendo grids, mobile apps, third-party clients), you can't just change the response format. If you rename `productName` to `name`, every client that reads `productName` breaks.

**The solution:** Versioning — you keep the old version working and introduce a new version alongside it.

---

## 18.2 Simple URL-Based Versioning (Recommended Approach)

The simplest approach: include the version in the URL.

```csharp
// V1 Controller — original version
[ApiController]
[Route("api/v1/products")]         // ← v1 in the route
public class ProductsV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // Original response format
        return Ok(new
        {
            products = GetProductsFromDb()  // ← wrapped in "products" key
        });
    }
}

// V2 Controller — new improved version
[ApiController]
[Route("api/v2/products")]         // ← v2 in the route
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // New format: richer data, different structure
        return Ok(new
        {
            data  = GetProductsWithImagesFromDb(),  // ← more data
            total = GetProductCount(),
            meta  = new { version = "2.0", generatedAt = DateTime.UtcNow }
        });
    }
}

// URLs:
// GET /api/v1/products → old clients use this (still works!)
// GET /api/v2/products → new clients use this (richer data)
```

---

## 18.3 Deprecating Old Versions

```csharp
[ApiController]
[Route("api/v1/products")]
[Obsolete("V1 is deprecated. Use /api/v2/products instead. Will be removed on 2025-01-01.")]
public class ProductsV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // Add deprecation warning header to responses
        Response.Headers["Deprecation"] = "true";
        Response.Headers["Sunset"]      = "Sat, 01 Jan 2025 00:00:00 GMT";
        Response.Headers["Link"]        = "</api/v2/products>; rel=\"successor-version\"";

        return Ok(/* old format */);
    }
}
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 19 — Swagger / OpenAPI

# Auto-Generated Interactive API Documentation

# ═══════════════════════════════════════════

---

## 19.1 What is Swagger?

**Definition:**

> Swagger (now called OpenAPI) is a specification and toolset for describing and documenting REST APIs. Swagger UI generates an interactive web page from your API code where developers can see all your endpoints, understand what data they need, and test them directly in the browser — without writing any code.

**Why use Swagger?**

- Documentation is generated automatically from your code — no manual docs to maintain
- Other developers (or your own team) can discover and test your API
- Kendo developers on your team can see exactly what JSON each endpoint returns
- Great for testing during development

---

## 19.2 Setting Up Swagger

**Install:**

```
dotnet add package Swashbuckle.AspNetCore
```

**Configure in Program.cs:**

```csharp
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();  // Needed for Swagger
builder.Services.AddSwaggerGen(options =>
{
    // API metadata — appears at the top of Swagger UI
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "My Application API",
        Version     = "v1",
        Description = "API for managing Products, Orders, and Customers",
        Contact     = new OpenApiContact
        {
            Name  = "Development Team",
            Email = "dev@mycompany.com"
        }
    });

    // ── Add JWT Authentication to Swagger UI ─────────────────────
    // This adds an "Authorize" button in Swagger to enter your JWT token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Enter JWT token in format: Bearer {your-token-here}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // ── Include XML comments in Swagger ──────────────────────────
    // Requires: Project Properties → Build → XML Documentation File ✓
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Enable Swagger UI:
var app = builder.Build();

// Show Swagger only in Development (or always, your choice):
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Generates the swagger.json spec
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = "swagger";   // Swagger UI at: /swagger
        // To put it at root (/): c.RoutePrefix = "";
    });
}
```

---

## 19.3 Documenting Endpoints with XML Comments

```csharp
/// <summary>
/// Retrieves all products with optional filtering and pagination.
/// </summary>
/// <param name="category">Filter by category name (optional)</param>
/// <param name="page">Page number (default: 1)</param>
/// <param name="pageSize">Records per page (default: 10, max: 100)</param>
/// <returns>Paginated list of products</returns>
/// <response code="200">Returns the paginated product list</response>
/// <response code="401">User is not authenticated</response>
/// <response code="500">Internal server error</response>
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<List<Product>>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Produces("application/json")]  // Response content type
public IActionResult GetAll(string category = null, int page = 1, int pageSize = 10)
{
    // ...
}

/// <summary>
/// Creates a new product.
/// </summary>
/// <param name="request">Product data to create</param>
/// <returns>The created product with its assigned ID</returns>
/// <response code="201">Product created successfully</response>
/// <response code="400">Validation error in the request data</response>
/// <response code="409">A product with this code already exists</response>
[HttpPost]
[ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public IActionResult Create([FromBody] Product request) { }
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 20 — Best Practices, Patterns & Security

# ═══════════════════════════════════════════

---

## 20.1 Use DTOs — Never Expose Your Database Entities Directly

**Definition:**

> DTO (Data Transfer Object) is a simple class designed specifically for transferring data over the wire (API). It contains only what the client needs — no sensitive fields, no navigation properties, no internal fields.

**Why DTOs?**

1. Security: Don't accidentally expose `PasswordHash`, `InternalNotes`, `CostPrice`
2. Control: Return only the fields the client needs
3. Stability: Client depends on the DTO shape, not the DB entity
4. Separation: DB schema changes don't break the API contract

```csharp
// The database entity (all fields — some sensitive)
public class User
{
    public int    Id             { get; set; }
    public string Username       { get; set; }
    public string PasswordHash   { get; set; }   // ← SENSITIVE
    public string Salt           { get; set; }   // ← SENSITIVE
    public string Email          { get; set; }
    public string Role           { get; set; }
    public string InternalNotes  { get; set; }   // ← SENSITIVE (HR notes)
    public decimal Salary        { get; set; }   // ← SENSITIVE
    public DateTime CreatedAt    { get; set; }
    public bool IsActive         { get; set; }
}

// DTO — only what the API should return (safe)
public class UserDto
{
    public int    Id        { get; set; }
    public string Username  { get; set; }
    public string Email     { get; set; }
    public string Role      { get; set; }
    // No password, salary, internal notes!
}

// DTO for creating a user (what client sends)
public class CreateUserRequest
{
    [Required] public string Username { get; set; }
    [Required][EmailAddress] public string Email { get; set; }
    [Required][MinLength(8)] public string Password { get; set; }
    // No Id (auto-generated), no Role (admin sets it), no PasswordHash (server generates it)
}

// In controller — map entity to DTO:
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    var user = _repo.GetById(id);
    if (user == null) return NotFound();

    // Return DTO — not the entity!
    var dto = new UserDto
    {
        Id       = user.Id,
        Username = user.Username,
        Email    = user.Email,
        Role     = user.Role
    };
    return Ok(dto);
}
```

---

## 20.2 Never Concatenate SQL — Always Use Parameters

**This is the #1 security vulnerability in web applications: SQL Injection**

```csharp
// ❌ DANGEROUS — SQL Injection vulnerability
// If userId is: "1; DROP TABLE Users; --"
// The resulting SQL would be:
// SELECT * FROM Users WHERE Id = 1; DROP TABLE Users; --
var sql = "SELECT * FROM Users WHERE Id = " + userId;
cmd.CommandText = sql;

// ✅ SAFE — Parameterized query
// @Id is a parameter — its value is sent separately, never interpreted as SQL
var sql = "SELECT * FROM Users WHERE Id = @Id";
cmd.Parameters.AddWithValue("@Id", userId);
// Even if userId = "1; DROP TABLE Users; --"
// It's treated as a string value, not SQL code. Your table is safe.
```

---

## 20.3 Meaningful API Naming Conventions

```
✅ CORRECT REST naming:

GET    /api/products              → Get all products
GET    /api/products/5            → Get product #5
GET    /api/products/5/reviews    → Get reviews for product #5
POST   /api/products              → Create a new product
PUT    /api/products/5            → Update product #5 (full update)
PATCH  /api/products/5            → Partially update product #5
DELETE /api/products/5            → Delete product #5
GET    /api/products?cat=Tech     → Filter products by category

❌ INCORRECT (not RESTful):

GET    /api/getProducts           → Verb in URL — wrong!
GET    /api/products/getById/5    → Verb in URL — wrong!
POST   /api/products/create       → "create" is redundant — POST implies create
DELETE /api/deleteProduct?id=5    → Use /api/products/5 with DELETE method
GET    /api/product               → Use plural nouns: /api/products
```

---

## 20.4 Handle All Error Cases — Return Helpful Messages

```csharp
[HttpGet("{id}")]
public IActionResult GetById(int id)
{
    // ✅ Validate input before even hitting the database
    if (id <= 0)
        return BadRequest(new { message = "ID must be a positive integer" });

    var product = _repo.GetById(id);

    // ✅ Handle not found with a helpful message
    if (product == null)
        return NotFound(new { message = $"Product with ID {id} does not exist" });

    // ✅ Success
    return Ok(product);
}

[HttpPost]
public IActionResult Create([FromBody] Product product)
{
    // ✅ Check for business rule violations
    if (ProductCodeAlreadyExists(product.ProductCode))
        return Conflict(new { message = $"Product code '{product.ProductCode}' is already in use" });

    if (!CategoryExists(product.CategoryId))
        return BadRequest(new { message = $"Category ID {product.CategoryId} does not exist" });

    var id = _repo.Create(product);
    product.Id = id;
    return CreatedAtAction(nameof(GetById), new { id }, product);
}
```

---

## 20.5 Async/Await — Best Practice for Database Calls

Always use async database calls in production apps for better performance:

```csharp
// ── The Async Pattern ─────────────────────────────────────────────

// 1. Action method becomes async, returns Task<IActionResult>
[HttpGet]
public async Task<IActionResult> GetAllAsync()
{
    var list = new List<Product>();

    using var con = new SqlConnection(_conn);
    using var cmd = new SqlCommand("SELECT * FROM Products", con);

    await con.OpenAsync();               // Async: thread released while connecting
    using var rdr = await cmd.ExecuteReaderAsync();   // Async: thread released while querying

    while (await rdr.ReadAsync())        // Async: thread released between rows
    {
        list.Add(MapProduct(rdr));
    }

    return Ok(list);
}

// 2. ExecuteScalar, ExecuteNonQuery — also have async versions
var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
int rows  = await cmd.ExecuteNonQueryAsync();
```

---

## 20.6 Security Checklist for Your API

```
✅ SQL Parameters — Never concatenate user input into SQL
✅ HTTPS — Always use HTTPS in production
✅ Input Validation — Use Data Annotations + business rule checks
✅ Authentication — Use JWT with [Authorize] on protected endpoints
✅ Authorization — Use [Authorize(Roles = "...")] for role-based access
✅ DTOs — Never expose entity classes with sensitive fields
✅ Error Messages — Return generic messages to clients, log details server-side
✅ CORS — Allow only specific origins in production
✅ Password Hashing — Never store plaintext passwords (use BCrypt)
✅ Token Expiry — Set reasonable JWT expiry (8 hours for internal apps)
✅ Pagination — Never return all records; always paginate
✅ Rate Limiting — Limit requests per user/IP to prevent abuse
✅ Logging — Log all errors, authentication failures, and suspicious activity
✅ Stack Traces — Never return stack traces in production responses
```

---

---

# ═══════════════════════════════════════════

# CHAPTER 21 — Complete Quick Reference

# Cheat Sheet

# ═══════════════════════════════════════════

---

## 21.1 Controller Attributes Quick Reference

```csharp
// Class-level attributes:
[ApiController]                  // Enable API behaviors (auto-validation, etc.)
[Route("api/[controller]")]      // Base route — [controller] = class name - "Controller"
[Route("api/products")]          // Hardcoded route (explicit, no surprises)
[Authorize]                      // All actions require authentication
[AllowAnonymous]                 // All actions are public (overrides Authorize)

// Action-level attributes:
[HttpGet]                        // Handles GET /api/products
[HttpGet("{id}")]                // Handles GET /api/products/5
[HttpGet("{id:int}")]            // Route constraint: must be integer
[HttpPost]                       // Handles POST /api/products
[HttpPut("{id}")]                // Handles PUT /api/products/5
[HttpPatch("{id}")]              // Handles PATCH /api/products/5
[HttpDelete("{id}")]             // Handles DELETE /api/products/5
[Authorize(Roles = "Admin")]     // Only Admin role allowed
[AllowAnonymous]                 // This action is public
[EnableCors("PolicyName")]       // Apply CORS policy to this action
[ProducesResponseType(200)]      // Document response type for Swagger
```

---

## 21.2 Return Methods Quick Reference

```csharp
// 2xx — Success
return Ok(data);                              // 200 + JSON body
return Created(locationUri, createdObject);   // 201 + body
return CreatedAtAction(nameof(Get), new{id},obj); // 201 + Location header
return NoContent();                           // 204 — no body

// 4xx — Client errors
return BadRequest();                          // 400
return BadRequest("message");                 // 400 + message
return BadRequest(new { message, field });    // 400 + custom JSON
return BadRequest(ModelState);               // 400 + validation errors
return Unauthorized();                        // 401
return Forbid();                              // 403
return NotFound();                            // 404
return NotFound(new { message });             // 404 + message
return Conflict();                            // 409
return Conflict(new { message });             // 409 + message

// 5xx — Server errors
return StatusCode(500, "message");            // 500 + message
return StatusCode(500, new { message, error }); // 500 + custom JSON
```

---

## 21.3 ADO.NET Quick Reference

```csharp
// ── Open connection ───────────────────────────────────────────────
using var con = new SqlConnection(connectionString);
con.Open();  // or: await con.OpenAsync();

// ── Create command ────────────────────────────────────────────────
using var cmd = new SqlCommand("SELECT * FROM T WHERE Id = @Id", con);
cmd.Parameters.AddWithValue("@Id", id);

// ── Stored procedure ──────────────────────────────────────────────
cmd.CommandType = CommandType.StoredProcedure;
cmd.CommandText = "ProcedureName";

// ── Output parameter ──────────────────────────────────────────────
var outParam = new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output };
cmd.Parameters.Add(outParam);
cmd.ExecuteNonQuery();
int newId = (int)outParam.Value;

// ── Execute and read rows ─────────────────────────────────────────
using var rdr = cmd.ExecuteReader();  // or: await cmd.ExecuteReaderAsync();
while (rdr.Read())  // or: await rdr.ReadAsync()
{
    var id   = (int)rdr["Id"];
    var name = rdr["Name"].ToString();
    var nullableField = rdr["Field"] == DBNull.Value ? null : rdr["Field"].ToString();
}

// ── Execute returning single value ────────────────────────────────
int count = (int)cmd.ExecuteScalar();  // or: await cmd.ExecuteScalarAsync();
int newId = Convert.ToInt32(cmd.ExecuteScalar());

// ── Execute INSERT/UPDATE/DELETE ──────────────────────────────────
int rowsAffected = cmd.ExecuteNonQuery();  // or: await cmd.ExecuteNonQueryAsync();

// ── Transaction ───────────────────────────────────────────────────
using var tx = con.BeginTransaction();
cmd.Transaction = tx;
try { cmd.ExecuteNonQuery(); tx.Commit(); }
catch { tx.Rollback(); }
```

---

## 21.4 AJAX Quick Reference

```javascript
// GET — read data
$.ajax({ url: '/api/products', type: 'GET',
    success: d => console.log(d),
    error: xhr => console.error(xhr.status, xhr.responseJSON)
});

// GET with query string — /api/products?cat=Electronics&page=2
$.ajax({ url: '/api/products', type: 'GET',
    data: { cat: 'Electronics', page: 2 },
    success: d => console.log(d)
});

// POST — create (MUST have contentType and JSON.stringify)
$.ajax({ url: '/api/products', type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify({ name: 'Pen', price: 5, category: 'Stationery' }),
    success: created => console.log('New ID:', created.id),
    error: xhr => {
        if (xhr.status === 400) console.log('Validation:', xhr.responseJSON.errors);
    }
});

// PUT — full update
$.ajax({ url: '/api/products/' + id, type: 'PUT',
    contentType: 'application/json',
    data: JSON.stringify({ id, name, price, category }),
    success: () => console.log('Updated'),
    error: xhr => console.error(xhr.status)
});

// DELETE
$.ajax({ url: '/api/products/' + id, type: 'DELETE',
    success: () => console.log('Deleted'),
    error: xhr => { if (xhr.status === 404) alert('Not found'); }
});

// Global JWT setup (call once on page load):
$.ajaxSetup({ headers: { 'Authorization': 'Bearer ' + localStorage.getItem('jwtToken') } });

// Global error handler:
$(document).ajaxError(function(e, xhr) {
    if (xhr.status === 401) window.location.href = '/Login';
    if (xhr.status === 500) alert('Server error. Please try again.');
});
```

---

## 21.5 Kendo Grid DataSource Quick Reference

```javascript
var ds = new kendo.data.DataSource({
    serverPaging: true,
    transport: {
        read:    { url: '/api/products',                  type: 'GET',    dataType: 'json' },
        create:  { url: '/api/products',                  type: 'POST',   contentType: 'application/json', dataType: 'json' },
        update:  { url: item => '/api/products/' + item.id, type: 'PUT',    contentType: 'application/json', dataType: 'json' },
        destroy: { url: item => '/api/products/' + item.id, type: 'DELETE' },
        parameterMap: function(data, op) {
            if (op === 'read') return data;
            return JSON.stringify(data);  // Send body as JSON for POST/PUT
        }
    },
    schema: {
        data:  'data',    // Property name in JSON that has the array
        total: 'total',   // Property name in JSON that has the count
        model: {
            id: 'id',
            fields: {
                id:       { type: 'number',  editable: false },
                name:     { type: 'string',  validation: { required: true } },
                price:    { type: 'number',  validation: { required: true, min: 0.01 } },
                category: { type: 'string',  validation: { required: true } },
                isActive: { type: 'boolean' }
            }
        }
    },
    pageSize: 10,
    error: function(e) { this.cancelChanges(); alert('Error: ' + e.xhr?.responseJSON?.message); }
});

// Useful grid operations:
var grid = $('#myGrid').data('kendoGrid');
grid.dataSource.read();         // Reload data from API
grid.addRow();                  // Open add form
grid.dataItem(grid.select());   // Get selected row data
```

---

## 21.6 Common Mistakes and Fixes

| #  | Mistake                                                  | What Goes Wrong                            | The Fix                                                 |
| -- | -------------------------------------------------------- | ------------------------------------------ | ------------------------------------------------------- |
| 1  | Missing `contentType: 'application/json'` in AJAX POST | `[FromBody]` receives null               | Add `contentType: 'application/json'`                 |
| 2  | Missing `JSON.stringify(data)` in AJAX POST            | Body is `"[object Object]"`              | Wrap with `JSON.stringify(data)`                      |
| 3  | Inheriting `Controller` for API                        | Wastes memory loading View infrastructure  | Inherit `ControllerBase` instead                      |
| 4  | Hardcoding user input in SQL string                      | SQL Injection vulnerability                | Always use `@Parameter`                               |
| 5  | Not using `using` with SqlConnection                   | Connection leak — runs out of connections | Always:`using var con = new SqlConnection(...)`       |
| 6  | Returning entity with nav properties                     | Circular reference JSON error              | Use DTOs or configure `ReferenceHandler.IgnoreCycles` |
| 7  | Kendo `schema.data` wrong property name                | Grid shows 0 rows or error                 | Match to your actual JSON property name                 |
| 8  | CORS not configured                                      | AJAX blocked by browser                    | Add `AddCors()` + `UseCors()` in Program.cs         |
| 9  | `UseAuthentication` after `UseAuthorization`         | Auth doesn't work                          | Authentication MUST come before Authorization           |
| 10 | Storing plain text passwords                             | Security breach exposes all passwords      | Use BCrypt:`BCrypt.HashPassword(password)`            |
| 11 | Not paginating results                                   | 100,000 row API response crashes browser   | Always paginate with OFFSET/FETCH                       |
| 12 | Returning 200 for created resources                      | Doesn't follow REST convention             | Return 201 `CreatedAtAction()` for POST               |
| 13 | JWT `[Authorize]` but no `UseAuthentication()`       | All requests get 401 even with valid token | Add `app.UseAuthentication()` in Program.cs           |
| 14 | `[ApiController]` on MVC controller                    | View() returns weird responses             | Use `[ApiController]` only on `ControllerBase`      |
| 15 | Route parameter not matching method parameter name       | Parameter is always 0 or null              | `{id}` in route must match `int id` in method       |

---

## 21.7 The Request Lifecycle — Full Picture

```
Browser / Kendo / AJAX Client
         │
         │  HTTP Request: GET /api/products/5
         ▼
   ┌─────────────────────────────┐
   │    GlobalExceptionMiddleware │  ← Wraps everything in try-catch
   │    (your custom middleware)  │
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    UseHttpsRedirection       │  ← Redirect HTTP → HTTPS
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    UseStaticFiles            │  ← Serve wwwroot files? Done here
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    UseRouting                │  ← Match URL → Controller + Action
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    UseCors                   │  ← Add CORS headers
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    UseAuthentication         │  ← Read JWT, identify user
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    UseAuthorization          │  ← Check if user has permission
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    Action Filter (before)    │  ← Log, validate API key, etc.
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    ProductsController        │
   │    .GetById(5)               │
   │         │                   │
   │    ADO.NET → SQL Server      │
   │    Map rows → List<Product>  │
   │    return Ok(product)        │
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    Action Filter (after)     │  ← Log response time
   └────────────┬────────────────┘
                │
   ┌────────────▼────────────────┐
   │    JSON Serialization        │  ← Product → { "id":5, "name":"Pen", ... }
   └────────────┬────────────────┘
                │
         ▼
   HTTP Response: 200 OK
   Body: {"id":5,"name":"Pen","price":5.0,"category":"Stationery"}
         │
         ▼
   $.ajax success callback
   OR Kendo DataSource processes the JSON
   OR Grid displays the data
```

---

*This guide covers everything you need to build professional ASP.NET Core Web APIs integrated with Kendo UI, AJAX, and ADO.NET. Every concept has been explained from first principles so no prior API knowledge is required.*

*Bookmark this document and use Ctrl+F to find any concept when you get stuck.*

---

**End of Guide — Happy Coding! 🚀**
