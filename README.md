# EMSAportesEnLinea
Sistema de Gestión de Empleados  AportesEnLinea
# .NET 8 Project with Entity Framework and SQL Server
<div align="center">
  
 <img src="https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet" alt=".NET"/>
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server"/>
  <img src="https://img.shields.io/badge/Entity_Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#"/>
</div>

## Prerequisites

Before you begin, ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express Edition or Developer Edition recommended)
- [Git](https://git-scm.com/downloads) (optional)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or VS Code

# Getting Started

## 1. Clone the Repository

## 2. Configure Database Connection
Locate the configuration file: appsettings.json 

Update the connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=your-server-name;Database=EMSDB;User Id=your-username;Password=your-password;TrustServerCertificate=true;"
}

## 3. Apply Database Migrations
### Using .NET CLI:

dotnet restore

dotnet ef database update

## 4. Run the Application
