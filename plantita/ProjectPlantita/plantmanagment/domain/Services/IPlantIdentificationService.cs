using plantita.ProjectPlantita.plantmanagment.Interfaces.Resources;

namespace plantita.ProjectPlantita.plantmanagment.domain.Services;

public interface IPlantIdentificationService
{
    Task<IdentifiedPlantResource?> IdentifyPlantAsync(IFormFile image);
}