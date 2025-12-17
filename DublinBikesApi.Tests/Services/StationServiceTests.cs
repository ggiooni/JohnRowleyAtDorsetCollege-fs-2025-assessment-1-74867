using DublinBikesApi.Services;
using DublinBikesApi.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Moq;
using FluentAssertions;

namespace DublinBikesApi.Tests.Services;

public class StationServiceTests : IDisposable
{
    private readonly Mock<ILogger<StationService>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly StationService _service;
    private readonly string _testDataPath;

    public StationServiceTests()
    {
        _loggerMock = new Mock<ILogger<StationService>>();
        _envMock = new Mock<IWebHostEnvironment>();

        // Create a test JSON file
        _testDataPath = Path.Combine(Path.GetTempPath(), "TestData");
        Directory.CreateDirectory(_testDataPath);
        var jsonPath = Path.Combine(_testDataPath, "Data");
        Directory.CreateDirectory(jsonPath);

        var testJsonContent = @"[
            {
                ""number"": 1,
                ""contract_name"": ""dublin"",
                ""name"": ""TEST STATION 1"",
                ""address"": ""Test Address 1"",
                ""position"": { ""lat"": 53.34, ""lng"": -6.26 },
                ""banking"": false,
                ""bonus"": false,
                ""bike_stands"": 20,
                ""available_bike_stands"": 15,
                ""available_bikes"": 5,
                ""status"": ""OPEN"",
                ""last_update"": 1729065138000
            },
            {
                ""number"": 2,
                ""contract_name"": ""dublin"",
                ""name"": ""TEST STATION 2"",
                ""address"": ""Test Address 2"",
                ""position"": { ""lat"": 53.35, ""lng"": -6.27 },
                ""banking"": false,
                ""bonus"": false,
                ""bike_stands"": 30,
                ""available_bike_stands"": 20,
                ""available_bikes"": 10,
                ""status"": ""OPEN"",
                ""last_update"": 1729065138000
            },
            {
                ""number"": 3,
                ""contract_name"": ""dublin"",
                ""name"": ""CLOSED STATION"",
                ""address"": ""Closed Address"",
                ""position"": { ""lat"": 53.36, ""lng"": -6.28 },
                ""banking"": false,
                ""bonus"": false,
                ""bike_stands"": 25,
                ""available_bike_stands"": 25,
                ""available_bikes"": 0,
                ""status"": ""CLOSED"",
                ""last_update"": 1729065138000
            }
        ]";

        File.WriteAllText(Path.Combine(jsonPath, "dublinbike.json"), testJsonContent);

        _envMock.Setup(e => e.ContentRootPath).Returns(_testDataPath);

        _service = new StationService(_loggerMock.Object, _envMock.Object);
    }

    [Fact]
    public async Task GetStationsAsync_ShouldReturnAllStations_WhenNoFiltersApplied()
    {
        // Act
        var result = await _service.GetStationsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetStationsAsync_ShouldFilterByStatus()
    {
        // Act
        var result = await _service.GetStationsAsync(status: "OPEN");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().AllSatisfy(s => s.Status.Should().Be("OPEN"));
    }

    [Fact]
    public async Task GetStationsAsync_ShouldFilterByMinBikes()
    {
        // Act
        var result = await _service.GetStationsAsync(minBikes: 8);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().AvailableBikes.Should().BeGreaterThanOrEqualTo(8);
    }

    [Fact]
    public async Task GetStationsAsync_ShouldSearchByNameOrAddress()
    {
        // Act
        var result = await _service.GetStationsAsync(q: "CLOSED");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Contain("CLOSED");
    }

    [Fact]
    public async Task GetStationsAsync_ShouldSortByName()
    {
        // Act
        var result = await _service.GetStationsAsync(sort: "name", dir: "asc");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.First().Name.Should().Be("CLOSED STATION");
    }

    [Fact]
    public async Task GetStationsAsync_ShouldPaginateResults()
    {
        // Act
        var result = await _service.GetStationsAsync(page: 1, pageSize: 2);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.HasNext.Should().BeTrue();
        result.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public async Task GetStationByNumberAsync_ShouldReturnStation_WhenExists()
    {
        // Act
        var result = await _service.GetStationByNumberAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Number.Should().Be(1);
        result.Name.Should().Be("TEST STATION 1");
    }

    [Fact]
    public async Task GetStationByNumberAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _service.GetStationByNumberAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStationsSummaryAsync_ShouldReturnCorrectSummary()
    {
        // Act
        var result = await _service.GetStationsSummaryAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalStations.Should().Be(3);
        result.TotalBikeStands.Should().Be(75);
        result.TotalAvailableBikes.Should().Be(15);
        result.StationsByStatus.Should().ContainKey("OPEN");
        result.StationsByStatus["OPEN"].Should().Be(2);
    }

    [Fact]
    public async Task CreateStationAsync_ShouldCreateNewStation()
    {
        // Arrange
        var createDto = new CreateStationDto
        {
            Number = 100,
            Name = "New Station",
            Address = "New Address",
            Latitude = 53.34,
            Longitude = -6.26,
            BikeStands = 20,
            AvailableBikes = 10
        };

        // Act
        var result = await _service.CreateStationAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Number.Should().Be(100);
        result.Name.Should().Be("New Station");

        // Verify it was added
        var allStations = await _service.GetStationsAsync();
        allStations.TotalCount.Should().Be(4);
    }

    [Fact]
    public async Task CreateStationAsync_ShouldThrow_WhenNumberExists()
    {
        // Arrange
        var createDto = new CreateStationDto
        {
            Number = 1, // Already exists
            Name = "Duplicate",
            Address = "Duplicate Address",
            Latitude = 53.34,
            Longitude = -6.26,
            BikeStands = 20,
            AvailableBikes = 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateStationAsync(createDto));
    }

    [Fact]
    public async Task UpdateStationAsync_ShouldUpdateStation_WhenExists()
    {
        // Arrange
        var updateDto = new UpdateStationDto
        {
            Name = "Updated Name",
            AvailableBikes = 15
        };

        // Act
        var result = await _service.UpdateStationAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.AvailableBikes.Should().Be(15);
    }

    [Fact]
    public async Task UpdateStationAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var updateDto = new UpdateStationDto { Name = "Updated" };

        // Act
        var result = await _service.UpdateStationAsync(999, updateDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void UpdateStationAvailability_ShouldUpdateAvailability()
    {
        // Act
        _service.UpdateStationAvailability(1, 8, 12);

        // Assert
        var station = _service.GetStationByNumberAsync(1).Result;
        station.Should().NotBeNull();
        station!.AvailableBikes.Should().Be(8);
        station.AvailableBikeStands.Should().Be(12);
    }

    public void Dispose()
    {
        // Cleanup test data
        if (Directory.Exists(_testDataPath))
        {
            Directory.Delete(_testDataPath, true);
        }
    }
}
