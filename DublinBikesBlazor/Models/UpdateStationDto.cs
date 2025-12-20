using System.ComponentModel.DataAnnotations;

namespace DublinBikesBlazor.Models;

public class UpdateStationDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }

    [Range(1, 100)]
    public int? BikeStands { get; set; }

    [Range(0, 100)]
    public int? AvailableBikes { get; set; }

    [Range(0, 100)]
    public int? AvailableBikeStands { get; set; }

    [RegularExpression("OPEN|CLOSED")]
    public string? Status { get; set; }
}
