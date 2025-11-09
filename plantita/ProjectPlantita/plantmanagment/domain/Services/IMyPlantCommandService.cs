using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates;
using plantita.ProjectPlantita.plantmanagment.domain.model.Entities;
using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;

namespace plantita.ProjectPlantita.plantmanagment.domain.Services;

public interface IMyPlantCommandService
{
    Task<MyPlant> CreateMyPlantAsync(MyPlant myPlant);
    Task<MyPlant> UpdateMyPlantAsync(Guid myPlantId, MyPlant updatedPlant);
    Task<bool> DeleteMyPlantAsync(Guid myPlantId);

    Task<CareTask> MarkCareTaskCompletedAsync(Guid taskId);
    Task<PlantHealthLog> LogPlantHealthAsync(Guid myPlantId, PlantHealthLog log);
    Task<List<CareTask>> GenerateCareTasksAsync(Guid myPlantId); // cronograma
    Task<MyPlant> RegisterMyPlantAsync(Guid userId,Guid planId,SaveMyPlantResource resource);

}