using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using plantita.User.Domain.Model.Aggregates;
using Xunit;

namespace Plantita.CoreTests.Infrastructure;

/// <summary>
/// Integration tests for repository operations with in-memory database
/// </summary>
public class RepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;

    public RepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task AuthUser_CanBeSaved_ToDatabase()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Name = "Test User",
            Timezone = "America/Lima",
            Language = "es"
        };

        // Act
        _context.Set<AuthUser>().Add(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Set<AuthUser>()
            .FirstOrDefaultAsync(u => u.Email == "[email protected]");

        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("[email protected]");
        savedUser.Name.Should().Be("Test User");
        savedUser.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task AuthUser_CanBeRetrieved_ByEmail()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Name = "Unique User",
            Timezone = "UTC",
            Language = "en"
        };

        _context.Set<AuthUser>().Add(user);
        await _context.SaveChangesAsync();

        // Act
        var retrievedUser = await _context.Set<AuthUser>()
            .FirstOrDefaultAsync(u => u.Email == "[email protected]");

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Email.Should().Be("[email protected]");
        retrievedUser.Name.Should().Be("Unique User");
    }

    [Fact]
    public async Task Plant_CanBeSaved_ToDatabase()
    {
        // Arrange
        var plant = new Plant
        {
            ScientificName = "Monstera deliciosa",
            CommonName = "Costilla de Adán",
            Description = "Planta tropical de hojas grandes",
            WateringFrequency = "Cada 7-10 días",
            SunlightRequirement = "Luz indirecta brillante"
        };

        // Act
        _context.Set<Plant>().Add(plant);
        await _context.SaveChangesAsync();

        // Assert
        var savedPlant = await _context.Set<Plant>()
            .FirstOrDefaultAsync(p => p.ScientificName == "Monstera deliciosa");

        savedPlant.Should().NotBeNull();
        savedPlant!.CommonName.Should().Be("Costilla de Adán");
        savedPlant.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MyPlant_WithRelations_CanBeSaved()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Name = "Plant Owner",
            Timezone = "UTC",
            Language = "en"
        };

        var plant = new Plant
        {
            ScientificName = "Test Plant",
            CommonName = "Test",
            Description = "Test",
            WateringFrequency = "Weekly",
            SunlightRequirement = "Medium"
        };

        _context.Set<AuthUser>().Add(user);
        _context.Set<Plant>().Add(plant);
        await _context.SaveChangesAsync();

        var myPlant = new MyPlant
        {
            PlantId = plant.Id,
            UserId = user.Id,
            CustomName = "Mi planta de prueba",
            Location = "Sala",
            Notes = "Notas de prueba"
        };

        // Act
        _context.Set<MyPlant>().Add(myPlant);
        await _context.SaveChangesAsync();

        // Assert
        var savedMyPlant = await _context.Set<MyPlant>()
            .Include(mp => mp.Plant)
            .FirstOrDefaultAsync(mp => mp.CustomName == "Mi planta de prueba");

        savedMyPlant.Should().NotBeNull();
        savedMyPlant!.Plant.Should().NotBeNull();
        savedMyPlant.Plant!.ScientificName.Should().Be("Test Plant");
        savedMyPlant.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task IoTDevice_CanBeSaved_ToDatabase()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Name = "Device Owner",
            Timezone = "UTC",
            Language = "en"
        };

        _context.Set<AuthUser>().Add(user);
        await _context.SaveChangesAsync();

        var device = new plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates.IoTDevice
        {
            Name = "Monitor Sala",
            ConnectionType = "WiFi",
            Location = "Sala de estar",
            FirmwareVersion = "v1.0.0",
            IsActive = true,
            UserId = user.Id
        };

        // Act
        _context.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates.IoTDevice>().Add(device);
        await _context.SaveChangesAsync();

        // Assert
        var savedDevice = await _context.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates.IoTDevice>()
            .FirstOrDefaultAsync(d => d.Name == "Monitor Sala");

        savedDevice.Should().NotBeNull();
        savedDevice!.ConnectionType.Should().Be("WiFi");
        savedDevice.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Sensor_WithReadings_CanBeSaved()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Name = "Sensor Owner",
            Timezone = "UTC",
            Language = "en"
        };

        _context.Set<AuthUser>().Add(user);
        await _context.SaveChangesAsync();

        var device = new plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates.IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            UserId = user.Id
        };

        _context.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates.IoTDevice>().Add(device);
        await _context.SaveChangesAsync();

        var sensor = new plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.Sensor
        {
            DeviceId = device.Id,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        _context.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.Sensor>().Add(sensor);
        await _context.SaveChangesAsync();

        // Add readings
        for (int i = 0; i < 5; i++)
        {
            sensor.Readings.Add(new plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.SensorReading
            {
                SensorId = sensor.Id,
                Value = 20.0 + i,
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var savedSensor = await _context.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.Sensor>()
            .Include(s => s.Readings)
            .FirstOrDefaultAsync(s => s.SensorType == "Temperature");

        savedSensor.Should().NotBeNull();
        savedSensor!.Readings.Should().HaveCount(5);
        savedSensor.Readings.Select(r => r.Value).Should().Contain(new[] { 20.0, 21.0, 22.0, 23.0, 24.0 });
    }

    [Fact]
    public async Task MultipleUsers_CanHave_SeparatePlants()
    {
        // Arrange
        var user1 = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Name = "User One",
            Timezone = "UTC",
            Language = "en"
        };

        var user2 = new AuthUser
        {
            Email = "[email protected]",
            Password = BCrypt.Net.BCrypt.HashPassword("Pass123!"),
            Name = "User Two",
            Timezone = "UTC",
            Language = "en"
        };

        _context.Set<AuthUser>().AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var basePlant = new Plant
        {
            ScientificName = "Shared Plant",
            CommonName = "Common",
            Description = "Test",
            WateringFrequency = "Weekly",
            SunlightRequirement = "Medium"
        };

        _context.Set<Plant>().Add(basePlant);
        await _context.SaveChangesAsync();

        var user1Plant = new MyPlant
        {
            PlantId = basePlant.Id,
            UserId = user1.Id,
            CustomName = "User 1's Plant"
        };

        var user2Plant = new MyPlant
        {
            PlantId = basePlant.Id,
            UserId = user2.Id,
            CustomName = "User 2's Plant"
        };

        // Act
        _context.Set<MyPlant>().AddRange(user1Plant, user2Plant);
        await _context.SaveChangesAsync();

        // Assert
        var user1Plants = await _context.Set<MyPlant>()
            .Where(mp => mp.UserId == user1.Id)
            .ToListAsync();

        var user2Plants = await _context.Set<MyPlant>()
            .Where(mp => mp.UserId == user2.Id)
            .ToListAsync();

        user1Plants.Should().HaveCount(1);
        user2Plants.Should().HaveCount(1);
        user1Plants.First().CustomName.Should().Be("User 1's Plant");
        user2Plants.First().CustomName.Should().Be("User 2's Plant");
    }

    [Fact]
    public async Task Plant_CanBeDeleted_FromDatabase()
    {
        // Arrange
        var plant = new Plant
        {
            ScientificName = "To Be Deleted",
            CommonName = "Test",
            Description = "Test",
            WateringFrequency = "Weekly",
            SunlightRequirement = "Medium"
        };

        _context.Set<Plant>().Add(plant);
        await _context.SaveChangesAsync();
        var plantId = plant.Id;

        // Act
        _context.Set<Plant>().Remove(plant);
        await _context.SaveChangesAsync();

        // Assert
        var deletedPlant = await _context.Set<Plant>()
            .FirstOrDefaultAsync(p => p.Id == plantId);

        deletedPlant.Should().BeNull();
    }

    [Fact]
    public async Task Plant_CanBeUpdated_InDatabase()
    {
        // Arrange
        var plant = new Plant
        {
            ScientificName = "Original Name",
            CommonName = "Test",
            Description = "Original Description",
            WateringFrequency = "Weekly",
            SunlightRequirement = "Medium"
        };

        _context.Set<Plant>().Add(plant);
        await _context.SaveChangesAsync();

        // Act
        plant.Description = "Updated Description";
        plant.WateringFrequency = "Bi-weekly";
        await _context.SaveChangesAsync();

        // Assert
        var updatedPlant = await _context.Set<Plant>()
            .FirstOrDefaultAsync(p => p.ScientificName == "Original Name");

        updatedPlant.Should().NotBeNull();
        updatedPlant!.Description.Should().Be("Updated Description");
        updatedPlant.WateringFrequency.Should().Be("Bi-weekly");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
