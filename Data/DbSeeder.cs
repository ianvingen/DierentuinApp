using Bogus;
using DierentuinApp.Models;

namespace DierentuinApp.Data;

public static class DbSeeder
{
    public static void Seed(ZooContext context)
    {
        // Alleen seeden als de database leeg is
        if (context.Animals.Any() || context.Categories.Any() || context.Enclosures.Any())
        {
            return;
        }

        // Seed categorieën
        var categories = GetCategories();
        context.Categories.AddRange(categories);
        context.SaveChanges();

        // Seed verblijven
        var enclosures = GetEnclosures();
        context.Enclosures.AddRange(enclosures);
        context.SaveChanges();

        // Seed dieren
        var animals = GetAnimals(categories, enclosures);
        context.Animals.AddRange(animals);
        context.SaveChanges();
    }

    private static List<Category> GetCategories()
    {
        return new List<Category>
        {
            new Category { Name = "Zoogdieren" },
            new Category { Name = "Vogels" },
            new Category { Name = "Reptielen" },
            new Category { Name = "Amfibieën" },
            new Category { Name = "Vissen" },
            new Category { Name = "Insecten" }
        };
    }

    private static List<Enclosure> GetEnclosures()
    {
        return new List<Enclosure>
        {
            new Enclosure
            {
                Name = "Savanne",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Grassland,
                SecurityLevel = SecurityLevel.High,
                Size = 5000.0
            },
            new Enclosure
            {
                Name = "Tropisch Regenwoud",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Forest,
                SecurityLevel = SecurityLevel.Medium,
                Size = 2000.0
            },
            new Enclosure
            {
                Name = "Aquarium",
                Climate = Climate.Temperate,
                HabitatType = HabitatType.Aquatic,
                SecurityLevel = SecurityLevel.Low,
                Size = 500.0
            },
            new Enclosure
            {
                Name = "Woestijn",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Desert,
                SecurityLevel = SecurityLevel.Medium,
                Size = 1500.0
            },
            new Enclosure
            {
                Name = "Noordpool",
                Climate = Climate.Arctic,
                HabitatType = HabitatType.Aquatic,
                SecurityLevel = SecurityLevel.Medium,
                Size = 3000.0
            },
            new Enclosure
            {
                Name = "Vlindertuin",
                Climate = Climate.Tropical,
                HabitatType = HabitatType.Forest,
                SecurityLevel = SecurityLevel.Low,
                Size = 300.0
            }
        };
    }

