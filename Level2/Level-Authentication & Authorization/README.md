# 🔐 Codeveda Task: ASP.NET MVC Authentication with Google Login

## 📌 Overview

This project demonstrates **authentication and authorization** in an ASP.NET MVC application using:

* **ASP.NET Identity (Individual Accounts)**
* **Google External Login (OAuth 2.0)**

It follows **real-world security best practices**, including proper handling of sensitive data and clean project architecture.

---

## 🚀 Features

### 🔑 1. ASP.NET Identity Authentication

* Built-in authentication system:

  * Login
  * Register
  * Logout
* Cookie-based authentication
* Secure password hashing & storage

---

### 🌐 2. Google External Login Integration

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
```

* Login using Google account
* OAuth 2.0 integration
* Secure redirect handling

---

### 🔒 3. Secure Secret Management

Sensitive data such as:

* `Google ClientId`
* `Google ClientSecret`

❌ **Never stored in source code**

✔ Instead, use:

#### 👉 User Secrets (Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

#### 👉 Environment Variables (Production)

---

### 🛡️ 4. Authorization (Protecting Pages)

```csharp
[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

* Only authenticated users can access protected routes

---

### 📁 5. Git Best Practices

`.gitignore` excludes:

```
bin/
obj/
.vs/
secrets.json
appsettings.Development.json
```

✔ Prevents:

* Uploading sensitive data
* Committing temporary files
* Exposing local configurations

---

## 🗂️ Project Structure

```
Codeveda-auth/
│
├── Controllers/
├── Views/
├── Models/
├── wwwroot/
├── Program.cs
├── appsettings.json
├── .gitignore
└── Codeveda-auth.csproj
```

---

## ▶️ How to Run

1. **Clone the repository**

```bash
git clone <your-repo-url>
cd Codeveda-auth
```

2. **Setup User Secrets**

```bash
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

3. **Run the project**

```bash
dotnet run
```

4. **Open in browser**

```
https://localhost:{port}/Identity/Account/Login
```

---

## ⚠️ Security Notes (Important)

* ❌ Never expose secrets in source code or GitHub
* 🔄 Always regenerate credentials if leaked
* 🔐 Use secure storage for sensitive data in production

---

## 🎓 Learning Outcomes

By completing this project, you will understand:

* ASP.NET Identity authentication system
* External authentication using Google OAuth
* Secure handling of sensitive data
* Authentication middleware in ASP.NET
* Real-world security best practices

---

## ✅ Conclusion

This project demonstrates how to build a **secure, production-ready authentication system** using ASP.NET MVC with external login providers.

It follows **industry best practices** for:

* Security
* Code organization
* Secret management

---

## ⭐ Support

If you found this project helpful, consider giving it a ⭐ on GitHub!
