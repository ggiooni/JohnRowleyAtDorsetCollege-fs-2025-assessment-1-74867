using System.ComponentModel.DataAnnotations;

namespace DublinBikesApi.DTOs;

/// <summary>
/// DTO for creating a new station
/// </summary>
public class CreateStationDto
{
    [Required]
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

    [Range(0, 100)]
    public int AvailableBikes { get; set; }

    public string Status { get; set; } = "OPEN";
}
