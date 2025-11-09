using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;

namespace plantita.ProjectPlantita.plantmanagment.domain.Services;

public interface IPlantQueryService
{
    Task<Plant?> GetByScientificNameAsync(string scientificName);
    Task<List<Plant>> GetAllPlantsAsync();
    Task<Plant?> GetByIdAsync(Guid plantId);
    Task<IEnumerable<Plant>> GetAllAsync();
    Task<Plant?> GetByCommonNameAsync(string name);
    
}