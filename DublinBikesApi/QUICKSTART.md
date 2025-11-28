# Dublin Bikes API - Quick Start Guide

## 🚀 Run the API in 3 Steps

### 1. Navigate to the project directory
```bash
cd DublinBikesApi
```

### 2. Run the application
```bash
dotnet run
```

### 3. Open Swagger UI in your browser
```
https://localhost:7000/swagger
```

That's it! The API is now running.

## 🧪 Run the Tests

```bash
cd ../DublinBikesApi.Tests
dotnet test
```

Expected: All 15 tests should pass ✅

## 📬 Test with Postman

1. Import `Postman/DublinBikesAPI.postman_collection.json`
2. Import `Postman/DublinBikesAPI.postman_environment.json`
3. Run the collection

## 🔍 Quick API Examples

### Get all stations
```bash
curl https://localhost:7000/api/v1/stations
```

### Get a specific station
```bash
curl https://localhost:7000/api/v1/stations/42
```

### Get summary statistics
```bash
curl https://localhost:7000/api/v1/stations/summary
```

### Search for stations
```bash
curl "https://localhost:7000/api/v1/stations?q=street&status=OPEN&minBikes=5"
```

## 📊 What's Happening?

- Data Loading: JSON file is loaded at startup
- Background Updates: Stations are updated every 15 seconds
- Caching: Responses are cached for 5 minutes
- API Versions: V1 (JSON) and V2 (CosmosDB-ready) available

## 🔗 Important URLs

- Swagger UI: https://localhost:7000/swagger
- Health Check: https://localhost:7000/health
- V1 Stations: https://localhost:7000/api/v1/stations
- V2 Stations: https://localhost:7000/api/v2/stations

## 📖 Full Documentation

See [README.md](README.md) for complete documentation.

## ❓ Troubleshooting

### Port already in use?
Edit `Properties/launchSettings.json` and change the ports.

### JSON file not found?
Make sure you're running from the `DublinBikesApi` directory.

### Tests failing?
```bash
dotnet clean
dotnet build
dotnet test
```

## 🎯 Key Features to Test

1. Filtering: `?status=OPEN&minBikes=10`
2. Searching: `?q=street`
3. Sorting: `?sort=name&dir=asc`
4. Pagination: `?page=2&pageSize=20`
5. Combining: `?status=OPEN&q=street&sort=occupancy&dir=desc&page=1&pageSize=5`

Enjoy exploring the Dublin Bikes API! 🚴‍♂️
