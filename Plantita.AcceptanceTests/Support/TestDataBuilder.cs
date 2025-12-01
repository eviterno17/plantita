using Bogus;

namespace Plantita.AcceptanceTests.Support;

/// <summary>
/// Builder for generating realistic test data using Bogus
/// </summary>
public class TestDataBuilder
{
    private readonly Faker _faker;

    public TestDataBuilder()
    {
        _faker = new Faker("es");
    }

    /// <summary>
    /// Generate a random user
    /// </summary>
    public object GenerateUser()
    {
        return new
        {
            email = _faker.Internet.Email(),
            password = GeneratePassword(),
            name = _faker.Name.FullName(),
            timezone = "America/Lima",
            language = "es"
        };
    }

    /// <summary>
    /// Generate a random plant
    /// </summary>
    public object GeneratePlant()
    {
        var plantNames = new[]
        {
            ("Monstera deliciosa", "Costilla de Adán"),
            ("Ficus elastica", "Árbol del caucho"),
            ("Pothos aureus", "Potus dorado"),
            ("Sansevieria trifasciata", "Lengua de suegra"),
            ("Spathiphyllum", "Lirio de la paz")
        };

        var plant = _faker.PickRandom(plantNames);

        return new
        {
            scientificName = plant.Item1,
            commonName = plant.Item2,
            description = _faker.Lorem.Sentence(),
            wateringFrequency = $"Cada {_faker.Random.Int(5, 14)} días",
            sunlightRequirement = _faker.PickRandom("Luz directa", "Luz indirecta brillante", "Luz indirecta moderada", "Sombra parcial")
        };
    }

    /// <summary>
    /// Generate a random IoT device
    /// </summary>
    public object GenerateIoTDevice()
    {
        return new
        {
            name = $"Monitor {_faker.Address.City()}",
            connectionType = _faker.PickRandom("WiFi", "Bluetooth", "Zigbee"),
            location = _faker.Address.City(),
            firmwareVersion = $"v{_faker.System.Version()}",
            isActive = true
        };
    }

    /// <summary>
    /// Generate a random sensor
    /// </summary>
    public object GenerateSensor(int deviceId)
    {
        return new
        {
            deviceId = deviceId,
            sensorType = _faker.PickRandom("Temperature", "SoilMoisture", "Light", "AirHumidity"),
            unit = _faker.PickRandom("Celsius", "Percentage", "Lux"),
            minRange = 0,
            maxRange = _faker.Random.Int(50, 100),
            isActive = true
        };
    }

    /// <summary>
    /// Generate a sensor reading
    /// </summary>
    public object GenerateSensorReading(int sensorId, string sensorType)
    {
        var value = sensorType switch
        {
            "Temperature" => _faker.Random.Double(15, 35),
            "SoilMoisture" => _faker.Random.Double(20, 80),
            "Light" => _faker.Random.Double(1000, 50000),
            "AirHumidity" => _faker.Random.Double(30, 70),
            _ => _faker.Random.Double(0, 100)
        };

        return new
        {
            sensorId = sensorId,
            value = value,
            timestamp = _faker.Date.Recent(7)
        };
    }

    /// <summary>
    /// Generate a strong password
    /// </summary>
    public string GeneratePassword()
    {
        return _faker.Internet.Password(12, true, @"[A-Z]", "!@#$");
    }

    /// <summary>
    /// Generate an email
    /// </summary>
    public string GenerateEmail()
    {
        return _faker.Internet.Email();
    }

    /// <summary>
    /// Generate a plant care task
    /// </summary>
    public object GenerateCareTask(int myPlantId)
    {
        return new
        {
            myPlantId = myPlantId,
            taskType = _faker.PickRandom("Riego", "Fertilización", "Poda", "Trasplante"),
            scheduledDate = _faker.Date.Future(30),
            status = "Pendiente",
            notes = _faker.Lorem.Sentence()
        };
    }

    /// <summary>
    /// Generate a plant health log
    /// </summary>
    public object GenerateHealthLog(int myPlantId)
    {
        return new
        {
            myPlantId = myPlantId,
            date = _faker.Date.Recent(30),
            healthStatus = _faker.PickRandom("Excelente", "Buena", "Regular", "Mala"),
            observations = _faker.Lorem.Sentence()
        };
    }

    /// <summary>
    /// Generate my plant data
    /// </summary>
    public object GenerateMyPlant(int plantId, int userId)
    {
        return new
        {
            plantId = plantId,
            userId = userId,
            customName = $"{_faker.Name.FirstName()}'s Plant",
            location = _faker.Address.City(),
            notes = _faker.Lorem.Sentence()
        };
    }
}
