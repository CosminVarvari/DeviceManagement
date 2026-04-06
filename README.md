# 📱 Device Management System

A full-stack enterprise application for tracking company-owned mobile devices, their specifications, locations, and assignments. Built with **ASP.NET Core 8**, **Angular 15**, and **MS SQL Server**.


## ✅ Prerequisites

Make sure you have the following installed before running the project:

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| Node.js | 18.x+ | https://nodejs.org |
| Angular CLI | 15.x | `npm install -g @angular/cli@15` |
| SQL Server | 2019/2022 | https://www.microsoft.com/sql-server |
| SQL Server Management Studio | Any | https://aka.ms/ssmsfullsetup |
| Git | Any | https://git-scm.com |

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/CosminVarvari/DeviceManagement.git
cd DeviceManagement
```

### 2. Set Up the Database

Open **SQL Server Management Studio (SSMS)** and run the following scripts **in order**:

```
DeviceManagement.Infrastructure/Scripts/001_CreateDatabase.sql
DeviceManagement.Infrastructure/Scripts/002_SeedData.sql
```

> Both scripts are **idempotent** — safe to run multiple times without side effects.

This will create:
- The `DeviceManagement` database
- `Users` and `Devices` tables with all constraints
- Sample data including 4 users and 10 devices

### 3. Configure the Backend

Open `DeviceManagement.API/appsettings.json` and update the following:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DeviceManagement;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "DeviceManagement.API",
    "Audience": "DeviceManagement.Client"
  },
  "Groq": {
    "ApiKey": "YOUR_GROQ_API_KEY_HERE",
    "Model": "llama-3.3-70b-versatile"
  }
}
```

> **Getting a Groq API Key (free):**
> 1. Go to https://console.groq.com
> 2. Sign up for a free account
> 3. Navigate to **API Keys** → **Create API Key**
> 4. Copy the key and paste it in `appsettings.json`

> **Connection String:** If your SQL Server uses a named instance (e.g. `SQLEXPRESS`), update to:
> `"Server=localhost\\SQLEXPRESS;..."`

### 4. Run the Backend

```bash
cd DeviceManagement.API
dotnet restore
dotnet run
```

The API will start at: **http://localhost:5208**

Swagger UI (API documentation) available at: **http://localhost:5208/swagger**

### 5. Configure the Frontend

Open `DeviceManagement.Angular/src/environments/environment.ts` and verify:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5208/api'
};
```

> Update the port if your API runs on a different port.

### 6. Run the Frontend

Open a **new terminal**:

```bash
cd DeviceManagement.Angular
npm install
ng serve
```

The application will be available at: **http://localhost:4200**

---

## 🧪 Running the Tests

```bash
cd DeviceManagement
dotnet test --verbosity normal
```

The test suite includes **22 integration tests** covering:
- Auth endpoints (register, login, validation)
- Device CRUD operations
- Device assignment and unassignment
- Error handling (404, 409, 401, 400)

> Tests use an **in-memory database** — no SQL Server required to run tests.

---

## 📄 License

This project was built as part of a technical assessment. All rights reserved.
