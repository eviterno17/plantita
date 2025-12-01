using FluentAssertions;
using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using Xunit;

namespace Plantita.CoreTests.Domain.PlantManagement;

/// <summary>
/// Unit tests for MyPlant aggregate root
/// </summary>
public class MyPlantTests
{
    [Fact]
    public void MyPlant_ShouldBeCreated_WithValidData()
    {
        // Arrange & Act
        var myPlant = new MyPlant
        {
            PlantId = 1,
            UserId = 100,
            CustomName = "Mi Monstera del salón",
            Location = "Salón junto a la ventana",
            Notes = "Regalo de cumpleaños"
        };

        // Assert
        myPlant.Should().NotBeNull();
        myPlant.PlantId.Should().Be(1);
        myPlant.UserId.Should().Be(100);
        myPlant.CustomName.Should().Be("Mi Monstera del salón");
        myPlant.Location.Should().Be("Salón junto a la ventana");
        myPlant.Notes.Should().Be("Regalo de cumpleaños");
    }

    [Fact]
    public void MyPlant_ShouldHaveHealthLogs_Collection()
    {
        // Arrange & Act
        var myPlant = new MyPlant
        {
            PlantId = 1,
            UserId = 100,
            CustomName = "Test Plant"
        };

        // Assert
        myPlant.HealthLogs.Should().NotBeNull();
        myPlant.HealthLogs.Should().BeEmpty();
    }

    [Fact]
    public void MyPlant_ShouldHaveCareTasks_Collection()
    {
        // Arrange & Act
        var myPlant = new MyPlant
        {
            PlantId = 1,
            UserId = 100,
            CustomName = "Test Plant"
        };

        // Assert
        myPlant.CareTasks.Should().NotBeNull();
        myPlant.CareTasks.Should().BeEmpty();
    }

    [Fact]
    public void MyPlant_CanAddHealthLog()
    {
        // Arrange
        var myPlant = new MyPlant
        {
            Id = 1,
            PlantId = 1,
            UserId = 100,
            CustomName = "Test Plant"
        };

        var healthLog = new PlantHealthLog
        {
            MyPlantId = myPlant.Id,
            Date = DateTime.UtcNow,
            HealthStatus = "Buena",
            Observations = "Nuevas hojas creciendo"
        };

        // Act
        myPlant.HealthLogs.Add(healthLog);

        // Assert
        myPlant.HealthLogs.Should().HaveCount(1);
        myPlant.HealthLogs.First().HealthStatus.Should().Be("Buena");
        myPlant.HealthLogs.First().Observations.Should().Be("Nuevas hojas creciendo");
    }

    [Fact]
    public void MyPlant_CanAddCareTask()
    {
        // Arrange
        var myPlant = new MyPlant
        {
            Id = 1,
            PlantId = 1,
            UserId = 100,
            CustomName = "Test Plant"
        };

        var careTask = new CareTask
        {
            MyPlantId = myPlant.Id,
            TaskType = "Riego",
            ScheduledDate = DateTime.UtcNow.AddDays(3),
            Status = "Pendiente",
            Notes = "Riego profundo"
        };

        // Act
        myPlant.CareTasks.Add(careTask);

        // Assert
        myPlant.CareTasks.Should().HaveCount(1);
        myPlant.CareTasks.First().TaskType.Should().Be("Riego");
        myPlant.CareTasks.First().Status.Should().Be("Pendiente");
    }

    [Fact]
    public void MyPlant_CanHaveMultipleHealthLogs()
    {
        // Arrange
        var myPlant = new MyPlant
        {
            Id = 1,
            PlantId = 1,
            UserId = 100,
            CustomName = "Test Plant"
        };

        // Act
        for (int i = 0; i < 5; i++)
        {
            myPlant.HealthLogs.Add(new PlantHealthLog
            {
                MyPlantId = myPlant.Id,
                Date = DateTime.UtcNow.AddDays(-i),
                HealthStatus = "Buena",
                Observations = $"Observación día {i}"
            });
        }

        // Assert
        myPlant.HealthLogs.Should().HaveCount(5);
    }

    [Fact]
    public void MyPlant_CanHaveMultipleCareTasks()
    {
        // Arrange
        var myPlant = new MyPlant
        {
            Id = 1,
            PlantId = 1,
            UserId = 100,
            CustomName = "Test Plant"
        };

        var taskTypes = new[] { "Riego", "Fertilización", "Poda", "Trasplante" };

        // Act
        foreach (var taskType in taskTypes)
        {
            myPlant.CareTasks.Add(new CareTask
            {
                MyPlantId = myPlant.Id,
                TaskType = taskType,
                ScheduledDate = DateTime.UtcNow.AddDays(7),
                Status = "Pendiente"
            });
        }

        // Assert
        myPlant.CareTasks.Should().HaveCount(4);
        myPlant.CareTasks.Select(t => t.TaskType).Should().Contain(taskTypes);
    }

    [Theory]
    [InlineData("Pendiente")]
    [InlineData("En Progreso")]
    [InlineData("Completada")]
    [InlineData("Cancelada")]
    public void CareTask_ShouldSupportDifferentStatuses(string status)
    {
        // Arrange & Act
        var task = new CareTask
        {
            MyPlantId = 1,
            TaskType = "Riego",
            ScheduledDate = DateTime.UtcNow,
            Status = status
        };

        // Assert
        task.Status.Should().Be(status);
    }

    [Theory]
    [InlineData("Excelente")]
    [InlineData("Buena")]
    [InlineData("Regular")]
    [InlineData("Mala")]
    [InlineData("Crítica")]
    public void PlantHealthLog_ShouldSupportDifferentHealthStatuses(string healthStatus)
    {
        // Arrange & Act
        var healthLog = new PlantHealthLog
        {
            MyPlantId = 1,
            Date = DateTime.UtcNow,
            HealthStatus = healthStatus,
            Observations = "Test observations"
        };

        // Assert
        healthLog.HealthStatus.Should().Be(healthStatus);
    }

    [Fact]
    public void MyPlant_ShouldBelongToSpecificUser()
    {
        // Arrange & Act
        var user1Plant = new MyPlant
        {
            PlantId = 1,
            UserId = 100,
            CustomName = "User 1 Plant"
        };

        var user2Plant = new MyPlant
        {
            PlantId = 1,
            UserId = 200,
            CustomName = "User 2 Plant"
        };

        // Assert
        user1Plant.UserId.Should().Be(100);
        user2Plant.UserId.Should().Be(200);
        user1Plant.UserId.Should().NotBe(user2Plant.UserId);
    }

    [Fact]
    public void CareTask_CanBeMarkedAsCompleted()
    {
        // Arrange
        var task = new CareTask
        {
            MyPlantId = 1,
            TaskType = "Riego",
            ScheduledDate = DateTime.UtcNow,
            Status = "Pendiente"
        };

        // Act
        task.Status = "Completada";
        task.CompletedDate = DateTime.UtcNow;

        // Assert
        task.Status.Should().Be("Completada");
        task.CompletedDate.Should().NotBeNull();
        task.CompletedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void PlantHealthLog_ShouldRecordObservationDate()
    {
        // Arrange
        var observationDate = new DateTime(2024, 12, 15, 10, 30, 0);

        // Act
        var healthLog = new PlantHealthLog
        {
            MyPlantId = 1,
            Date = observationDate,
            HealthStatus = "Buena",
            Observations = "Test"
        };

        // Assert
        healthLog.Date.Should().Be(observationDate);
    }
}
