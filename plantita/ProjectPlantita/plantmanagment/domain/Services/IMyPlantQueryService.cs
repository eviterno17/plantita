using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;

namespace plantita.ProjectPlantita.plantmanagment.domain.Services;

public interface IMyPlantQueryService
{
    Task<MyPlant?> GetByIdAsync(Guid myPlantId);
    Task<IEnumerable<MyPlant>> GetAllByUserIdAsync(Guid userId);
    
    Task<IEnumerable<CareTask>> GetCareTasksAsync(Guid myPlantId);
    Task<CareTask?> GetCareTaskByIdAsync(Guid taskId);

    Task<IEnumerable<PlantHealthLog>> GetHealthLogsAsync(Guid myPlantId);
}