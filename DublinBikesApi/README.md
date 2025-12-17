## Dublin Bikes API

A .NET 8 Web API for managing Dublin Bikes station data with real-time updates, caching, and comprehensive filtering capabilities.

## Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Architecture](#architecture)
- [Testing](#testing)
- [Postman Collection](#postman-collection)
- [Design Decisions](#design-decisions)

## Features

- API Versioning: Two versions (V1: JSON file-based, V2: CosmosDB-ready)
- In-Memory Caching: 5-minute cache for improved performance
- Real-Time Updates: Background service simulates live station data updates every 15 seconds
- Comprehensive Filtering: Filter by status, minimum bikes, search by name/address
- Sorting: Sort by name, available bikes, or occupancy
- Pagination: Configurable page size with navigation support
- CRUD Operations: Create, read, update station data
- Automated Tests: 15 unit tests with 100% pass rate
- Swagger Documentation: Interactive API documentation

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or JetBrains Rider (optional)
- Postman (for API testing)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ggiooni/JohnRowleyAtDorsetCollege-fs-2025-assessment-1-74867.git
cd DublinBikesApi
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the API

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7000`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7000/swagger`

### 5. Run Tests

```bash
cd ../DublinBikesApi.Tests
dotnet test
```

## API Documentation

### Base URLs

- V1 (JSON File): `/api/v1/stations`
- V2 (CosmosDB): `/api/v2/stations`

### Endpoints

#### GET /api/v{version}/stations

Get all stations with optional filtering, searching, sorting, and paging.

Query Parameters:
- `status` (string): Filter by status (OPEN/CLOSED)
- `minBikes` (int): Minimum available bikes
- `q` (string): Search term (searches name and address)
- `sort` (string): Sort field (name, availableBikes, occupancy)
- `dir` (string): Sort direction (asc/desc)
- `page` (int): Page number (default: 1)
- `pageSize` (int): Items per page (default: 10, max: 100)

Example Requests:

```bash
# Get all stations
curl https://localhost:7000/api/v1/stations

# Get only OPEN stations
curl https://localhost:7000/api/v1/stations?status=OPEN

# Get stations with at least 5 bikes
curl https://localhost:7000/api/v1/stations?minBikes=5

# Search for stations containing "Street"
curl https://localhost:7000/api/v1/stations?q=street

# Sort by name ascending
curl https://localhost:7000/api/v1/stations?sort=name&dir=asc

# Get page 2 with 20 items per page
curl https://localhost:7000/api/v1/stations?page=2&pageSize=20

# Combine filters
curl https://localhost:7000/api/v1/stations?status=OPEN&minBikes=5&sort=occupancy&dir=desc&page=1&pageSize=10
```

Response Example:

```json
{
  "data": [
    {
      "number": 42,
      "name": "SMITHFIELD NORTH",
      "address": "Smithfield North",
      "latitude": 53.349562,
      "longitude": -6.278198,
      "bikeStands": 30,
      "availableBikes": 15,
      "availableBikeStands": 15,
      "status": "OPEN",
      "occupancy": 50.0,
      "lastUpdateLocal": "2024-10-16T10:45:38",
      "lastUpdateEpoch": 1729065138000
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 95,
  "totalPages": 10,
  "hasPrevious": false,
  "hasNext": true
}
```

#### GET /api/v{version}/stations/{number}

Get a specific station by its number.

Example Request:

```bash
curl https://localhost:7000/api/v1/stations/42
```

Response (200 OK):

```json
{
  "number": 42,
  "name": "SMITHFIELD NORTH",
  "address": "Smithfield North",
  "latitude": 53.349562,
  "longitude": -6.278198,
  "bikeStands": 30,
  "availableBikes": 15,
  "availableBikeStands": 15,
  "status": "OPEN",
  "occupancy": 50.0,
  "lastUpdateLocal": "2024-10-16T10:45:38",
  "lastUpdateEpoch": 1729065138000
}
```

Response (404 Not Found):

```json
{
  "message": "Station with number 99999 not found"
}
```

#### GET /api/v{version}/stations/summary

Get aggregate statistics for all stations.

Example Request:

```bash
curl https://localhost:7000/api/v1/stations/summary
```

Response:

```json
{
  "totalStations": 95,
  "totalBikeStands": 2850,
  "totalAvailableBikes": 1425,
  "totalAvailableBikeStands": 1425,
  "stationsByStatus": {
    "OPEN": 93,
    "CLOSED": 2
  },
  "averageOccupancy": 50.0
}
```

#### POST /api/v{version}/stations

Create a new station.

Request Body:

```json
{
  "number": 200,
  "name": "New Station",
  "address": "New Address",
  "latitude": 53.34,
  "longitude": -6.26,
  "bikeStands": 30,
  "availableBikes": 15,
  "status": "OPEN"
}
```

Response (201 Created):

```json
{
  "number": 200,
  "name": "New Station",
  "address": "New Address",
  "latitude": 53.34,
  "longitude": -6.26,
  "bikeStands": 30,
  "availableBikes": 15,
  "availableBikeStands": 15,
  "status": "OPEN",
  "occupancy": 50.0,
  "lastUpdateLocal": "2024-10-16T11:00:00",
  "lastUpdateEpoch": 1729066000000
}
```

#### PUT /api/v{version}/stations/{number}

Update an existing station.

Request Body:

```json
{
  "name": "Updated Station Name",
  "availableBikes": 20
}
```

Response (200 OK):

```json
{
  "number": 42,
  "name": "Updated Station Name",
  "address": "Smithfield North",
  "latitude": 53.349562,
  "longitude": -6.278198,
  "bikeStands": 30,
  "availableBikes": 20,
  "availableBikeStands": 10,
  "status": "OPEN",
  "occupancy": 66.67,
  "lastUpdateLocal": "2024-10-16T11:05:00",
  "lastUpdateEpoch": 1729066300000
}
```

#### GET /health

Health check endpoint.

Response:

```json
{
  "status": "Healthy",
  "timestamp": "2024-10-16T11:00:00Z"
}
```

## Architecture

### Project Structure

```
DublinBikesApi/
├── Controllers/
│   ├── V1/
│   │   └── StationsController.cs
│   └── V2/
│       └── StationsController.cs
├── Models/
│   ├── Station.cs
│   └── Position.cs
├── DTOs/
│   ├── StationDto.cs
│   ├── PagedResponse.cs
│   ├── StationsSummaryDto.cs
│   ├── CreateStationDto.cs
│   └── UpdateStationDto.cs
├── Services/
│   ├── IStationService.cs
│   ├── StationService.cs
│   ├── ICacheService.cs
│   ├── CacheService.cs
│   └── StationUpdateBackgroundService.cs
├── Data/
│   └── dublinbike.json
├── Postman/
│   ├── DublinBikesAPI.postman_collection.json
│   └── DublinBikesAPI.postman_environment.json
└── Program.cs
```

### Key Components

#### 1. Models
- Station: Core domain model representing a bike station
- Position: Geographical coordinates (latitude/longitude)

#### 2. DTOs (Data Transfer Objects)
- StationDto: Response model for station data
- PagedResponse<T>: Generic paged response wrapper
- StationsSummaryDto: Aggregate statistics
- CreateStationDto: Request model for creating stations
- UpdateStationDto: Request model for updating stations

#### 3. Services

StationService
- Loads JSON data at startup
- Implements filtering, searching, sorting, and pagination
- Thread-safe operations using locks
- CRUD operations

CacheService
- In-memory caching using IMemoryCache
- 5-minute expiration for all cached data
- Automatic cache invalidation on data modifications

StationUpdateBackgroundService
- Inherits from BackgroundService
- Updates all stations every 15 seconds
- Simulates live data feed
- Clears cache after updates

#### 4. Controllers

Both V1 and V2 controllers provide identical endpoints:
- V1: Uses JSON file-based data
- V2: Prepared for CosmosDB integration (currently uses same service)

### Data Flow

1. Startup: StationService loads JSON data into memory
2. Request: Client makes API request
3. Cache Check: CacheService checks for cached response
4. Processing: If not cached, StationService processes request
5. Caching: Response is cached for 5 minutes
6. Background: Every 15 seconds, stations are updated and cache is cleared

### Time Handling

- last_update: Stored as Unix epoch milliseconds
- LastUpdateDateTime: Converted to DateTime (UTC)
- LastUpdateLocal: Converted to Europe/Dublin timezone (GMT Standard Time)

### Business Logic

Occupancy Calculation:
```csharp
Occupancy = (AvailableBikes / BikeStands) * 100
```
- Safe division (returns 0 if BikeStands is 0)
- Rounded to 2 decimal places

## Testing

### Unit Tests

The project includes 15 comprehensive unit tests covering:

- ✅ Get all stations
- ✅ Filter by status
- ✅ Filter by minimum bikes
- ✅ Search by name/address
- ✅ Sort by different fields
- ✅ Pagination
- ✅ Get station by number
- ✅ Get station summary
- ✅ Create new station
- ✅ Create duplicate station (error case)
- ✅ Update existing station
- ✅ Update non-existent station
- ✅ Update station availability

### Running Tests

```bash
cd DublinBikesApi.Tests
dotnet test
```

Expected Output:
```
Passed!  - Failed:     0, Passed:    15, Skipped:     0, Total:    15
```

## Postman Collection

### Importing the Collection

1. Open Postman
2. Click Import
3. Select `Postman/DublinBikesAPI.postman_collection.json`
4. Import `Postman/DublinBikesAPI.postman_environment.json`

### Running Tests

The collection includes automated tests for:
- Response status codes
- Response structure validation
- Data type validation
- Business logic validation
- Error handling

To run all tests:
1. Select the collection
2. Click Run
3. Select environment: "Dublin Bikes API - Local"
4. Click Run Dublin Bikes API

### Test Coverage

- V1 Endpoints: 11 requests with automated tests
- V2 Endpoints: 3 requests with automated tests
- Health Check: 1 request with automated tests

Total: 15 requests, 50+ automated test assertions

## Design Decisions

### 1. In-Memory Data Storage

Reason: Requirement specified no database for V1. Provides fast access and simplicity.

Trade-offs:
- ✅ Fast performance
- ✅ Simple implementation
- ❌ Data lost on restart
- ❌ Not suitable for production at scale

### 2. Singleton Service Lifetime

Reason: Data needs to be shared across all requests and maintained in memory.

Implementation:
```csharp
builder.Services.AddSingleton<IStationService, StationService>();
```

### 3. Thread-Safe Operations

Reason: Background service and multiple concurrent requests require thread safety.

Implementation: Using `lock` statements for all data modifications.

### 4. 5-Minute Cache Expiration

Reason: Balance between performance and data freshness.

Considerations:
- Background service updates every 15 seconds
- Cache ensures repeated queries don't hit service
- 5 minutes allows for reasonable freshness

### 5. API Versioning

Reason: Requirement specified V1 (JSON) and V2 (CosmosDB).

Implementation: URL-based versioning (`/api/v1/...`, `/api/v2/...`)

Benefits:
- Clear separation
- Easy to maintain
- Future-proof

### 6. DTO Pattern

Reason: Separate internal models from API contracts.

Benefits:
- Flexibility to change internal models
- Control over exposed data
- Clear API contracts

### 7. Validation Attributes

Reason: Ensure data integrity at API boundary.

Implementation:
```csharp
[Required]
[Range(1, 100)]
public int BikeStands { get; set; }
```

### 8. Background Service Update Interval

Chosen: 15 seconds

Reasoning:
- Requirement: 10-20 seconds
- 15 seconds provides good balance
- Frequent enough to simulate live data
- Not too aggressive on resources

## Assumptions

1. Station Numbers: Unique identifiers that don't change
2. Bike Stands: Total capacity can vary (simulated in background service)
3. Time Zone: Europe/Dublin (GMT Standard Time) for local times
4. Search: Case-insensitive search on name and address
5. Pagination: Default page size of 10, maximum of 100
6. Cache: Cleared completely on any data modification
7. Background Service: Starts 5 seconds after application startup

## Future Enhancements

For V2 (CosmosDB implementation):

1. Implement `IStationCosmosService`
2. Configure Cosmos DB connection
3. Add repository pattern
4. Implement async LINQ queries
5. Add partition key strategy
6. Implement change feed for real-time updates

## Troubleshooting

### API doesn't start

```bash
# Check if port is in use
netstat -ano | findstr :7000

# Try different port in appsettings.json or launchSettings.json
```

### Tests fail

```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

### JSON file not found

Ensure `Data/dublinbike.json` is marked as Copy Always in the project file:

```xml
<ItemGroup>
  <None Update="Data\dublinbike.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

## Author

Student Number: 74867

Date: November 2025

Module: Full Stack Development (M3.3)

Assignment: Dublin Bikes API - Assessment 1
