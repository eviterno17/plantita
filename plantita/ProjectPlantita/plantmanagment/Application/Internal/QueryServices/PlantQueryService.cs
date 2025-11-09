using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;

namespace plantita.ProjectPlantita.plantmanagment.Application.Internal.QueryServices;

public class PlantQueryService : IPlantQueryService
{
    private readonly IPlantRepository _plantRepository;

    public PlantQueryService(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<Plant?> GetByIdAsync(Guid plantId)
    {
        return await _plantRepository.FindByIdGuidAsync(plantId);
    }

    public async Task<Plant?> GetByScientificNameAsync(string commonName)
    {
        return await _plantRepository.GetByCommonNameAsync(commonName);
    }
    
    public async Task<Plant?> GetByCommonNameAsync(string scientificName)
    {
        return await _plantRepository.GetByScientificNameAsync(scientificName);
    }

    public async Task<IEnumerable<Plant>> GetAllAsync()
    {
        return await _plantRepository.GetAllPlantsAsync();
    }
    
    public async Task<List<Plant>> GetAllPlantsAsync()
    {
        return await _plantRepository.GetAllPlantsAsync();
    }
}