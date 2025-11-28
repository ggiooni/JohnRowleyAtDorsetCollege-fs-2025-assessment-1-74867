# Dublin Bikes API - Full Stack Development Assignment 1

**Student Number:** 74867
**Module:** Full Stack Development (M3.3)
**Assignment:** Dublin Bikes API - Assessment 1
**Submission Date:** November 2025

---

## 📋 Project Overview

A comprehensive .NET 8 Web API for managing Dublin Bikes station data with real-time updates, intelligent caching, and extensive filtering capabilities.

---

## ✨ Key Features

- ✅ **API Versioning:** V1 (JSON file-based) and V2 (CosmosDB-ready)
- ✅ **In-Memory Caching:** 5-minute cache for optimal performance
- ✅ **Real-Time Updates:** Background service updates station data every 15 seconds
- ✅ **Advanced Filtering:** Filter by status, minimum bikes, search by name/address
- ✅ **Sorting & Pagination:** Sort by name, bikes, or occupancy with pagination support
- ✅ **CRUD Operations:** Create, read, and update station data
- ✅ **Comprehensive Testing:** 15 unit tests with 100% pass rate
- ✅ **Postman Collection:** 15 pre-configured requests with automated tests
- ✅ **Swagger Documentation:** Interactive API documentation

---

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- Postman (optional, for API testing)

### Running the API

```bash
cd DublinBikesApi
dotnet restore
dotnet build
dotnet run
```

The API will be available at:
- **HTTPS:** https://localhost:7000
- **HTTP:** http://localhost:5000
- **Swagger UI:** https://localhost:7000/swagger

### Running Tests

```bash
cd DublinBikesApi.Tests
dotnet test
```

Expected: ✅ 15/15 tests passing

---

## 📁 Project Structure

```
fs-2025-assessment-1-74867/
├── DublinBikesApi/              # Main API project
│   ├── Controllers/             # API controllers (V1 & V2)
│   ├── Models/                  # Domain models
│   ├── Services/                # Business logic & background services
│   ├── DTOs/                    # Data transfer objects
│   ├── Data/                    # JSON data file
│   ├── Postman/                 # Postman collection & environment
│   ├── Program.cs               # Application entry point
│   ├── README.md                # Detailed API documentation
│   └── QUICKSTART.md            # Quick start guide
│
└── DublinBikesApi.Tests/        # Unit tests project
    ├── Services/                # Service tests
    └── DublinBikesApi.Tests.csproj
```

---

## 📚 Documentation

- **[API Documentation](DublinBikesApi/README.md)** - Complete API reference with examples
- **[Quick Start Guide](DublinBikesApi/QUICKSTART.md)** - Fast setup instructions
- **[Postman Collection](DublinBikesApi/Postman/)** - Ready-to-use API requests

---

## 🧪 Testing

### Unit Tests
```bash
cd DublinBikesApi.Tests
dotnet test --verbosity normal
```

### Postman Tests
1. Import `DublinBikesApi/Postman/DublinBikesAPI.postman_collection.json`
2. Import `DublinBikesApi/Postman/DublinBikesAPI.postman_environment.json`
3. Select environment "Dublin Bikes API - Local"
4. Run collection

---

## 📊 API Endpoints

### V1 Endpoints (JSON File Based)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/stations` | Get all stations with filtering & pagination |
| GET | `/api/v1/stations/{number}` | Get specific station by number |
| GET | `/api/v1/stations/summary` | Get aggregate statistics |
| POST | `/api/v1/stations` | Create new station |
| PUT | `/api/v1/stations/{number}` | Update existing station |

### V2 Endpoints (CosmosDB Ready)
Same endpoints as V1, ready for database integration.

### Health Check
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | API health status |

---

## 🎯 Assignment Requirements Met

- ✅ Load JSON data at application startup
- ✅ V1 endpoints (JSON file-based)
- ✅ V2 endpoints (CosmosDB-ready)
- ✅ API versioning implementation
- ✅ In-memory caching (5 minutes)
- ✅ Background service (15-second updates)
- ✅ Filtering, searching, sorting, and pagination
- ✅ Create and update operations
- ✅ Date/time conversion (Europe/Dublin timezone)
- ✅ Unit tests with high coverage
- ✅ Postman collection with automated tests
- ✅ Comprehensive documentation

---

## 🛠️ Technologies Used

- **.NET 8.0** - Framework
- **ASP.NET Core Web API** - API framework
- **Swashbuckle** - Swagger/OpenAPI documentation
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Test assertions

---

## 👨‍💻 Author

**Student Number:** 74867
**Course:** Full Stack Development
**Institution:** Dorset College

---

## 📝 License

This project is submitted as part of academic coursework.

---

## 🎓 Learning Outcomes Achieved

- ✅ **MIMLO 1:** Evaluate requirements for developing and deploying full-stack web applications
- ✅ **MIMLO 4:** Connect reactive UI to 3rd party API libraries and render data collections appropriately

---

For detailed API documentation, see [DublinBikesApi/README.md](DublinBikesApi/README.md)
