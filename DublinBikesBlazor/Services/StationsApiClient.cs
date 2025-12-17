using DublinBikesBlazor.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace DublinBikesBlazor.Services;

public class StationsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StationsApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public StationsApiClient(HttpClient httpClient, ILogger<StationsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<PagedResponse<StationDto>?> GetStationsAsync(
        string? status = null,
        int? minBikes = null,
        string? searchQuery = null,
        string? sort = null,
        string? dir = null,
        int page = 1,
        int pageSize = 10)
    {
        try
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(status))
                queryParams.Add($"status={Uri.EscapeDataString(status)}");

            if (minBikes.HasValue)
                queryParams.Add($"minBikes={minBikes.Value}");

            if (!string.IsNullOrWhiteSpace(searchQuery))
                queryParams.Add($"q={Uri.EscapeDataString(searchQuery)}");

            if (!string.IsNullOrWhiteSpace(sort))
                queryParams.Add($"sort={Uri.EscapeDataString(sort)}");

            if (!string.IsNullOrWhiteSpace(dir))
                queryParams.Add($"dir={Uri.EscapeDataString(dir)}");

            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");
            queryParams.Add("api-version=2.0");

            var queryString = string.Join("&", queryParams);
            var url = $"api/v2/stations?{queryString}";

            _logger.LogInformation("Fetching stations from: {Url}", url);

            var response = await _httpClient.GetFromJsonAsync<PagedResponse<StationDto>>(url, _jsonOptions);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stations");
            throw;
        }
    }

    public async Task<StationDto?> GetStationAsync(int number)
    {
        try
        {
            var url = $"api/v2/stations/{number}?api-version=2.0";
            _logger.LogInformation("Fetching station {Number} from: {Url}", number, url);

            var response = await _httpClient.GetFromJsonAsync<StationDto>(url, _jsonOptions);
            return response;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Station {Number} not found", number);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching station {Number}", number);
            throw;
        }
    }

    public async Task<StationDto?> CreateStationAsync(CreateStationDto createDto)
    {
        try
        {
            _logger.LogInformation("Creating new station: {Name}", createDto.Name);

            var response = await _httpClient.PostAsJsonAsync("api/v2/stations?api-version=2.0", createDto, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var station = await response.Content.ReadFromJsonAsync<StationDto>(_jsonOptions);
            _logger.LogInformation("Station created successfully: {Number}", station?.Number);

            return station;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating station");
            throw;
        }
    }

    public async Task<StationDto?> UpdateStationAsync(int number, UpdateStationDto updateDto)
    {
        try
        {
            _logger.LogInformation("Updating station {Number}", number);

            var response = await _httpClient.PutAsJsonAsync($"api/v2/stations/{number}?api-version=2.0", updateDto, _jsonOptions);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Station {Number} not found for update", number);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var station = await response.Content.ReadFromJsonAsync<StationDto>(_jsonOptions);
            _logger.LogInformation("Station {Number} updated successfully", number);

            return station;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating station {Number}", number);
            throw;
        }
    }

    public async Task<bool> DeleteStationAsync(int number)
    {
        try
        {
            _logger.LogInformation("Deleting station {Number}", number);

            var response = await _httpClient.DeleteAsync($"api/v2/stations/{number}?api-version=2.0");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Station {Number} not found for deletion", number);
                return false;
            }

            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Station {Number} deleted successfully", number);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting station {Number}", number);
            throw;
        }
    }
}
