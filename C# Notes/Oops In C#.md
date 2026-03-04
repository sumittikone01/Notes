# 🔷 C# Object-Oriented Programming — Complete Notes

## 📑 Table of Contents

1. [Classes &amp; Objects](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#1-classes--objects)
2. [Constructors &amp; Constructor Overloading](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#2-constructors--constructor-overloading)
3. [Properties — The C# Way of Encapsulation](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#3-properties--the-c-way-of-encapsulation)
4. [Access Modifiers](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#4-access-modifiers)
5. [Static Members](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#5-static-members)
6. [this Keyword](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#6-this-keyword)
7. [Inheritance](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#7-inheritance)
8. [Method Overloading — Compile-Time Polymorphism](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#8-method-overloading--compile-time-polymorphism)
9. [Method Overriding — Runtime Polymorphism](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#9-method-overriding--runtime-polymorphism)
10. [Abstract Classes](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#10-abstract-classes)
11. [Interfaces](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#11-interfaces)
12. [sealed Keyword](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#12-sealed-keyword)
13. [readonly &amp; const](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#13-readonly--const)
14. [Type Casting — Upcasting &amp; Downcasting](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#14-type-casting--upcasting--downcasting)
15. [struct vs class](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#15-struct-vs-class)
16. [C# Extras — Record, Partial Class, Nullable](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#16-c-extras--record-partial-class-nullable)
17. [Full Comparison: Java OOP vs C# OOP](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#17-full-comparison-java-oop-vs-c-oop)

---

## 1. Classes & Objects

### Definition

* **Class** = Blueprint of an Object. Defines what data (fields/properties) and behavior (methods) an object will have.
* **Object** = Instance of a class. A real thing created from the blueprint.

### Java vs C# — Syntax Comparison

```
JAVA                                    C#
────────────────────────────────────    ────────────────────────────────────
public class Car {                      public class Car {
    String color;                           public string Color;
    int speed;                              public int Speed;

    void drive() {                          public void Drive() {
        System.out.println("Vroom");            Console.WriteLine("Vroom");
    }                                       }
}                                       }

Car c1 = new Car();                     Car c1 = new Car();
c1.color = "Red";                       c1.Color = "Red";
```

### Key Differences from Java

```
Java                        C#
──────────────────────      ──────────────────────────────
System.out.println()        Console.WriteLine()
String (capital S)          string (lowercase, alias)
int, double, bool           int, double, bool  ← same
No built-in properties      Properties (get/set) — C# exclusive
Packages (com.example)      Namespaces (MyApp.Models)
.java file → .class         .cs file → compiled by .NET
```

### C# Object Creation

```csharp
// Namespace = C# version of Java's package
namespace MyApp.Models
{
    public class Person
    {
        // Fields (same as Java member variables)
        public string Name;
        public int Age;

        // Method
        public void Greet()
        {
            Console.WriteLine($"Hello, I am {Name}");  // $ = string interpolation
        }
    }
}

// Creating objects
Person p1 = new Person();
p1.Name = "Sumit";
p1.Age = 22;
p1.Greet();   // Output: Hello, I am Sumit

// var keyword (type inferred at compile time — still strongly typed)
var p2 = new Person();   // compiler knows p2 is of type Person
```

### Object Diagram

```
Person class (Blueprint)             p1 object (in memory)
───────────────────────              ───────────────────────
  Name : string                        Name = "Sumit"
  Age  : int                           Age  = 22
  Greet() method                       Greet() → "Hello, I am Sumit"
```

### Key Points

```
✔ Method names in C# use PascalCase:  Drive(), GetName(), CalculateSalary()
✔ Variable names use camelCase:       name, empId, totalSalary
✔ String is lowercase alias in C#:    string == System.String
✔ Console.WriteLine() replaces System.out.println()
✔ $ prefix for string interpolation:  $"Hello {name}" replaces "Hello "+name
✔ No 'package' keyword — use 'namespace'
```

---

## 2. Constructors & Constructor Overloading

### Definition

* **Constructor** = Special method that runs automatically at the time of object creation. Used to initialize the object's state.
* Same rules as Java: same name as class, no return type (not even void).

### C# Constructor Syntax

```csharp
public class Robot
{
    public string Name;
    public int Speed;
    public double Memory;

    // Constructor — same name as class, no return type
    public Robot(string name, int speed, double memory)
    {
        this.Name   = name;       // 'this' works exactly like Java
        this.Speed  = speed;
        this.Memory = memory;
    }

    public void Details()
    {
        Console.WriteLine($"{Name} | Speed: {Speed} kmph | Memory: {Memory} TB");
    }
}

// Usage
Robot r1 = new Robot("Chitti", 200, 1.5);
r1.Details();   // Chitti | Speed: 200 kmph | Memory: 1.5 TB
```

### Constructor Overloading

```csharp
public class Student
{
    public string Name;
    public int Roll;
    public string Course;

    // Constructor 1 — no params (default)
    public Student()
    {
        Name   = "Unknown";
        Roll   = 0;
        Course = "N/A";
    }

    // Constructor 2 — name + roll only
    public Student(string name, int roll)
    {
        this.Name   = name;
        this.Roll   = roll;
        this.Course = "N/A";
    }

    // Constructor 3 — all params
    public Student(string name, int roll, string course)
    {
        this.Name   = name;
        this.Roll   = roll;
        this.Course = course;
    }
}

Student s1 = new Student();                         // constructor 1
Student s2 = new Student("Sumit", 21);              // constructor 2
Student s3 = new Student("Akash", 22, "C#.NET");    // constructor 3
```

### Constructor Chaining with : this() — C# Exclusive

In Java you call `this()` inside the constructor body.
In C# you chain in the **method signature** using `: this(...)`.

```csharp
public class Employee
{
    public string Name;
    public int EmpId;
    public double Salary;

    // Full constructor
    public Employee(string name, int empId, double salary)
    {
        this.Name   = name;
        this.EmpId  = empId;
        this.Salary = salary;
    }

    // Calls full constructor — passes default salary
    public Employee(string name, int empId) : this(name, empId, 50000)
    {
        // No need to repeat — this() handles it
    }

    // Calls above — passes default empId too
    public Employee(string name) : this(name, 100)
    {
        // Chain continues up
    }
}

Employee e1 = new Employee("Sumit");            // Name=Sumit, EmpId=100, Salary=50000
Employee e2 = new Employee("Akash", 102);       // Name=Akash, EmpId=102, Salary=50000
Employee e3 = new Employee("Rahul", 103, 75000);// all three set explicitly
```

### Constructor Chain Diagram

```
new Employee("Sumit")
       │
       └──► Employee(string name) : this(name, 100)
                    │
                    └──► Employee(string name, int empId) : this(name, empId, 50000)
                                    │
                                    └──► Employee(string, int, double)
                                              ↑  Final initialization here
```

### Base Constructor Call : base() — Replaces Java's super()

```csharp
public class Person
{
    public string Name;
    public int Age;

    public Person(string name, int age)
    {
        this.Name = name;
        this.Age  = age;
    }
}

public class Student : Person   // ':' replaces 'extends'
{
    public int Roll;
    public string College;

    public Student(string name, int age, int roll, string college)
        : base(name, age)       // calls Person constructor — replaces super()
    {
        this.Roll    = roll;
        this.College = college;
    }
}
```

### Key Points

```
Java                       C#
──────────────────         ──────────────────────────────────
super(args)                : base(args)  ← in signature, NOT in body
this(args)                 : this(args)  ← in signature, NOT in body
Inside constructor body    In constructor header — BIG syntax difference
```

---

## 3. Properties — The C# Way of Encapsulation

> **This is the BIGGEST difference from Java. Properties are purely C#.**

### Java Approach (Getter/Setter methods)

```java
// Java Bean — verbose
private String name;
public String getName() { return name; }
public void setName(String name) { this.name = name; }
```

### C# Property Syntax

```csharp
// C# Property — looks like a field, behaves like a method
private string _name;        // backing field (convention: underscore prefix)

public string Name           // Property — PascalCase
{
    get { return _name; }         // getter
    set { _name = value; }        // setter — 'value' = incoming data
}
```

### Auto-Implemented Property — Most Common Style

```csharp
public class Person
{
    // Auto-property — compiler creates backing field automatically
    public string Name    { get; set; }   // read + write
    public int    Age     { get; set; }
    public string Email   { get; set; }

    // Read-only — can only be set in constructor
    public int    EmpId   { get; }

    // Init-only — set only at object creation (C# 9+)
    public string Country { get; init; }
}

var p = new Person();
p.Name = "Sumit";               // uses setter
Console.WriteLine(p.Name);      // uses getter

// Object Initializer syntax
var p2 = new Person { Name = "Akash", Age = 22, Email = "a@b.com" };
```

### Property with Validation

```csharp
public class BankAccount
{
    private double _balance;

    public double Balance
    {
        get { return _balance; }
        set
        {
            if (value >= 0)
                _balance = value;
            else
                Console.WriteLine("Balance cannot be negative!");
        }
    }
}
```

### Java Bean vs C# Property Comparison

```
Java (verbose — 6 methods for 3 fields)    C# (3 lines for 3 fields)
────────────────────────────────────────   ──────────────────────────────────
private String name;                       public string Name    { get; set; }
private int age;                           public int    Age     { get; set; }
private double salary;                     public double Salary  { get; set; }

public String getName()   { return name; }
public void setName(String n) { name=n;  }
public int getAge()       { return age;  }
public void setAge(int a) { age=a;       }
public double getSalary() { return salary; }
public void setSalary(double s) { salary=s; }
```

### Key Points

```
✔ Properties replace Java's getters/setters completely
✔ 'value' keyword inside setter = the incoming data
✔ { get; set; }   → read + write property
✔ { get; }        → read-only (set in constructor only)
✔ { get; init; }  → can only set at object creation time (C# 9+)
✔ PascalCase for properties: Name, Age, EmpId, TotalSalary
✔ Private backing field convention: _name, _age (underscore prefix)
```

---

## 4. Access Modifiers

### C# has 6 access modifiers (Java has 4)

```
┌──────────────────────┬──────────────────────────────────────────────────────┬────────────┐
│  Modifier            │  Accessible From                                     │  Java Equiv│
├──────────────────────┼──────────────────────────────────────────────────────┼────────────┤
│  public              │  Anywhere — same/different class, same/diff assembly │  public    │
│  private             │  Only within the same class                          │  private   │
│  protected           │  Same class + derived (child) classes only           │  protected │
│  internal            │  Only within the same project (.dll / .exe)          │  (default) │
│  protected internal  │  Same assembly OR derived classes in other assembly  │  N/A       │
│  private protected   │  Same class OR derived in same assembly only         │  N/A       │
└──────────────────────┴──────────────────────────────────────────────────────┴────────────┘
```

### Usage Example

```csharp
public class Employee
{
    public    string Name       { get; set; }    // accessible everywhere
    private   double _salary;                    // only this class
    protected int    EmpId      { get; set; }    // this class + child classes
    internal  string Department { get; set; }    // only within this project
}
```

### Key Difference from Java

```
Java 'default' (no modifier)  =  same package only
C# class members              =  default is 'private' if no modifier written
C# class itself               =  default is 'internal' if no modifier written
```

---

## 5. Static Members

### Same concept as Java — one shared copy for all objects

```csharp
public class Counter
{
    // Static variable — ONE copy, shared across ALL objects
    public static int TotalCount = 0;

    // Non-static — each object gets its own copy
    public int Id;
    public string Name;

    public Counter(string name)
    {
        TotalCount++;           // shared counter increments for every new object
        this.Id   = TotalCount;
        this.Name = name;
    }

    // Static method — called on class name, not object
    public static void ShowTotal()
    {
        Console.WriteLine($"Total objects: {TotalCount}");
    }
}

var c1 = new Counter("Alice");
var c2 = new Counter("Bob");
var c3 = new Counter("Charlie");

Counter.ShowTotal();              // Total objects: 3
Console.WriteLine(Counter.TotalCount);  // 3
```

### Static Constructor — C# Exclusive (replaces Java's static block)

```csharp
public class Config
{
    public static string AppName;
    public static string Version;

    // Static constructor — no access modifier, no parameters
    static Config()
    {
        AppName = "MyApp";
        Version = "1.0.0";
        Console.WriteLine("Config initialized once!");
    }
}

Console.WriteLine(Config.AppName);  // prints "Config initialized once!" then "MyApp"
Console.WriteLine(Config.Version);  // just "1.0.0" — static constructor already ran
```

```
Java                       C#
──────────────────────     ─────────────────────────────
static {                   static Config()
    // init code           {
}                              // init code
                           }
Runs before main()         Runs on first access to the class
```

---

## 6. this Keyword

Same as Java — refers to the current object.

```csharp
public class Robot
{
    public string Name;
    public int Speed;

    public Robot(string name, int speed)
    {
        this.Name  = name;    // 'this.Name' = object field, 'name' = parameter
        this.Speed = speed;
    }

    public void Start()
    {
        Console.WriteLine($"{this.Name} starting...");
        this.Accelerate();    // call own method using 'this'
    }

    public void Accelerate()
    {
        Console.WriteLine($"Speed: {this.Speed}");
    }
}
```

---

## 7. Inheritance

### Definition

One class acquiring the properties and behaviors of another class.

* **Base class** (Parent/Super) = class being inherited FROM
* **Derived class** (Child/Sub) = class that inherits

### Java uses `extends`, C# uses `:` (colon)

```
Java                        C#
──────────────────────      ──────────────────────
class Dog extends Animal    class Dog : Animal
class Cat extends Animal    class Cat : Animal
```

### Basic Inheritance Example

```csharp
// Base class
public class Animal
{
    public string Name;
    public int Age;

    public Animal(string name, int age)
    {
        this.Name = name;
        this.Age  = age;
    }

    public void Eat()   => Console.WriteLine($"{Name} is eating");
    public void Sleep() => Console.WriteLine($"{Name} is sleeping");
}

// Derived class
public class Dog : Animal
{
    public string Breed;

    public Dog(string name, int age, string breed)
        : base(name, age)       // calls Animal's constructor
    {
        this.Breed = breed;
    }

    public void Bark() => Console.WriteLine($"{Name} says: Woof!");
}

Dog d1 = new Dog("Tommy", 3, "Labrador");
d1.Eat();    // inherited → "Tommy is eating"
d1.Sleep();  // inherited → "Tommy is sleeping"
d1.Bark();   // own method → "Tommy says: Woof!"
```

### All Inheritance Types in C#

```
1. Single         :  Animal ←── Dog

2. Multi-Level    :  Animal ←── Dog ←── Puppy

3. Hierarchical   :  Animal ←── Dog
                     Animal ←── Cat
                     Animal ←── Snake

4. Multiple via   :  ISwimmable ──┐
   Interface         IFlyable   ──┼──► Duck (implements both)
                     IRunnable  ──┘

5. Hybrid         :  Combination of above
```

### Multi-Level Inheritance

```csharp
public class Vehicle
{
    public int Speed;
    public void Move() => Console.WriteLine("Vehicle moves");
}

public class Car : Vehicle
{
    public int Doors;
    public void Drive() => Console.WriteLine("Car drives");
}

public class ElectricCar : Car
{
    public double BatteryCapacity;
    public void Charge() => Console.WriteLine("Charging...");
}

// ElectricCar inherits: Speed, Doors, BatteryCapacity, Move(), Drive(), Charge()
var tesla = new ElectricCar();
tesla.Move();    // from Vehicle
tesla.Drive();   // from Car
tesla.Charge();  // from ElectricCar
```

### Why Multiple Class Inheritance is NOT Allowed (Same as Java)

```
Diamond Problem:
        Animal
       /      \
    Dog        Cat          ← Both override Sound()
       \      /
    HybridPet ← ???         ← Which Sound() runs? AMBIGUITY!

Solution: Use Interfaces for multiple inheritance behavior.
          A class can implement MANY interfaces but extend only ONE class.
```

### Key Points

```
Java               C#
────────────────   ───────────────────────────────────
extends            : (colon)
super()            : base()  in constructor signature
super.method()     base.Method() inside method body
java.lang.Object   System.Object — root of all classes
```

---

## 8. Method Overloading — Compile-Time Polymorphism

### Definition

Multiple methods with the **same name** but **different parameter lists** in the same class.

Same rules as Java — parameter list differs in: Type, Count, or Sequence.

```csharp
public class Calculator
{
    public int    Add(int a, int b)             => a + b;
    public double Add(double a, double b)       => a + b;
    public int    Add(int a, int b, int c)      => a + b + c;
    public string Add(string a, string b)       => a + b;   // concatenation
}

var calc = new Calculator();
Console.WriteLine(calc.Add(5, 10));            // 15
Console.WriteLine(calc.Add(5.5, 2.5));         // 8.0
Console.WriteLine(calc.Add(1, 2, 3));          // 6
Console.WriteLine(calc.Add("Hello ", "C#"));  // Hello C#
```

### Optional Parameters — C# Exclusive

```csharp
// Default values = optional parameters
public void ShowInfo(string name, int age = 18, string city = "Pune")
{
    Console.WriteLine($"{name} | {age} | {city}");
}

ShowInfo("Sumit");                   // Sumit | 18 | Pune
ShowInfo("Akash", 22);               // Akash | 22 | Pune
ShowInfo("Rahul", 25, "Mumbai");     // Rahul | 25 | Mumbai
```

### Named Parameters — C# Exclusive

```csharp
ShowInfo(age: 22, name: "Sumit");    // order doesn't matter when using names
```

---

## 9. Method Overriding — Runtime Polymorphism

### Definition

Child class provides its **own implementation** of a method already defined in the parent class.

### Java vs C# — THE KEY DIFFERENCE

```
Java                                    C#
──────────────────────────────────      ──────────────────────────────────────────
ANY non-private, non-final method       Parent method MUST be marked 'virtual'
can be overridden automatically.        to allow overriding — you OPT-IN.

@Override annotation (optional check)   override keyword is MANDATORY in child.
```

### C# Override Example

```csharp
public class Animal
{
    public string Name;
    public Animal(string name) { this.Name = name; }

    // 'virtual' = I ALLOW my child to override this
    public virtual void Sound()
    {
        Console.WriteLine($"{Name} makes a sound");
    }

    // NOT virtual — cannot be overridden
    public void Breathe()
    {
        Console.WriteLine($"{Name} breathes");
    }
}

public class Dog : Animal
{
    public Dog(string name) : base(name) { }

    // 'override' = I am REPLACING parent's Sound()
    public override void Sound()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }
}

public class Cat : Animal
{
    public Cat(string name) : base(name) { }

    public override void Sound()
    {
        Console.WriteLine($"{Name} says: Meow!");
    }
}

// Runtime Polymorphism — same as Java
Animal a1 = new Dog("Tommy");       // Upcasting
Animal a2 = new Cat("Whiskers");

a1.Sound();   // Woof!   → Dog's version decided at RUNTIME
a2.Sound();   // Meow!   → Cat's version decided at RUNTIME
```

### Calling Parent Method Inside Override — base.Method()

```csharp
public class Dog : Animal
{
    public Dog(string name) : base(name) { }

    public override void Sound()
    {
        base.Sound();   // calls Animal's Sound() first  ← replaces super.sound()
        Console.WriteLine($"{Name} also says: Woof!");
    }
}
```

### Method Hiding with new keyword — C# Exclusive

```csharp
public class Parent
{
    public void Show() => Console.WriteLine("Parent Show");
}

public class Child : Parent
{
    // 'new' = HIDES parent method — NOT overriding — no polymorphism!
    public new void Show() => Console.WriteLine("Child Show");
}

Child c  = new Child();
c.Show();               // "Child Show"

Parent p = new Child(); // Upcasting
p.Show();               // "Parent Show" — NOT Child Show!
//                         Because 'new' hides, it does NOT replace
//                         If it was 'override' it would say "Child Show" here
```

### virtual vs override vs new — Quick Summary

```
┌──────────────┬──────────────────────────────────────────────────────────┐
│  Keyword     │  Meaning                                                 │
├──────────────┼──────────────────────────────────────────────────────────┤
│  virtual     │  Parent: "My children ARE ALLOWED to override this"      │
│  override    │  Child:  "I am REPLACING parent's virtual method"        │
│  new         │  Child:  "I am HIDING parent's method (not replacing)"   │
│  sealed      │  Child:  "No further derived class can override this"    │
└──────────────┴──────────────────────────────────────────────────────────┘
```

---

## 10. Abstract Classes

### Definition

A class that  **cannot be instantiated** . Forces child classes to implement certain methods. Define a common contract with some shared behavior.

### Java vs C# Abstract — Just Syntax Differs

```
Java                              C#
──────────────────────────        ──────────────────────────────────
abstract class Shape              abstract class Shape
abstract void draw();             public abstract void Draw();
@Override                         public override void Draw() { ... }
void draw() { ... }
```

### C# Abstract Class Example

```csharp
public abstract class Shape
{
    public string Color;

    public Shape(string color)
    {
        this.Color = color;
    }

    // Abstract method — NO body — MUST be overridden in concrete child
    public abstract double Area();
    public abstract double Perimeter();

    // Concrete method — HAS body — inherited as-is
    public void DisplayColor()
    {
        Console.WriteLine($"Color: {Color}");
    }
}

// Concrete class — MUST implement all abstract methods
public class Circle : Shape
{
    public double Radius;

    public Circle(string color, double radius) : base(color)
    {
        this.Radius = radius;
    }

    public override double Area()       => Math.PI * Radius * Radius;
    public override double Perimeter()  => 2 * Math.PI * Radius;
}

public class Rectangle : Shape
{
    public double Width, Height;

    public Rectangle(string color, double w, double h) : base(color)
    {
        Width = w; Height = h;
    }

    public override double Area()      => Width * Height;
    public override double Perimeter() => 2 * (Width + Height);
}

// Shape s = new Shape("red");   // ERROR — cannot instantiate abstract class

Shape c = new Circle("Red", 5);
Shape r = new Rectangle("Blue", 4, 6);

Console.WriteLine(c.Area());       // 78.53...
Console.WriteLine(r.Perimeter());  // 20
c.DisplayColor();                  // Color: Red
```

### Abstract Class Diagram

```
        ┌───────────────────────────────┐
        │  abstract class Shape         │
        │  ───────────────────────────  │
        │  Color : string               │
        │  ───────────────────────────  │
        │  abstract Area() : double     │  ← No body — child MUST implement
        │  abstract Perimeter(): double │  ← No body
        │  DisplayColor() : void        │  ← Has body — inherited as-is
        └──────────────┬────────────────┘
                       │
           ┌───────────┴────────────┐
           │                        │
    ┌──────▼──────┐          ┌──────▼──────────┐
    │  Circle     │          │  Rectangle      │
    │  Radius     │          │  Width, Height  │
    │  override   │          │  override       │
    │  Area()     │          │  Area()         │
    │  Perimeter()│          │  Perimeter()    │
    └─────────────┘          └─────────────────┘
```

### Rules — Same as Java

```
✔ Can have both abstract (no body) and concrete (has body) methods
✔ Cannot create objects of abstract class
✔ Child class MUST override ALL abstract methods
✔ OR make child also abstract (defer the implementation)
✗ abstract method cannot be private
✗ abstract method cannot be static
✗ abstract method cannot be sealed
```

---

## 11. Interfaces

### Definition

A **contract** that defines what a class MUST do, but not HOW to do it.
A class can implement multiple interfaces — this is how C# (like Java) achieves multiple inheritance.

### C# Interface Naming Convention

> **Always prefix with capital 'I'**
> `IAnimal`, `IRepository`, `ILogger`, `IDisposable`, `IPaymentGateway`

### Java vs C# Interface

```
Java                                C#
──────────────────────────────      ──────────────────────────────────────
interface Animal { }                interface IAnimal { }   ← 'I' prefix
class Dog implements Animal         class Dog : IAnimal     ← same ':' colon
All methods public abstract         All methods public abstract by default
Variables = public static final     Variables = public static (readonly)
default methods (Java 8+)           default implementations (C# 8+)
```

### Basic Interface Example

```csharp
public interface IShape
{
    double Area();          // no body, no access modifier needed
    double Perimeter();
    void Display();
}

public class Circle : IShape
{
    public double Radius;
    public Circle(double r) { Radius = r; }

    // Must implement ALL interface methods
    public double Area()      => Math.PI * Radius * Radius;
    public double Perimeter() => 2 * Math.PI * Radius;
    public void Display()     => Console.WriteLine($"Circle | Radius: {Radius}");
}
```

### Multiple Interface Implementation (Multiple Inheritance)

```csharp
public interface ISwimmable  { void Swim(); }
public interface IFlyable    { void Fly();  }
public interface IRunnable   { void Run();  }

// Duck implements THREE interfaces — multiple inheritance!
public class Duck : ISwimmable, IFlyable, IRunnable
{
    public string Name;
    public Duck(string name) { this.Name = name; }

    public void Swim() => Console.WriteLine($"{Name} is swimming");
    public void Fly()  => Console.WriteLine($"{Name} is flying");
    public void Run()  => Console.WriteLine($"{Name} is running");
}

Duck d = new Duck("Donald");
d.Swim();   // Donald is swimming
d.Fly();    // Donald is flying
d.Run();    // Donald is running

// Interface reference (Upcasting — same as Java)
ISwimmable swimmer = new Duck("Daffy");
swimmer.Swim();   // works — only Swim() visible via ISwimmable reference
```

### Interface + Class Inheritance Together

```csharp
public class Animal
{
    public string Name;
    public void Breathe() => Console.WriteLine($"{Name} breathes");
}

public interface ITrainable
{
    void Train();
}

// Dog extends Animal AND implements ITrainable
public class Dog : Animal, ITrainable
{
    public Dog(string name) { this.Name = name; }
    public void Bark()  => Console.WriteLine($"{Name}: Woof!");
    public void Train() => Console.WriteLine($"{Name} learned a trick!");
}
```

### Interface with Default Implementation (C# 8+)

```csharp
public interface ILogger
{
    void Log(string message);

    // Default method — implementing class doesn't HAVE to override it
    void LogWarning(string message)
    {
        Console.WriteLine($"[WARNING] {message}");
    }
}

public class FileLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[FILE] {message}");
    }
    // LogWarning not overridden — uses interface's default
}
```

### Abstract Class vs Interface — When to Use What

```
┌───────────────────────┬──────────────────────────┬────────────────────────────┐
│  Feature              │  Abstract Class           │  Interface                 │
├───────────────────────┼──────────────────────────┼────────────────────────────┤
│  Object creation      │  No                       │  No                        │
│  Constructor          │  Yes                      │  No                        │
│  Fields/State         │  Yes                      │  Constants only            │
│  Concrete methods     │  Yes                      │  Yes (C# 8+ default)       │
│  Multiple inheritance │  No (one class only)       │  Yes (many interfaces)     │
├───────────────────────┼──────────────────────────┼────────────────────────────┤
│  USE WHEN             │  IS-A relationship         │  CAN-DO relationship       │
│                       │  "A Dog IS AN Animal"      │  "Duck CAN FLY, CAN SWIM"  │
│                       │  Share base behavior       │  Define a contract         │
└───────────────────────┴──────────────────────────┴────────────────────────────┘
```

---

## 12. sealed Keyword

### Definition

* `sealed` **class** = cannot be inherited (Java's `final class`)
* `sealed` **method** = cannot be overridden further (Java's `final` method)

```csharp
// sealed class — no one can inherit this
public sealed class Config
{
    public static string AppName = "MyApp";
}

// public class MyConfig : Config { }  // ERROR — cannot inherit sealed class
```

```csharp
public class Animal
{
    public virtual void Move() => Console.WriteLine("Moving");
}

public class Dog : Animal
{
    // sealed override = Dog's version is final — no further override allowed
    public sealed override void Move() => Console.WriteLine("Dog runs");
}

public class GoldenRetriever : Dog
{
    // public override void Move() { }  // ERROR — Move() is sealed in Dog
}
```

```
Java                   C#
──────────────────     ─────────────────────
final class            sealed class
final method           sealed override
```

---

## 13. readonly & const

### const — Compile-time constant

```csharp
public class MathHelper
{
    public const double PI          = 3.14159;
    public const int    MaxStudents = 60;
    public const string AppVersion  = "1.0";
    // Must be assigned at declaration
    // Access: MathHelper.PI
}
```

### readonly — Runtime constant

```csharp
public class Employee
{
    public readonly int    EmpId;
    public readonly string JoinDate;

    public Employee(int empId)
    {
        this.EmpId    = empId;
        this.JoinDate = DateTime.Now.ToString("yyyy-MM-dd");
        // Can only assign readonly here in constructor
    }

    public void ChangeId()
    {
        // this.EmpId = 999;   // ERROR — readonly after constructor
    }
}
```

### const vs readonly

```
┌──────────────┬──────────────────────────────┬────────────────────────────────┐
│  Feature     │  const                       │  readonly                      │
├──────────────┼──────────────────────────────┼────────────────────────────────┤
│  When set    │  Declaration only            │  Declaration OR constructor    │
│  Value       │  Compile-time (fixed always) │  Runtime (can use calculations)│
│  Static?     │  Always implicitly static    │  Can be instance or static     │
│  Java equiv  │  static final                │  final (instance field)        │
└──────────────┴──────────────────────────────┴────────────────────────────────┘
```

---

## 14. Type Casting — Upcasting & Downcasting

### Same concept as Java — C# adds 'as' and pattern matching

```csharp
public class Animal { public void Breathe() => Console.WriteLine("Breathing"); }
public class Dog : Animal { public void Bark() => Console.WriteLine("Woof!"); }

// UPCASTING — child → parent (implicit, always safe)
Animal a = new Dog();    // no cast syntax needed
a.Breathe();             // works
// a.Bark();             // ERROR — Bark() not visible via Animal reference

// DOWNCASTING — parent → child (explicit, may throw exception)
Dog d = (Dog)a;          // explicit cast — same as Java
d.Bark();                // works now
```

### Safe Casting — 'as' and 'is' (better than Java's instanceof + cast)

```csharp
// 'is' keyword — same as Java's instanceof
if (a is Dog)
    Console.WriteLine("a is a Dog");

// 'as' keyword — tries cast, returns null if fails (no exception thrown)
Dog d2 = a as Dog;
if (d2 != null)
    d2.Bark();

// Pattern matching with 'is' — C# 7+ — cleanest way
if (a is Dog dog)        // casts AND assigns in one statement
{
    dog.Bark();          // 'dog' is already the Dog reference — no separate cast
}
```

### Java vs C# Casting Syntax

```
Java                            C#
──────────────────────────      ──────────────────────────────────
(Dog) a                         (Dog) a           ← explicit cast (same)
a instanceof Dog                a is Dog          ← type check
N/A                             a as Dog          ← safe cast (null if fails)
N/A                             if(a is Dog dog)  ← pattern matching
```

---

## 15. struct vs class

> **C# has structs. Java doesn't (primitives are the closest concept).**

### struct = Value Type | class = Reference Type

```csharp
public struct Point
{
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
}

public class PointClass
{
    public int X;
    public int Y;
}

// VALUE TYPE behaviour — struct
Point p1 = new Point(1, 2);
Point p2 = p1;           // COPY — separate object
p2.X = 99;
Console.WriteLine(p1.X); // still 1 — p1 not affected

// REFERENCE TYPE behaviour — class
PointClass pc1 = new PointClass { X = 1, Y = 2 };
PointClass pc2 = pc1;    // same reference — SAME object
pc2.X = 99;
Console.WriteLine(pc1.X); // 99 — pc1 also changed!
```

### struct vs class Comparison

```
┌──────────────────┬─────────────────────────┬──────────────────────────────┐
│  Feature         │  struct                 │  class                       │
├──────────────────┼─────────────────────────┼──────────────────────────────┤
│  Type            │  Value type (stack)      │  Reference type (heap)       │
│  Assignment      │  Creates a copy          │  Copies the reference        │
│  Inheritance     │  Cannot inherit          │  Can inherit                 │
│  null            │  Cannot be null          │  Can be null                 │
│  Use for         │  Small, simple data      │  Complex objects with logic  │
│  Examples        │  Point, Color, DateTime  │  Person, Order, Student      │
└──────────────────┴─────────────────────────┴──────────────────────────────┘
```

---

## 16. C# Extras — Record, Partial Class, Nullable

### record — Immutable Data Class (C# 9+)

Records are designed for storing data. Properties are immutable by default. Built-in equality comparison.

```csharp
// Single line — compiler generates constructor, properties, ToString, Equals
public record Person(string Name, int Age, string Email);

var p1 = new Person("Sumit", 22, "sumit@email.com");
Console.WriteLine(p1.Name);   // Sumit
Console.WriteLine(p1);        // Person { Name = Sumit, Age = 22, Email = sumit@email.com }

// 'with' expression — create modified copy (original unchanged)
var p2 = p1 with { Age = 23 };
Console.WriteLine(p2.Age);    // 23
Console.WriteLine(p1.Age);    // still 22

// Built-in value equality (unlike class — class uses reference equality)
var p3 = new Person("Sumit", 22, "sumit@email.com");
Console.WriteLine(p1 == p3);  // TRUE — same values = equal
```

### Partial Class — Split class across multiple files

```csharp
// File: Student.cs
public partial class Student
{
    public string Name { get; set; }
    public int Roll    { get; set; }
}

// File: Student.Methods.cs
public partial class Student
{
    public void Display()
    {
        Console.WriteLine($"{Roll}: {Name}");
    }
}
// Compiler combines both into ONE class. Used heavily in auto-generated code.
```

### Nullable Types

```csharp
// Value types cannot be null by default
// int age = null;        // ERROR

// '?' makes any value type nullable
int? age    = null;
double? sal = null;

// Null coalescing — default if null
int actualAge = age ?? 0;       // 0 if age is null

// Null conditional — don't throw if null
string? name = null;
int? length  = name?.Length;    // null — doesn't throw NullReferenceException
```

---

## 17. Full Comparison: Java OOP vs C# OOP

```
┌──────────────────────────┬────────────────────────────┬──────────────────────────────┐
│  Concept                 │  Java                      │  C#                          │
├──────────────────────────┼────────────────────────────┼──────────────────────────────┤
│  Class declaration       │  public class Foo { }      │  public class Foo { }        │
│  Namespace/Package       │  package com.example;      │  namespace MyApp;            │
│  Inheritance keyword     │  extends                   │  : (colon)                   │
│  Interface implement     │  implements                │  : (same colon)              │
│  Super constructor       │  super(args)  in body      │  : base(args) in signature   │
│  This constructor        │  this(args)   in body      │  : this(args) in signature   │
│  Print                   │  System.out.println()      │  Console.WriteLine()         │
│  Encapsulation           │  getter/setter methods     │  Properties { get; set; }    │
│  Interface prefix        │  No convention             │  'I' prefix — IAnimal        │
│  Multiple inheritance    │  interfaces only           │  interfaces only             │
│  final class             │  final class               │  sealed class                │
│  final method            │  final method override     │  sealed override             │
│  final variable          │  final variable            │  readonly OR const           │
│  static block            │  static { }                │  static ClassName() { }      │
│  instanceof              │  instanceof                │  is                          │
│  Safe cast               │  try/catch ClassCastEx     │  as keyword → null if fails  │
│  Pattern matching        │  N/A (Java 16+)            │  if(a is Dog dog) { }        │
│  Overridable method      │  All non-final methods     │  Must mark virtual           │
│  Override keyword        │  @Override annotation      │  override keyword mandatory  │
│  String interpolation    │  "Hello " + name           │  $"Hello {name}"             │
│  var keyword             │  var (Java 10+)            │  var (always)                │
│  Records                 │  record (Java 16+)         │  record (C# 9+)              │
│  Struct                  │  No equivalent             │  struct (value type)         │
│  Nullable value type     │  Optional<T> wrapper       │  int?, double?               │
└──────────────────────────┴────────────────────────────┴──────────────────────────────┘
```

---

## 🧠 Interview Cheat Sheet

```
Q: What is the difference between abstract class and interface in C#?
A: Abstract class can have constructor, fields, concrete + abstract methods.
   Interface defines only contracts (methods/properties, no state).
   Use abstract for IS-A (shared identity). Use interface for CAN-DO (behavior contract).
   A class inherits ONE abstract class but implements MULTIPLE interfaces.

Q: What is the difference between virtual, override, and new?
A: virtual   = parent says "children are allowed to override this"
   override  = child REPLACES parent's virtual method → polymorphism works
   new       = child HIDES parent's method → polymorphism does NOT work on parent reference

Q: What is the difference between const and readonly?
A: const    = compile-time constant, always static, set at declaration only.
   readonly = runtime constant, can be set in constructor, can be instance-level.

Q: What is the difference between class and struct?
A: class  = reference type (heap) → assignment copies reference, can be null, can inherit.
   struct = value type (stack)   → assignment copies data, cannot be null, cannot inherit.
   Use struct for small, simple data (Point, Color). Use class for everything else.

Q: How does C# handle multiple inheritance?
A: Same as Java — a class can extend only ONE class but implement MULTIPLE interfaces.
   Diamond problem is avoided this way.

Q: What replaces Java's getters/setters in C#?
A: Properties. public string Name { get; set; }
   Much cleaner — reads like a field, acts like a method.

Q: What is upcasting and downcasting?
A: Upcasting   = child object → parent reference (implicit, always safe)
   Downcasting = parent reference → child type (explicit, use 'as' for safety)
   Use 'is' to check type before cast (like instanceof)
   Use pattern matching: if(a is Dog dog) — cleanest C# way

Q: Is C# purely object-oriented?
A: No — like Java, C# has value types (int, double, struct) and static members
   which are not purely OOP concepts.
```

---

*📌 C# OOP Complete Notes | ASP.NET Core MVC Learning Series | Sumit Tikone | March 2026*
