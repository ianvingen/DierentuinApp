using DierentuinApp.Data;
using DierentuinApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DierentuinApp.Services;

public class ZooService
{
    private readonly ZooContext _context;

    public ZooService(ZooContext context)
    {
        _context = context;
    }

    #region Sunrise/Sunset

    // Bepaalt wat er met een dier gebeurt bij zonsopgang
    public string GetSunriseStatus(Animal animal)
    {
        return animal.ActivityPattern switch
        {
            ActivityPattern.Diurnal => $"{animal.Name} wordt wakker",
            ActivityPattern.Nocturnal => $"{animal.Name} gaat slapen",
            ActivityPattern.Cathemeral => $"{animal.Name} is altijd actief",
            _ => "Onbekend"
        };
    }

    // Bepaalt wat er met een dier gebeurt bij zonsondergang
    public string GetSunsetStatus(Animal animal)
    {
        return animal.ActivityPattern switch
        {
            ActivityPattern.Diurnal => $"{animal.Name} gaat slapen",
            ActivityPattern.Nocturnal => $"{animal.Name} wordt wakker",
            ActivityPattern.Cathemeral => $"{animal.Name} is altijd actief",
            _ => "Onbekend"
        };
    }

    // Sunrise voor alle dieren in een verblijf
    public List<string> GetEnclosureSunrise(Enclosure enclosure)
    {
        return enclosure.Animals.Select(a => GetSunriseStatus(a)).ToList();
    }

    // Sunset voor alle dieren in een verblijf
    public List<string> GetEnclosureSunset(Enclosure enclosure)
    {
        return enclosure.Animals.Select(a => GetSunsetStatus(a)).ToList();
    }

    // Sunrise voor hele dierentuin
    public async Task<Dictionary<string, List<string>>> GetZooSunriseAsync()
    {
        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();

        var result = new Dictionary<string, List<string>>();
        foreach (var enclosure in enclosures)
        {
            result[enclosure.Name] = GetEnclosureSunrise(enclosure);
        }

        // Dieren zonder verblijf
        var unassigned = await _context.Animals
            .Where(a => a.EnclosureId == null)
            .ToListAsync();
        if (unassigned.Any())
        {
            result["Zonder verblijf"] = unassigned.Select(a => GetSunriseStatus(a)).ToList();
        }

        return result;
    }

    // Sunset voor hele dierentuin
    public async Task<Dictionary<string, List<string>>> GetZooSunsetAsync()
    {
        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();

        var result = new Dictionary<string, List<string>>();
        foreach (var enclosure in enclosures)
        {
            result[enclosure.Name] = GetEnclosureSunset(enclosure);
        }

        var unassigned = await _context.Animals
            .Where(a => a.EnclosureId == null)
            .ToListAsync();
        if (unassigned.Any())
        {
            result["Zonder verblijf"] = unassigned.Select(a => GetSunsetStatus(a)).ToList();
        }

        return result;
    }

    #endregion

    #region FeedingTime

    // Bepaalt wat een dier eet (prooi in hetzelfde verblijf gaat voor)
    public string GetFeedingInfo(Animal animal, List<Animal>? enclosureAnimals = null)
    {
        var dietDescription = animal.DietaryClass switch
        {
            DietaryClass.Carnivore => "vlees",
            DietaryClass.Herbivore => "planten",
            DietaryClass.Omnivore => "alles",
            DietaryClass.Insectivore => "insecten",
            DietaryClass.Piscivore => "vis",
            _ => "onbekend"
        };

        // Check of dit dier andere dieren eet en of er prooi in hetzelfde verblijf zit
        if (animal.DietaryClass == DietaryClass.Carnivore || animal.DietaryClass == DietaryClass.Omnivore)
        {
            if (!string.IsNullOrEmpty(animal.Prey) && enclosureAnimals != null)
            {
                // Zoek of er prooi-dieren in hetzelfde verblijf zitten
                var preyTypes = animal.Prey.Split(',').Select(p => p.Trim().ToLower()).ToList();
                var potentialPrey = enclosureAnimals
                    .Where(a => a.Id != animal.Id && preyTypes.Any(p => a.Species.ToLower().Contains(p)))
                    .ToList();

                if (potentialPrey.Any())
                {
                    var preyNames = string.Join(", ", potentialPrey.Select(p => p.Name));
                    return $"{animal.Name} eet {dietDescription} - LET OP: kan {preyNames} opeten!";
                }
            }

            if (!string.IsNullOrEmpty(animal.Prey))
            {
                return $"{animal.Name} eet {dietDescription} (jaagt op: {animal.Prey})";
            }
        }

        return $"{animal.Name} eet {dietDescription}";
    }

