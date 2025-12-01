using Bogus;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.User.Domain.Model.Aggregates;

namespace Plantita.CoreTests.Fixtures;

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
    /// Generate a random AuthUser
    /// </summary>
    public AuthUser GenerateAuthUser()
    {
        return new AuthUser
        {
            Email = _faker.Internet.Email(),
            Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Name = _faker.Name.FullName(),
            Timezone = _faker.PickRandom("America/Lima", "America/New_York", "Europe/Madrid", "UTC"),
            Language = _faker.PickRandom("es", "en", "pt")
        };
    }

    /// <summary>
    /// Generate a random Plant
    /// </summary>
    public Plant GeneratePlant()
    {
        var plantData = _faker.PickRandom(new[]
        {
            ("Monstera deliciosa", "Costilla de Adán", "Planta tropical de hojas grandes con perforaciones naturales"),
            ("Ficus elastica", "Árbol del caucho", "Planta de interior resistente con hojas grandes y brillantes"),
            ("Pothos aureus", "Potus dorado", "Planta trepadora muy resistente y fácil de cuidar"),
            ("Sansevieria trifasciata", "Lengua de suegra", "Planta suculenta extremadamente resistente"),
            ("Spathiphyllum", "Lirio de la paz", "Planta de interior que purifica el aire")
        });

        return new Plant
        {
            ScientificName = plantData.Item1,
            CommonName = plantData.Item2,
            Description = plantData.Item3,
            WateringFrequency = $"Cada {_faker.Random.Int(5, 14)} días",
            SunlightRequirement = _faker.PickRandom("Luz directa", "Luz indirecta brillante", "Luz indirecta moderada", "Sombra parcial"),
            OptimalTemperature = $"{_faker.Random.Int(15, 20)}-{_faker.Random.Int(24, 30)}°C",
            Humidity = _faker.PickRandom("Baja (20-40%)", "Media (40-60%)", "Alta (60-80%)")
        };
    }

    /// <summary>
    /// Generate a random MyPlant
    /// </summary>
    public MyPlant GenerateMyPlant(int plantId, int userId)
    {
        return new MyPlant
        {
            PlantId = plantId,
            UserId = userId,
            CustomName = $"{_faker.Name.FirstName()}'s {_faker.PickRandom("Plant", "Green Friend", "Baby")}",
            Location = _faker.PickRandom("Sala", "Dormitorio", "Cocina", "Baño", "Oficina", "Terraza"),
            Notes = _faker.Lorem.Sentence()
        };
    }

    /// <summary>
    /// Generate a random IoT Device
    /// </summary>
    public IoTDevice GenerateIoTDevice(int userId)
    {
        return new IoTDevice
        {
            Name = $"Monitor {_faker.Address.City()}",
            ConnectionType = _faker.PickRandom("WiFi", "Bluetooth", "Zigbee", "LoRa"),
            Location = _faker.Address.City(),
            FirmwareVersion = $"v{_faker.System.Version()}",
            IsActive = _faker.Random.Bool(0.8f), // 80% active
            UserId = userId
        };
    }

    /// <summary>
    /// Generate a random Sensor
    /// </summary>
    public Sensor GenerateSensor(int deviceId)
    {
        var sensorType = _faker.PickRandom("Temperature", "SoilMoisture", "Light", "AirHumidity");
        var (unit, minRange, maxRange) = sensorType switch
        {
            "Temperature" => ("Celsius", -10.0, 50.0),
            "SoilMoisture" => ("Percentage", 0.0, 100.0),
            "Light" => ("Lux", 0.0, 100000.0),
            "AirHumidity" => ("Percentage", 0.0, 100.0),
            _ => ("Unit", 0.0, 100.0)
        };

        return new Sensor
        {
            DeviceId = deviceId,
            SensorType = sensorType,
            Unit = unit,
            MinRange = minRange,
            MaxRange = maxRange,
            IsActive = _faker.Random.Bool(0.9f) // 90% active
        };
    }

    /// <summary>
    /// Generate a sensor reading
    /// </summary>
    public SensorReading GenerateSensorReading(int sensorId, string sensorType)
    {
        var value = sensorType switch
        {
            "Temperature" => _faker.Random.Double(15, 35),
            "SoilMoisture" => _faker.Random.Double(20, 80),
            "Light" => _faker.Random.Double(1000, 50000),
            "AirHumidity" => _faker.Random.Double(30, 70),
            _ => _faker.Random.Double(0, 100)
        };

        return new SensorReading
        {
            SensorId = sensorId,
            Value = value,
            Timestamp = _faker.Date.Recent(7)
        };
    }

    /// <summary>
    /// Generate a PlantHealthLog
    /// </summary>
    public plantita.ProjectPlantita.plantmanagment.domain.model.Entities.PlantHealthLog GenerateHealthLog(int myPlantId)
    {
        return new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.PlantHealthLog
        {
            MyPlantId = myPlantId,
            Date = _faker.Date.Recent(30),
            HealthStatus = _faker.PickRandom("Excelente", "Buena", "Regular", "Mala", "Crítica"),
            Observations = _faker.Lorem.Sentence()
        };
    }

    /// <summary>
    /// Generate a CareTask
    /// </summary>
    public plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask GenerateCareTask(int myPlantId)
    {
        return new plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask
        {
            MyPlantId = myPlantId,
            TaskType = _faker.PickRandom("Riego", "Fertilización", "Poda", "Trasplante", "Inspección"),
            ScheduledDate = _faker.Date.Future(30),
            Status = _faker.PickRandom("Pendiente", "En Progreso", "Completada", "Cancelada"),
            Notes = _faker.Lorem.Sentence()
        };
    }

    /// <summary>
    /// Generate multiple AuthUsers
    /// </summary>
    public List<AuthUser> GenerateAuthUsers(int count)
    {
        var users = new List<AuthUser>();
        for (int i = 0; i < count; i++)
        {
            users.Add(GenerateAuthUser());
        }
        return users;
    }

    /// <summary>
    /// Generate multiple Plants
    /// </summary>
    public List<Plant> GeneratePlants(int count)
    {
        var plants = new List<Plant>();
        for (int i = 0; i < count; i++)
        {
            plants.Add(GeneratePlant());
        }
        return plants;
    }

    /// <summary>
    /// Generate multiple SensorReadings
    /// </summary>
    public List<SensorReading> GenerateSensorReadings(int sensorId, string sensorType, int count)
    {
        var readings = new List<SensorReading>();
        for (int i = 0; i < count; i++)
        {
            readings.Add(GenerateSensorReading(sensorId, sensorType));
        }
        return readings;
    }
}
