namespace DublinBikesBlazor.Models;

public class StationDto
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int BikeStands { get; set; }
    public int AvailableBikes { get; set; }
    public int AvailableBikeStands { get; set; }
    public string Status { get; set; } = string.Empty;
    public double Occupancy { get; set; }
    public DateTime LastUpdateLocal { get; set; }
    public long LastUpdateEpoch { get; set; }
}
