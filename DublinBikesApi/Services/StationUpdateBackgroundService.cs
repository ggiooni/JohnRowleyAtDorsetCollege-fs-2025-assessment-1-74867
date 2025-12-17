namespace DublinBikesApi.Services;

/// <summary>
/// Background service that periodically updates station availability data
/// to simulate a live feed
/// </summary>
public class StationUpdateBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StationUpdateBackgroundService> _logger;
    private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(15); // Update every 15 seconds
    private readonly Random _random = new();

    public StationUpdateBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<StationUpdateBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Station Update Background Service is starting");

        // Wait a few seconds before first update to allow services to initialize
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateStationsAsync();
                await Task.Delay(_updateInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal during shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating stations");
                // Continue running even if an error occurs
                await Task.Delay(_updateInterval, stoppingToken);
            }
        }

        _logger.LogInformation("Station Update Background Service is stopping");
    }

    private async Task UpdateStationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var stationService = scope.ServiceProvider.GetRequiredService<IStationService>();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        var stations = stationService.GetAllStations();
        var updatedCount = 0;

        foreach (var station in stations)
        {
            // Generate random capacity (total stands)
            // Keep it close to original value with some variation
            var newCapacity = Math.Max(10, station.BikeStands + _random.Next(-5, 6));

            // Generate random availability (bikes)
            // Ensure it's never greater than capacity
            var newAvailableBikes = _random.Next(0, newCapacity + 1);

            // Calculate available stands
            var newAvailableStands = newCapacity - newAvailableBikes;

            // Update the station
            stationService.UpdateStationAvailability(
                station.Number,
                newAvailableBikes,
                newAvailableStands);

            updatedCount++;
        }

        // Clear cache after updating stations
        cacheService.Clear();

        _logger.LogInformation($"Updated {updatedCount} stations at {DateTime.UtcNow:HH:mm:ss}");

        await Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Station Update Background Service is stopping gracefully");
        return base.StopAsync(cancellationToken);
    }
}
