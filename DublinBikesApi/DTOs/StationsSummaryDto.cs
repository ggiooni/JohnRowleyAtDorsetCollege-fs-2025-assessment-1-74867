namespace DublinBikesApi.DTOs;

/// <summary>
/// Summary statistics for all stations
/// </summary>
public class StationsSummaryDto
{
    public int TotalStations { get; set; }
    public int TotalBikeStands { get; set; }
    public int TotalAvailableBikes { get; set; }
    public int TotalAvailableBikeStands { get; set; }
    public Dictionary<string, int> StationsByStatus { get; set; } = new();
    public double AverageOccupancy { get; set; }
}
