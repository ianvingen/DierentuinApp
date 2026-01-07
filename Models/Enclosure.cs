namespace DierentuinApp.Models;

public class Enclosure
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Animal> Animals { get; set; } = new List<Animal>();

    public Climate Climate { get; set; }
    public HabitatType HabitatType { get; set; }

    public SecurityLevel SecurityLevel { get; set; }

    // In vierkante meters
    public double Size { get; set; }
}