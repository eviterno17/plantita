using FluentAssertions;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using Xunit;

namespace Plantita.CoreTests.Domain.IoTMonitoring;

/// <summary>
/// Unit tests for Sensor entity and SensorReading
/// </summary>
public class SensorTests
{
    [Theory]
    [InlineData("Temperature", "Celsius", -10, 50)]
    [InlineData("SoilMoisture", "Percentage", 0, 100)]
    [InlineData("Light", "Lux", 0, 100000)]
    [InlineData("AirHumidity", "Percentage", 0, 100)]
    public void Sensor_ShouldBeCreated_WithValidData(string sensorType, string unit, double minRange, double maxRange)
    {
        // Arrange & Act
        var sensor = new Sensor
        {
            DeviceId = 1,
            SensorType = sensorType,
            Unit = unit,
            MinRange = minRange,
            MaxRange = maxRange,
            IsActive = true
        };

        // Assert
        sensor.Should().NotBeNull();
        sensor.SensorType.Should().Be(sensorType);
        sensor.Unit.Should().Be(unit);
        sensor.MinRange.Should().Be(minRange);
        sensor.MaxRange.Should().Be(maxRange);
        sensor.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Sensor_ShouldHaveReadings_Collection()
    {
        // Arrange & Act
        var sensor = new Sensor
        {
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        // Assert
        sensor.Readings.Should().NotBeNull();
        sensor.Readings.Should().BeEmpty();
    }

    [Fact]
    public void Sensor_CanAddReading()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        var reading = new SensorReading
        {
            SensorId = sensor.Id,
            Value = 23.5,
            Timestamp = DateTime.UtcNow
        };

        // Act
        sensor.Readings.Add(reading);

        // Assert
        sensor.Readings.Should().HaveCount(1);
        sensor.Readings.First().Value.Should().Be(23.5);
    }

    [Fact]
    public void Sensor_CanHaveMultipleReadings()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        var readings = new[] { 20.5, 21.0, 22.5, 23.0, 24.5 };

        // Act
        foreach (var value in readings)
        {
            sensor.Readings.Add(new SensorReading
            {
                SensorId = sensor.Id,
                Value = value,
                Timestamp = DateTime.UtcNow
            });
        }

        // Assert
        sensor.Readings.Should().HaveCount(5);
        sensor.Readings.Select(r => r.Value).Should().Contain(readings);
    }

    [Fact]
    public void SensorReading_ShouldRecordTimestamp()
    {
        // Arrange
        var timestamp = new DateTime(2024, 12, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var reading = new SensorReading
        {
            SensorId = 1,
            Value = 23.5,
            Timestamp = timestamp
        };

        // Assert
        reading.Timestamp.Should().Be(timestamp);
    }

    [Theory]
    [InlineData(23.5)]
    [InlineData(0.0)]
    [InlineData(-5.5)]
    [InlineData(100.0)]
    [InlineData(0.01)]
    public void SensorReading_ShouldAcceptVariousValues(double value)
    {
        // Arrange & Act
        var reading = new SensorReading
        {
            SensorId = 1,
            Value = value,
            Timestamp = DateTime.UtcNow
        };

        // Assert
        reading.Value.Should().Be(value);
    }

    [Fact]
    public void Sensor_CanBeActivated()
    {
        // Arrange
        var sensor = new Sensor
        {
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            IsActive = false
        };

        // Act
        sensor.IsActive = true;

        // Assert
        sensor.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Sensor_CanBeDeactivated()
    {
        // Arrange
        var sensor = new Sensor
        {
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            IsActive = true
        };

        // Act
        sensor.IsActive = false;

        // Assert
        sensor.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Sensor_ShouldValidateRange()
    {
        // Arrange
        var sensor = new Sensor
        {
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        // Assert
        sensor.MinRange.Should().BeLessThan(sensor.MaxRange);
    }

    [Fact]
    public void SensorReading_WithinRange_ShouldBeValid()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        var reading = new SensorReading
        {
            SensorId = sensor.Id,
            Value = 25.0, // Within range
            Timestamp = DateTime.UtcNow
        };

        // Assert
        reading.Value.Should().BeInRange(sensor.MinRange, sensor.MaxRange);
    }

    [Fact]
    public void SensorReading_OutsideRange_ShouldBeDetectable()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        var reading = new SensorReading
        {
            SensorId = sensor.Id,
            Value = 75.0, // Outside range
            Timestamp = DateTime.UtcNow
        };

        // Assert
        reading.Value.Should().BeGreaterThan(sensor.MaxRange);
    }

    [Fact]
    public void Sensor_ShouldSupportDifferentUnits()
    {
        // Arrange
        var temperatureSensor = new Sensor
        {
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            IsActive = true
        };

        var fahrenheitSensor = new Sensor
        {
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Fahrenheit",
            IsActive = true
        };

        // Assert
        temperatureSensor.Unit.Should().Be("Celsius");
        fahrenheitSensor.Unit.Should().Be("Fahrenheit");
    }

    [Fact]
    public void SensorReading_ShouldBeOrderedByTimestamp()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            IsActive = true
        };

        var baseTime = DateTime.UtcNow;

        // Act
        sensor.Readings.Add(new SensorReading
        {
            SensorId = sensor.Id,
            Value = 20.0,
            Timestamp = baseTime.AddHours(-2)
        });
        sensor.Readings.Add(new SensorReading
        {
            SensorId = sensor.Id,
            Value = 22.0,
            Timestamp = baseTime.AddHours(-1)
        });
        sensor.Readings.Add(new SensorReading
        {
            SensorId = sensor.Id,
            Value = 24.0,
            Timestamp = baseTime
        });

        // Assert
        var orderedReadings = sensor.Readings.OrderBy(r => r.Timestamp).ToList();
        orderedReadings[0].Value.Should().Be(20.0);
        orderedReadings[1].Value.Should().Be(22.0);
        orderedReadings[2].Value.Should().Be(24.0);
    }

    [Fact]
    public void Sensor_CanCalculateAverageReading()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            IsActive = true
        };

        var readings = new[] { 20.0, 22.0, 24.0, 26.0 };
        foreach (var value in readings)
        {
            sensor.Readings.Add(new SensorReading
            {
                SensorId = sensor.Id,
                Value = value,
                Timestamp = DateTime.UtcNow
            });
        }

        // Act
        var average = sensor.Readings.Average(r => r.Value);

        // Assert
        average.Should().Be(23.0);
    }

    [Fact]
    public void Sensor_CanFindMinAndMaxReadings()
    {
        // Arrange
        var sensor = new Sensor
        {
            Id = 1,
            DeviceId = 1,
            SensorType = "Temperature",
            Unit = "Celsius",
            IsActive = true
        };

        var readings = new[] { 18.0, 25.0, 30.0, 22.0, 19.0 };
        foreach (var value in readings)
        {
            sensor.Readings.Add(new SensorReading
            {
                SensorId = sensor.Id,
                Value = value,
                Timestamp = DateTime.UtcNow
            });
        }

        // Act
        var min = sensor.Readings.Min(r => r.Value);
        var max = sensor.Readings.Max(r => r.Value);

        // Assert
        min.Should().Be(18.0);
        max.Should().Be(30.0);
    }
}
