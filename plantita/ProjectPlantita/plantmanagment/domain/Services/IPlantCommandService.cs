using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;

namespace plantita.ProjectPlantita.plantmanagment.domain.Services;

public interface IPlantCommandService
{
    Task<Plant> RegisterPlantAsync(Plant plant); // desde identificación
    Task<Plant> UpdatePlantAsync(Guid plantId, Plant updatedPlant);
    Task<bool> DeletePlantAsync(Guid plantId);
    Task<Plant?> IdentifyAndRegisterPlantAsync(IFormFile image);

}