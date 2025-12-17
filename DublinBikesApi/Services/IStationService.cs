using DublinBikesApi.Models;
using DublinBikesApi.DTOs;

namespace DublinBikesApi.Services;

/// <summary>
/// Interface for station data operations
/// </summary>
public interface IStationService
{
    /// <summary>
    /// Gets all stations with optional filtering, searching, sorting, and paging
    /// </summary>
    Task<PagedResponse<StationDto>> GetStationsAsync(
        string? status = null,
        int? minBikes = null,
        string? q = null,
        string? sort = null,
        string? dir = null,
        int page = 1,
        int pageSize = 10);

    /// <summary>
    /// Gets a single station by its number
    /// </summary>
    Task<StationDto?> GetStationByNumberAsync(int number);

    /// <summary>
    /// Gets summary statistics for all stations
    /// </summary>
    Task<StationsSummaryDto> GetStationsSummaryAsync();

    /// <summary>
    /// Creates a new station
    /// </summary>
    Task<StationDto> CreateStationAsync(CreateStationDto createDto);

    /// <summary>
    /// Updates an existing station
    /// </summary>
    Task<StationDto?> UpdateStationAsync(int number, UpdateStationDto updateDto);

    /// <summary>
    /// Deletes a station by its number
    /// </summary>
    Task<bool> DeleteStationAsync(int number);

    /// <summary>
    /// Gets all stations (for internal use)
    /// </summary>
    List<Station> GetAllStations();

    /// <summary>
    /// Updates station availability (used by background service)
    /// </summary>
    void UpdateStationAvailability(int number, int availableBikes, int availableBikeStands);
}
