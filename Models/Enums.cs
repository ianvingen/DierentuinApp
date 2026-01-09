using System.ComponentModel.DataAnnotations;

namespace DierentuinApp.Models;

public enum AnimalSize
{
    [Display(Name = "Microscopisch")]
    Microscopic,
    [Display(Name = "Heel Klein")]
    VerySmall,
    [Display(Name = "Klein")]
    Small,
    [Display(Name = "Middel")]
    Medium,
    [Display(Name = "Groot")]
    Large,
    [Display(Name = "Heel Groot")]
    VeryLarge
}

public enum DietaryClass
{
    [Display(Name = "Vleeseter")]
    Carnivore,
    [Display(Name = "Planteneter")]
    Herbivore,
    [Display(Name = "Alleseter")]
    Omnivore,
    [Display(Name = "Insecteneter")]
    Insectivore,
    [Display(Name = "Viseter")]
    Piscivore
}

public enum ActivityPattern
{
    [Display(Name = "Dagactief")]
    Diurnal,
    [Display(Name = "Nachtactief")]
    Nocturnal,
    [Display(Name = "Altijd Actief")]
    Cathemeral
}

public enum SecurityLevel
{
    [Display(Name = "Laag")]
    Low,
    [Display(Name = "Middel")]
    Medium,
    [Display(Name = "Hoog")]
    High
}

public enum Climate
{
    [Display(Name = "Tropisch")]
    Tropical,
    [Display(Name = "Gematigd")]
    Temperate,
    [Display(Name = "Arctisch")]
    Arctic
}

[Flags]
public enum HabitatType
{
    [Display(Name = "Bos")]
    Forest = 1,
    [Display(Name = "Water")]
    Aquatic = 2,
    [Display(Name = "Woestijn")]
    Desert = 4,
    [Display(Name = "Grasland")]
    Grassland = 8
}
