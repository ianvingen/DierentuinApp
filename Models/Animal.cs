namespace DierentuinApp.Models;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Category { get; set; }

    public AnimalSize Size { get; set; }
    public DietaryClass DietaryClass { get; set; }
    public ActivityPattern ActivityPattern { get; set; }

    public string Prey { get; set; }

    public int EnclosureId { get; set; }
    public Enclosure Enclosure { get; set; }

    public double SpaceRequirement { get; set; }

    public SecurityLevel SecurityRequirement { get; set; }
}
