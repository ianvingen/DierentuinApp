using DierentuinApp.Models;
using DierentuinApp.Services;
using Microsoft.EntityFrameworkCore;
using DierentuinApp.Data;

namespace DierentuinApp.Tests;

public class ZooServiceTests
{
    // We maken een in-memory database context voor tests die de service nodig hebben
    private ZooContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ZooContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ZooContext(options);
    }

    #region GetSunriseStatus Tests

    /// Test: Dagactieve dieren worden wakker bij zonsopgang.
    /// Onderbouwing: Diurnal dieren zijn actief overdag, dus ze moeten wakker worden bij sunrise.
    [Fact]
    public void GetSunriseStatus_DiurnalAnimal_ReturnsWordtWakker()
    {
        // Arrange - Maak een dagactief dier aan
        var animal = new Animal
        {
            Name = "TestLeeuw",
            Species = "Leeuw",
            ActivityPattern = ActivityPattern.Diurnal
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act - Vraag de sunrise status op
        var result = service.GetSunriseStatus(animal);

        // Assert - Controleer dat het dier wakker wordt
        Assert.Contains("wordt wakker", result);
        Assert.Contains("TestLeeuw", result);
    }

    /// Test: Nachtactieve dieren gaan slapen bij zonsopgang.
    /// Onderbouwing: Nocturnal dieren zijn 's nachts actief, dus bij sunrise gaan ze slapen.

    [Fact]
    public void GetSunriseStatus_NocturnalAnimal_ReturnsGaatSlapen()
    {
        // Arrange - Maak een nachtactief dier aan
        var animal = new Animal
        {
            Name = "TestUil",
            Species = "Uil",
            ActivityPattern = ActivityPattern.Nocturnal
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetSunriseStatus(animal);

        // Assert
        Assert.Contains("gaat slapen", result);
    }

    /// Test: Cathemerale dieren zijn altijd actief.
    /// Onderbouwing: Cathemeral dieren hebben geen vast dag/nacht ritme.
    [Fact]
    public void GetSunriseStatus_CathemeraalAnimal_ReturnsAltijdActief()
    {
        // Arrange
        var animal = new Animal
        {
            Name = "TestKat",
            Species = "Kat",
            ActivityPattern = ActivityPattern.Cathemeral
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetSunriseStatus(animal);

        // Assert
        Assert.Contains("altijd actief", result);
    }

    #endregion

    #region GetSunsetStatus Tests

    /// Test: Dagactieve dieren gaan slapen bij zonsondergang.
    /// Onderbouwing: Diurnal dieren zijn actief overdag, bij sunset gaan ze rusten.
    [Fact]
    public void GetSunsetStatus_DiurnalAnimal_ReturnsGaatSlapen()
    {
        // Arrange
        var animal = new Animal
        {
            Name = "TestZebra",
            Species = "Zebra",
            ActivityPattern = ActivityPattern.Diurnal
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetSunsetStatus(animal);

        // Assert
        Assert.Contains("gaat slapen", result);
    }

    /// Test: Nachtactieve dieren worden wakker bij zonsondergang.
    /// Onderbouwing: Nocturnal dieren beginnen hun actieve periode bij sunset.
    [Fact]
    public void GetSunsetStatus_NocturnalAnimal_ReturnsWordtWakker()
    {
        // Arrange
        var animal = new Animal
        {
            Name = "TestVleermuis",
            Species = "Vleermuis",
            ActivityPattern = ActivityPattern.Nocturnal
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetSunsetStatus(animal);

        // Assert
        Assert.Contains("wordt wakker", result);
    }

    #endregion

    #region GetFeedingInfo Tests

    /// Test: Herbivoren eten planten.
    /// Onderbouwing: De basis feeding info moet het juiste dieet tonen.
    [Fact]
    public void GetFeedingInfo_Herbivore_ReturnsPlanten()
    {
        // Arrange
        var animal = new Animal
        {
            Name = "TestOlifant",
            Species = "Olifant",
            DietaryClass = DietaryClass.Herbivore
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetFeedingInfo(animal);

        // Assert
        Assert.Contains("planten", result);
    }

    /// Test: Carnivoren met prooi in hetzelfde verblijf krijgen een waarschuwing.
    /// Onderbouwing: Dit is een belangrijke veiligheidsfunctie - roofdieren kunnen
    /// hun prooi opeten als ze in hetzelfde verblijf zitten.
    [Fact]
    public void GetFeedingInfo_CarnivoreWithPreyInEnclosure_ReturnsWarning()
    {
        // Arrange - Leeuw die zebra's eet, met een zebra in hetzelfde verblijf
        var leeuw = new Animal
        {
            Id = 1,
            Name = "Alex",
            Species = "Leeuw",
            DietaryClass = DietaryClass.Carnivore,
            Prey = "Zebra"
        };
        var zebra = new Animal
        {
            Id = 2,
            Name = "Marty",
            Species = "Zebra",
            DietaryClass = DietaryClass.Herbivore
        };
        var enclosureAnimals = new List<Animal> { leeuw, zebra };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetFeedingInfo(leeuw, enclosureAnimals);

        // Assert - Er moet een waarschuwing zijn dat de leeuw Marty kan opeten
        Assert.Contains("LET OP", result);
        Assert.Contains("Marty", result);
    }

    /// Test: Piscivoren (viseters) krijgen de juiste voedselinfo.
    /// Onderbouwing: Elke dietary class moet correct worden weergegeven.
    [Fact]
    public void GetFeedingInfo_Piscivore_ReturnsVis()
    {
        // Arrange
        var animal = new Animal
        {
            Name = "TestPinguin",
            Species = "Pinguïn",
            DietaryClass = DietaryClass.Piscivore
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.GetFeedingInfo(animal);

        // Assert
        Assert.Contains("vis", result);
    }

    #endregion

    #region CheckAnimalConstraints Tests

    /// Test: Een dier zonder verblijf krijgt een melding.
    /// Onderbouwing: Elk dier moet ergens gehuisvest zijn.
    [Fact]
    public void CheckAnimalConstraints_AnimalWithoutEnclosure_ReturnsIssue()
    {
        // Arrange
        var animal = new Animal
        {
            Name = "TestDier",
            Species = "Test",
            Enclosure = null
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var issues = service.CheckAnimalConstraints(animal);

        // Assert
        Assert.Single(issues);
        Assert.Contains("geen verblijf", issues[0]);
    }

    /// Test: Een dier met te hoge beveiligingseis krijgt een waarschuwing.
    /// Onderbouwing: Gevaarlijke dieren mogen niet in verblijven met lage beveiliging.
    [Fact]
    public void CheckAnimalConstraints_SecurityTooLow_ReturnsIssue()
    {
        // Arrange - Gevaarlijk dier in laag beveiligd verblijf
        var enclosure = new Enclosure
        {
            Name = "TestVerblijf",
            SecurityLevel = SecurityLevel.Low,
            Size = 1000
        };
        var animal = new Animal
        {
            Name = "TestTijger",
            Species = "Tijger",
            SecurityRequirement = SecurityLevel.High,
            Enclosure = enclosure
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var issues = service.CheckAnimalConstraints(animal);

        // Assert
        Assert.Single(issues);
        Assert.Contains("Beveiligingsniveau te laag", issues[0]);
    }

    /// Test: Een dier dat aan alle eisen voldoet heeft geen issues.
    /// Onderbouwing: Happy path - als alles goed is moet de lijst leeg zijn.
    [Fact]
    public void CheckAnimalConstraints_AllRequirementsMet_ReturnsNoIssues()
    {
        // Arrange
        var enclosure = new Enclosure
        {
            Name = "TestVerblijf",
            SecurityLevel = SecurityLevel.High,
            Size = 1000
        };
        var animal = new Animal
        {
            Name = "TestKonijn",
            Species = "Konijn",
            SecurityRequirement = SecurityLevel.Low,
            Enclosure = enclosure
        };
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var issues = service.CheckAnimalConstraints(animal);

        // Assert
        Assert.Empty(issues);
    }

    #endregion

    #region CheckEnclosureConstraints Tests

    /// Test: Verblijf met te weinig ruimte geeft een waarschuwing.
    /// Onderbouwing: Dieren hebben voldoende ruimte nodig voor hun welzijn.
    [Fact]
    public void CheckEnclosureConstraints_NotEnoughSpace_ReturnsIssue()
    {
        // Arrange - Klein verblijf met groot dier
        var enclosure = new Enclosure
        {
            Name = "KleinVerblijf",
            Size = 100, // 100 m²
            SecurityLevel = SecurityLevel.High,
            Animals = new List<Animal>
            {
                new Animal
                {
                    Name = "GroteOlifant",
                    Species = "Olifant",
                    SpaceRequirement = 500, // Heeft 500 m² nodig
                    SecurityRequirement = SecurityLevel.Medium
                }
            }
        };
        // Zet de enclosure referentie
        enclosure.Animals[0].Enclosure = enclosure;
        
        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.CheckEnclosureConstraints(enclosure);

        // Assert
        Assert.True(result.ContainsKey("Verblijf"));
        Assert.Contains(result["Verblijf"], issue => issue.Contains("Te weinig ruimte"));
    }

    /// Test: Verblijf met roofdier en prooi geeft een gevaar-waarschuwing.
    /// Onderbouwing: Dit is kritiek voor dierenwelzijn - prooidieren mogen niet
    /// bij hun roofdieren in hetzelfde verblijf.
    [Fact]
    public void CheckEnclosureConstraints_PredatorWithPrey_ReturnsGevaarWarning()
    {
        // Arrange - Leeuw en zebra in hetzelfde verblijf
        var leeuw = new Animal
        {
            Id = 1,
            Name = "Alex",
            Species = "Leeuw",
            DietaryClass = DietaryClass.Carnivore,
            Prey = "Zebra",
            SpaceRequirement = 100,
            SecurityRequirement = SecurityLevel.High
        };
        var zebra = new Animal
        {
            Id = 2,
            Name = "Marty",
            Species = "Zebra",
            DietaryClass = DietaryClass.Herbivore,
            SpaceRequirement = 100,
            SecurityRequirement = SecurityLevel.Medium
        };
        var enclosure = new Enclosure
        {
            Name = "Savanne",
            Size = 5000,
            SecurityLevel = SecurityLevel.High,
            Animals = new List<Animal> { leeuw, zebra }
        };
        leeuw.Enclosure = enclosure;
        zebra.Enclosure = enclosure;

        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.CheckEnclosureConstraints(enclosure);

        // Assert
        Assert.True(result.ContainsKey("Verblijf"));
        Assert.Contains(result["Verblijf"], issue => issue.Contains("GEVAAR"));
        Assert.Contains(result["Verblijf"], issue => issue.Contains("Marty"));
    }

    /// Test: Verblijf waar alles in orde is toont succesmelding.
    /// Onderbouwing: Gebruiker moet feedback krijgen dat alles goed is.
    [Fact]
    public void CheckEnclosureConstraints_AllOk_ReturnsSuccess()
    {
        // Arrange - Verblijf met alleen herbivoren
        var enclosure = new Enclosure
        {
            Name = "Herbivoor Weide",
            Size = 1000,
            SecurityLevel = SecurityLevel.Medium,
            Animals = new List<Animal>
            {
                new Animal
                {
                    Id = 1,
                    Name = "Konijn1",
                    Species = "Konijn",
                    DietaryClass = DietaryClass.Herbivore,
                    SpaceRequirement = 10,
                    SecurityRequirement = SecurityLevel.Low
                },
                new Animal
                {
                    Id = 2,
                    Name = "Konijn2",
                    Species = "Konijn",
                    DietaryClass = DietaryClass.Herbivore,
                    SpaceRequirement = 10,
                    SecurityRequirement = SecurityLevel.Low
                }
            }
        };
        enclosure.Animals[0].Enclosure = enclosure;
        enclosure.Animals[1].Enclosure = enclosure;

        var context = CreateInMemoryContext();
        var service = new ZooService(context);

        // Act
        var result = service.CheckEnclosureConstraints(enclosure);

        // Assert
        Assert.True(result.ContainsKey("Status"));
        Assert.Contains(result["Status"], msg => msg.Contains("voldaan"));
    }

    #endregion
}