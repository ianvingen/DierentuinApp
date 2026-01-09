using System.ComponentModel.DataAnnotations;

namespace DierentuinApp.Models;

public class Animal
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naam is verplicht")]
    [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soort is verplicht")]
    [StringLength(100, ErrorMessage = "Soort mag maximaal 100 karakters zijn")]
    public string Species { get; set; } = string.Empty;

    // Nullable foreign key naar Category (default NULL volgens opdracht)
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    [Required]
    public AnimalSize Size { get; set; }

    [Required]
    public DietaryClass DietaryClass { get; set; }

    [Required]
    public ActivityPattern ActivityPattern { get; set; }

    // Nullable: niet elk dier heeft prooi (herbivoren bijvoorbeeld)
    [StringLength(200, ErrorMessage = "Prooi mag maximaal 200 karakters zijn")]
    public string? Prey { get; set; }

    // Nullable foreign key naar Enclosure (default NULL volgens opdracht)
    public int? EnclosureId { get; set; }
    public Enclosure? Enclosure { get; set; }

    [Required]
    [Range(0.01, 10000, ErrorMessage = "Ruimtebehoefte moet tussen 0.01 en 10000 m² zijn")]
    public double SpaceRequirement { get; set; }

    [Required]
    public SecurityLevel SecurityRequirement { get; set; }
}
