# Products Catalog API

Headless backend application for managing a products catalog built with .NET Core, Entity Framework, and SQL Server.

## 🚀 Features

- RESTful API for product management
- Full CRUD operations
- SQL Server database with Entity Framework Core
- API documentation (Swagger/OpenAPI)
- Scalable architecture

## 🛠️ Technologies

- **.NET Core** - Backend framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **Swagger** - API documentation

## 📋 Prerequisites

Before running this project, make sure you have:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or higher)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server Express
- Your favorite IDE (Visual Studio, VS Code, Rider)

## 🔧 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Kiriar21/products-catalog.git
   cd products-catalog
   ```

2. **Configure database connection**
   
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ProductsCatalog;Trusted_Connection=True;"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   
   Navigate to: `https://localhost:5001/swagger` (or the port shown in your console)

## 📚 API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

## 🗄️ Database Schema

### Products Table

```sql
- Id (int, Primary Key)
- Name (string)
- Description (string)
- Price (decimal)
- Category (string)
- Stock (int)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

## 🔐 Configuration

Edit `appsettings.json` to configure:

- Database connection string
- Logging levels
- CORS policies
- API settings

## 🧪 Testing

Run tests with:
```bash
dotnet test
```

## 📦 Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Kiriar21**

- GitHub: [@Kiriar21](https://github.com/Kiriar21)

## 📞 Support

If you have any questions or issues, please open an issue on GitHub.

---

⭐ If you like this project, give it a star on GitHub!