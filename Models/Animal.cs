namespace DierentuinApp.Models;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;

    // Nullable foreign key naar Category (default NULL volgens opdracht)
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public AnimalSize Size { get; set; }
    public DietaryClass DietaryClass { get; set; }
    public ActivityPattern ActivityPattern { get; set; }

    // Nullable: niet elk dier heeft prooi (herbivoren bijvoorbeeld)
    public string? Prey { get; set; }

    // Nullable foreign key naar Enclosure (default NULL volgens opdracht)
    public int? EnclosureId { get; set; }
    public Enclosure? Enclosure { get; set; }

    public double SpaceRequirement { get; set; }
    public SecurityLevel SecurityRequirement { get; set; }
}
