using Microsoft.AspNetCore.Mvc;
using DublinBikesApi.Services;
using DublinBikesApi.DTOs;

namespace DublinBikesApi.Controllers.V2;

/// <summary>
/// V2 API Controller for Dublin Bikes Stations (CosmosDB based - placeholder)
/// </summary>
[ApiController]
[Route("api/v2/stations")]
[ApiVersion("2.0")]
public class StationsController : ControllerBase
{
    private readonly IStationService _stationService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<StationsController> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public StationsController(
        IStationService stationService,
        ICacheService cacheService,
        ILogger<StationsController> logger)
    {
        _stationService = stationService;
        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Get all stations with optional filtering, searching, sorting, and paging
    /// </summary>
    /// <param name="status">Filter by status (OPEN/CLOSED)</param>
    /// <param name="minBikes">Filter by minimum available bikes</param>
    /// <param name="q">Search term for name and address</param>
    /// <param name="sort">Sort field (name, availableBikes, occupancy)</param>
    /// <param name="dir">Sort direction (asc/desc)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <returns>Paged list of stations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<StationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<StationDto>>> GetStations(
        [FromQuery] string? status = null,
        [FromQuery] int? minBikes = null,
        [FromQuery] string? q = null,
        [FromQuery] string? sort = null,
        [FromQuery] string? dir = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var cacheKey = $"stations_v2_{status}_{minBikes}_{q}_{sort}_{dir}_{page}_{pageSize}";

        var cachedResult = _cacheService.Get<PagedResponse<StationDto>>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogDebug("Returning cached result for GetStations (V2)");
            return Ok(cachedResult);
        }

        // Note: In a real implementation, this would use CosmosDB service
        // For now, it uses the same in-memory service as V1
        var result = await _stationService.GetStationsAsync(status, minBikes, q, sort, dir, page, pageSize);

        _cacheService.Set(cacheKey, result, _cacheExpiration);

        return Ok(result);
    }

    /// <summary>
    /// Get a single station by its number
    /// </summary>
    /// <param name="number">Station number</param>
    /// <returns>Station details</returns>
    [HttpGet("{number}")]
    [ProducesResponseType(typeof(StationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StationDto>> GetStation(int number)
    {
        var cacheKey = $"station_v2_{number}";

        var cachedResult = _cacheService.Get<StationDto>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogDebug($"Returning cached result for station {number} (V2)");
            return Ok(cachedResult);
        }

        var station = await _stationService.GetStationByNumberAsync(number);

        if (station == null)
        {
            _logger.LogWarning($"Station {number} not found (V2)");
            return NotFound(new { message = $"Station with number {number} not found" });
        }

        _cacheService.Set(cacheKey, station, _cacheExpiration);

        return Ok(station);
    }

    /// <summary>
    /// Get summary statistics for all stations
    /// </summary>
    /// <returns>Summary statistics</returns>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(StationsSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StationsSummaryDto>> GetSummary()
    {
        var cacheKey = "stations_summary_v2";

        var cachedResult = _cacheService.Get<StationsSummaryDto>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogDebug("Returning cached result for GetSummary (V2)");
            return Ok(cachedResult);
        }

        var summary = await _stationService.GetStationsSummaryAsync();

        _cacheService.Set(cacheKey, summary, _cacheExpiration);

        return Ok(summary);
    }

    /// <summary>
    /// Create a new station
    /// </summary>
    /// <param name="createDto">Station creation data</param>
    /// <returns>Created station</returns>
    [HttpPost]
    [ProducesResponseType(typeof(StationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StationDto>> CreateStation([FromBody] CreateStationDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var station = await _stationService.CreateStationAsync(createDto);

            // Clear relevant caches
            _cacheService.Clear();

            return CreatedAtAction(
                nameof(GetStation),
                new { number = station.Number },
                station);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create station (V2)");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing station
    /// </summary>
    /// <param name="number">Station number</param>
    /// <param name="updateDto">Station update data</param>
    /// <returns>Updated station</returns>
    [HttpPut("{number}")]
    [ProducesResponseType(typeof(StationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StationDto>> UpdateStation(
        int number,
        [FromBody] UpdateStationDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var station = await _stationService.UpdateStationAsync(number, updateDto);

        if (station == null)
        {
            _logger.LogWarning($"Station {number} not found for update (V2)");
            return NotFound(new { message = $"Station with number {number} not found" });
        }

        // Clear relevant caches
        _cacheService.Clear();

        return Ok(station);
    }

    /// <summary>
    /// Delete a station
    /// </summary>
    /// <param name="number">Station number</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{number}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStation(int number)
    {
        var deleted = await _stationService.DeleteStationAsync(number);

        if (!deleted)
        {
            _logger.LogWarning($"Station {number} not found for deletion (V2)");
            return NotFound(new { message = $"Station with number {number} not found" });
        }

        // Clear relevant caches
        _cacheService.Clear();

        _logger.LogInformation($"Station {number} deleted successfully (V2)");
        return NoContent();
    }
}
