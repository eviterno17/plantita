using FluentAssertions;
using plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using Xunit;

namespace Plantita.CoreTests.Domain.IoTMonitoring;

/// <summary>
/// Unit tests for IoTDevice aggregate root
/// </summary>
public class IoTDeviceTests
{
    [Fact]
    public void IoTDevice_ShouldBeCreated_WithValidData()
    {
        // Arrange & Act
        var device = new IoTDevice
        {
            Name = "Monitor Sala Principal",
            ConnectionType = "WiFi",
            Location = "Sala de estar",
            FirmwareVersion = "v1.2.3",
            IsActive = true,
            UserId = 100
        };

        // Assert
        device.Should().NotBeNull();
        device.Name.Should().Be("Monitor Sala Principal");
        device.ConnectionType.Should().Be("WiFi");
        device.Location.Should().Be("Sala de estar");
        device.FirmwareVersion.Should().Be("v1.2.3");
        device.IsActive.Should().BeTrue();
        device.UserId.Should().Be(100);
    }

    [Theory]
    [InlineData("WiFi")]
    [InlineData("Bluetooth")]
    [InlineData("Zigbee")]
    [InlineData("LoRa")]
    public void IoTDevice_ShouldSupportDifferentConnectionTypes(string connectionType)
    {
        // Arrange & Act
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = connectionType,
            Location = "Test Location",
            UserId = 100
        };

        // Assert
        device.ConnectionType.Should().Be(connectionType);
    }

    [Fact]
    public void IoTDevice_ShouldHaveSensors_Collection()
    {
        // Arrange & Act
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            UserId = 100
        };

        // Assert
        device.Sensors.Should().NotBeNull();
        device.Sensors.Should().BeEmpty();
    }

    [Fact]
    public void IoTDevice_CanAddSensor()
    {
        // Arrange
        var device = new IoTDevice
        {
            Id = 1,
            Name = "Test Device",
            ConnectionType = "WiFi",
            UserId = 100
        };

        var sensor = new Sensor
        {
            DeviceId = device.Id,
            SensorType = "Temperature",
            Unit = "Celsius",
            MinRange = -10,
            MaxRange = 50,
            IsActive = true
        };

        // Act
        device.Sensors.Add(sensor);

        // Assert
        device.Sensors.Should().HaveCount(1);
        device.Sensors.First().SensorType.Should().Be("Temperature");
    }

    [Fact]
    public void IoTDevice_CanHaveMultipleSensors()
    {
        // Arrange
        var device = new IoTDevice
        {
            Id = 1,
            Name = "Multi-Sensor Device",
            ConnectionType = "WiFi",
            UserId = 100
        };

        var sensorTypes = new[] { "Temperature", "SoilMoisture", "Light", "AirHumidity" };

        // Act
        foreach (var sensorType in sensorTypes)
        {
            device.Sensors.Add(new Sensor
            {
                DeviceId = device.Id,
                SensorType = sensorType,
                Unit = "Unit",
                MinRange = 0,
                MaxRange = 100,
                IsActive = true
            });
        }

        // Assert
        device.Sensors.Should().HaveCount(4);
        device.Sensors.Select(s => s.SensorType).Should().Contain(sensorTypes);
    }

    [Fact]
    public void IoTDevice_CanBeActivated()
    {
        // Arrange
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            IsActive = false,
            UserId = 100
        };

        // Act
        device.IsActive = true;

        // Assert
        device.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IoTDevice_CanBeDeactivated()
    {
        // Arrange
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            IsActive = true,
            UserId = 100
        };

        // Act
        device.IsActive = false;

        // Assert
        device.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IoTDevice_ShouldBelongToSpecificUser()
    {
        // Arrange & Act
        var user1Device = new IoTDevice
        {
            Name = "User 1 Device",
            ConnectionType = "WiFi",
            UserId = 100
        };

        var user2Device = new IoTDevice
        {
            Name = "User 2 Device",
            ConnectionType = "WiFi",
            UserId = 200
        };

        // Assert
        user1Device.UserId.Should().Be(100);
        user2Device.UserId.Should().Be(200);
        user1Device.UserId.Should().NotBe(user2Device.UserId);
    }

    [Theory]
    [InlineData("v1.0.0")]
    [InlineData("v2.1.5")]
    [InlineData("v3.0.0-beta")]
    public void IoTDevice_ShouldTrackFirmwareVersion(string firmwareVersion)
    {
        // Arrange & Act
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            FirmwareVersion = firmwareVersion,
            UserId = 100
        };

        // Assert
        device.FirmwareVersion.Should().Be(firmwareVersion);
    }

    [Fact]
    public void IoTDevice_CanUpdateFirmwareVersion()
    {
        // Arrange
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            FirmwareVersion = "v1.0.0",
            UserId = 100
        };

        // Act
        device.FirmwareVersion = "v2.0.0";

        // Assert
        device.FirmwareVersion.Should().Be("v2.0.0");
    }

    [Fact]
    public void IoTDevice_ShouldStoreLocation()
    {
        // Arrange & Act
        var device = new IoTDevice
        {
            Name = "Test Device",
            ConnectionType = "WiFi",
            Location = "Living Room - North Window",
            UserId = 100
        };

        // Assert
        device.Location.Should().Be("Living Room - North Window");
    }
}
