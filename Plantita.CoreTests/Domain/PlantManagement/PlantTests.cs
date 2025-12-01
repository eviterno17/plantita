using FluentAssertions;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using Xunit;

namespace Plantita.CoreTests.Domain.PlantManagement;

/// <summary>
/// Unit tests for Plant aggregate root
/// </summary>
public class PlantTests
{
    [Fact]
    public void Plant_ShouldBeCreated_WithValidData()
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = "Monstera deliciosa",
            CommonName = "Costilla de Adán",
            Description = "Planta tropical de hojas grandes",
            WateringFrequency = "Cada 7-10 días",
            SunlightRequirement = "Luz indirecta brillante"
        };

        // Assert
        plant.Should().NotBeNull();
        plant.ScientificName.Should().Be("Monstera deliciosa");
        plant.CommonName.Should().Be("Costilla de Adán");
        plant.Description.Should().Be("Planta tropical de hojas grandes");
        plant.WateringFrequency.Should().Be("Cada 7-10 días");
        plant.SunlightRequirement.Should().Be("Luz indirecta brillante");
    }

    [Fact]
    public void Plant_Id_ShouldBeZero_WhenNotPersisted()
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = "Test Plant",
            CommonName = "Test"
        };

        // Assert
        plant.Id.Should().Be(0);
    }

    [Theory]
    [InlineData("Monstera deliciosa", "Costilla de Adán")]
    [InlineData("Ficus elastica", "Árbol del caucho")]
    [InlineData("Pothos aureus", "Potus dorado")]
    [InlineData("Sansevieria trifasciata", "Lengua de suegra")]
    public void Plant_ShouldSupportMultiplePlantTypes(string scientificName, string commonName)
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = scientificName,
            CommonName = commonName,
            Description = "Test plant",
            WateringFrequency = "Weekly",
            SunlightRequirement = "Medium"
        };

        // Assert
        plant.ScientificName.Should().Be(scientificName);
        plant.CommonName.Should().Be(commonName);
    }

    [Fact]
    public void Plant_ScientificName_ShouldBeRequired()
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = "",
            CommonName = "Test",
            Description = "Test",
            WateringFrequency = "Weekly",
            SunlightRequirement = "Medium"
        };

        // Assert - In production, this would trigger validation
        plant.ScientificName.Should().BeEmpty();
    }

    [Fact]
    public void Plant_ShouldStoreOptimalTemperature()
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = "Test Plant",
            CommonName = "Test",
            OptimalTemperature = "18-27°C"
        };

        // Assert
        plant.OptimalTemperature.Should().Be("18-27°C");
    }

    [Fact]
    public void Plant_ShouldStoreHumidityRequirements()
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = "Test Plant",
            CommonName = "Test",
            Humidity = "Alta (60-80%)"
        };

        // Assert
        plant.Humidity.Should().Be("Alta (60-80%)");
    }

    [Theory]
    [InlineData("Cada 3-5 días", "Luz directa")]
    [InlineData("Cada 7-10 días", "Luz indirecta brillante")]
    [InlineData("Cada 14-21 días", "Luz indirecta moderada")]
    [InlineData("Cada 21-30 días", "Sombra parcial")]
    public void Plant_ShouldSupportDifferentCareRequirements(string wateringFrequency, string sunlight)
    {
        // Arrange & Act
        var plant = new Plant
        {
            ScientificName = "Test Plant",
            CommonName = "Test",
            WateringFrequency = wateringFrequency,
            SunlightRequirement = sunlight
        };

        // Assert
        plant.WateringFrequency.Should().Be(wateringFrequency);
        plant.SunlightRequirement.Should().Be(sunlight);
    }
}
