using System.ComponentModel.DataAnnotations;

namespace DublinBikesBlazor.Models;

public class CreateStationDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Number { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Required]
    [Range(1, 100)]
    public int BikeStands { get; set; }

    [Required]
    [Range(0, 100)]
    public int AvailableBikes { get; set; }

    [Required]
    [RegularExpression("OPEN|CLOSED")]
    public string Status { get; set; } = "OPEN";
}