    private static List<Animal> GetAnimals(List<Category> categories, List<Enclosure> enclosures)
    {
        var faker = new Faker("nl");

        // Zoek categorieën op naam
        var zoogdieren = categories.First(c => c.Name == "Zoogdieren");
        var vogels = categories.First(c => c.Name == "Vogels");
        var reptielen = categories.First(c => c.Name == "Reptielen");
        var vissen = categories.First(c => c.Name == "Vissen");
        var insecten = categories.First(c => c.Name == "Insecten");

        // Zoek verblijven op naam
        var savanne = enclosures.First(e => e.Name == "Savanne");
        var regenwoud = enclosures.First(e => e.Name == "Tropisch Regenwoud");
        var aquarium = enclosures.First(e => e.Name == "Aquarium");
        var woestijn = enclosures.First(e => e.Name == "Woestijn");
        var noordpool = enclosures.First(e => e.Name == "Noordpool");
        var vlindertuin = enclosures.First(e => e.Name == "Vlindertuin");

        var animals = new List<Animal>
        {
            // Savanne dieren
            new Animal
            {
                Name = "Alex",
                Species = "Leeuw",
                Category = zoogdieren,
                Size = AnimalSize.Large,
                DietaryClass = DietaryClass.Carnivore,
                ActivityPattern = ActivityPattern.Cathemeral,
                Prey = "Zebra's, Antilopen",
                Enclosure = savanne,
                SpaceRequirement = 500.0,
                SecurityRequirement = SecurityLevel.High
            },
            new Animal
            {
                Name = "Marty",
                Species = "Zebra",
                Category = zoogdieren,
                Size = AnimalSize.Large,
                DietaryClass = DietaryClass.Herbivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = savanne,
                SpaceRequirement = 300.0,
                SecurityRequirement = SecurityLevel.Medium
            },
            new Animal
            {
                Name = "Melman",
                Species = "Giraffe",
                Category = zoogdieren,
                Size = AnimalSize.VeryLarge,
                DietaryClass = DietaryClass.Herbivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = savanne,
                SpaceRequirement = 400.0,
                SecurityRequirement = SecurityLevel.Medium
            },
            // Regenwoud dieren
            new Animal
            {
                Name = "Zazu",
                Species = "Toekan",
                Category = vogels,
                Size = AnimalSize.Small,
                DietaryClass = DietaryClass.Omnivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = regenwoud,
                SpaceRequirement = 20.0,
                SecurityRequirement = SecurityLevel.Low
            },
            new Animal
            {
                Name = "Harambe",
                Species = "Gorilla",
                Category = zoogdieren,
                Size = AnimalSize.Large,
                DietaryClass = DietaryClass.Herbivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = regenwoud,
                SpaceRequirement = 200.0,
                SecurityRequirement = SecurityLevel.High
            },
            new Animal
            {
                Name = "Kaa",
                Species = "Python",
                Category = reptielen,
                Size = AnimalSize.Medium,
                DietaryClass = DietaryClass.Carnivore,
                ActivityPattern = ActivityPattern.Nocturnal,
                Prey = "Knaagdieren, Vogels",
                Enclosure = regenwoud,
                SpaceRequirement = 30.0,
                SecurityRequirement = SecurityLevel.High
            },
            // Aquarium dieren
            new Animal
            {
                Name = "Nemo",
                Species = "Clownvis",
                Category = vissen,
                Size = AnimalSize.VerySmall,
                DietaryClass = DietaryClass.Omnivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = aquarium,
                SpaceRequirement = 0.5,
                SecurityRequirement = SecurityLevel.Low
            },
            new Animal
            {
                Name = "Jaws",
                Species = "Haai",
                Category = vissen,
                Size = AnimalSize.Large,
                DietaryClass = DietaryClass.Carnivore,
                ActivityPattern = ActivityPattern.Cathemeral,
                Prey = "Kleine vissen",
                Enclosure = aquarium,
                SpaceRequirement = 100.0,
                SecurityRequirement = SecurityLevel.High
            },
            // Woestijn dieren
            new Animal
            {
                Name = "Kammie",
                Species = "Kameel",
                Category = zoogdieren,
                Size = AnimalSize.Large,
                DietaryClass = DietaryClass.Herbivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = woestijn,
                SpaceRequirement = 100.0,
                SecurityRequirement = SecurityLevel.Low
            },
            new Animal
            {
                Name = "Rango",
                Species = "Leguaan",
                Category = reptielen,
                Size = AnimalSize.Medium,
                DietaryClass = DietaryClass.Herbivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = woestijn,
                SpaceRequirement = 15.0,
                SecurityRequirement = SecurityLevel.Low
            },
            // Noordpool dieren
            new Animal
            {
                Name = "Esbjaerg",
                Species = "IJsbeer",
                Category = zoogdieren,
                Size = AnimalSize.VeryLarge,
                DietaryClass = DietaryClass.Carnivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = "Zeehonden, Vissen",
                Enclosure = noordpool,
                SpaceRequirement = 800.0,
                SecurityRequirement = SecurityLevel.High
            },
            new Animal
            {
                Name = "Kowalski",
                Species = "Pinguïn",
                Category = vogels,
                Size = AnimalSize.Medium,
                DietaryClass = DietaryClass.Piscivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = noordpool,
                SpaceRequirement = 15.0,
                SecurityRequirement = SecurityLevel.Low
            },
            // Vlindertuin
            new Animal
            {
                Name = "Butterfree",
                Species = "Monarchvlinder",
                Category = insecten,
                Size = AnimalSize.Microscopic,
                DietaryClass = DietaryClass.Herbivore,
                ActivityPattern = ActivityPattern.Diurnal,
                Prey = null,
                Enclosure = vlindertuin,
                SpaceRequirement = 0.1,
                SecurityRequirement = SecurityLevel.Low
            }
        };

        // Bogus voegt nog wat extra dieren toe voor variatie
        var extraAnimals = GenerateExtraAnimals(faker, zoogdieren, savanne, 3);
        animals.AddRange(extraAnimals);

        return animals;
    }

    private static List<Animal> GenerateExtraAnimals(Faker faker, Category category, Enclosure enclosure, int count)
    {
        var animalFaker = new Faker<Animal>("nl")
            .RuleFor(a => a.Name, f => f.Name.FirstName())
            .RuleFor(a => a.Species, f => f.PickRandom("Olifant", "Neushoorn", "Nijlpaard"))
            .RuleFor(a => a.Category, category)
            .RuleFor(a => a.Size, AnimalSize.VeryLarge)
            .RuleFor(a => a.DietaryClass, DietaryClass.Herbivore)
            .RuleFor(a => a.ActivityPattern, f => f.PickRandom<ActivityPattern>())
            .RuleFor(a => a.Prey, (string?)null)
            .RuleFor(a => a.Enclosure, enclosure)
            .RuleFor(a => a.SpaceRequirement, f => f.Random.Double(200, 600))
            .RuleFor(a => a.SecurityRequirement, SecurityLevel.High);

        return animalFaker.Generate(count);
    }
}

