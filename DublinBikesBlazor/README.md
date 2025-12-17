# Dublin Bikes Blazor Client

A Blazor Server application for managing Dublin Bikes stations with real-time data visualization, filtering, sorting, and full CRUD operations.

## Features

- **Master/Detail View**: Browse stations in a responsive table with detailed side panel
- **Advanced Filtering**: Filter by status (OPEN/CLOSED), minimum available bikes, and search by name/address
- **Sorting**: Sort stations by name or available bikes
- **Paging**: Navigate through stations with pagination controls
- **CRUD Operations**: Create, update, and delete bike stations
- **Real-time Updates**: Data updates from the API background service
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Accessibility**: ARIA labels, keyboard navigation, and screen reader support

## Prerequisites

- .NET 9.0 SDK or later
- Dublin Bikes API running (Assignment 1)
- Modern web browser

## Configuration

### API Base URL

The application connects to the Dublin Bikes API. Configure the base URL in `appsettings.json`:

```json
{
  "ApiBaseUrl": "https://localhost:7000"
}
```

**Default**: `https://localhost:7000` (API from Assignment 1)

### Alternative Configuration

You can also set the API URL via environment variable:

```bash
export ApiBaseUrl="https://your-api-url.com"
```

## Running the Application

### 1. Ensure the API is Running

First, start the Dublin Bikes API from Assignment 1:

```bash
cd ../DublinBikesApi
dotnet run
```

The API should be running on `https://localhost:7000`

### 2. Start the Blazor Application

```bash
cd DublinBikesBlazor
dotnet run
```

The Blazor app will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### 3. Navigate to Stations Page

Open your browser and go to:
```
https://localhost:5001/stations
```

## Features Guide

### Master List View

The main stations list displays:
- Station number
- Station name
- Address
- Status badge (OPEN/CLOSED)
- Available bikes / Total stands
- Occupancy percentage
- Action buttons (View, Edit, Delete)

### Filtering & Search

**Search Box**: Type to filter stations by name or address (real-time)

**Status Filter**:
- All Stations
- Open Only
- Closed Only

**Minimum Bikes**: Set a minimum threshold for available bikes

**Apply Filters Button**: Apply selected filters

**Clear Filters**: Reset all filters to default

### Sorting

Click on column headers to sort:
- **Station Name**: Alphabetical (A-Z / Z-A)
- **Available Bikes**: Numerical (Low to High / High to Low)

### Pagination

Navigate through stations with:
- Previous/Next buttons
- Direct page number selection
- Shows current page and total pages
- Configurable page size (default: 10 per page)

### Detail View

Select any station to view:
- Full station information
- Real-time availability
- Capacity visualization (progress bar)
- Last update timestamp
- Geographic coordinates (click to open in Google Maps)
- Edit and Delete buttons

### Creating a Station

1. Click **"Add New Station"** button
2. Fill in the required fields:
   - Station Number (unique)
   - Name
   - Address
   - Latitude/Longitude
   - Total Bike Stands
   - Available Bikes
   - Status (OPEN/CLOSED)
3. Click **"Create Station"**

### Editing a Station

1. Click the **Edit** button (pencil icon) on any station
2. Modify the desired fields (blank = no change)
3. Click **"Update Station"**

### Deleting a Station

1. Click the **Delete** button (trash icon) on any station
2. Confirm the deletion in the dialog
3. Station is removed from the system

## Project Structure

```
DublinBikesBlazor/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Home.razor
│   │   └── Stations.razor          # Main stations page
│   └── StationComponents/
│       ├── StationDetail.razor     # Detail view panel
│       ├── StationCreateForm.razor # Create station form
│       └── StationEditForm.razor   # Edit station form
├── Models/
│   ├── StationDto.cs               # Station data model
│   ├── PagedResponse.cs            # Paged API response model
│   ├── CreateStationDto.cs         # Create station model
│   └── UpdateStationDto.cs         # Update station model
├── Services/
│   └── StationsApiClient.cs        # API HTTP client service
├── wwwroot/                        # Static assets
├── Program.cs                      # Application configuration
├── appsettings.json                # Configuration file
└── README.md                       # This file
```

## API Integration

The application uses the **V2 API endpoints** from Assignment 1:

### Endpoints Used

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v2/stations` | Get paginated, filtered, sorted stations |
| GET | `/api/v2/stations/{number}` | Get single station by number |
| POST | `/api/v2/stations` | Create new station |
| PUT | `/api/v2/stations/{number}` | Update existing station |
| DELETE | `/api/v2/stations/{number}` | Delete station |

### Query Parameters

The GET `/api/v2/stations` endpoint supports:
- `status`: Filter by OPEN/CLOSED
- `minBikes`: Minimum available bikes
- `q`: Search term (name/address)
- `sort`: Sort field (name, availableBikes)
- `dir`: Sort direction (asc/desc)
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)

## Accessibility Features

- **ARIA Labels**: All interactive elements have descriptive labels
- **Keyboard Navigation**: Full keyboard support (Tab, Enter, Escape)
- **Screen Reader Support**: Semantic HTML and ARIA attributes
- **Color Independence**: Status indicated by text + color
- **Focus Management**: Clear focus indicators
- **Responsive Design**: Works with screen magnification

## UI/UX Highlights

- **Bootstrap 5**: Modern, responsive styling
- **Bootstrap Icons**: Consistent iconography
- **Loading States**: Spinners during API calls
- **Error Handling**: User-friendly error messages
- **Success Notifications**: Confirmation messages
- **Empty States**: Helpful messages when no data
- **Confirm Dialogs**: Prevent accidental deletions
- **Progress Bars**: Visual capacity indicators

## Troubleshooting

### Cannot Connect to API

**Error**: "Failed to load stations"

**Solutions**:
1. Verify the API is running: `curl https://localhost:7000/health`
2. Check `appsettings.json` has correct `ApiBaseUrl`
3. Ensure API V2 endpoints are accessible
4. Check for CORS issues (API should allow Blazor origin)

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Port Already in Use

Change the port in `Properties/launchSettings.json`:

```json
{
  "applicationUrl": "https://localhost:5101;http://localhost:5100"
}
```

## Development Notes

- **Render Mode**: InteractiveServer (enables real-time updates)
- **State Management**: Component-level state
- **API Calls**: Async/await pattern
- **Form Validation**: Data annotations
- **Error Handling**: Try-catch with user feedback
- **Styling**: Bootstrap 5 + Custom CSS

## Author

**Student Number**: 74867

**Date**: December 2025

**Module**: Full Stack Development (M3.3)

**Assignment**: Dublin Bikes Blazor Client - Assessment 2

## Assignment Requirements Fulfilled

✅ **API Integration**: Connects to V2 API with full HTTP client implementation

✅ **Master List View**: Responsive table with key station information

✅ **Detail View**: Comprehensive side panel with all station details

✅ **Search & Filters**: Text search, status filter, minimum bikes filter

✅ **Sorting**: Name and available bikes sorting (asc/desc)

✅ **Paging**: Full pagination with navigation controls

✅ **CRUD Operations**: Create, Update, Delete stations via API

✅ **UI/UX**: Clean, readable, responsive layout

✅ **Accessibility**: ARIA labels, keyboard navigation, screen reader support

✅ **Documentation**: This comprehensive README

## License

Educational project for Dorset College Dublin.
