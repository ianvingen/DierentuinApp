namespace DierentuinApp.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property: een categorie heeft meerdere dieren
    public List<Animal> Animals { get; set; } = new List<Animal>();
}

