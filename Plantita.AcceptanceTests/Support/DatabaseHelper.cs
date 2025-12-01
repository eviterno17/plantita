using Microsoft.Extensions.DependencyInjection;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace Plantita.AcceptanceTests.Support;

/// <summary>
/// Helper for database operations during testing
/// </summary>
public class DatabaseHelper
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseHelper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Clear all data from the database
    /// </summary>
    public async Task ClearDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Clear all tables in reverse order to respect foreign keys

        // IoT Monitoring
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.SensorReading>());
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.SensorConfig>());
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.Entities.Sensor>());
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.iotmonitoring.domain.model.aggregates.IoTDevice>());

        // Plant Management
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.plantmanagment.domain.model.Entities.PlantHealthLog>());
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.plantmanagment.domain.model.Entities.CareTask>());
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.MyPlant>());
        dbContext.RemoveRange(dbContext.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant>());

        // User
        dbContext.RemoveRange(dbContext.Set<plantita.User.Domain.Model.Aggregates.AuthUserRefreshToken>());
        dbContext.RemoveRange(dbContext.Set<plantita.User.Domain.Model.Aggregates.AuthUser>());

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Reset database to clean state
    /// </summary>
    public async Task ResetDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Clear all data
        await ClearDatabase();

        // Optionally seed initial data here if needed
        await SeedBasicData(dbContext);

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Seed basic test data
    /// </summary>
    private async Task SeedBasicData(AppDbContext dbContext)
    {
        // Add any basic seed data needed for all tests
        // For example, a few plants in the catalog

        var basicPlants = new[]
        {
            new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                ScientificName = "Monstera deliciosa",
                CommonName = "Costilla de Adán",
                Description = "Planta tropical de hojas grandes con perforaciones naturales",
                WateringFrequency = "Cada 7-10 días",
                SunlightRequirement = "Luz indirecta brillante"
            },
            new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                ScientificName = "Ficus elastica",
                CommonName = "Árbol del caucho",
                Description = "Planta de interior resistente con hojas grandes y brillantes",
                WateringFrequency = "Cada 10-14 días",
                SunlightRequirement = "Luz indirecta moderada"
            },
            new plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant
            {
                ScientificName = "Pothos aureus",
                CommonName = "Potus dorado",
                Description = "Planta trepadora muy resistente y fácil de cuidar",
                WateringFrequency = "Cada 7-10 días",
                SunlightRequirement = "Luz indirecta baja a media"
            }
        };

        // Note: Only add if they don't exist
        foreach (var plant in basicPlants)
        {
            if (!dbContext.Set<plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.Plant>()
                .Any(p => p.ScientificName == plant.ScientificName))
            {
                dbContext.Add(plant);
            }
        }
    }

    /// <summary>
    /// Get DbContext for direct database access in tests
    /// </summary>
    public AppDbContext GetDbContext()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Execute action within a database scope
    /// </summary>
    public async Task ExecuteInScope(Func<AppDbContext, Task> action)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await action(dbContext);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Execute function within a database scope and return result
    /// </summary>
    public async Task<T> ExecuteInScope<T>(Func<AppDbContext, Task<T>> func)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await func(dbContext);
    }
}
