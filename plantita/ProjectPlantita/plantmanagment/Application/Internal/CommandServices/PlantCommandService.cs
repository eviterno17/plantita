using plantita.ProjectPlantita.plantmanagment.domain.model.aggregates.PlantID;
using plantita.ProjectPlantita.plantmanagment.domain.Repositories;
using plantita.ProjectPlantita.plantmanagment.domain.Services;
using plantita.Shared.Domain.Repositories;

namespace plantita.ProjectPlantita.plantmanagment.Application.Internal.CommandServices;


public class PlantCommandService : IPlantCommandService
{
    private readonly IPlantRepository _plantRepository;
    private readonly IPlantIdentificationService _plantIdentificationService;
    private readonly IUnitOfWork _unitOfWork;

    public PlantCommandService(IPlantRepository plantRepository, IUnitOfWork unitOfWork, IPlantIdentificationService plantIdentificationService)
    {
        _plantRepository = plantRepository;
        _unitOfWork = unitOfWork;
        _plantIdentificationService = plantIdentificationService;
    }

    public async Task<Plant?> IdentifyAndRegisterPlantAsync(IFormFile image)
    {
        var identified = await _plantIdentificationService.IdentifyPlantAsync(image);
        if (identified == null) return null;

        // Verifica si ya existe por ScientificName
        var existing = await _plantRepository.GetByScientificNameAsync(identified.ScientificName);
        if (existing != null) return existing;

        // Crear nuevo registro
        var newPlant = new Plant
        {
            PlantId = Guid.NewGuid(),
            ScientificName = identified.ScientificName,
            CommonName = identified.CommonName,
            Description = identified.Description,
            WikiUrl = identified.WikiUrl,
            ImageUrl = identified.ImageUrl,
            Watering = identified.Watering,
            Sunlight = identified.Sunlight,
            CreatedAt = DateTime.UtcNow
        };

        await _plantRepository.AddAsync(newPlant);
        await _unitOfWork.CompleteAsync();

        return newPlant;
    }
    public async Task<Plant> RegisterPlantAsync(Plant plant)
    {
        // Verificar si ya existe por nombre científico
        var existing = await _plantRepository.GetByScientificNameAsync(plant.ScientificName);
        if (existing != null) return existing;

        await _plantRepository.AddAsync(plant);
        await _unitOfWork.CompleteAsync();
        return plant;
    }

    public async Task<Plant> UpdatePlantAsync(Guid plantId, Plant updatedPlant)
    {
        var existing = await _plantRepository.FindByIdGuidAsync(plantId);
        if (existing == null) throw new KeyNotFoundException("Plant not found.");

        existing.ScientificName = updatedPlant.ScientificName;
        existing.CommonName = updatedPlant.CommonName;
        existing.Description = updatedPlant.Description;
        existing.Watering = updatedPlant.Watering;
        existing.Sunlight = updatedPlant.Sunlight;
        existing.ImageUrl = updatedPlant.ImageUrl;
        existing.WikiUrl = updatedPlant.WikiUrl;

        await _unitOfWork.CompleteAsync();
        return existing;
    }

    public async Task<bool> DeletePlantAsync(Guid plantId)
    {
        var existing = await _plantRepository.FindByIdGuidAsync(plantId);
        if (existing == null) return false;

        await _plantRepository.DeleteAsync(existing);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}