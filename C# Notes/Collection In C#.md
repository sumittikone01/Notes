# 📦 C# Collections — Complete Notes

---

## 📑 Table of Contents

1. [The Big Picture — Collections Landscape](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#1-the-big-picture--collections-landscape)
2. [Array — The Foundation](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#2-array--the-foundation)
3. [List&lt;T&gt; — Dynamic Array](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#3-listt--dynamic-array)
4. [LinkedList&lt;T&gt; — Doubly Linked List](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#4-linkedlistt--doubly-linked-list)
5. [Stack&lt;T&gt; — LIFO](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#5-stackt--lifo)
6. [Queue&lt;T&gt; — FIFO](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#6-queuet--fifo)
7. [Dictionary&lt;TKey,TValue&gt; — Hash Map](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#7-dictionarytkeytvalue--hash-map)
8. [HashSet&lt;T&gt; — Unique Values](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#8-hashsett--unique-values)
9. [SortedDictionary&lt;TKey,TValue&gt; — Sorted Map](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#9-sorteddictionarytkeytvalue--sorted-map)
10. [SortedList&lt;TKey,TValue&gt; — Sorted Array-Map](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#10-sortedlisttkeytvalue--sorted-array-map)
11. [SortedSet&lt;T&gt; — Sorted Unique Values](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#11-sortedsett--sorted-unique-values)
12. [PriorityQueue&lt;T,TPriority&gt; — Heap-Based Queue](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#12-priorityqueuettpriority--heap-based-queue)
13. [Concurrent Collections — Thread-Safe](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#13-concurrent-collections--thread-safe)
14. [Immutable Collections](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#14-immutable-collections)
15. [Non-Generic (Legacy) Collections](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#15-non-generic-legacy-collections)
16. [IEnumerable, ICollection, IList — The Interfaces](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#16-ienumerable-icollection-ilist--the-interfaces)
17. [LINQ with Collections](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#17-linq-with-collections)
18. [Choosing the Right Collection — Decision Guide](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#18-choosing-the-right-collection--decision-guide)
19. [Java vs C# Collections Master Map](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#19-java-vs-c-collections-master-map)
20. [Interview Cheat Sheet](https://claude.ai/chat/f4fb37e3-52e8-422f-8620-450ee3de1344#20-interview-cheat-sheet)

---

## 1. The Big Picture — Collections Landscape

### All C# Collections at a Glance

```
System.Collections.Generic (use these — type-safe, no boxing)
├── List<T>                    → Dynamic array (ArrayList in Java)
├── LinkedList<T>              → Doubly linked list
├── Stack<T>                   → LIFO — push/pop
├── Queue<T>                   → FIFO — enqueue/dequeue
├── Dictionary<TKey,TValue>    → Hash map (HashMap in Java)
├── HashSet<T>                 → Unique values (HashSet in Java)
├── SortedDictionary<K,V>      → Red-Black Tree map (TreeMap in Java)
├── SortedList<K,V>            → Sorted array-based map
└── SortedSet<T>               → Red-Black Tree set (TreeSet in Java)

System.Collections (legacy — avoid, use Generic versions above)
├── ArrayList                  → Old List (no type safety)
├── Hashtable                  → Old Dictionary
├── Stack                      → Old Stack
└── Queue                      → Old Queue

System.Collections.Concurrent (thread-safe — for multi-threading)
├── ConcurrentDictionary<K,V>  → Thread-safe Dictionary
├── ConcurrentQueue<T>         → Thread-safe Queue
├── ConcurrentStack<T>         → Thread-safe Stack
├── ConcurrentBag<T>           → Unordered thread-safe bag
└── BlockingCollection<T>      → Producer-consumer pattern

System.Collections.Immutable (never change after creation)
├── ImmutableList<T>
├── ImmutableDictionary<K,V>
├── ImmutableHashSet<T>
└── ImmutableArray<T>

.NET 6+ additions
└── PriorityQueue<T,TPriority> → Min-heap priority queue
```

### Performance Big-O Overview

```
┌──────────────────────────┬─────────────┬─────────────┬──────────────┬─────────────┐
│  Collection              │  Access     │  Search     │  Insert      │  Delete     │
├──────────────────────────┼─────────────┼─────────────┼──────────────┼─────────────┤
│  Array / List<T>         │  O(1) index │  O(n)       │  O(1) end    │  O(n) mid   │
│  (by index)              │             │             │  O(n) mid    │             │
├──────────────────────────┼─────────────┼─────────────┼──────────────┼─────────────┤
│  LinkedList<T>           │  O(n)       │  O(n)       │  O(1) node   │  O(1) node  │
├──────────────────────────┼─────────────┼─────────────┼──────────────┼─────────────┤
│  Stack<T> / Queue<T>     │  O(1) top   │  O(n)       │  O(1)        │  O(1)       │
├──────────────────────────┼─────────────┼─────────────┼──────────────┼─────────────┤
│  Dictionary<K,V>         │  O(1) avg   │  O(1) avg   │  O(1) avg    │  O(1) avg   │
│  HashSet<T>              │  —          │  O(1) avg   │  O(1) avg    │  O(1) avg   │
├──────────────────────────┼─────────────┼─────────────┼──────────────┼─────────────┤
│  SortedDictionary<K,V>   │  O(log n)   │  O(log n)   │  O(log n)    │  O(log n)   │
│  SortedSet<T>            │  —          │  O(log n)   │  O(log n)    │  O(log n)   │
├──────────────────────────┼─────────────┼─────────────┼──────────────┼─────────────┤
│  SortedList<K,V>         │  O(log n)   │  O(log n)   │  O(n)        │  O(n)       │
└──────────────────────────┴─────────────┴─────────────┴──────────────┴─────────────┘
```

### Java → C# Collections Quick Map

```
Java                    C#
─────────────────────   ──────────────────────────────────
ArrayList<T>            List<T>
LinkedList<T>           LinkedList<T>
HashMap<K,V>            Dictionary<TKey, TValue>
LinkedHashMap<K,V>      (no direct — use OrderedDictionary)
TreeMap<K,V>            SortedDictionary<TKey, TValue>
HashSet<T>              HashSet<T>
TreeSet<T>              SortedSet<T>
Stack<T>                Stack<T>
Queue<T>                Queue<T>
PriorityQueue<T>        PriorityQueue<T, TPriority>
ConcurrentHashMap       ConcurrentDictionary<K,V>
```

---

## 2. Array — The Foundation

### Definition

Fixed-size, ordered collection of elements of the  **same type** .
Stored in **contiguous memory** — fastest possible access by index.

### Internal Working

```
int[] arr = new int[5];

MEMORY (contiguous block):
┌───────┬───────┬───────┬───────┬───────┐
│  arr[0]│ arr[1]│ arr[2]│ arr[3]│ arr[4]│
│   0   │   0   │   0   │   0   │   0   │
└───────┴───────┴───────┴───────┴───────┘
Address:  1000    1004    1008    1012    1016
          (each int = 4 bytes apart)

Access arr[3]:
  Address = BaseAddress + (index × elementSize)
           = 1000 + (3 × 4) = 1012  → O(1) direct jump!
```

### Syntax & Usage

```csharp
// Declaration & Initialization
int[]    nums    = new int[5];                      // default 0s
string[] names   = new string[3];                   // default nulls
int[]    primes  = { 2, 3, 5, 7, 11 };             // array literal
int[]    squares = new int[] { 1, 4, 9, 16, 25 };  // explicit

// Access
Console.WriteLine(primes[0]);   // 2
Console.WriteLine(primes[4]);   // 11
primes[2] = 99;                 // modify

// Length
Console.WriteLine(primes.Length);  // 5

// Loop
for (int i = 0; i < primes.Length; i++)
    Console.Write(primes[i] + " ");    // 2 3 99 7 11

foreach (int p in primes)
    Console.Write(p + " ");            // same output

// 2D Array
int[,] matrix = new int[3, 3];
matrix[0, 0] = 1;
matrix[1, 1] = 5;
matrix[2, 2] = 9;

int[,] grid = { { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 } };

Console.WriteLine(grid[1, 2]);  // 6

// Jagged Array (array of arrays — each row can have different length)
int[][] jagged = new int[3][];
jagged[0] = new int[] { 1 };
jagged[1] = new int[] { 2, 3 };
jagged[2] = new int[] { 4, 5, 6 };
Console.WriteLine(jagged[2][1]);   // 5
```

### Array Methods

```csharp
int[] data = { 5, 2, 8, 1, 9, 3 };

Array.Sort(data);                          // { 1, 2, 3, 5, 8, 9 }
Array.Reverse(data);                       // { 9, 8, 5, 3, 2, 1 }
int idx = Array.BinarySearch(data, 5);    // (array must be sorted first)
Array.Fill(data, 0);                      // { 0, 0, 0, 0, 0, 0 }
Array.Copy(data, dest, data.Length);      // copy to dest array

// Useful properties
Console.WriteLine(data.Length);           // 6 (total elements)
Console.WriteLine(data.Rank);             // 1 (dimensions)
```

### Key Points

```
✔ Fixed size — cannot grow or shrink after creation
✔ O(1) index access — fastest possible random access
✔ All elements same type (unlike ArrayList — avoids boxing)
✔ 2D array: int[,]   vs   Jagged array: int[][]
   2D: rectangular grid, stored contiguously
   Jagged: rows can have different lengths, each row is separate array
✗ No add/remove — use List<T> for dynamic sizing
```

---

## 3. List`<T>` — Dynamic Array

### Definition

A dynamically resizable array. The  **most used collection in C#** .
Equivalent to Java's `ArrayList<T>`.

### Internal Working — The Resize Magic

```
INTERNAL STRUCTURE:
List<T> contains:
  _items  : T[]    ← internal array (the actual storage)
  _size   : int    ← number of elements currently stored
  _version: int    ← for detecting modification during enumeration

Initial state (new List<int>()):
  _items  = [] (empty array, capacity = 0)
  _size   = 0

After Add(1):
  Capacity = 4 (first growth: allocates array of size 4)
  _items  = [1, _, _, _]   (_ = unused slots)
  _size   = 1
  Count   = 1

After Add(2), Add(3), Add(4):
  _items  = [1, 2, 3, 4]
  _size   = 4
  Count   = 4
  Capacity= 4   ← FULL

After Add(5):  ← RESIZE TRIGGERED!
  New capacity = 4 × 2 = 8   (doubles every time it's full)
  New _items   = [1, 2, 3, 4, 5, _, _, _]
  _size        = 5
  Count        = 5
  Capacity     = 8
```

### Resize Diagram

```
Capacity growth pattern:
  0 → 4 → 8 → 16 → 32 → 64 → ...  (doubles each time)

Step 1: List full                    Step 2: Resize (double)
┌───┬───┬───┬───┐                   ┌───┬───┬───┬───┬───┬───┬───┬───┐
│ 1 │ 2 │ 3 │ 4 │  Add(5)  ──────► │ 1 │ 2 │ 3 │ 4 │ 5 │   │   │   │
└───┴───┴───┴───┘                   └───┴───┴───┴───┴───┴───┴───┴───┘
  capacity = 4                        capacity = 8
  count    = 4                        count    = 5
                                      ↑ old array COPIED to new, then GC'd
```

### Complete API

```csharp
// ── Creation ──────────────────────────────────────────────────
var list = new List<int>();                   // empty
var list2 = new List<int> { 10, 20, 30 };    // with initial values
var list3 = new List<int>(100);              // pre-allocate capacity=100 (avoid resizes)

// ── Adding ────────────────────────────────────────────────────
list.Add(5);              // [5]
list.Add(10);             // [5, 10]
list.Add(15);             // [5, 10, 15]
list.AddRange(new[] { 20, 25 });   // [5, 10, 15, 20, 25]
list.Insert(1, 99);       // [5, 99, 10, 15, 20, 25]  insert at index 1

// ── Accessing ─────────────────────────────────────────────────
Console.WriteLine(list[0]);        // 5
Console.WriteLine(list.Count);     // 6
Console.WriteLine(list.Capacity);  // internal array size (might be > Count)
int first = list.First();          // 5  (LINQ extension)
int last  = list.Last();           // 25 (LINQ extension)

// ── Searching ─────────────────────────────────────────────────
bool has99 = list.Contains(99);           // true
int  idx   = list.IndexOf(99);            // 1
int  lastI = list.LastIndexOf(10);        // 2
int  found = list.Find(x => x > 15);     // 20 (first match)
var  all   = list.FindAll(x => x > 15);  // [20, 25]
int  fi    = list.FindIndex(x => x > 15);// 4

// ── Removing ──────────────────────────────────────────────────
list.Remove(99);           // removes first occurrence of 99
list.RemoveAt(0);          // removes element at index 0
list.RemoveAll(x => x > 20); // removes all elements > 20
list.Clear();              // removes all

// ── Sorting ───────────────────────────────────────────────────
var data = new List<int> { 5, 2, 8, 1, 9, 3 };
data.Sort();                               // [1, 2, 3, 5, 8, 9] — ascending
data.Sort((a, b) => b.CompareTo(a));      // [9, 8, 5, 3, 2, 1] — descending
data.Reverse();                            // reverse in place

// Sort objects by property
var people = new List<Person>
{
    new Person { Name = "Sumit",  Age = 22 },
    new Person { Name = "Akash",  Age = 20 },
    new Person { Name = "Rahul",  Age = 25 },
};
people.Sort((a, b) => a.Age.CompareTo(b.Age));  // sort by Age ascending

// ── Conversion ────────────────────────────────────────────────
int[]        arr  = data.ToArray();
List<int>    copy = data.ToList();                    // new copy
List<string> strs = data.Select(x => x.ToString()).ToList();  // LINQ

// ── Capacity Management ───────────────────────────────────────
list.TrimExcess();  // reduces Capacity to match Count — saves memory
```

### Visual: Insert vs Add

```
Add(99) at end — O(1) amortized:
[1][2][3][4][_][_]  →  [1][2][3][4][99][_]   ← just write at _size index

Insert(1, 99) in middle — O(n):
[1][2][3][4][_][_]
                ↓ shift elements right from index 1
[1][_][2][3][4][_]
    ↓ write 99
[1][99][2][3][4][_]   ← expensive! every element after moved
```

### Key Points

```
✔ Most used collection in C# — default choice for ordered data
✔ O(1) access by index (like array)
✔ O(1) amortized Add() at end (occasional O(n) resize)
✔ O(n) Insert/Remove in middle (shifting required)
✔ Capacity doubles on resize — amortized O(1) per Add
✔ Pre-allocate with new List<T>(expectedSize) if size known
✔ Equivalent to Java's ArrayList<T>
✗ Not thread-safe — use List with lock or ConcurrentBag<T>
```

---

## 4. LinkedList`<T>` — Doubly Linked List

### Internal Working

```
LinkedList<T> stores elements as NODES.
Each node has:
  - Value: the actual data
  - Next:  reference to next node
  - Prev:  reference to previous node (doubly linked)

LinkedList itself stores:
  - First: reference to head node
  - Last:  reference to tail node
  - Count: number of nodes

var list = new LinkedList<string>();
list.AddLast("A");
list.AddLast("B");
list.AddLast("C");

MEMORY:
         ┌─────────────────────────────────────────────────────┐
         │                LinkedList<string>                   │
         │  First ──────────────────────────────►              │
         │  Last  ──────────────────────────────────────────►  │
         └────────────────────────────────────────────────────-┘

null ◄── [Prev│ "A" │Next] ←──► [Prev│ "B" │Next] ←──► [Prev│ "C" │Next] ──► null
              ↑                                               ↑
            First                                           Last

Each "node" is a LinkedListNode<T> object on the heap
```

### Syntax & Usage

```csharp
var list = new LinkedList<string>();

// ── Adding ────────────────────────────────────────────────────
list.AddFirst("B");                          // [B]
list.AddFirst("A");                          // [A, B]
list.AddLast("D");                           // [A, B, D]
list.AddLast("E");                           // [A, B, D, E]

LinkedListNode<string> nodeD = list.Find("D");  // get the node
list.AddBefore(nodeD, "C");                 // [A, B, C, D, E]
list.AddAfter(nodeD, "D+");                 // [A, B, C, D, D+, E]

// ── Accessing ─────────────────────────────────────────────────
Console.WriteLine(list.First.Value);        // A
Console.WriteLine(list.Last.Value);         // E
Console.WriteLine(list.Count);              // 6

// Traversal — forward
LinkedListNode<string> current = list.First;
while (current != null)
{
    Console.Write(current.Value + " ");
    current = current.Next;
}
// Output: A B C D D+ E

// Traversal — backward
current = list.Last;
while (current != null)
{
    Console.Write(current.Value + " ");
    current = current.Prev;
}
// Output: E D+ D C B A

// ── Removing ──────────────────────────────────────────────────
list.RemoveFirst();                          // removes A
list.RemoveLast();                           // removes E
list.Remove("C");                            // removes first "C" node
list.Remove(nodeD);                          // removes specific node O(1)!

// ── Searching ─────────────────────────────────────────────────
bool hasB = list.Contains("B");             // true
LinkedListNode<string> n = list.Find("B");  // gets node reference
```

### List`<T>` vs LinkedList`<T>` — When to Use Which

```
┌────────────────────────┬────────────────────────┬────────────────────────┐
│  Operation             │  List<T>               │  LinkedList<T>         │
├────────────────────────┼────────────────────────┼────────────────────────┤
│  Access by index       │  O(1) ✅ fast           │  O(n) ❌ slow          │
│  Add at end            │  O(1) amortized ✅      │  O(1) ✅               │
│  Add at beginning      │  O(n) ❌ slow           │  O(1) ✅ fast          │
│  Insert at known node  │  O(n) ❌ (find+shift)  │  O(1) ✅ (just relink) │
│  Remove at known node  │  O(n) ❌ (shift)       │  O(1) ✅ (just relink) │
│  Memory                │  Compact (array)       │  High (node objects)   │
│  Cache friendly?       │  ✅ Yes (contiguous)   │  ❌ No (scattered)     │
├────────────────────────┼────────────────────────┼────────────────────────┤
│  USE WHEN              │  Most cases!           │  Frequent insert/remove│
│                        │  Index access needed   │  at BOTH ends          │
│                        │                        │  or at known positions │
└────────────────────────┴────────────────────────┴────────────────────────┘
```

---

## 5. Stack`<T>` — LIFO

### Definition

**Last In, First Out** — like a stack of plates. You can only add/remove from the TOP.

### Internal Working

```
Stack<T> internally uses a T[] array (same as List<T>).
_array  : T[]   ← internal storage
_size   : int   ← index of top + 1

Push(item): _array[_size++] = item   (add to top)
Pop()     : return _array[--_size]    (remove from top)
Peek()    : return _array[_size - 1]  (look at top, no remove)

Stack<int> s = new Stack<int>();
s.Push(10);
s.Push(20);
s.Push(30);

MEMORY (top is right side):
                    TOP
              ┌───┬───┬───┬───┐
Internal arr: │10 │20 │30 │   │
              └───┴───┴───┴───┘
  _size = 3    ↑  conceptual view

s.Pop()  → returns 30, _size becomes 2
s.Peek() → returns 20, _size unchanged
```

### Syntax & Usage

```csharp
var stack = new Stack<int>();

// ── Adding (Push) ─────────────────────────────────────────────
stack.Push(10);   // [10]
stack.Push(20);   // [10, 20]
stack.Push(30);   // [10, 20, 30]  ← 30 is on top

// ── Accessing ─────────────────────────────────────────────────
Console.WriteLine(stack.Peek());   // 30 — looks at top WITHOUT removing
Console.WriteLine(stack.Count);    // 3

// ── Removing (Pop) ────────────────────────────────────────────
Console.WriteLine(stack.Pop());    // 30 — removes and returns top
Console.WriteLine(stack.Pop());    // 20
Console.WriteLine(stack.Count);    // 1

// ── Safe Pop ──────────────────────────────────────────────────
if (stack.TryPop(out int val))
    Console.WriteLine(val);        // 10 — safe, no exception if empty

if (stack.TryPeek(out int top))
    Console.WriteLine(top);        // safe peek

// ── Search / Contains ─────────────────────────────────────────
bool has10 = stack.Contains(10);   // true

// ── Iteration ─────────────────────────────────────────────────
foreach (int item in stack)        // iterates top → bottom (without removing)
    Console.Write(item + " ");

// Convert to array (top = index 0)
int[] arr = stack.ToArray();
```

### Real-World Use Cases

```csharp
// Use Case 1: Undo/Redo system
var undoStack = new Stack<string>();
undoStack.Push("typed 'Hello'");
undoStack.Push("typed ' World'");
undoStack.Push("deleted 3 chars");

string lastAction = undoStack.Pop();   // undo "deleted 3 chars"

// Use Case 2: Balanced parentheses checker
static bool IsBalanced(string expression)
{
    var stack = new Stack<char>();
    foreach (char c in expression)
    {
        if (c == '(' || c == '{' || c == '[')
            stack.Push(c);
        else if (c == ')' || c == '}' || c == ']')
        {
            if (stack.Count == 0) return false;
            char top = stack.Pop();
            if ((c == ')' && top != '(') ||
                (c == '}' && top != '{') ||
                (c == ']' && top != '['))
                return false;
        }
    }
    return stack.Count == 0;
}

Console.WriteLine(IsBalanced("({[]})"));  // true
Console.WriteLine(IsBalanced("({[})"));   // false

// Use Case 3: Call stack simulation / DFS traversal
var dfsStack = new Stack<int>();
dfsStack.Push(startNode);
while (dfsStack.Count > 0)
{
    int node = dfsStack.Pop();
    // process node, push neighbors
}
```

---

## 6. Queue`<T>` — FIFO

### Definition

**First In, First Out** — like a real-world queue/line. You add to the BACK, remove from the FRONT.

### Internal Working

```
Queue<T> uses a CIRCULAR ARRAY internally.
_array : T[]  ← internal circular buffer
_head  : int  ← index of first element (dequeue from here)
_tail  : int  ← index where next element goes (enqueue here)
_size  : int  ← number of elements

Circular Buffer — why circular?
  Regular array would waste space after Dequeue.
  Circular: _head and _tail wrap around using modulo.

Enqueue(A): tail=1, [A,_,_,_]
Enqueue(B): tail=2, [A,B,_,_]
Enqueue(C): tail=3, [A,B,C,_]
Dequeue():  head=1, [_,B,C,_]  returns A
Enqueue(D): tail=0! wraps around: [D,B,C,_]  ← circular!
Dequeue():  head=2, [D,_,C,_]  returns B

VISUAL:
         head                    tail
          ↓                       ↓
  ┌───┬───┬───┬───┬───┬───┬───┬───┐
  │   │   │ C │ D │ E │   │   │   │
  └───┴───┴───┴───┴───┴───┴───┴───┘
  
  Enqueue from tail →
  Dequeue from head →
  When tail reaches end, it wraps around to start
```

### Syntax & Usage

```csharp
var queue = new Queue<string>();

// ── Adding (Enqueue) ──────────────────────────────────────────
queue.Enqueue("Alice");    // [Alice]
queue.Enqueue("Bob");      // [Alice, Bob]
queue.Enqueue("Charlie");  // [Alice, Bob, Charlie]

// ── Accessing ─────────────────────────────────────────────────
Console.WriteLine(queue.Peek());   // "Alice" — front element, no remove
Console.WriteLine(queue.Count);    // 3

// ── Removing (Dequeue) ────────────────────────────────────────
Console.WriteLine(queue.Dequeue()); // "Alice" — removes from front
Console.WriteLine(queue.Dequeue()); // "Bob"
Console.WriteLine(queue.Count);     // 1

// ── Safe operations ───────────────────────────────────────────
if (queue.TryDequeue(out string person))
    Console.WriteLine(person);     // "Charlie"

if (queue.TryPeek(out string front))
    Console.WriteLine(front);      // safe peek

// ── Contains / Iteration ──────────────────────────────────────
bool hasBob = queue.Contains("Bob");    // false (already dequeued)
foreach (string item in queue)
    Console.WriteLine(item);            // front → back (no removing)
```

### Real-World Use Cases

```csharp
// Use Case 1: Request processing queue
var requestQueue = new Queue<string>();
requestQueue.Enqueue("Request 1");
requestQueue.Enqueue("Request 2");
requestQueue.Enqueue("Request 3");

while (requestQueue.Count > 0)
{
    string req = requestQueue.Dequeue();
    Console.WriteLine($"Processing: {req}");
}

// Use Case 2: BFS (Breadth-First Search)
var bfsQueue = new Queue<int>();
bfsQueue.Enqueue(rootNode);
while (bfsQueue.Count > 0)
{
    int node = bfsQueue.Dequeue();
    // process, enqueue neighbors
}
```

---

## 7. Dictionary<TKey,TValue> — Hash Map

### Definition

Key-value pairs where keys are  **unique** . Provides O(1) average access by key.
This is the most important collection to understand deeply.
Equivalent to Java's `HashMap<K,V>`.

### Internal Working — How Hashing Works

```
Dictionary<TKey, TValue> internally uses:
  _buckets : int[]       ← array of bucket indices (size = prime number)
  _entries : Entry[]     ← array of Entry structs
  _count   : int
  _freeList: int

Entry struct:
  {
    int    hashCode;   ← pre-computed hash of key
    int    next;       ← index of next entry in same bucket (collision chain)
    TKey   key;
    TValue value;
  }
```

### Step-by-Step: How Add Works

```
var dict = new Dictionary<string, int>();
dict["Alice"] = 90;
dict["Bob"]   = 85;
dict["Charlie"] = 92;

STEP 1: Compute hash code of key
───────────────────────────────────────────────────────────────────
  hash("Alice")   = 3527982 (example)
  hash("Bob")     = 9182347 (example)
  hash("Charlie") = 7381920 (example)

STEP 2: Find bucket index (hash → bucket)
───────────────────────────────────────────────────────────────────
  bucketIndex = hash % buckets.Length   (buckets.Length is a prime, say 7)
  "Alice"   → 3527982 % 7 = 3   → bucket[3]
  "Bob"     → 9182347 % 7 = 5   → bucket[5]
  "Charlie" → 7381920 % 7 = 0   → bucket[0]

STEP 3: Store entry in _entries array
───────────────────────────────────────────────────────────────────

  _buckets array:          _entries array:
  ┌───┬───┬───┬───┬───┬───┬───┐    ┌────────────────────────────────────────────┐
  │ 2 │-1 │-1 │ 0 │-1 │ 1 │-1│    │ idx │ hashCode  │ next │  key      │ value │
  └───┴───┴───┴───┴───┴───┴───┘    ├─────┼───────────┼──────┼───────────┼───────┤
    0   1   2   3   4   5   6       │  0  │ 3527982   │  -1  │ "Alice"   │  90   │
    ↑   ↑                           │  1  │ 9182347   │  -1  │ "Bob"     │  85   │
    │   └── bucket[5] points        │  2  │ 7381920   │  -1  │ "Charlie" │  92   │
    │        to entry[1] ("Bob")    └─────┴───────────┴──────┴───────────┴───────┘
    └── bucket[0] points to entry[2] ("Charlie")
```

### Hash Collision — How It's Handled (Chaining)

```
What if two keys hash to the SAME bucket?
This is called a COLLISION.
C# Dictionary uses CHAINING via linked list in _entries.

dict["Dave"] = 78;
hash("Dave") = 5381760
5381760 % 7 = 3   → bucket[3] ← SAME as "Alice"!

COLLISION! bucket[3] already points to Alice's entry.

Resolution — chain them:
  entries[0] (Alice):  next = 3   ← now points to Dave's entry
  entries[3] (Dave):   next = -1  ← end of chain

  _buckets[3] still points to entry[3] (most recent = Dave)
  Dave's entry.next points to entry[0] (Alice)

  _buckets:
  ┌───┬───┬───┬───┬───┬───┬───┐
  │ 2 │-1 │-1 │ 3 │-1 │ 1 │-1│
  └───┴───┴───┴───┴───┴───┴───┘
              ↑
              bucket[3] → entry[3] (Dave) → entry[0] (Alice) → -1

Lookup of dict["Alice"]:
  1. hash("Alice") % 7 = 3 → bucket[3]
  2. bucket[3] → entry[3]: key = "Dave"? NO → follow next
  3. entry[0]: key = "Alice"? YES → return value 90  ✓
```

### Resize (Rehashing)

```
When Count / Capacity > load factor (default ~0.72 in .NET):
  Dictionary DOUBLES its bucket count to next prime
  ALL entries re-hashed and re-inserted into new bucket array

This is O(n) but happens rarely → amortized O(1) per Add
```

### Complete API

```csharp
// ── Creation ──────────────────────────────────────────────────
var scores = new Dictionary<string, int>();
var scores2 = new Dictionary<string, int>
{
    { "Alice", 90 },
    { "Bob",   85 },
    { "Charlie", 92 }
};
var scores3 = new Dictionary<string, int>(capacity: 100); // pre-size

// ── Adding / Updating ─────────────────────────────────────────
scores["Alice"]   = 90;       // add if not exists, update if exists
scores["Bob"]     = 85;
scores.Add("Charlie", 92);    // throws if key already exists!

// Safe add
scores.TryAdd("Dave", 78);    // returns false if key exists, no exception

// ── Accessing ─────────────────────────────────────────────────
int aliceScore = scores["Alice"];       // 90 — throws KeyNotFoundException if missing

// Safe access
if (scores.TryGetValue("Eve", out int eveScore))
    Console.WriteLine(eveScore);        // only runs if "Eve" exists
else
    Console.WriteLine("Eve not found"); // safe — no exception

// Default if missing
int frank = scores.GetValueOrDefault("Frank", 0);  // 0 if Frank not in dict

// ── Checking ──────────────────────────────────────────────────
bool hasAlice = scores.ContainsKey("Alice");     // true — O(1)
bool has90    = scores.ContainsValue(90);         // true — O(n) scan!

// ── Removing ──────────────────────────────────────────────────
scores.Remove("Bob");                     // removes Bob's entry
scores.Remove("Dave", out int daveScore); // removes AND gets the value

// ── Iterating ─────────────────────────────────────────────────
// All key-value pairs
foreach (KeyValuePair<string, int> kvp in scores)
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");

// Modern destructuring (C# 7+)
foreach (var (name, score) in scores)
    Console.WriteLine($"{name}: {score}");

// Keys only
foreach (string key in scores.Keys)
    Console.WriteLine(key);

// Values only
foreach (int val in scores.Values)
    Console.WriteLine(val);

// ── Useful Patterns ───────────────────────────────────────────
// Word frequency count
string text = "hello world hello sumit hello world";
var freq = new Dictionary<string, int>();

foreach (string word in text.Split(' '))
{
    if (freq.ContainsKey(word))
        freq[word]++;
    else
        freq[word] = 1;
}

// Cleaner version using GetValueOrDefault
foreach (string word in text.Split(' '))
    freq[word] = freq.GetValueOrDefault(word, 0) + 1;

// Count: {hello: 3, world: 2, sumit: 1}
```

### Key Points

```
✔ O(1) average for Add, Remove, ContainsKey, indexer
✔ Keys must be unique — duplicate key throws exception
✔ Use TryGetValue instead of direct [] to avoid KeyNotFoundException
✔ ContainsValue is O(n) — avoid in loops
✔ Iteration order is insertion order in .NET 5+ (implementation detail, not guaranteed)
✔ Custom objects as keys: must override GetHashCode() and Equals()
✔ Equivalent to Java's HashMap
✗ NOT thread-safe — use ConcurrentDictionary for multi-threading
```

### Custom Key — GetHashCode + Equals

```csharp
public class StudentId
{
    public int Id;
    public string School;

    // MUST override GetHashCode if used as Dictionary key
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, School);  // combine fields into hash
    }

    // MUST override Equals — Dictionary uses this for collision resolution
    public override bool Equals(object? obj)
    {
        if (obj is StudentId other)
            return Id == other.Id && School == other.School;
        return false;
    }
}

var dict = new Dictionary<StudentId, string>();
dict[new StudentId { Id = 1, School = "MIT" }] = "Alice";
```

---

## 8. HashSet`<T>` — Unique Values

### Definition

A collection of **unique** elements with O(1) add, remove, and lookup.
No keys, no values — just items. Duplicates silently ignored.
Equivalent to Java's `HashSet<T>`.

### Internal Working

```
HashSet<T> internally is EXACTLY like Dictionary<T, (nothing)>.
It uses the same hashing mechanism — just no values, only keys.

Elements stored in slots array.
Slot: { hashCode, next, value }  ← same chaining as Dictionary

HashSet<int> set = new HashSet<int>();
set.Add(5);   → hash(5) → bucket → store
set.Add(10);  → hash(10) → bucket → store
set.Add(5);   → hash(5) → bucket → ALREADY EXISTS → ignored, returns false
```

### Syntax & Usage

```csharp
var set = new HashSet<int> { 1, 2, 3, 4, 5 };

// ── Adding ────────────────────────────────────────────────────
bool added1 = set.Add(6);    // true  — 6 added
bool added2 = set.Add(3);    // false — 3 already exists, NOT added again
Console.WriteLine(set.Count); // 6 (not 7 — no duplicate)

// ── Checking ──────────────────────────────────────────────────
bool has3 = set.Contains(3);   // true  — O(1)!
bool has9 = set.Contains(9);   // false

// ── Removing ──────────────────────────────────────────────────
set.Remove(1);              // removes 1
set.ExceptWith(new[] { 2, 3 }); // removes 2 and 3

// ── Set Operations — THE POWER OF HASHSET ─────────────────────
var setA = new HashSet<int> { 1, 2, 3, 4, 5 };
var setB = new HashSet<int> { 4, 5, 6, 7, 8 };

// UNION — all elements from both
var union = new HashSet<int>(setA);
union.UnionWith(setB);
// { 1, 2, 3, 4, 5, 6, 7, 8 }

// INTERSECTION — only elements in BOTH
var inter = new HashSet<int>(setA);
inter.IntersectWith(setB);
// { 4, 5 }

// DIFFERENCE — in A but NOT in B
var diff = new HashSet<int>(setA);
diff.ExceptWith(setB);
// { 1, 2, 3 }

// SYMMETRIC DIFFERENCE — in A or B, but NOT both
var symDiff = new HashSet<int>(setA);
symDiff.SymmetricExceptWith(setB);
// { 1, 2, 3, 6, 7, 8 }

// SUBSET / SUPERSET checks
bool isSubset   = new HashSet<int> { 1, 2 }.IsSubsetOf(setA);       // true
bool isSuperset = setA.IsSupersetOf(new HashSet<int> { 1, 2 });      // true
bool overlaps   = setA.Overlaps(setB);                               // true
bool equals     = setA.SetEquals(new HashSet<int> { 1, 2, 3, 4, 5 }); // true
```

### Visual: Set Operations

```
setA = { 1, 2, 3, 4, 5 }
setB = {          4, 5, 6, 7, 8 }

      setA           setB
  ┌────────────┐  ┌────────────┐
  │  1  2  3   │  │   6  7  8  │
  │      ┌─────┴──┴──┐         │
  │      │  4   5    │         │
  └──────┴───────────┴─────────┘
         Intersection

Union     = { 1,2,3,4,5,6,7,8 } ← everything
Intersect = {     4,5         } ← middle only
Difference= { 1,2,3           } ← left only (A - B)
SymDiff   = { 1,2,3,  6,7,8  } ← everything EXCEPT middle
```

### Key Points

```
✔ O(1) Add, Remove, Contains — perfect for "have I seen this before?"
✔ Automatically ignores duplicates
✔ Powerful set math: Union, Intersection, Difference
✔ No index access — cannot do set[0]
✔ Equivalent to Java's HashSet<T>
USE FOR: removing duplicates from a list, membership testing, set math
```

---

## 9. SortedDictionary<TKey,TValue> — Sorted Map

### Definition

Dictionary where  **keys are always kept sorted** . Uses a **Red-Black Tree** internally.
Every operation maintains sorted order. O(log n) for all operations.
Equivalent to Java's `TreeMap<K,V>`.

### Internal Working — Red-Black Tree

```
Red-Black Tree = Self-balancing Binary Search Tree

Properties:
  1. Every node is Red or Black
  2. Root is Black
  3. No two consecutive Red nodes
  4. Equal number of Black nodes on any path to leaf
  These rules keep the tree BALANCED → height = O(log n)

SortedDictionary<string, int>:
  dict["Charlie"] = 92;
  dict["Alice"]   = 90;
  dict["Bob"]     = 85;
  dict["Dave"]    = 78;

TREE (sorted by key):
                   Bob(Black)
                  /          \
           Alice(Red)       Charlie(Red)
                                 \
                               Dave(Red)
                                 ↑
                           (tree stays balanced,
                            height ≤ 2×log n)

Keys traverse In-Order: Alice → Bob → Charlie → Dave  ← SORTED!

Add, Remove, Lookup:
  Binary search down tree = O(log n) steps
  Tree auto-rebalances after each Add/Remove
```

### Syntax & Usage

```csharp
var dict = new SortedDictionary<string, int>
{
    { "Charlie", 92 },
    { "Alice",   90 },
    { "Bob",     85 },
    { "Dave",    78 }
};

// ── Iteration — ALWAYS sorted by key ──────────────────────────
foreach (var (name, score) in dict)
    Console.WriteLine($"{name}: {score}");
// Output — alphabetical order:
// Alice: 90
// Bob: 85
// Charlie: 92
// Dave: 78

// ── First/Last key ────────────────────────────────────────────
string firstKey = dict.Keys.First();   // "Alice"
string lastKey  = dict.Keys.Last();    // "Dave"

// ── All other operations same as Dictionary ───────────────────
dict["Eve"] = 95;
dict.Remove("Bob");
bool hasAlice = dict.ContainsKey("Alice");
int score = dict["Alice"];
dict.TryGetValue("Frank", out int frankScore);
```

### Dictionary vs SortedDictionary

```
┌──────────────────────┬──────────────────────┬──────────────────────────┐
│  Feature             │  Dictionary<K,V>     │  SortedDictionary<K,V>   │
├──────────────────────┼──────────────────────┼──────────────────────────┤
│  Internal structure  │  Hash table          │  Red-Black Tree          │
│  Add/Remove/Find     │  O(1) average        │  O(log n)                │
│  Key order           │  Unordered           │  Always sorted           │
│  Iteration order     │  Insertion order*    │  Sorted key order        │
│  Memory              │  More (buckets)      │  Less (tree nodes)       │
│  Range queries       │  Not supported       │  Possible (tree)         │
├──────────────────────┼──────────────────────┼──────────────────────────┤
│  USE WHEN            │  No order needed     │  Need sorted keys        │
│                      │  Speed is priority   │  Range scan needed       │
└──────────────────────┴──────────────────────┴──────────────────────────┘
```

---

## 10. SortedList<TKey,TValue> — Sorted Array-Map

### Internal Working

```
SortedList<TKey, TValue> stores data in TWO parallel sorted arrays:
  _keys   : TKey[]    ← sorted array of keys
  _values : TValue[]  ← corresponding values

var list = new SortedList<string, int>();
list["Charlie"] = 92;
list["Alice"]   = 90;
list["Bob"]     = 85;

Internally:
  _keys:   [ "Alice",  "Bob",   "Charlie" ]   ← sorted
  _values: [   90,       85,       92     ]   ← matching order
             ↑             ↑
           index 0       index 1

Lookup by KEY:  Binary search on _keys array → O(log n)
Lookup by INDEX: _values[i] → O(1)  ← SortedList unique advantage!
```

### Syntax & Usage

```csharp
var list = new SortedList<string, int>
{
    { "Charlie", 92 },
    { "Alice",   90 },
    { "Bob",     85 }
};

// ── KEY DIFFERENCE: Index access! ─────────────────────────────
string key0   = list.Keys[0];      // "Alice"   — by index
int    val0   = list.Values[0];    // 90        — by index
int    score  = list["Bob"];       // 85        — by key

// Key index
int aliceIdx = list.IndexOfKey("Alice");    // 0
int scoreIdx = list.IndexOfValue(92);       // 2

// ── Iteration ─────────────────────────────────────────────────
foreach (var (name, score) in list)
    Console.WriteLine($"{name}: {score}");  // sorted alphabetically
```

### SortedDictionary vs SortedList

```
┌──────────────────────┬──────────────────────────┬──────────────────────────┐
│  Feature             │  SortedDictionary<K,V>   │  SortedList<K,V>         │
├──────────────────────┼──────────────────────────┼──────────────────────────┤
│  Internal structure  │  Red-Black Tree          │  Two sorted arrays        │
│  Insert/Delete       │  O(log n)                │  O(n) — array shift!      │
│  Lookup by key       │  O(log n)                │  O(log n) binary search   │
│  Lookup by index     │  Not supported           │  O(1) — unique feature!   │
│  Memory              │  More (node objects)     │  Less (compact arrays)    │
├──────────────────────┼──────────────────────────┼──────────────────────────┤
│  USE WHEN            │  Frequent inserts/deletes│  Mostly reads, need index │
│                      │  large datasets          │  Small/static datasets    │
└──────────────────────┴──────────────────────────┴──────────────────────────┘
```

---

## 11. SortedSet`<T>` — Sorted Unique Values

### Definition

A set that keeps elements  **sorted at all times** . Uses Red-Black Tree internally.
Equivalent to Java's `TreeSet<T>`.

```csharp
var set = new SortedSet<int> { 5, 2, 8, 1, 9, 3, 5, 2 };
// Duplicates removed, sorted: { 1, 2, 3, 5, 8, 9 }

// ── Min/Max ───────────────────────────────────────────────────
Console.WriteLine(set.Min);   // 1
Console.WriteLine(set.Max);   // 9

// ── Range queries — SortedSet exclusive power! ────────────────
SortedSet<int> range = set.GetViewBetween(3, 8);
// Returns view of elements: { 3, 5, 8 }  (inclusive)

// ── All HashSet set operations available ──────────────────────
set.Add(7);
set.Remove(1);
bool has5 = set.Contains(5);

var setA = new SortedSet<int> { 1, 2, 3, 4, 5 };
var setB = new SortedSet<int> { 4, 5, 6, 7, 8 };
setA.UnionWith(setB);        // { 1,2,3,4,5,6,7,8 } — sorted
setA.IntersectWith(setB);    // { 4,5 }

// ── Reverse iteration ─────────────────────────────────────────
foreach (int n in set.Reverse())
    Console.Write(n + " ");  // 9 8 5 3 2 (descending)
```

---

## 12. PriorityQueue<T,TPriority> — Heap-Based Queue

### Definition

Elements are dequeued in **priority order** (lowest priority number first — min-heap).
Added in .NET 6. Equivalent to Java's `PriorityQueue<T>`.

### Internal Working — Binary Min-Heap

```
PriorityQueue uses a BINARY MIN-HEAP stored in an array.

Min-Heap property:
  Parent's priority ≤ both children's priorities
  Minimum is always at the ROOT (index 0)

Array representation of heap:
  Parent of index i  = (i - 1) / 2
  Left child         = 2*i + 1
  Right child        = 2*i + 2

Example: Enqueue (Task, Priority):
  ("Email", 3), ("Deploy", 1), ("Meeting", 2), ("Review", 5), ("Fix Bug", 1)

Heap array:
  [("Deploy",1), ("Fix Bug",1), ("Meeting",2), ("Email",3), ("Review",5)]
  index:  0            1              2              3             4

Tree view:
                  Deploy(1)
                /           \
           Fix Bug(1)      Meeting(2)
           /     \
       Email(3)  Review(5)

Dequeue() → returns Deploy(1) (minimum)
  Then heap re-heapifies → Fix Bug(1) moves to top
  O(log n) — sifts down to maintain heap property
```

### Syntax & Usage

```csharp
// PriorityQueue<TElement, TPriority>
// Lower priority number = dequeued FIRST (min-heap)
var pq = new PriorityQueue<string, int>();

// ── Adding ────────────────────────────────────────────────────
pq.Enqueue("Send Email",      3);
pq.Enqueue("Deploy to Prod",  1);  // highest priority
pq.Enqueue("Team Meeting",    2);
pq.Enqueue("Code Review",     5);  // lowest priority
pq.Enqueue("Fix Critical Bug",1);  // same priority as Deploy

// ── Dequeue — always gets MINIMUM priority first ───────────────
Console.WriteLine(pq.Dequeue());   // "Deploy to Prod" or "Fix Critical Bug" (both 1)
Console.WriteLine(pq.Dequeue());   // the other priority-1 item
Console.WriteLine(pq.Dequeue());   // "Team Meeting" (priority 2)
Console.WriteLine(pq.Dequeue());   // "Send Email" (priority 3)
Console.WriteLine(pq.Dequeue());   // "Code Review" (priority 5)

// ── Peek (look without removing) ─────────────────────────────
pq.Enqueue("Task A", 1);
pq.Enqueue("Task B", 3);
string top = pq.Peek();           // "Task A" — priority 1, not removed

// ── Safe operations ───────────────────────────────────────────
if (pq.TryDequeue(out string item, out int priority))
    Console.WriteLine($"{item} (priority {priority})");

// ── Real use case: Dijkstra shortest path ─────────────────────
var distQueue = new PriorityQueue<int, int>();  // (nodeId, distance)
distQueue.Enqueue(startNode, 0);
while (distQueue.Count > 0)
{
    distQueue.TryDequeue(out int node, out int dist);
    // process shortest-distance node first
}
```

---

## 13. Concurrent Collections — Thread-Safe

### Why Regular Collections Fail in Multi-Threading

```csharp
// UNSAFE — race condition!
var dict = new Dictionary<string, int>();

// Thread 1                          Thread 2
dict["count"] = dict["count"] + 1;  dict["count"] = dict["count"] + 1;
// Both read 5, both write 6 → should be 7 but result is 6!

// SAFE solution:
var concDict = new ConcurrentDictionary<string, int>();
concDict.AddOrUpdate("count", 1, (key, old) => old + 1);  // atomic!
```

### ConcurrentDictionary<TKey,TValue>

```csharp
var dict = new ConcurrentDictionary<string, int>();

// Thread-safe add
dict.TryAdd("Alice", 90);

// Thread-safe update — atomic read-modify-write
dict.AddOrUpdate(
    key:            "Alice",
    addValue:       1,               // value if key doesn't exist
    updateValueFactory: (key, old) => old + 1  // new value if exists
);

// Get or add
int val = dict.GetOrAdd("Bob", 85); // adds if not exists, returns value

// Thread-safe read
dict.TryGetValue("Alice", out int score);

// Thread-safe remove
dict.TryRemove("Alice", out int removed);
```

### ConcurrentQueue`<T>`

```csharp
var queue = new ConcurrentQueue<string>();

// Multiple threads can safely Enqueue/Dequeue
queue.Enqueue("item1");          // thread-safe
queue.Enqueue("item2");

if (queue.TryDequeue(out string item))   // safe — no exception if empty
    Console.WriteLine(item);

queue.TryPeek(out string front); // safe peek
```

### BlockingCollection`<T>` — Producer-Consumer Pattern

```csharp
// Perfect for producer-consumer pattern
var buffer = new BlockingCollection<int>(boundedCapacity: 10);

// Producer thread
Task.Run(() =>
{
    for (int i = 0; i < 100; i++)
    {
        buffer.Add(i);          // BLOCKS if buffer is full (capacity=10)
        Thread.Sleep(10);
    }
    buffer.CompleteAdding();    // signal: no more items coming
});

// Consumer thread
Task.Run(() =>
{
    foreach (int item in buffer.GetConsumingEnumerable())
    {
        Console.WriteLine($"Processing: {item}");
        // BLOCKS waiting for items, exits when CompleteAdding() called
    }
});
```

### Concurrent Collections Summary

```
┌───────────────────────────┬────────────────────────┬──────────────────────────┐
│  Collection               │  Use Case              │  Java Equivalent         │
├───────────────────────────┼────────────────────────┼──────────────────────────┤
│  ConcurrentDictionary<K,V>│  Thread-safe key-value │  ConcurrentHashMap       │
│  ConcurrentQueue<T>       │  Thread-safe FIFO       │  ConcurrentLinkedQueue   │
│  ConcurrentStack<T>       │  Thread-safe LIFO       │  ConcurrentLinkedDeque   │
│  ConcurrentBag<T>         │  Thread-safe unordered  │  No direct equivalent    │
│  BlockingCollection<T>    │  Producer-consumer      │  BlockingQueue           │
└───────────────────────────┴────────────────────────┴──────────────────────────┘
```

---

## 14. Immutable Collections

### Definition

Once created,  **cannot be changed** . Any "modification" returns a  **new collection** .
Thread-safe by nature — no mutations, no race conditions.

```csharp
using System.Collections.Immutable;

// ── ImmutableList<T> ──────────────────────────────────────────
var list = ImmutableList.Create(1, 2, 3, 4, 5);

var list2 = list.Add(6);          // NEW list: [1,2,3,4,5,6]
var list3 = list2.Remove(3);      // NEW list: [1,2,4,5,6]
var list4 = list3.SetItem(0, 99); // NEW list: [99,2,4,5,6]

Console.WriteLine(list.Count);    // still 5 — ORIGINAL unchanged!

// ── ImmutableDictionary<K,V> ──────────────────────────────────
var dict = ImmutableDictionary.Create<string, int>()
    .Add("Alice", 90)
    .Add("Bob", 85);

var dict2 = dict.SetItem("Alice", 95);  // NEW dict — Alice=95
Console.WriteLine(dict["Alice"]);       // 90 — original unchanged

// ── ImmutableHashSet<T> ───────────────────────────────────────
var set  = ImmutableHashSet.Create(1, 2, 3);
var set2 = set.Add(4);     // new set: {1,2,3,4}
var set3 = set.Remove(2);  // new set: {1,3}
// Original set still {1,2,3}

// ── Builder pattern — efficient bulk construction ──────────────
var builder = ImmutableList.CreateBuilder<int>();
for (int i = 0; i < 1000; i++)
    builder.Add(i);                // mutable during build
ImmutableList<int> immutable = builder.ToImmutable();  // lock it
// Use builder when creating immutable collection from many operations
```

---

## 15. Non-Generic (Legacy) Collections

> **Avoid these. Use Generic versions. Listed here so you understand old code.**

```csharp
// ArrayList — old List<T>  (no type safety — everything is object → boxing!)
ArrayList list = new ArrayList();
list.Add(1);          // boxing int → object
list.Add("hello");    // can mix types — dangerous!
list.Add(3.14);
int x = (int)list[0]; // must cast — unboxing

// Use instead: List<int>, List<string>

// Hashtable — old Dictionary  (no type safety)
Hashtable ht = new Hashtable();
ht["key"] = "value";
string val = (string)ht["key"];  // must cast

// Use instead: Dictionary<string, string>

// Stack/Queue — non-generic (no type safety)
Stack oldStack = new Stack();  // stores objects — avoid
Queue oldQueue = new Queue();  // stores objects — avoid

// Use instead: Stack<T>, Queue<T>
```

---

## 16. IEnumerable, ICollection, IList — The Interfaces

### The Interface Hierarchy

```
IEnumerable<T>
│  GetEnumerator() — can iterate with foreach
│  Used for: LINQ, foreach loops, deferred execution
│
└── ICollection<T> : IEnumerable<T>
    │  Count, Add(), Remove(), Contains(), Clear()
    │  Used for: any collection you can count and modify
    │
    └── IList<T> : ICollection<T>
        │  this[index] — index access
        │  IndexOf(), Insert(), RemoveAt()
        │  Used for: ordered, index-accessible collections
        │
        └── concrete: List<T>, T[] (array)

IEnumerable<T>
└── IReadOnlyCollection<T>
    └── IReadOnlyList<T>    — index access but no modification
```

### Why This Matters — Program to Interface

```csharp
// DON'T do this (too specific):
void Process(List<int> data) { ... }

// DO this (accept any enumerable):
void Process(IEnumerable<int> data) { ... }
// Now you can pass: List<int>, int[], ImmutableList<int>, any LINQ result

// DO this if you need Count but not index:
void Process(ICollection<int> data)
{
    Console.WriteLine(data.Count);  // available
    data.Add(5);                    // available
}

// DO this if you need index access:
void Process(IList<int> data)
{
    Console.WriteLine(data[0]);     // index access available
}

// Return read-only from public methods:
public IReadOnlyList<string> GetNames()  // caller can read but not modify
{
    return _names.AsReadOnly();
}
```

### Implementing IEnumerable`<T>` — Custom Collection

```csharp
public class NumberRange : IEnumerable<int>
{
    private int _start, _end;

    public NumberRange(int start, int end)
    {
        _start = start;
        _end   = end;
    }

    // Must implement GetEnumerator
    public IEnumerator<int> GetEnumerator()
    {
        for (int i = _start; i <= _end; i++)
            yield return i;   // 'yield return' = lazy/deferred generation
    }

    // Required by non-generic IEnumerable
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// Usage
var range = new NumberRange(1, 5);
foreach (int n in range)
    Console.Write(n + " ");   // 1 2 3 4 5

// Works with ALL LINQ operators automatically!
var evens = range.Where(x => x % 2 == 0).ToList();  // [2, 4]
```

---

## 17. LINQ with Collections

### Definition

LINQ (Language Integrated Query) = SQL-like query syntax over any `IEnumerable<T>`.
Applied to all collections. Most powerful feature of C# collections ecosystem.

```csharp
var students = new List<Student>
{
    new Student { Name = "Sumit",   Age = 22, Score = 88, City = "Pune"   },
    new Student { Name = "Akash",   Age = 20, Score = 92, City = "Mumbai" },
    new Student { Name = "Rahul",   Age = 25, Score = 75, City = "Pune"   },
    new Student { Name = "Priya",   Age = 21, Score = 95, City = "Delhi"  },
    new Student { Name = "Vikram",  Age = 23, Score = 82, City = "Mumbai" },
};

// ── Filtering ─────────────────────────────────────────────────
var fromPune = students.Where(s => s.City == "Pune").ToList();
// [Sumit, Rahul]

// ── Projection (Select) ───────────────────────────────────────
var names = students.Select(s => s.Name).ToList();
// ["Sumit", "Akash", "Rahul", "Priya", "Vikram"]

var nameScore = students.Select(s => new { s.Name, s.Score }).ToList();
// [{ Name=Sumit, Score=88 }, ...]

// ── Ordering ──────────────────────────────────────────────────
var byScore    = students.OrderByDescending(s => s.Score).ToList();
var multiSort  = students.OrderBy(s => s.City).ThenBy(s => s.Name).ToList();

// ── Aggregation ───────────────────────────────────────────────
int    total   = students.Sum(s => s.Score);          // 432
double avg     = students.Average(s => s.Score);      // 86.4
int    max     = students.Max(s => s.Score);           // 95
int    min     = students.Min(s => s.Score);           // 75
int    count   = students.Count(s => s.Score > 80);   // 4

// ── Grouping ──────────────────────────────────────────────────
var byCity = students.GroupBy(s => s.City);
foreach (var group in byCity)
{
    Console.WriteLine($"City: {group.Key}");
    foreach (var s in group)
        Console.WriteLine($"  {s.Name}: {s.Score}");
}
// City: Pune
//   Sumit: 88
//   Rahul: 75
// City: Mumbai
//   Akash: 92
//   Vikram: 82
// City: Delhi
//   Priya: 95

// ── Searching ─────────────────────────────────────────────────
Student top    = students.First(s => s.Score == 95);        // Priya or throws
Student topSafe= students.FirstOrDefault(s => s.Score > 99);// null if not found
bool   any95  = students.Any(s => s.Score >= 95);           // true
bool   all75  = students.All(s => s.Score >= 75);           // true

// ── Flattening ────────────────────────────────────────────────
var courses = students.SelectMany(s => s.EnrolledCourses).Distinct().ToList();

// ── Pagination ────────────────────────────────────────────────
var page2 = students.Skip(2).Take(2).ToList();  // 3rd and 4th students

// ── Joining ───────────────────────────────────────────────────
var cityPopulation = new Dictionary<string, int>
{
    { "Pune", 7000000 }, { "Mumbai", 21000000 }, { "Delhi", 32000000 }
};

var enriched = students.Join(
    cityPopulation,
    s    => s.City,
    kvp  => kvp.Key,
    (s, kvp) => new { s.Name, s.City, Population = kvp.Value }
).ToList();

// ── Query syntax (SQL-like alternative) ───────────────────────
var query = from s in students
            where s.Score > 80
            orderby s.Score descending
            select new { s.Name, s.Score };

// Deferred execution — query runs WHEN you iterate, not when you define it
var query2 = students.Where(s => s.Score > 80);  // NOT executed yet
students.Add(new Student { Name = "New", Score = 95 });  // add after query def
var results = query2.ToList();  // NOW executed — includes "New"!
```

---

## 18. Choosing the Right Collection — Decision Guide

```
WHAT DO YOU NEED?
│
├── Ordered list of items?
│   ├── Fixed size → Array (int[])
│   └── Dynamic size?
│       ├── Access by index mainly → List<T>  ← DEFAULT CHOICE
│       └── Frequent insert/remove at both ends → LinkedList<T>
│
├── LIFO (last in, first out)?
│   └── Stack<T>
│
├── FIFO (first in, first out)?
│   └── Queue<T>
│
├── Priority-based ordering?
│   └── PriorityQueue<T, TPriority>
│
├── Key → Value lookup?
│   ├── No ordering needed, speed priority → Dictionary<K,V>  ← DEFAULT
│   ├── Need keys sorted? → SortedDictionary<K,V>
│   └── Need index access to sorted items → SortedList<K,V>
│
├── Unique items only?
│   ├── No ordering, speed priority → HashSet<T>
│   ├── Need items sorted? → SortedSet<T>
│   └── Need set math (union/intersect/diff)? → HashSet<T> or SortedSet<T>
│
├── Multi-threaded access?
│   ├── Key-Value → ConcurrentDictionary<K,V>
│   ├── FIFO → ConcurrentQueue<T>
│   ├── LIFO → ConcurrentStack<T>
│   └── Producer-Consumer → BlockingCollection<T>
│
└── Never change after creation?
    └── ImmutableList<T>, ImmutableDictionary<K,V>, etc.
```

### One-Line Rule for Each Collection

```
Array              → Fixed size, fastest index access
List<T>            → Your go-to dynamic list (like ArrayList in Java)
LinkedList<T>      → Frequent inserts/deletes at known positions
Stack<T>           → Undo, backtracking, DFS, call stack simulation
Queue<T>           → Requests, BFS, print queue, task processing
Dictionary<K,V>    → Key-value lookup — THE most used after List
HashSet<T>         → "Have I seen this?" — fast unique membership
SortedDictionary   → Like Dictionary but keys always alphabetical/sorted
SortedList         → Like SortedDictionary but also supports index access
SortedSet<T>       → Like HashSet but always sorted
PriorityQueue      → Process highest priority item first
ConcurrentDict     → Thread-safe Dictionary
BlockingCollection → Producer-consumer background task queues
ImmutableList      → Read-only sharing across threads, no defensive copy
```

---

## 19. Java vs C# Collections Master Map

```
┌────────────────────────────────┬────────────────────────────────┬──────────────┐
│  Java Collection               │  C# Equivalent                 │  Notes       │
├────────────────────────────────┼────────────────────────────────┼──────────────┤
│  ArrayList<T>                  │  List<T>                       │  Same idea   │
│  LinkedList<T>                 │  LinkedList<T>                 │  Both doubly │
│  Stack<T>                      │  Stack<T>                      │  Same        │
│  ArrayDeque<T> (as Queue)      │  Queue<T>                      │  FIFO        │
│  ArrayDeque<T> (as Deque)      │  LinkedList<T> (manual)        │  No Deque<T> │
│  HashMap<K,V>                  │  Dictionary<K,V>               │  Same idea   │
│  LinkedHashMap<K,V>            │  (no direct — use OrderedDict) │  Preserves   │
│                                │  or Dictionary (.NET 5+)       │  insertion   │
│  TreeMap<K,V>                  │  SortedDictionary<K,V>         │  Red-Black   │
│  HashSet<T>                    │  HashSet<T>                    │  Same        │
│  LinkedHashSet<T>              │  (no direct equivalent)        │  —           │
│  TreeSet<T>                    │  SortedSet<T>                  │  Red-Black   │
│  PriorityQueue<T>              │  PriorityQueue<T,TPriority>    │  Min-heap    │
│  ConcurrentHashMap             │  ConcurrentDictionary<K,V>     │  Thread-safe │
│  CopyOnWriteArrayList          │  ImmutableList<T>              │  Similar use │
│  BlockingQueue                 │  BlockingCollection<T>         │  Same idea   │
│  Collections.unmodifiableList  │  List.AsReadOnly() /           │  Read-only   │
│                                │  ImmutableList<T>              │  wrapper     │
│  int[] / Integer[]             │  int[] / int[] (no boxing!)    │  C# better   │
│  Arrays.asList()               │  new List<T> { } or ToList()   │  Similar     │
│  Collections.sort()            │  list.Sort() or LINQ OrderBy   │  Both O(n log n) │
│  Stream API (map/filter)       │  LINQ (.Where/.Select/.Sum)    │  Very similar│
│  Iterator<T>                   │  IEnumerator<T>                │  Same concept│
│  Iterable<T>                   │  IEnumerable<T>                │  Same concept│
└────────────────────────────────┴────────────────────────────────┴──────────────┘
```

---

## 20. Interview Cheat Sheet

```
Q: What is the difference between Array and List<T>?
A: Array is fixed size, contiguous memory, fastest O(1) index access.
   List<T> is dynamic — resizes by doubling when full.
   Both are O(1) index access. Use List<T> when size is not known upfront.

Q: How does List<T> resize internally?
A: It starts with capacity 4. When full, it creates a new array of DOUBLE
   the size and copies all elements. This is O(n) but happens rarely —
   amortized O(1) per Add. Pre-allocate with new List<T>(n) if size known.

Q: How does Dictionary<K,V> work internally?
A: Uses a hash table. Key's GetHashCode() → modulo bucket count → bucket index.
   Stores entries in a separate array. Handles collisions via chaining (linked
   next pointers in the entry). Resizes when count/capacity > 0.72 (load factor).
   Lookup: hash key → find bucket → scan chain for matching Equals(). O(1) avg.

Q: What is the difference between Dictionary and SortedDictionary?
A: Dictionary uses hash table → O(1) operations, unordered.
   SortedDictionary uses Red-Black Tree → O(log n) operations, keys always sorted.
   Use Dictionary for speed. SortedDictionary when sorted iteration is needed.

Q: What is the difference between SortedDictionary and SortedList?
A: SortedDictionary = Red-Black tree → O(log n) insert/delete, no index access.
   SortedList = two sorted arrays → O(n) insert/delete, O(1) index access, less memory.
   Use SortedList for read-heavy, small/static datasets that need index access.

Q: When would you use LinkedList<T> over List<T>?
A: When you have a node reference and need O(1) insert/remove at that position.
   List<T> needs O(n) to insert in middle (shifting). LinkedList is O(1) with node.
   BUT: LinkedList has poor cache locality and O(n) index access.
   In practice, List<T> is faster for most real-world sizes due to cache effects.

Q: What is the difference between HashSet<T> and SortedSet<T>?
A: HashSet = hash table → O(1) add/remove/contains, unordered.
   SortedSet = Red-Black tree → O(log n) operations, always sorted.
   Both support set operations (union, intersect, except).

Q: How does HashSet<T> prevent duplicates?
A: Same as Dictionary — hash the element, find the bucket, compare with Equals().
   If a matching element exists → Add() returns false, item NOT added.
   Custom objects: must override GetHashCode() AND Equals().

Q: What is the difference between Stack<T> and Queue<T>?
A: Stack = LIFO (Last In First Out) → Push/Pop → use for undo, DFS, brackets.
   Queue = FIFO (First In First Out) → Enqueue/Dequeue → use for BFS, requests.
   Both use internal array. Stack uses top index. Queue uses circular buffer.

Q: Why should you avoid non-generic collections (ArrayList, Hashtable)?
A: They store everything as 'object' → boxing/unboxing for value types → slow.
   No type safety at compile time → runtime errors.
   Use List<T>, Dictionary<K,V> etc. — type-safe, no boxing, better performance.

Q: What is boxing and how does it affect collections?
A: Boxing = wrapping a value type (int) in a heap object.
   ArrayList.Add(5) → boxes 5 to object → heap allocation → slow.
   List<int>.Add(5) → stores directly as int → no boxing → fast.
   Generics eliminate boxing for value types.

Q: What is deferred execution in LINQ?
A: LINQ queries are NOT executed when defined — only when iterated (ToList, foreach).
   var q = list.Where(x => x > 5); // not executed yet
   q.ToList(); // NOW it executes
   Advantage: compose queries without multiple passes.

Q: How do you choose between ConcurrentDictionary and Dictionary with lock?
A: ConcurrentDictionary: better for high read/write ratio, uses fine-grained
   locking (per-bucket). lock(dict): simple but locks entire collection.
   Use ConcurrentDictionary when multiple threads frequently access different keys.

Q: What must custom objects implement to be used as Dictionary keys?
A: Must override GetHashCode() and Equals().
   GetHashCode(): equal objects MUST return same hash.
   Equals(): used for collision resolution — is this the exact key?
   If GetHashCode is inconsistent, keys become unfindable in the dictionary!
```

---

*📌 C# Collections Complete Notes | ASP.NET Core MVC Learning Series | Sumit Tikone | March 2026*
