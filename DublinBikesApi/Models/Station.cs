using System.Text.Json.Serialization;

namespace DublinBikesApi.Models;

/// <summary>
/// Represents a Dublin Bikes station
/// </summary>
public class Station
{
    /// <summary>
    /// Unique station number identifier
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Contract name (e.g., "dublin")
    /// </summary>
    [JsonPropertyName("contract_name")]
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// Station name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Station address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Geographical position of the station
    /// </summary>
    public Position Position { get; set; } = new();

    /// <summary>
    /// Whether banking services are available
    /// </summary>
    public bool Banking { get; set; }

    /// <summary>
    /// Whether bonus is available
    /// </summary>
    public bool Bonus { get; set; }

    /// <summary>
    /// Total number of bike stands at the station
    /// </summary>
    [JsonPropertyName("bike_stands")]
    public int BikeStands { get; set; }

    /// <summary>
    /// Number of currently available bike stands
    /// </summary>
    [JsonPropertyName("available_bike_stands")]
    public int AvailableBikeStands { get; set; }

    /// <summary>
    /// Number of currently available bikes
    /// </summary>
    [JsonPropertyName("available_bikes")]
    public int AvailableBikes { get; set; }

    /// <summary>
    /// Station status (e.g., "OPEN", "CLOSED")
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Last update timestamp in epoch milliseconds
    /// </summary>
    [JsonPropertyName("last_update")]
    public long LastUpdate { get; set; }

    /// <summary>
    /// Last update as DateTime (computed property)
    /// </summary>
    [JsonIgnore]
    public DateTime LastUpdateDateTime => DateTimeOffset.FromUnixTimeMilliseconds(LastUpdate).DateTime;

    /// <summary>
    /// Last update in Europe/Dublin timezone (computed property)
    /// </summary>
    [JsonIgnore]
    public DateTime LastUpdateLocal
    {
        get
        {
            var dto = DateTimeOffset.FromUnixTimeMilliseconds(LastUpdate);
            var dublinTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"); // Europe/Dublin
            return TimeZoneInfo.ConvertTime(dto, dublinTimeZone).DateTime;
        }
    }

    /// <summary>
    /// Occupancy percentage (available_bikes / bike_stands)
    /// </summary>
    [JsonIgnore]
    public double Occupancy
    {
        get
        {
            if (BikeStands == 0) return 0;
            return Math.Round((double)AvailableBikes / BikeStands * 100, 2);
        }
    }
}
