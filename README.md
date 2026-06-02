# Patient Management System

A full-stack web application built with ASP.NET MVC and SQL Server 
to manage patients, doctors, and appointments — simulating a 
real-world pharma client requirement.

## Technologies Used
- ASP.NET MVC (.NET 8.0)
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
- 🤖 AI Symptom Checker - describes symptoms, 
     AI suggests doctor specialization
- 💬 AI Dashboard Chatbot - ask questions about 
     your data in plain English
- ⚙️ CI/CD pipeline via GitHub Actions
- 🔒 Secure API key management via .env and 
     GitHub Secrets


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

## Api Key
   -Rename .env.example ---> .env
   -Generate tour oen personal key from Groq 
   -Paste and Replace "your_groq_key_here"

## Docker setup 
- clone the repo.
- Create a .env file in the project root(refer Api Key)
- Build and start the containers:
- docker compose up --build
- Once the containers are running, open:
    http://localhost:5000

## Author
Devesh Kaushik