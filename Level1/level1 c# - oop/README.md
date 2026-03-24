# 🎓 C# Basics & OOP – Student Management System

## 📌 Overview

The **Student Management System** is a console-based application developed using **C#** as part of the **Codveda Technology .NET Development Internship**.

This project demonstrates the **core fundamentals of Object-Oriented Programming (OOP)** through a simple and interactive console application.

Users can:

* ➕ Add new students
* 📋 View stored student records

---

## 🚀 Features

* Add new student
* View all students
* Simple and user-friendly console interface
* Clean and structured code
* Practical implementation of OOP principles

---

## 🧠 Concepts Covered

This project showcases essential C# concepts:

* C# syntax
* Variables and data types
* Classes and objects
* Constructors
* Methods and properties
* Access modifiers
* Encapsulation
* Inheritance
* Polymorphism

---

## 🏗️ OOP Implementation

### 🔒 Encapsulation

The `StudentService` class encapsulates the student list and exposes controlled methods to manage data.

---

### 🧬 Inheritance

The `Student` class inherits from the `Person` base class:

```csharp id="w1s92k"
public class Student : Person
{
    // Additional properties and methods
}
```

---

### 🔁 Polymorphism

The `DisplayInfo()` method is overridden in the `Student` class to provide specific behavior:

```csharp id="r4k8ps"
public override void DisplayInfo()
{
    Console.WriteLine($"Student: {Name}, Age: {Age}");
}
```

---

## 🛠️ Technologies Used

* **C#**
* **.NET**
* **Console Application**
* **Object-Oriented Programming (OOP)**

---

## 📁 Project Structure

```id="z8x1mt"
StudentManagementSystem/
│
├── Models/
│   ├── Person.cs
│   └── Student.cs
├── Services/
│   └── StudentService.cs
├── Program.cs
└── StudentManagementSystem.csproj
```

---

## ▶️ How to Run

1. **Clone the repository**

```bash id="k2l9ds"
git clone <your-repo-url>
cd StudentManagementSystem
```

2. **Run the application**

```bash id="x7d0qp"
dotnet run
```

---

## 🎓 Learning Outcomes

By completing this project, you will:

* Understand the fundamentals of **OOP in C#**
* Learn how to structure a console application
* Apply **encapsulation, inheritance, and polymorphism** in real scenarios
* Write clean and maintainable code

---

## 🏢 Internship

This project was completed as part of the **Codveda Technology .NET Development Internship**, focusing on building strong foundations in C# and OOP principles.

---

## ✅ Conclusion

The **Student Management System** is a beginner-friendly project that builds a solid understanding of:

* Core C# programming
* Object-Oriented Design
* Real-world coding practices

It serves as a strong foundation for progressing into more advanced .NET development.

---

## ⭐ Support

If you found this project helpful, consider giving it a ⭐ on GitHub!
