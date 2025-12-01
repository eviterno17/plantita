using FluentAssertions;
using Moq;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.CommandServices;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
using plantita.Shared.Domain.Repositories;
using Xunit;

namespace plantita.Tests.Unit.ProjectPlantita.PlantManagement;

public class MyPlantCommandServiceTests
{
    private readonly Mock<IMyPlantRepository> _mockMyPlantRepository;
    private readonly Mock<ICareTaskRepository> _mockCareTaskRepository;
    private readonly Mock<IPlantHealthLogRepository> _mockHealthLogRepository;
    private readonly Mock<IPlantRepository> _mockPlantRepository;
    private readonly Mock<IPlantQueryService> _mockPlantQueryService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly MyPlantCommandService _service;

    public MyPlantCommandServiceTests()
    {
        _mockMyPlantRepository = new Mock<IMyPlantRepository>();
        _mockCareTaskRepository = new Mock<ICareTaskRepository>();
        _mockHealthLogRepository = new Mock<IPlantHealthLogRepository>();
        _mockPlantRepository = new Mock<IPlantRepository>();
        _mockPlantQueryService = new Mock<IPlantQueryService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _service = new MyPlantCommandService(
            _mockMyPlantRepository.Object,
            _mockCareTaskRepository.Object,
            _mockHealthLogRepository.Object,
            _mockPlantRepository.Object,
            _mockPlantQueryService.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateMyPlantAsync_WithValidData_CreatesMyPlant()
    {
        // Arrange
        var myPlant = new MyPlant
        {
            MyPlantId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PlantId = Guid.NewGuid(),
            CustomName = "My Rose",
            Location = "Living Room",
            Note = "Birthday gift",
            CurrentStatus = "Healthy"
        };

        _mockMyPlantRepository
            .Setup(r => r.AddAsync(myPlant))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateMyPlantAsync(myPlant);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(myPlant);
        _mockMyPlantRepository.Verify(r => r.AddAsync(myPlant), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterMyPlantAsync_WithValidData_CreatesMyPlantWithPlantImage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plantId = Guid.NewGuid();
        var resource = new SaveMyPlantResource
        {
            CustomName = "My Cactus",
            Location = "Office",
            Note = "Easy to care for"
        };

        var plant = new Plant
        {
            PlantId = plantId,
            ScientificName = "Cactaceae",
            CommonName = "Cactus",
            ImageUrl = "https://example.com/cactus.jpg",
            Watering = "Low",
            Sunlight = "Full sun"
        };

        _mockPlantQueryService
            .Setup(s => s.GetByIdAsync(plantId))
            .ReturnsAsync(plant);

        _mockMyPlantRepository
            .Setup(r => r.AddAsync(It.IsAny<MyPlant>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.RegisterMyPlantAsync(userId, plantId, resource);

        // Assert
        result.Should().NotBeNull();
        result.CustomName.Should().Be("My Cactus");
        result.Location.Should().Be("Office");
        result.Note.Should().Be("Easy to care for");
        result.UserId.Should().Be(userId);
        result.PlantId.Should().Be(plantId);
        result.PhotoUrl.Should().Be("https://example.com/cactus.jpg");
        result.CurrentStatus.Should().Be("Healthy");

        _mockMyPlantRepository.Verify(r => r.AddAsync(It.Is<MyPlant>(
            mp => mp.CustomName == "My Cactus" && mp.PhotoUrl == plant.ImageUrl)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterMyPlantAsync_WithNonExistentPlant_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var plantId = Guid.NewGuid();
        var resource = new SaveMyPlantResource
        {
            CustomName = "Test",
            Location = "Test"
        };

        _mockPlantQueryService
            .Setup(s => s.GetByIdAsync(plantId))
            .ReturnsAsync((Plant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _service.RegisterMyPlantAsync(userId, plantId, resource));

        _mockMyPlantRepository.Verify(r => r.AddAsync(It.IsAny<MyPlant>()), Times.Never);
    }

    [Fact]
    public async Task UpdateMyPlantAsync_WithValidId_UpdatesPlant()
    {
        // Arrange
        var myPlantId = Guid.NewGuid();
        var existingPlant = new MyPlant
        {
            MyPlantId = myPlantId,
            CustomName = "Old Name",
            Location = "Old Location",
            Note = "Old Note",
            PhotoUrl = "old.jpg",
            CurrentStatus = "Healthy"
        };

        var updatedPlant = new MyPlant
        {
            CustomName = "New Name",
            Location = "New Location",
            Note = "New Note",
            PhotoUrl = "new.jpg",
            CurrentStatus = "Sick"
        };

        _mockMyPlantRepository
            .Setup(r => r.FindByIdGuidAsync(myPlantId))
            .ReturnsAsync(existingPlant);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateMyPlantAsync(myPlantId, updatedPlant);

        // Assert
        result.Should().NotBeNull();
        result.CustomName.Should().Be("New Name");
        result.Location.Should().Be("New Location");
        result.Note.Should().Be("New Note");
        result.PhotoUrl.Should().Be("new.jpg");
        result.CurrentStatus.Should().Be("Sick");

        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateMyPlantAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var myPlantId = Guid.NewGuid();
        var updatedPlant = new MyPlant();

        _mockMyPlantRepository
            .Setup(r => r.FindByIdGuidAsync(myPlantId))
            .ReturnsAsync((MyPlant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateMyPlantAsync(myPlantId, updatedPlant));
    }

    [Fact]
    public async Task DeleteMyPlantAsync_WithValidId_DeletesPlant()
    {
        // Arrange
        var myPlantId = Guid.NewGuid();
        var existingPlant = new MyPlant { MyPlantId = myPlantId };

        _mockMyPlantRepository
            .Setup(r => r.FindByIdGuidAsync(myPlantId))
            .ReturnsAsync(existingPlant);

        _mockMyPlantRepository
            .Setup(r => r.Delete(existingPlant))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteMyPlantAsync(myPlantId);

        // Assert
        result.Should().BeTrue();
        _mockMyPlantRepository.Verify(r => r.Delete(existingPlant), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteMyPlantAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var myPlantId = Guid.NewGuid();

        _mockMyPlantRepository
            .Setup(r => r.FindByIdGuidAsync(myPlantId))
            .ReturnsAsync((MyPlant?)null);

        // Act
        var result = await _service.DeleteMyPlantAsync(myPlantId);

        // Assert
        result.Should().BeFalse();
        _mockMyPlantRepository.Verify(r => r.Delete(It.IsAny<MyPlant>()), Times.Never);
    }

    [Fact]
    public async Task MarkCareTaskCompletedAsync_WithValidTask_MarksAsCompleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new CareTask
        {
            TaskId = taskId,
            Status = "Pending",
            CompletedAt = null
        };

        _mockCareTaskRepository
            .Setup(r => r.FindByIdGuidAsync(taskId))
            .ReturnsAsync(task);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.MarkCareTaskCompletedAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Completed");
        result.CompletedAt.Should().NotBeNull();
        result.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task MarkCareTaskCompletedAsync_WithInvalidTask_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockCareTaskRepository
            .Setup(r => r.FindByIdGuidAsync(taskId))
            .ReturnsAsync((CareTask?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.MarkCareTaskCompletedAsync(taskId));
    }

    [Fact]
    public async Task LogPlantHealthAsync_WithValidData_CreatesHealthLog()
    {
        // Arrange
        var myPlantId = Guid.NewGuid();
        var healthLog = new PlantHealthLog
        {
            HealthStatus = "Good",
            Notes = "Plant is thriving"
        };

        _mockHealthLogRepository
            .Setup(r => r.AddAsync(It.IsAny<PlantHealthLog>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.LogPlantHealthAsync(myPlantId, healthLog);

        // Assert
        result.Should().NotBeNull();
        result.MyPlantId.Should().Be(myPlantId);
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockHealthLogRepository.Verify(r => r.AddAsync(It.Is<PlantHealthLog>(
            log => log.MyPlantId == myPlantId)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateCareTasksAsync_CreatesWateringAndFertilizingTasks()
    {
        // Arrange
        var myPlantId = Guid.NewGuid();

        _mockCareTaskRepository
            .Setup(r => r.AddAsync(It.IsAny<CareTask>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GenerateCareTasksAsync(myPlantId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var wateringTask = result.FirstOrDefault(t => t.TaskType == "Riego");
        wateringTask.Should().NotBeNull();
        wateringTask!.MyPlantId.Should().Be(myPlantId);
        wateringTask.Status.Should().Be("Pending");
        wateringTask.ScheduledFor.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromMinutes(1));

        var fertilizingTask = result.FirstOrDefault(t => t.TaskType == "Fertilizar");
        fertilizingTask.Should().NotBeNull();
        fertilizingTask!.MyPlantId.Should().Be(myPlantId);
        fertilizingTask.Status.Should().Be("Pending");
        fertilizingTask.ScheduledFor.Should().BeCloseTo(DateTime.UtcNow.AddDays(10), TimeSpan.FromMinutes(1));

        _mockCareTaskRepository.Verify(r => r.AddAsync(It.IsAny<CareTask>()), Times.Exactly(2));
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
