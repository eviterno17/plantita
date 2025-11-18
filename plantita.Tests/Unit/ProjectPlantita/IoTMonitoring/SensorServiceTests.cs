using FluentAssertions;
using Moq;
using plantita.ProjectPlantita.iotmonitoring.Application.Internal.Services;
using plantita.ProjectPlantita.iotmonitoring.domain.model.Entities;
using plantita.ProjectPlantita.iotmonitoring.domain.repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using Xunit;

namespace plantita.Tests.Unit.ProjectPlantita.IoTMonitoring;

public class SensorServiceTests
{
    private readonly Mock<ISensorRepository> _mockRepository;
    private readonly Mock<AppDbContext> _mockContext;
    private readonly SensorService _service;

    public SensorServiceTests()
    {
        _mockRepository = new Mock<ISensorRepository>();
        _mockContext = new Mock<AppDbContext>();

        _service = new SensorService(
            _mockRepository.Object,
            _mockContext.Object);
    }

    [Fact]
    public async Task ListAsync_ReturnsAllSensors()
    {
        // Arrange
        var sensors = new List<Sensor>
        {
            new Sensor
            {
                SensorId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                SensorType = "Temperature",
                Unit = "Celsius",
                RangeMin = -10,
                RangeMax = 50,
                Model = "DHT22",
                IsActive = true
            },
            new Sensor
            {
                SensorId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                SensorType = "Humidity",
                Unit = "Percent",
                RangeMin = 0,
                RangeMax = 100,
                Model = "DHT22",
                IsActive = true
            }
        };

        _mockRepository
            .Setup(r => r.ListAsync())
            .ReturnsAsync(sensors);

        // Act
        var result = await _service.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(sensors);

        _mockRepository.Verify(r => r.ListAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsSensor()
    {
        // Arrange
        var sensorId = Guid.NewGuid();
        var sensor = new Sensor
        {
            SensorId = sensorId,
            DeviceId = Guid.NewGuid(),
            SensorType = "Soil Moisture",
            Unit = "Percent",
            RangeMin = 0,
            RangeMax = 100,
            Model = "Capacitive",
            IsActive = true
        };

        _mockRepository
            .Setup(r => r.FindByIdAsync(sensorId))
            .ReturnsAsync(sensor);

        // Act
        var result = await _service.GetByIdAsync(sensorId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(sensor);
        result.SensorId.Should().Be(sensorId);

        _mockRepository.Verify(r => r.FindByIdAsync(sensorId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var sensorId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.FindByIdAsync(sensorId))
            .ReturnsAsync((Sensor?)null);

        // Act
        var result = await _service.GetByIdAsync(sensorId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllByDeviceIdAsync_WithValidDeviceId_ReturnsSensors()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var sensors = new List<Sensor>
        {
            new Sensor
            {
                SensorId = Guid.NewGuid(),
                DeviceId = deviceId,
                SensorType = "Temperature",
                Unit = "Celsius"
            },
            new Sensor
            {
                SensorId = Guid.NewGuid(),
                DeviceId = deviceId,
                SensorType = "Humidity",
                Unit = "Percent"
            }
        };

        _mockRepository
            .Setup(r => r.ListByDeviceIdAsync(deviceId))
            .ReturnsAsync(sensors);

        // Act
        var result = await _service.GetAllByDeviceIdAsync(deviceId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(s => s.DeviceId == deviceId).Should().BeTrue();

        _mockRepository.Verify(r => r.ListByDeviceIdAsync(deviceId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidSensor_CreatesSensor()
    {
        // Arrange
        var sensor = new Sensor
        {
            SensorId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            SensorType = "Light",
            Unit = "Lux",
            RangeMin = 0,
            RangeMax = 100000,
            Model = "BH1750",
            InstalledAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockRepository
            .Setup(r => r.AddAsync(sensor))
            .Returns(Task.CompletedTask);

        _mockContext
            .Setup(c => c.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(sensor);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(sensor);

        _mockRepository.Verify(r => r.AddAsync(sensor), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesSensor()
    {
        // Arrange
        var sensorId = Guid.NewGuid();
        var existingSensor = new Sensor
        {
            SensorId = sensorId,
            DeviceId = Guid.NewGuid(),
            SensorType = "Old Type",
            Unit = "Old Unit",
            RangeMin = 0,
            RangeMax = 50,
            Model = "Old Model",
            InstalledAt = DateTime.UtcNow.AddDays(-30),
            IsActive = true
        };

        var updatedSensor = new Sensor
        {
            SensorId = sensorId,
            DeviceId = Guid.NewGuid(),
            SensorType = "New Type",
            Unit = "New Unit",
            RangeMin = 10,
            RangeMax = 100,
            Model = "New Model",
            InstalledAt = DateTime.UtcNow,
            IsActive = false
        };

        _mockRepository
            .Setup(r => r.FindByIdAsync(sensorId))
            .ReturnsAsync(existingSensor);

        _mockRepository
            .Setup(r => r.Update(It.IsAny<Sensor>()))
            .Verifiable();

        _mockContext
            .Setup(c => c.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(sensorId, updatedSensor);

        // Assert
        result.Should().NotBeNull();
        result.SensorType.Should().Be("New Type");
        result.Unit.Should().Be("New Unit");
        result.RangeMin.Should().Be(10);
        result.RangeMax.Should().Be(100);
        result.Model.Should().Be("New Model");
        result.IsActive.Should().BeFalse();

        _mockRepository.Verify(r => r.FindByIdAsync(sensorId), Times.Once);
        _mockRepository.Verify(r => r.Update(existingSensor), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var sensorId = Guid.NewGuid();
        var updatedSensor = new Sensor();

        _mockRepository
            .Setup(r => r.FindByIdAsync(sensorId))
            .ReturnsAsync((Sensor?)null);

        // Act
        var result = await _service.UpdateAsync(sensorId, updatedSensor);

        // Assert
        result.Should().BeNull();

        _mockRepository.Verify(r => r.Update(It.IsAny<Sensor>()), Times.Never);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesSensor()
    {
        // Arrange
        var sensorId = Guid.NewGuid();
        var existingSensor = new Sensor
        {
            SensorId = sensorId,
            DeviceId = Guid.NewGuid(),
            SensorType = "Temperature",
            Unit = "Celsius"
        };

        _mockRepository
            .Setup(r => r.FindByIdAsync(sensorId))
            .ReturnsAsync(existingSensor);

        _mockRepository
            .Setup(r => r.Remove(existingSensor))
            .Verifiable();

        _mockContext
            .Setup(c => c.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(sensorId);

        // Assert
        _mockRepository.Verify(r => r.FindByIdAsync(sensorId), Times.Once);
        _mockRepository.Verify(r => r.Remove(existingSensor), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_DoesNothing()
    {
        // Arrange
        var sensorId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.FindByIdAsync(sensorId))
            .ReturnsAsync((Sensor?)null);

        // Act
        await _service.DeleteAsync(sensorId);

        // Assert
        _mockRepository.Verify(r => r.Remove(It.IsAny<Sensor>()), Times.Never);
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_VerifiesAllPropertiesAreSet()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var installedAt = DateTime.UtcNow;
        var sensor = new Sensor
        {
            SensorId = Guid.NewGuid(),
            DeviceId = deviceId,
            SensorType = "pH Sensor",
            Unit = "pH",
            RangeMin = 0m,
            RangeMax = 14m,
            Model = "Atlas Scientific",
            InstalledAt = installedAt,
            IsActive = true
        };

        _mockRepository
            .Setup(r => r.AddAsync(sensor))
            .Returns(Task.CompletedTask);

        _mockContext
            .Setup(c => c.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(sensor);

        // Assert
        result.DeviceId.Should().Be(deviceId);
        result.SensorType.Should().Be("pH Sensor");
        result.Unit.Should().Be("pH");
        result.RangeMin.Should().Be(0m);
        result.RangeMax.Should().Be(14m);
        result.Model.Should().Be("Atlas Scientific");
        result.InstalledAt.Should().Be(installedAt);
        result.IsActive.Should().BeTrue();
    }
}
