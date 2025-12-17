using DublinBikesApi.Models;
using DublinBikesApi.DTOs;
using System.Text.Json;

namespace DublinBikesApi.Services;

/// <summary>
/// In-memory implementation of the station service
/// </summary>
public class StationService : IStationService
{
    private readonly List<Station> _stations;
    private readonly ILogger<StationService> _logger;
    private readonly object _lock = new();

    public StationService(ILogger<StationService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _stations = new List<Station>();
        LoadStationsFromJson(env);
    }

    private void LoadStationsFromJson(IWebHostEnvironment env)
    {
        try
        {
            var jsonPath = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");

            if (!File.Exists(jsonPath))
            {
                _logger.LogError($"JSON file not found at {jsonPath}");
                throw new FileNotFoundException($"JSON file not found at {jsonPath}");
            }

            var jsonContent = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var stations = JsonSerializer.Deserialize<List<Station>>(jsonContent, options);

            if (stations == null || !stations.Any())
            {
                _logger.LogError("No stations found in JSON file");
                throw new InvalidDataException("No stations found in JSON file");
            }

            lock (_lock)
            {
                _stations.Clear();
                _stations.AddRange(stations);
            }

            _logger.LogInformation($"Loaded {_stations.Count} stations from JSON file");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading stations from JSON");
            throw;
        }
    }

    public List<Station> GetAllStations()
    {
        lock (_lock)
        {
            return new List<Station>(_stations);
        }
    }

    public async Task<PagedResponse<StationDto>> GetStationsAsync(
        string? status = null,
        int? minBikes = null,
        string? q = null,
        string? sort = null,
        string? dir = null,
        int page = 1,
        int pageSize = 10)
    {
        await Task.CompletedTask; // Make method async-compatible

        IEnumerable<Station> query;
        lock (_lock)
        {
            query = _stations.AsEnumerable();
        }

        // Apply filters
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        if (minBikes.HasValue)
        {
            query = query.Where(s => s.AvailableBikes >= minBikes.Value);
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(s =>
                s.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                s.Address.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting
        query = ApplySorting(query, sort, dir);

        // Get total count before paging
        var totalCount = query.Count();

        // Apply paging
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResponse<StationDto>
        {
            Data = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPrevious = page > 1,
            HasNext = page < totalPages
        };
    }

    public async Task<StationDto?> GetStationByNumberAsync(int number)
    {
        await Task.CompletedTask;

        Station? station;
        lock (_lock)
        {
            station = _stations.FirstOrDefault(s => s.Number == number);
        }

        return station != null ? MapToDto(station) : null;
    }

    public async Task<StationsSummaryDto> GetStationsSummaryAsync()
    {
        await Task.CompletedTask;

        List<Station> stations;
        lock (_lock)
        {
            stations = new List<Station>(_stations);
        }

        var statusCounts = stations
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        var avgOccupancy = stations
            .Where(s => s.BikeStands > 0)
            .Average(s => s.Occupancy);

        return new StationsSummaryDto
        {
            TotalStations = stations.Count,
            TotalBikeStands = stations.Sum(s => s.BikeStands),
            TotalAvailableBikes = stations.Sum(s => s.AvailableBikes),
            TotalAvailableBikeStands = stations.Sum(s => s.AvailableBikeStands),
            StationsByStatus = statusCounts,
            AverageOccupancy = Math.Round(avgOccupancy, 2)
        };
    }

    public async Task<StationDto> CreateStationAsync(CreateStationDto createDto)
    {
        await Task.CompletedTask;

        lock (_lock)
        {
            if (_stations.Any(s => s.Number == createDto.Number))
            {
                throw new InvalidOperationException($"Station with number {createDto.Number} already exists");
            }

            var station = new Station
            {
                Number = createDto.Number,
                ContractName = "dublin",
                Name = createDto.Name,
                Address = createDto.Address,
                Position = new Position
                {
                    Lat = createDto.Latitude,
                    Lng = createDto.Longitude
                },
                BikeStands = createDto.BikeStands,
                AvailableBikes = createDto.AvailableBikes,
                AvailableBikeStands = createDto.BikeStands - createDto.AvailableBikes,
                Status = createDto.Status,
                Banking = false,
                Bonus = false,
                LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            _stations.Add(station);
            _logger.LogInformation($"Created new station: {station.Number} - {station.Name}");

            return MapToDto(station);
        }
    }

    public async Task<StationDto?> UpdateStationAsync(int number, UpdateStationDto updateDto)
    {
        await Task.CompletedTask;

        lock (_lock)
        {
            var station = _stations.FirstOrDefault(s => s.Number == number);
            if (station == null) return null;

            if (updateDto.Name != null) station.Name = updateDto.Name;
            if (updateDto.Address != null) station.Address = updateDto.Address;
            if (updateDto.Latitude.HasValue) station.Position.Lat = updateDto.Latitude.Value;
            if (updateDto.Longitude.HasValue) station.Position.Lng = updateDto.Longitude.Value;
            if (updateDto.BikeStands.HasValue) station.BikeStands = updateDto.BikeStands.Value;
            if (updateDto.AvailableBikes.HasValue) station.AvailableBikes = updateDto.AvailableBikes.Value;
            if (updateDto.AvailableBikeStands.HasValue) station.AvailableBikeStands = updateDto.AvailableBikeStands.Value;
            if (updateDto.Status != null) station.Status = updateDto.Status;

            station.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            _logger.LogInformation($"Updated station: {station.Number} - {station.Name}");

            return MapToDto(station);
        }
    }

    public async Task<bool> DeleteStationAsync(int number)
    {
        await Task.CompletedTask;

        lock (_lock)
        {
            var station = _stations.FirstOrDefault(s => s.Number == number);
            if (station == null)
            {
                _logger.LogWarning($"Station {number} not found for deletion");
                return false;
            }

            _stations.Remove(station);
            _logger.LogInformation($"Deleted station: {station.Number} - {station.Name}");
            return true;
        }
    }

    public void UpdateStationAvailability(int number, int availableBikes, int availableBikeStands)
    {
        lock (_lock)
        {
            var station = _stations.FirstOrDefault(s => s.Number == number);
            if (station != null)
            {
                station.AvailableBikes = availableBikes;
                station.AvailableBikeStands = availableBikeStands;
                station.LastUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }
    }

    private IEnumerable<Station> ApplySorting(IEnumerable<Station> query, string? sort, string? dir)
    {
        var isDescending = dir?.ToLower() == "desc";

        return sort?.ToLower() switch
        {
            "name" => isDescending
                ? query.OrderByDescending(s => s.Name)
                : query.OrderBy(s => s.Name),
            "availablebikes" => isDescending
                ? query.OrderByDescending(s => s.AvailableBikes)
                : query.OrderBy(s => s.AvailableBikes),
            "occupancy" => isDescending
                ? query.OrderByDescending(s => s.Occupancy)
                : query.OrderBy(s => s.Occupancy),
            _ => query.OrderBy(s => s.Number) // Default sort by number
        };
    }

    private StationDto MapToDto(Station station)
    {
        return new StationDto
        {
            Number = station.Number,
            Name = station.Name,
            Address = station.Address,
            Latitude = station.Position.Lat,
            Longitude = station.Position.Lng,
            BikeStands = station.BikeStands,
            AvailableBikes = station.AvailableBikes,
            AvailableBikeStands = station.AvailableBikeStands,
            Status = station.Status,
            Occupancy = station.Occupancy,
            LastUpdateLocal = station.LastUpdateLocal,
            LastUpdateEpoch = station.LastUpdate
        };
    }
}
