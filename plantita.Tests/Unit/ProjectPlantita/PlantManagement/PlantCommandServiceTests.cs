using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using plantita.ProjectPlantita.plantmanagment.Application.Internal.CommandServices;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;
using plantita.Shared.Domain.Repositories;
using Xunit;

namespace plantita.Tests.Unit.ProjectPlantita.PlantManagement;

public class PlantCommandServiceTests
{
    private readonly Mock<IPlantRepository> _mockPlantRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPlantIdentificationService> _mockIdentificationService;
    private readonly PlantCommandService _service;

    public PlantCommandServiceTests()
    {
        _mockPlantRepository = new Mock<IPlantRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockIdentificationService = new Mock<IPlantIdentificationService>();

        _service = new PlantCommandService(
            _mockPlantRepository.Object,
            _mockUnitOfWork.Object,
            _mockIdentificationService.Object);
    }

    [Fact]
    public async Task IdentifyAndRegisterPlantAsync_WithNewPlant_CreatesAndReturnsPlant()
    {
        // Arrange
        var mockImage = Mock.Of<IFormFile>();
        var identifiedPlant = new IdentifiedPlantResource
        {
            ScientificName = "Rosa chinensis",
            CommonName = "China Rose",
            Description = "A beautiful flowering plant",
            WikiUrl = "https://en.wikipedia.org/wiki/Rosa_chinensis",
            ImageUrl = "https://example.com/rose.jpg",
            Watering = "Average",
            Sunlight = "Full sun"
        };

        _mockIdentificationService
            .Setup(s => s.IdentifyPlantAsync(mockImage))
            .ReturnsAsync(identifiedPlant);

        _mockPlantRepository
            .Setup(r => r.GetByScientificNameAsync(identifiedPlant.ScientificName))
            .ReturnsAsync((Plant?)null);

        _mockPlantRepository
            .Setup(r => r.AddAsync(It.IsAny<Plant>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.IdentifyAndRegisterPlantAsync(mockImage);

        // Assert
        result.Should().NotBeNull();
        result!.ScientificName.Should().Be("Rosa chinensis");
        result.CommonName.Should().Be("China Rose");
        result.Description.Should().Be("A beautiful flowering plant");

        _mockPlantRepository.Verify(r => r.AddAsync(It.IsAny<Plant>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task IdentifyAndRegisterPlantAsync_WithExistingPlant_ReturnsExistingPlant()
    {
        // Arrange
        var mockImage = Mock.Of<IFormFile>();
        var identifiedPlant = new IdentifiedPlantResource
        {
            ScientificName = "Rosa chinensis",
            CommonName = "China Rose"
        };

        var existingPlant = new Plant
        {
            PlantId = Guid.NewGuid(),
            ScientificName = "Rosa chinensis",
            CommonName = "China Rose",
            Description = "Existing plant",
            Watering = "Average",
            Sunlight = "Full sun",
            ImageUrl = "https://example.com/existing.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        _mockIdentificationService
            .Setup(s => s.IdentifyPlantAsync(mockImage))
            .ReturnsAsync(identifiedPlant);

        _mockPlantRepository
            .Setup(r => r.GetByScientificNameAsync(identifiedPlant.ScientificName))
            .ReturnsAsync(existingPlant);

        // Act
        var result = await _service.IdentifyAndRegisterPlantAsync(mockImage);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(existingPlant);
        result!.PlantId.Should().Be(existingPlant.PlantId);

        _mockPlantRepository.Verify(r => r.AddAsync(It.IsAny<Plant>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task IdentifyAndRegisterPlantAsync_WithIdentificationFailure_ReturnsNull()
    {
        // Arrange
        var mockImage = Mock.Of<IFormFile>();

        _mockIdentificationService
            .Setup(s => s.IdentifyPlantAsync(mockImage))
            .ReturnsAsync((IdentifiedPlantResource?)null);

        // Act
        var result = await _service.IdentifyAndRegisterPlantAsync(mockImage);

        // Assert
        result.Should().BeNull();
        _mockPlantRepository.Verify(r => r.AddAsync(It.IsAny<Plant>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterPlantAsync_WithNewPlant_AddsPlant()
    {
        // Arrange
        var newPlant = new Plant
        {
            PlantId = Guid.NewGuid(),
            ScientificName = "Ficus elastica",
            CommonName = "Rubber Plant",
            Description = "Indoor plant",
            Watering = "Moderate",
            Sunlight = "Indirect light",
            ImageUrl = "https://example.com/ficus.jpg"
        };

        _mockPlantRepository
            .Setup(r => r.GetByScientificNameAsync(newPlant.ScientificName))
            .ReturnsAsync((Plant?)null);

        _mockPlantRepository
            .Setup(r => r.AddAsync(newPlant))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.RegisterPlantAsync(newPlant);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(newPlant);
        _mockPlantRepository.Verify(r => r.AddAsync(newPlant), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterPlantAsync_WithExistingScientificName_ReturnsExisting()
    {
        // Arrange
        var existingPlant = new Plant
        {
            PlantId = Guid.NewGuid(),
            ScientificName = "Ficus elastica",
            CommonName = "Rubber Plant",
            Description = "Existing",
            Watering = "Moderate",
            Sunlight = "Indirect",
            ImageUrl = "existing.jpg"
        };

        var newPlant = new Plant
        {
            PlantId = Guid.NewGuid(),
            ScientificName = "Ficus elastica", // Same scientific name
            CommonName = "Different name",
            Description = "New",
            Watering = "Different",
            Sunlight = "Different",
            ImageUrl = "new.jpg"
        };

        _mockPlantRepository
            .Setup(r => r.GetByScientificNameAsync(newPlant.ScientificName))
            .ReturnsAsync(existingPlant);

        // Act
        var result = await _service.RegisterPlantAsync(newPlant);

        // Assert
        result.Should().Be(existingPlant);
        _mockPlantRepository.Verify(r => r.AddAsync(It.IsAny<Plant>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePlantAsync_WithValidId_UpdatesPlant()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var existingPlant = new Plant
        {
            PlantId = plantId,
            ScientificName = "Old Name",
            CommonName = "Old Common",
            Description = "Old Description",
            Watering = "Old",
            Sunlight = "Old",
            ImageUrl = "old.jpg"
        };

        var updatedPlant = new Plant
        {
            PlantId = plantId,
            ScientificName = "New Name",
            CommonName = "New Common",
            Description = "New Description",
            Watering = "New Watering",
            Sunlight = "New Sunlight",
            ImageUrl = "new.jpg",
            WikiUrl = "https://new.url"
        };

        _mockPlantRepository
            .Setup(r => r.FindByIdGuidAsync(plantId))
            .ReturnsAsync(existingPlant);

        // Act
        var result = await _service.UpdatePlantAsync(plantId, updatedPlant);

        // Assert
        result.Should().NotBeNull();
        result.ScientificName.Should().Be("New Name");
        result.CommonName.Should().Be("New Common");
        result.Description.Should().Be("New Description");
        result.Watering.Should().Be("New Watering");
        result.Sunlight.Should().Be("New Sunlight");
        result.ImageUrl.Should().Be("new.jpg");
        result.WikiUrl.Should().Be("https://new.url");

        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePlantAsync_WithInvalidId_ThrowsException()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var updatedPlant = new Plant();

        _mockPlantRepository
            .Setup(r => r.FindByIdGuidAsync(plantId))
            .ReturnsAsync((Plant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdatePlantAsync(plantId, updatedPlant));
    }

    [Fact]
    public async Task DeletePlantAsync_WithValidId_DeletesPlant()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var existingPlant = new Plant { PlantId = plantId };

        _mockPlantRepository
            .Setup(r => r.FindByIdGuidAsync(plantId))
            .ReturnsAsync(existingPlant);

        _mockPlantRepository
            .Setup(r => r.DeleteAsync(existingPlant))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeletePlantAsync(plantId);

        // Assert
        result.Should().BeTrue();
        _mockPlantRepository.Verify(r => r.DeleteAsync(existingPlant), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePlantAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var plantId = Guid.NewGuid();

        _mockPlantRepository
            .Setup(r => r.FindByIdGuidAsync(plantId))
            .ReturnsAsync((Plant?)null);

        // Act
        var result = await _service.DeletePlantAsync(plantId);

        // Assert
        result.Should().BeFalse();
        _mockPlantRepository.Verify(r => r.DeleteAsync(It.IsAny<Plant>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }
}