    // FeedingTime voor een verblijf
    public List<string> GetEnclosureFeedingTime(Enclosure enclosure)
    {
        return enclosure.Animals.Select(a => GetFeedingInfo(a, enclosure.Animals)).ToList();
    }

    // FeedingTime voor hele dierentuin
    public async Task<Dictionary<string, List<string>>> GetZooFeedingTimeAsync()
    {
        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();

        var result = new Dictionary<string, List<string>>();
        foreach (var enclosure in enclosures)
        {
            result[enclosure.Name] = GetEnclosureFeedingTime(enclosure);
        }

        var unassigned = await _context.Animals
            .Where(a => a.EnclosureId == null)
            .ToListAsync();
        if (unassigned.Any())
        {
            result["Zonder verblijf"] = unassigned.Select(a => GetFeedingInfo(a)).ToList();
        }

        return result;
    }

    #endregion

    #region CheckConstraints

    // Controleert of een dier aan alle eisen voldoet in zijn huidige verblijf
    public List<string> CheckAnimalConstraints(Animal animal)
    {
        var issues = new List<string>();

        if (animal.Enclosure == null)
        {
            issues.Add("Heeft geen verblijf toegewezen");
            return issues;
        }

        // Dier mag niet gevaarlijker zijn dan verblijf aankan
        if (animal.SecurityRequirement > animal.Enclosure.SecurityLevel)
        {
            issues.Add($"Beveiligingsniveau te laag (nodig: {animal.SecurityRequirement}, verblijf: {animal.Enclosure.SecurityLevel})");
        }

        return issues;
    }

    // Controleert alle constraints voor een verblijf
    public Dictionary<string, List<string>> CheckEnclosureConstraints(Enclosure enclosure)
    {
        var result = new Dictionary<string, List<string>>();

        // Verblijf-niveau checks
        var enclosureIssues = new List<string>();

        // Ruimte check: totale ruimtebehoefte vs beschikbare ruimte
        var totalSpaceNeeded = enclosure.Animals.Sum(a => a.SpaceRequirement);
        if (totalSpaceNeeded > enclosure.Size)
        {
            enclosureIssues.Add($"Te weinig ruimte (nodig: {totalSpaceNeeded}m², beschikbaar: {enclosure.Size}m²)");
        }

        // Check of er roofdieren bij hun prooi zitten
        var carnivores = enclosure.Animals
            .Where(a => (a.DietaryClass == DietaryClass.Carnivore || a.DietaryClass == DietaryClass.Omnivore) 
                        && !string.IsNullOrEmpty(a.Prey))
            .ToList();

        foreach (var predator in carnivores)
        {
            var preyTypes = predator.Prey!.Split(',').Select(p => p.Trim().ToLower()).ToList();
            var preyInEnclosure = enclosure.Animals
                .Where(a => a.Id != predator.Id && preyTypes.Any(p => a.Species.ToLower().Contains(p)))
                .ToList();

            if (preyInEnclosure.Any())
            {
                var preyNames = string.Join(", ", preyInEnclosure.Select(p => p.Name));
                enclosureIssues.Add($"GEVAAR: {predator.Name} kan {preyNames} opeten!");
            }
        }

        if (enclosureIssues.Any())
        {
            result["Verblijf"] = enclosureIssues;
        }

        // Per-dier checks
        foreach (var animal in enclosure.Animals)
        {
            var animalIssues = CheckAnimalConstraints(animal);
            if (animalIssues.Any())
            {
                result[animal.Name] = animalIssues;
            }
        }

        // Toon status als alles goed is
        if (!result.Any())
        {
            result["Status"] = new List<string> { "Alle constraints voldaan ✓" };
        }

        return result;
    }

    // CheckConstraints voor hele dierentuin
    public async Task<Dictionary<string, Dictionary<string, List<string>>>> CheckZooConstraintsAsync()
    {
        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();

        var result = new Dictionary<string, Dictionary<string, List<string>>>();

        foreach (var enclosure in enclosures)
        {
            result[enclosure.Name] = CheckEnclosureConstraints(enclosure);
        }

        // Check dieren zonder verblijf
        var unassigned = await _context.Animals
            .Where(a => a.EnclosureId == null)
            .ToListAsync();

        if (unassigned.Any())
        {
            var unassignedIssues = new Dictionary<string, List<string>>();
            foreach (var animal in unassigned)
            {
                unassignedIssues[animal.Name] = new List<string> { "Heeft geen verblijf toegewezen" };
            }
            result["Zonder verblijf"] = unassignedIssues;
        }

        return result;
    }

