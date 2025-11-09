using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.Shared.Domain.Repositories;

namespace plantita.ProjectPlantita.plantmanagment.domain.Repositories;

public interface IPlantRepository : IBaseRepository<Plant>
{
    Task<Plant?> GetByScientificNameAsync(string scientificName);
    Task<Plant?> GetByCommonNameAsync(string commonName);
    Task DeleteAsync(Plant plant);
    Task<List<Plant>> GetAllPlantsAsync();
}