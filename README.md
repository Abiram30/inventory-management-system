# Inventory Management System (IMS)

Full-stack web application for tracking warehouse inventory, stock levels, stock adjustments, purchase orders, and analytics reports.

## Tech Stack
- **Frontend:** React, TypeScript, Vite, Tailwind CSS, Recharts, Redux Toolkit
- **Backend:** C# .NET Core Web API, Entity Framework Core
- **Database:** MySQL

## Run Locally

### 1. Database Setup
1. Import `project/DB_project.sql` into MySQL (database: `inventory_db`).
2. Update the MySQL connection string in `project/Inventory.Api/appsettings.json` (or `appsettings.Development.json`).

### 2. Start Backend API
```bash
cd project/Inventory.Api
dotnet run