    #endregion

    #region AutoAssign

    // Wijst automatisch dieren toe aan verblijven
    public async Task<string> AutoAssignAsync(bool clearExisting)
    {
        if (clearExisting)
        {
            // Reset alle dieren naar geen verblijf
            var allAnimals = await _context.Animals.ToListAsync();
            foreach (var animal in allAnimals)
            {
                animal.EnclosureId = null;
            }
            await _context.SaveChangesAsync();
        }

        // Haal alle dieren zonder verblijf op
        var unassignedAnimals = await _context.Animals
            .Where(a => a.EnclosureId == null)
            .OrderByDescending(a => a.SpaceRequirement) // Grote dieren eerst
            .ThenByDescending(a => a.SecurityRequirement)
            .ToListAsync();

        var enclosures = await _context.Enclosures
            .Include(e => e.Animals)
            .ToListAsync();

        int assignedCount = 0;
        int newEnclosuresCount = 0;

        foreach (var animal in unassignedAnimals)
        {
            // Zoek geschikt verblijf
            var suitableEnclosure = FindSuitableEnclosure(animal, enclosures);

            if (suitableEnclosure != null)
            {
                animal.EnclosureId = suitableEnclosure.Id;
                suitableEnclosure.Animals.Add(animal);
                assignedCount++;
            }
            else
            {
                // Maak nieuw verblijf aan
                var newEnclosure = CreateEnclosureForAnimal(animal);
                _context.Enclosures.Add(newEnclosure);
                await _context.SaveChangesAsync();

                animal.EnclosureId = newEnclosure.Id;
                newEnclosure.Animals.Add(animal);
                enclosures.Add(newEnclosure);
                assignedCount++;
                newEnclosuresCount++;
            }
        }

        await _context.SaveChangesAsync();

        return $"AutoAssign voltooid: {assignedCount} dieren toegewezen, {newEnclosuresCount} nieuwe verblijven aangemaakt";
    }

    // Zoekt een geschikt verblijf voor een dier
    private Enclosure? FindSuitableEnclosure(Animal animal, List<Enclosure> enclosures)
    {
        foreach (var enclosure in enclosures)
        {
            // Check security level
            if (enclosure.SecurityLevel < animal.SecurityRequirement)
                continue;

            // Check beschikbare ruimte
            var usedSpace = enclosure.Animals.Sum(a => a.SpaceRequirement);
            if (usedSpace + animal.SpaceRequirement > enclosure.Size)
                continue;

            // Check of dit dier geen gevaar vormt voor anderen (en vice versa)
            if (IsConflictingAnimal(animal, enclosure.Animals))
                continue;

            return enclosure;
        }

        return null;
    }

    // Checkt of een dier conflicteert met andere dieren (prooi/roofdier relatie)
    private bool IsConflictingAnimal(Animal newAnimal, List<Animal> existingAnimals)
    {
        // Check of nieuwe dier bestaande dieren kan opeten
        if (!string.IsNullOrEmpty(newAnimal.Prey))
        {
            var preyTypes = newAnimal.Prey.Split(',').Select(p => p.Trim().ToLower()).ToList();
            if (existingAnimals.Any(a => preyTypes.Any(p => a.Species.ToLower().Contains(p))))
                return true;
        }

        // Check of bestaande dieren het nieuwe dier kunnen opeten
        foreach (var existing in existingAnimals)
        {
            if (!string.IsNullOrEmpty(existing.Prey))
            {
                var preyTypes = existing.Prey.Split(',').Select(p => p.Trim().ToLower()).ToList();
                if (preyTypes.Any(p => newAnimal.Species.ToLower().Contains(p)))
                    return true;
            }
        }

        return false;
    }

    // Maakt een nieuw verblijf aan dat past bij het dier
    private Enclosure CreateEnclosureForAnimal(Animal animal)
    {
        return new Enclosure
        {
            Name = $"Verblijf {animal.Species}",
            Size = animal.SpaceRequirement * 2,
            SecurityLevel = animal.SecurityRequirement,
            Climate = Climate.Temperate,
            HabitatType = HabitatType.Grassland
        };
    }

    #endregion
}

