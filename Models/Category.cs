using System.ComponentModel.DataAnnotations;

namespace DierentuinApp.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naam is verplicht")]
    [StringLength(50, ErrorMessage = "Naam mag maximaal 50 karakters zijn")]
    public string Name { get; set; } = string.Empty;

    // Navigation property: een categorie heeft meerdere dieren
    public List<Animal> Animals { get; set; } = new List<Animal>();
}

