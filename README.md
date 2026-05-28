# Patient Management System

A full-stack web application built with ASP.NET MVC and SQL Server 
to manage patients, doctors, and appointments — simulating a 
real-world pharma client requirement.

## Technologies Used
- ASP.NET MVC (.NET 10)
- C#
- SQL Server 2025 Express
- SSMS (SQL Server Management Studio)
- Bootstrap 5
- Chart.js
- GitHub Actions (CI/CD)
- Microsoft Azure (upcoming)
- Docker (upcoming)

## Features
- 📊 Dashboard with live stats and bar chart
- 🏥 Patient management - Add, View, Delete
- 👨‍⚕️ Doctor management - Add, View, Delete
- 📅 Appointment booking - Book, View, Cancel
- 🔗 JOIN queries linking patients and doctors
- ⚙️ CI/CD pipeline via GitHub Actions

## Architecture
Browser → Controller → Model → SQL Server
↑                              ↓
└──────── View (Bootstrap) ←──┘

## How to Run Locally
1. Clone the repo
2. Set up SQL Server and run database setup:
```sql
CREATE DATABASE PatientManagementDB;
-- Then run the table creation scripts
```

3. Update connection string in appsettings.json:
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;
Database=PatientManagementDB;
Trusted_Connection=True;
TrustServerCertificate=True"
```

4. Run the app: dotnet run
5. Open browser: http://localhost:5000

## CI/CD Pipeline
Every push to main branch automatically:
- Builds the project
- Publishes release version
- Shows build status on GitHub

## Project Status
✅ Fully functional locally
🚧 Azure deployment - upcoming
🚧 Docker containerization - upcoming

## Author
Devesh Kaushik