using System.ComponentModel.DataAnnotations;

namespace DierentuinApp.Models;

public class Enclosure
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naam is verplicht")]
    [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
    public string Name { get; set; } = string.Empty;

    public List<Animal> Animals { get; set; } = new List<Animal>();

    [Required]
    public Climate Climate { get; set; }

    [Required]
    public HabitatType HabitatType { get; set; }

    [Required]
    public SecurityLevel SecurityLevel { get; set; }

    // In vierkante meters
    [Required]
    [Range(1, 100000, ErrorMessage = "Grootte moet tussen 1 en 100000 m² zijn")]
    public double Size { get; set; }
}