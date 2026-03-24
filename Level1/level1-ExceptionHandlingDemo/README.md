# ⚠️ Codeveda Task 3: Exception Handling & Logging

## 📌 Overview

This project demonstrates **advanced exception handling and logging** in a C# Console Application.

It is designed to showcase **senior-level coding practices** that improve:

* Application reliability
* Debugging efficiency
* Error traceability

---

## 🚀 Features

### 🧱 1. Structured Exception Handling

* Uses `try-catch-finally` blocks to handle runtime errors safely
* Prevents application crashes
* Ensures proper cleanup of resources

---

### ❗ 2. Custom Exception Handling

A custom exception is implemented to handle business-specific errors:

```csharp
public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string message) : base(message) { }
}
```

#### ✅ Benefits:

* Clear separation of **business logic errors**
* Improved readability and maintainability
* More meaningful error messages

---

### 🐞 3. Debugging-Friendly Structure

* Designed for easy debugging using:

  * Breakpoints
  * Watch variables
* Clean flow helps identify issues quickly

---

### 📝 4. Logging with Serilog

Integrated **Serilog** for structured logging:

* Logs written to:

  * Console
  * File

#### Example Setup:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

#### ✅ Advantages:

* Persistent logs for production debugging
* Easy monitoring of application behavior
* Structured and readable log output

---

## 📁 Project Structure

```
Codeveda-ExceptionHandling/
│
├── Program.cs
├── Services/
├── Exceptions/
├── Logs/
└── Codeveda-ExceptionHandling.csproj
```

---

## ▶️ How to Run

1. **Clone the repository**

```bash
git clone <your-repo-url>
cd Codeveda-ExceptionHandling
```

2. **Restore dependencies**

```bash
dotnet restore
```

3. **Run the application**

```bash
dotnet run
```

---

## ⚠️ Best Practices Applied

* ✔ Proper use of exception hierarchy
* ✔ Avoiding generic exception misuse
* ✔ Logging instead of silent failures
* ✔ Clean and maintainable code structure

---

## 🎓 Learning Outcomes

By completing this project, you will learn:

* How to implement robust exception handling in C#
* Creating and using custom exceptions
* Logging with Serilog in real-world applications
* Writing maintainable and production-ready code

---

## ✅ Conclusion

This project demonstrates how to build a **reliable and maintainable console application** using:

* Structured exception handling
* Custom business exceptions
* Professional logging practices

It reflects **real-world development standards** used in production systems.

---

## ⭐ Support

If you found this project helpful, consider giving it a ⭐ on GitHub!
